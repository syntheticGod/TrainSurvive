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
        public TownInfo(XmlNode node)
        {
            ID = int.Parse(node.Attributes["id"].Value);
            Type = ETownType.COMMON + int.Parse(node.Attributes["typeId"].Value);
            Name = node.Attributes["name"].Value;
            Description = node.Attributes["description"].Value;
            int posx = int.Parse(node.Attributes["posx"].Value);
            int posy = int.Parse(node.Attributes["posy"].Value);
            PosInArea = new Vector2Int(posx, posy);
            TavernName = node.Attributes["tavernName"].Value;
        }
        public TownInfo(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt32("ID");
            Type = (ETownType)info.GetValue("Type", typeof(ETownType));
            Name = info.GetString("Name");
            Description = info.GetString("Description");
            TavernName = info.GetString("TavernName");
            int posx = info.GetInt32("PosX");
            int posy = info.GetInt32("PosY");
            PosInArea = new Vector2Int(posx, posy);
            Increasement = info.GetInt32("Increasement");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID, typeof(int));
            info.AddValue("Type", Type, typeof(ETownType));
            info.AddValue("Name", Name, typeof(string));
            info.AddValue("Description", Description, typeof(string));
            info.AddValue("TavernName", TavernName, typeof(string));
            info.AddValue("PosX", PosInArea.x, typeof(int));
            info.AddValue("PosY", PosInArea.y, typeof(int));
            info.AddValue("Increasement", Increasement, typeof(int));
            
        }
        private TownInfo() { }
        private static int Increasement = 1000;
        /// <summary>
        /// 根据城镇名字 和 坐标 随机一个城镇信息
        /// 城镇ID 从1000起头
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <returns></returns>
        public static TownInfo Random(string name, int posx, int posy)
        {
            TownInfo townInfo = new TownInfo();
            townInfo.ID = Increasement++;
            townInfo.Type = ETownType.COMMON;
            townInfo.Name = name;
            townInfo.Description = "";
            townInfo.TavernName = "酒馆";
            townInfo.PosInArea = new Vector2Int(posx, posy);
            return townInfo;
        }

    }
}
