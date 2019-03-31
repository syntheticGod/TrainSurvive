/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/27 23:30:05
 * 版本：v0.7
 */
using UnityEngine;
using System;
using System.Xml;
using System.Runtime.Serialization;
using TTT.Utility;
using TTT.Xml;

namespace WorldMap.Model
{
    [Serializable]
    public class TownInfo : ISerializable
    {
        /// <summary>
        /// 城镇ID
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// 当前城镇的类型
        /// </summary>
        public ETownType Type { get; private set; }
        /// <summary>
        /// 城镇名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 城镇描述
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// 酒馆名字
        /// </summary>
        public string TavernName { get; private set; }
        /// <summary>
        /// 5X5 大区块中的位置
        /// </summary>
        public Vector2Int PosInArea { get; private set; }
        /// <summary>
        /// 城镇开场白
        /// </summary>
        public DialogueInfo Dialogue { get; private set; }
        public TownInfo(XmlNode node)
        {
            ID = int.Parse(node.Attributes["id"].Value);
            Type = ETownType.COMMON + int.Parse(node.Attributes["typeId"].Value);
            Name = node.Attributes["name"].Value;
            Description = node.Attributes["description"].Value;
            if (node.Attributes["posx"] != null && node.Attributes["posy"] != null)
            {
                int posx = int.Parse(node.Attributes["posx"].Value);
                int posy = int.Parse(node.Attributes["posy"].Value);
                PosInArea = new Vector2Int(posx, posy);
            }
            TavernName = node.Attributes["tavernName"].Value;
            Dialogue = new TavernDialogueInfo(TavernName, node.SelectSingleNode("dialogue"));
        }
        public TownInfo(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt32("ID");
            Type = (ETownType)info.GetValue("Type", typeof(int));
            Name = info.GetString("Name");
            Description = info.GetString("Description");
            PosInArea = new Vector2Int(info.GetInt32("PosXInArea"), info.GetInt32("PosYInArea"));
            //普通城镇的基本信息 在 xml中
            TownInfo baseInfo = TownInfoLoader.Instance.FindSTownInfoByID(Type == ETownType.COMMON ? 0 : ID);
            //与城镇开场白对话 和 城镇名 不存档
            Dialogue = baseInfo.Dialogue;
            TavernName = baseInfo.TavernName;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID, typeof(int));
            info.AddValue("Type", Type, typeof(int));
            info.AddValue("Name", Name, typeof(string));
            info.AddValue("Description", Description, typeof(string));
            info.AddValue("PosXInArea", PosInArea.x, typeof(int));
            info.AddValue("PosYInArea", PosInArea.y, typeof(int));
        }
        private TownInfo() { }
        /// <summary>
        /// 根据城镇名字 和 坐标 随机一个城镇信息
        /// 城镇ID 从1000起头
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posInAreaX"></param>
        /// <param name="posInAreaY"></param>
        /// <returns></returns>
        public static TownInfo Random(string name, int posInAreaX, int posInAreaY)
        {
            //获取随机城镇的默认信息
            TownInfo townInfo = TownInfoLoader.Instance.FindSTownInfoByID(0).Clone();
            townInfo.ID = World.getInstance().Towns.NewID();
            townInfo.Name = name;
            townInfo.PosInArea = new Vector2Int(posInAreaX, posInAreaY);
            return townInfo;
        }
        public TownInfo Clone()
        {
            TownInfo ret = new TownInfo();
            ret.ID = ID;
            ret.Type = Type;
            ret.Name = Name;
            ret.Description = Description;
            ret.TavernName = TavernName;
            ret.PosInArea = PosInArea;
            return ret;
        }
    }
}
