/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/11 20:36:54
 * 版本：v0.7
 */
using System.Xml;
using TTT.Item;
using TTT.UI;
using UnityEngine;


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
                        case "battle": ans[i] = new BattleAction(words); break;
                        case "task": ans[i] = new TaskAction(words); break;
                        case "npc": ans[i] = new NpcAction(words); break;
                        case "money": ans[i] = new MoneyAction(words); break;
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
                default:
                    throw new XmlException("不支持的物品指令");
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
    public class BattleAction : SentenceAction
    {
        /// <summary>
        /// 战斗指令类型：{0 马上进入战斗|1 在指定地点生成战斗}
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        /// 特殊战斗的ID
        /// </summary>
        public int BattleID { get; private set; }
        /// <summary>
        /// 生成战斗的地点 如果坐标为-1，-1表示
        /// </summary>
        public Vector2Int PosInArea { get; private set; }
        public BattleAction(string[] words)
        {
            switch (words[0])
            {
                case "start": Type = 0; break;
                case "generate":
                    Type = 1;
                    char[] charsToTrim = { '(', ')' };
                    string[] coor = words[4].Trim(charsToTrim).Split(',');
                    int x = int.Parse(coor[0]);
                    int y = int.Parse(coor[1]);
                    PosInArea = new Vector2Int(x, y);
                    break;
                default:
                    throw new XmlException("不支持的战斗指令");
            }
            BattleID = int.Parse(words[2]);
        }

        public override void DoAction()
        {
            SpecialBattle specialBattle = SpecialBattleInitializer.getInstance().loadBattle(BattleID);
            if (specialBattle == null)
            {
                Debug.LogError("特殊战斗不存在 ID：" + BattleID);
                return;
            }
            switch (Type)
            {
                case 0:
                    break;
                case 1:
                    SpecialBattleInitializer.getInstance().generateSpecialBattle(specialBattle);
                    break;
            }
        }
    }
    public class TaskAction : SentenceAction
    {
        /// <summary>
        /// {0：接收任务|1：完成任务}
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
                case "fulfil": Type = 1; break;
            }
            string[] ids = words[2].Split(',');
            TaskIDs = new int[ids.Length];
            for (int i = 0; i < TaskIDs.Length; i++)
                TaskIDs[i] = int.Parse(ids[i]);
        }

        public override void DoAction()
        {
            foreach(int TaskID in TaskIDs)
            {
                Task task = TaskController.getInstance().getTask(TaskID);
                if (task == null)
                {
                    Debug.LogError("任务不存在 ID" + TaskID);
                    return;
                }
                switch (Type)
                {
                    case 0:
                        if(task.condition == TaskController.TASKCONDITION.CAN_DO)
                        {
                            task.get_task();
                            FlowInfo.ShowInfo("接受新任务", task.name);
                        }
                        else
                        {
                            Debug.LogError("接受任务失败 任务ID：" + TaskID);
                        }
                        break;
                    case 1:
                        task.achieve_task();
                        FlowInfo.ShowInfo("完成任务", task.name);
                        task.finish_task();
                        break;
                }

            }
        }
    }
    public class NpcAction : SentenceAction
    {
        /// <summary>
        /// {0：招募NPC | 1：在大地图上消失NPC}
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
                case 0: World.getInstance().Persons.RecruitNpc(NpcID); break;
                case 1: throw new System.NotImplementedException();
            }
        }
    }
    public class MoneyAction : SentenceAction
    {
        public int Type { get; private set; }
        public int Money { get; private set; }
        public MoneyAction(string[] words)
        {
            switch (words[0])
            {
                case "loss": Type = 0; break;
            }
            Money = int.Parse(words[2]);
        }

        public override void DoAction()
        {
            World.getInstance().PayByMoney(Money);
        }
    }
}