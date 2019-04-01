/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/11 20:36:54
 * 版本：v0.7
 */
using System.Xml;
using TTT.Item;
using TTT.UI;
using TTT.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBattle;

namespace WorldMap.Model
{
    public abstract class SentenceAction
    {
        /// <summary>
        /// 句子结束后的行为，如：获得物品等等
        /// </summary>
        /// <returns></returns>
        public abstract void DoAction();
        public static SentenceAction[] Compile(string action)
        {
            SentenceAction[] ans;
            if (action != null && action.Length != 0)
            {
                string[] actions = action.Split(';');
                ans = new SentenceAction[actions.Length];
                for (int i = 0; i < ans.Length; i++)
                {
                    string[] words = actions[i].Split(' ');
                    switch (words[1])
                    {
                        case "item": ans[i] = new ItemAction(words); break;
                        case "task": ans[i] = new TaskAction(words); break;
                        case "npc": ans[i] = new NpcAction(words); break;
                        case "money": ans[i] = new MoneyAction(words); break;
                        case "food": ans[i] = new FoodAction(words); break;
                        case "power": ans[i] = new PowerAction(words); break;
                        default: throw new XmlException("不支持的指令：" + words[1]);
                    }
                }
            }
            else
            {
                ans = new SentenceAction[0];
            }
            return ans;
        }
    }
    public class ItemAction : SentenceAction
    {
        /// <summary>
        /// 行为类型：{0 获得物品|1 失去物品|2 提供物品}
        /// 失去物品 和 提供物品 
        /// 区别是 失去的物品可以不必拥有，而提供物品必须拥有该物品
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemID { get; private set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Number { get; private set; }
        public ItemAction(string[] words)
        {
            switch (words[0])
            {
                case "get": Type = 0; break;
                case "loss": Type = 1; break;
                case "offer": Type = 2; break;
                default: throw new XmlException("不支持的指令：" + words[0]);
            }
            string[] item = words[2].Split(':');
            ItemID = int.Parse(item[0]);
            Number = int.Parse(item[1]);
        }

        public override void DoAction()
        {
            Storage storage = World.getInstance().storage;
            switch (Type)
            {
                case 0:
                    storage.AddItem(ItemID, Number);
                    break;
                case 1:
                case 2:
                    storage.RemoveItem(ItemID, Number);
                    break;
            }
        }
    }
    public class TaskAction : SentenceAction
    {
        /// <summary>
        /// {0：接收任务|1：完成任务|2：结束任务}
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int[] TaskIDs { get; private set; }

        public TaskAction(string[] words)
        {
            switch (words[0])
            {
                case "take": Type = 0; break;
                case "achieve": Type = 1; break;
                case "finish": Type = 2; break;
                case "fight": Type = 3; break;
                default: throw new XmlException("不支持的指令：" + words[0]);
            }
            string[] ids = words[2].Split(',');
            TaskIDs = new int[ids.Length];
            for (int i = 0; i < TaskIDs.Length; i++)
                TaskIDs[i] = int.Parse(ids[i]);
        }

        public override void DoAction()
        {
            Debug.Log("task句子开始DoAction,type="+ Type);
            foreach (int TaskID in TaskIDs)
            {
                Task task = TaskController.getInstance().getTask(TaskID);
                Debug.Log("任务id=" + TaskID + "  任务状态=" + task.condition);
                if (task == null)
                {
                    Debug.LogError("任务不存在 ID" + TaskID);
                    return;
                }
                switch (Type)
                {
                    case 0:
                        if (task.condition == TaskController.TASKCONDITION.CAN_DO)
                        {
                            task.get_task();
                            FlowInfo.ShowInfo("接受新任务", task.name);
                        }
                        else
                        {
                            Debug.LogError("接受任务失败 任务ID：" + TaskID);
                        }
                        break;
                    case 1: task.achieve_task(); break;
                    case 2:
                        FlowInfo.ShowInfo("完成任务", task.name);
                        task.finish_task();
                        break;
                    case 3: task.enter_talk_battle(); break;
                    default: throw new XmlException("不支持的指令：" + Type);
                }

            }
        }
    }
    public class NpcAction : SentenceAction
    {
        /// <summary>
        /// {0：招募NPC | 1：消失NPC}
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        /// NPC的ID
        /// </summary>
        public int NpcID { get; private set; }
        public NpcAction(string[] words)
        {
            switch (words[0])
            {
                case "recruit": Type = 0; break;
                case "disappear": Type = 1; break;
            }
            NpcID = int.Parse(words[2]);
        }

        public override void DoAction()
        {
            switch (Type)
            {
                case 0:
                    {
                        World.getInstance().Persons.RecruitNpc(NpcID);
                        FlowInfo.ShowInfo("招募新人", NpcInfoLoader.Instance.Find(NpcID).Name + "加入小队！");
                    }
                    break;
                case 1:
                    {
                        TownData town;
                        if (World.getInstance().Towns.RemoveNpc(NpcID, out town))
                        {
                            FlowInfo.ShowInfo("事件", NpcInfoLoader.Instance.Find(NpcID).Name + "离开了" + town.Info.Name);
                        }
                    }
                    break;
            }
        }
    }
    public class MoneyAction : SentenceAction
    {
        /// <summary>
        /// 0 失去 | 1 获得
        /// </summary>
        public int Type { get; private set; }
        public int Money { get; private set; }
        public MoneyAction(string[] words)
        {
            switch (words[0])
            {
                case "loss": Type = 0; break;
                case "get": Type = 1; break;
            }
            Money = int.Parse(words[2]);
        }

        public override void DoAction()
        {
            int money = Money;
            switch (Type)
            {
                case 0: money = -money; break;
                case 1: break;
            }
            World.getInstance().addMoney(money);
        }
    }
    public class FoodAction : SentenceAction
    {
        public int Type { get; private set; }
        public int Food { get; private set; }
        public FoodAction(string[] words)
        {
            switch (words[0])
            {
                case "get": Type = 0; break;
            }
            Food = int.Parse(words[2]);
        }

        public override void DoAction()
        {
            World.getInstance().addFood(Food);
        }
    }
    public class PowerAction : SentenceAction
    {
        public int Type { get; private set; }
        public int Power { get; private set; }
        public PowerAction(string[] words)
        {
            switch (words[0])
            {
                case "get": Type = 0; break;
            }
            Power = int.Parse(words[2]);
        }

        public override void DoAction()
        {
            World.getInstance().addEnergy(Power);
        }
    }
}