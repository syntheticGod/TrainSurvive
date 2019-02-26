/*
 * 描述：对话剧情的的前提条件
 *          必须满足对话剧情中的所有前提条件，才能够触发该对话剧情，或句子。
 * 作者：项叶盛
 * 创建时间：2019/2/11 22:03:25
 * 版本：v0.7
 */
using System.Xml;
using TTT.Common;
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
                        case "girl":
                        case "boy": preconditions[i] = new HeroCondition(words); break;
                        default: throw new XmlException("不支持的指令：" + words[1]);
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
        protected bool SemanticCompare(int a, int b, string op)
        {

            switch (op)
            {
                case ">": return a > b;
                case ">=": return a >= b;
                case "<": return a < b;
                case "<=": return a <= b;
                case "==": return a == b;
                default: throw new XmlException("不支持的指令：" + op);
            }
        }
        protected int SemanticCompareIndex(string op)
        {
            switch (op)
            {
                case ">": return 0;
                case ">=": return 1;
                case "<": return 2;
                case "<=": return 3;
                case "==": return 4;
                default: throw new XmlException("不支持的指令：" + op);
            }
        }
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
                default: throw new XmlException("不支持的指令：" + Type);
            }
            string[] items = words[2].Split(',');
            ItemIDs = new int[items.Length];
            Numbers = new int[items.Length];
            for (int i = 0; i < ItemIDs.Length; i++)
            {
                string[] item = items[i].Split(':');
                ItemIDs[i] = int.Parse(item[0]);
                Numbers[i] = int.Parse(item[1]);
            }
        }
        public override bool IfSatisfy()
        {
            for (int i = 0; i < ItemIDs.Length; i++)
            {
                int itemID = ItemIDs[i];
                int number = Numbers[i];
                switch (Type)
                {
                    case 0: if (!World.getInstance().storage.ContainItem(itemID, number)) return false; break;
                    default: throw new XmlException("不支持的指令：" + Type);
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
                        default: throw new XmlException("不支持的指令：" + Type);
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
                default: throw new XmlException("不支持的指令：" + words[0]);
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
        /// 类型：{0 正在进行任务| 1 可完成任务 | 2 已完成任务 | 3 解锁任务}
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
                case "done": Type = 1; break;
                case "finish": Type = 2; break;
                case "unlock": Type = 3; break;
                default: throw new XmlException("不支持的指令：" + cmd[0]);
            }
            TaskID = int.Parse(cmd[2]);
        }
        public override bool IfSatisfy()
        {
            Task task = TaskController.getInstance().getTask(TaskID);
            switch (Type)
            {
                case 0: if (task.condition == TaskController.TASKCONDITION.DOING) return true; break;
                case 1: if (task.condition == TaskController.TASKCONDITION.CAN_FINISH) return true; break;
                case 2: if (task.condition == TaskController.TASKCONDITION.FINISH) return true; break;
                case 3: if (task.condition == TaskController.TASKCONDITION.CAN_DO) return true; break;
                default: throw new XmlException("不支持的指令：" + Type);
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
                default: throw new XmlException("不支持的指令：" + Type);
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
        public string CompareOperator { get; private set; }
        public NpcCondition(string[] cmd)
        {
            switch (cmd[0])
            {
                case "count":
                    Type = 0;
                    CompareOperator = cmd[2];
                    Count = int.Parse(cmd[3]);
                    break;
                default: throw new XmlException("不支持的指令：" + cmd[0]);
            }
        }
        public override bool IfSatisfy()
        {
            switch (Type)
            {
                case 0:
                    TownData town = World.getInstance().PMarker.GetCurrentTown();
                    if (town == null) return false;
                    return SemanticCompare(town.Npcs.Count, Count, CompareOperator);
                default: throw new XmlException("不支持的指令：" + Type);
            }
        }

        public override string FailureMessage()
        {
            string[] message = { "不多于", "少于", "不少于", "多于", "不等于" };
            return "该城的NPC的数量" + message[SemanticCompareIndex(CompareOperator)] + Count + "个";
        }
    }
    public class HeroCondition : Precondition
    {
        /// <summary>
        /// {0 拥有}
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        /// 性别 0 男 1 女
        /// </summary>
        public int Gender { get; private set; }
        public int[] AttriNumber { get; private set; }
        public string[] AttriComparer { get; private set; }
        public HeroCondition(string[] cmd)
        {
            switch (cmd[0])
            {
                case "have": Type = 0; break;
                default: throw new XmlException("不支持的指令：" + cmd[0]);
            }
            switch (cmd[1])
            {
                case "boy": Gender = 0; break;
                case "girl": Gender = 1; break;
                default: throw new XmlException("不支持的指令：" + cmd[1]);
            }
            if (cmd.Length < 6)
                throw new XmlException("指令长度不足");
            AttriNumber = new int[(int)EAttribute.NUM];
            AttriComparer = new string[(int)EAttribute.NUM];
            for (int i = 3; i < cmd.Length; i += 4)
            {
                int index = AttriTool.Compile(cmd[i]);
                AttriComparer[index] = cmd[i + 1];
                AttriNumber[index] = int.Parse(cmd[i + 2]);
            }
        }
        public override bool IfSatisfy()
        {
            foreach (Person person in World.getInstance().Persons)
            {
                bool satisfy = true;
                for (int i = 0; i < AttriNumber.Length; i++)
                {
                    if (AttriComparer[i].Length != 0 && !SemanticCompare(person.AttriNumbers[i], AttriNumber[i], AttriComparer[i]))
                    {
                        satisfy = false;
                        break;
                    }
                }
                if (satisfy) return true;
            }
            return false;
        }
        public override string FailureMessage()
        {
            string[] message = { "多于", "多于或等于", "少于", "少于或等于", "等于" };
            string ans = "队伍中不存在";
            for (int i = 0; i < AttriNumber.Length; i++)
            {
                if (AttriComparer[i].Length != 0)
                {
                    ans += AttriTool.NameC[i] + message[SemanticCompareIndex(AttriComparer[i])] + "且";
                }
            }
            return ans.Remove(ans.Length - 1) + "的人物";
        }
    }
}
