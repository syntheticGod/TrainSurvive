/*
 * 描述：对话剧情的的前提条件
 *          必须满足对话剧情中的所有前提条件，才能够触发该对话剧情，或句子。
 * 作者：项叶盛
 * 创建时间：2019/2/11 22:03:25
 * 版本：v0.7
 */
using System.Xml;
using TTT.Item;
using TTT.Resource;
using UnityEngine;

namespace WorldMap.Model
{
    public abstract class Precondition
    {
        /// <summary>
        /// 根据上下文判断
        /// </summary>
        /// <example>
        /// 例如：
        ///     判断城镇中NPC的人数，则需要传入城镇对象上下文
        /// </example>
        /// <param name="context">上下文</param>
        /// <returns>
        /// TRUE：满足条件 或 上下文不为空时，上下文不匹配。
        /// FLASE：不满足条件
        /// </returns>
        public abstract bool IfSatisfy();
        public static Precondition[] Compile(string condition)
        {
            Precondition[] preconditions;
            if (condition != null && condition.Length != 0)
            {
                string[] precon = condition.Split(';');
                preconditions = new Precondition[precon.Length];
                for (int i = 0; i < precon.Length; i++)
                {
                    string[] words = precon[i].Split(' ');
                    switch (words[1])
                    {
                        case "item": preconditions[i] = new ItemCondition(words); break;
                        case "money": preconditions[i] = new MoneyCondition(words); break;
                        case "task": preconditions[i] = new TaskCondition(words); break;
                        case "other": preconditions[i] = new OtherCondition(words); break;
                        case "npc": preconditions[i] = new NpcCondition(words); break;
                        default: throw new XmlException("不支持的物品指令");
                    }
                }
            }
            else
            {
                preconditions = new Precondition[0];
            }
            return preconditions;
        }
        public abstract string FailureMessage();
    }
    public class ItemCondition : Precondition
    {
        /// <summary>
        /// 类型 {0 | 拥有物品 }
        /// </summary>
        public int Type { get; private set; }
        public int[] ItemIDs { get; private set; }
        public int[] Numbers { get; private set; }
        public ItemCondition(string[] words)
        {
            switch (words[0])
            {
                case "have": Type = 0; break;
                default: throw new XmlException("不支持的物品指令");
            }
            string[] items = words[2].Split(',');
            ItemIDs = new int[items.Length];
            Numbers = new int[items.Length];
            for (int i = 0; i < ItemIDs.Length; i++)
            {
                Debug.Log(items[i]);
                string[] item = items[i].Split(':');
                ItemIDs[i] = int.Parse(item[0]);
                Numbers[i] = int.Parse(item[1]);
            }
        }
        public override bool IfSatisfy()
        {
            for(int i = 0; i < ItemIDs.Length; i++)
            {
                int itemID = ItemIDs[i];
                int number = Numbers[i];
                switch (Type)
                {
                    case 0:if (!World.getInstance().storage.ContainItem(itemID, number)) return false;break;
                    default: throw new XmlException("不支持的物品指令");
                }
            }
            return true;
        }

        public override string FailureMessage()
        {
            string ans = "";
            for (int i = 0; i < ItemIDs.Length; i++)
            {
                int itemID = ItemIDs[i];
                int number = Numbers[i];
                if (!World.getInstance().storage.ContainItem(itemID, number))
                {
                    ItemInfo info = StaticResource.GetItemInfoByID<ItemInfo>(itemID);
                    switch (Type)
                    {
                        case 0: ans = "背包中的" + info.Name + "少于" + number + "个"; break;
                        default: throw new System.NotImplementedException();
                    }
                }
            }
            return ans;
        }
    }
    public class MoneyCondition : Precondition
    {
        /// <summary>
        /// 类型 { 0 | 拥有金额 }
        /// </summary>
        public int Type { get; private set; }
        public int Money { get; private set; }
        public MoneyCondition(string[] words)
        {
            switch (words[0])
            {
                case "have": Type = 0; break;
                default:throw new XmlException("不支持的物品指令");
            }
            Money = int.Parse(words[2]);
        }
        public override bool IfSatisfy()
        {
            return World.getInstance().IfMoneyEnough(Money);
        }

        public override string FailureMessage()
        {
            return "金币不足";
        }
    }
    public class OtherCondition : Precondition
    {
        /// <summary>
        /// 前置对话ID
        /// </summary>
        public int[] OtherDialogueID { get; private set; }
        public OtherCondition(string[] words)
        {
            char[] charsToTrim = { '(', ')' };
            string[] OtherID = words[2].Trim(charsToTrim).Split(',');
            OtherDialogueID = new int[OtherID.Length];
            for (int i = 0; i < OtherID.Length; i++)
                OtherDialogueID[i] = int.Parse(OtherID[i]);
        }
        public override bool IfSatisfy()
        {
            return World.getInstance().Dialogues.IfTalked(OtherDialogueID);
        }

        public override string FailureMessage()
        {
            return "";
        }
    }
    public class TaskCondition : Precondition
    {
        /// <summary>
        /// 类型：{0 正在进行任务| 1 完成任务 | 2 解锁任务}
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskID { get; private set; }
        public TaskCondition(string[] cmd)
        {
            switch (cmd[0])
            {
                case "doing": Type = 0; break;
                case "finish": Type = 1; break;
                case "unlock":Type = 2;break;
                default: throw new XmlException("不支持的物品指令");
            }
            TaskID = int.Parse(cmd[2]);
        }
        public override bool IfSatisfy()
        {
            Task task = TaskController.getInstance().getTask(TaskID);
            switch (Type)
            {
                case 0: if (task.condition == TaskController.TASKCONDITION.DOING) return true; break;
                case 1: if (task.condition == TaskController.TASKCONDITION.FINISH) return true; break;
                case 2:if (task.condition == TaskController.TASKCONDITION.CAN_DO) return true;break;
                default: throw new XmlException("不支持的物品指令");
            }
            return false;
        }

        public override string FailureMessage()
        {
            string ans = "";
            switch (Type)
            {
                case 0: ans = "未接受指定任务"; break;
                case 1: ans = "任务未完成"; break;
                case 2: ans = "前置任务未解锁"; break;
                default: throw new XmlException("不支持的物品指令");
            }
            return ans;
        }
    }
    public class NpcCondition : Precondition
    {
        /// <summary>
        /// 类型：{0 当前城镇的数量}
        /// </summary>
        public int Type { get; private set; }
        public int Count { get; private set; }
        private string m_comparer;
        public NpcCondition(string[] cmd)
        {
            switch (cmd[0])
            {
                case "count":
                    Type = 0;
                    m_comparer = cmd[2];
                    Count = int.Parse(cmd[3]);
                    break;
                default: throw new XmlException("不支持的物品指令");
            }

        }
        public override bool IfSatisfy()
        {
            switch (Type)
            {
                case 0:
                    TownData town = World.getInstance().PMarker.GetCurrentTown();
                    if (town == null) return false;
                    switch (m_comparer)
                    {
                        case ">": return town.Npcs.Count > Count;
                        case ">=": return town.Npcs.Count >= Count;
                        case "<": return town.Npcs.Count < Count;
                        case "<=": return town.Npcs.Count <= Count;
                        case "==": return town.Npcs.Count == Count;
                        default: throw new XmlException("不支持的物品指令");
                    }
                default: throw new XmlException("不支持的物品指令");
            }
        }

        public override string FailureMessage()
        {
            string ans = "该城的NPC的数量";
            switch (m_comparer)
            {
                case ">": return ans += "不多于" + Count + "个";
                case ">=": return ans += "少于" + Count + "个";
                case "<": return ans += "不少于" + Count + "个";
                case "<=": return ans += "多于" + Count + "个";
                case "==": return ans += "不等于" + Count + "个";
                default: throw new XmlException("不支持的物品指令");
            }
        }
    }
}
