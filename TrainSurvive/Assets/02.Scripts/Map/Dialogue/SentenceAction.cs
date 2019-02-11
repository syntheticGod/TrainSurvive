/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/11 20:36:54
 * 版本：v0.7
 */
using System.Xml;
using TTT.Item;
using UnityEngine;


namespace WorldMap.Model
{
    public abstract class SentenceAction
    {
        /// <summary>
        /// 句子结束后的行为，如：获得物品等等
        /// </summary>
        /// <returns></returns>
        public abstract bool DoAction();
        /// <summary>
        /// 当行为组中的某个行为失败之后，需要撤销之前做过的行为。
        /// </summary>
        public abstract void UnDoAction();
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

        public override bool DoAction()
        {
            Storage storage = World.getInstance().storage;
            switch (Type)
            {
                case 0:
                    storage.AddItem(ItemID, Number);
                    break;
                case 1:
                case 2:
                    if (!storage.RemoveItem(ItemID, Number)) return false;
                    break;
            }
            return true;
        }

        public override void UnDoAction()
        {
            Storage storage = World.getInstance().storage;
            switch (Type)
            {
                case 0:
                    if (!storage.RemoveItem(ItemID, Number))
                        Debug.LogError("Undo sentence action失败，物品" + ItemID + "的数量少于" + Number);
                    break;
                case 1:
                case 2:
                    storage.AddItem(ItemID, Number);
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

        public override bool DoAction()
        {
            throw new System.NotImplementedException();
        }

        public override void UnDoAction()
        {
            throw new System.NotImplementedException();
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
        public int TaskID { get; private set; }

        public TaskAction(string[] words)
        {
            switch (words[0])
            {
                case "take": Type = 0; break;
                case "fulfil": Type = 1; break;
            }
            TaskID = int.Parse(words[2]);
        }

        public override bool DoAction()
        {
            throw new System.NotImplementedException();
        }

        public override void UnDoAction()
        {
            throw new System.NotImplementedException();
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

        public override bool DoAction()
        {
            throw new System.NotImplementedException();
        }

        public override void UnDoAction()
        {
            throw new System.NotImplementedException();
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

        public override bool DoAction()
        {
            throw new System.NotImplementedException();
        }

        public override void UnDoAction()
        {
            throw new System.NotImplementedException();
        }
    }
}