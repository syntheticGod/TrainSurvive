/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/27 23:11:51
 * 版本：v0.7
 */
using System.Xml;
using TTT.Utility;
using UnityEngine;
using WorldMap.Model;

namespace TTT.Xml
{
    public class TownInfoLoader : BaseXmlLoader
    {
        static TownInfoLoader loader;
        public static TownInfoLoader Instance { get { if (loader == null) loader = new TownInfoLoader(); return loader; } }
        protected TownInfoLoader() : base("TownInfo")
        { }
        /// <summary>
        /// 按照城镇的ID 排序，ID为0的城镇信息为随机城镇的基本信息
        /// </summary>
        TownInfo[] specialTowns;
        /// <summary>
        /// 特殊城镇的个数，不包括随机城镇的基本信息
        /// </summary>
        public int SpecailTownsCount { get { return specialTowns.Length - 1; } }
        string[] randomName;
        protected override void LoadFromXml(XmlDocument document)
        {
            XmlNode rootNode = document.SelectSingleNode("Towns");
            XmlNodeList townsNode = rootNode.SelectNodes("Town");
            specialTowns = new TownInfo[townsNode.Count];
            for (int i = 0; i < townsNode.Count; i++)
                specialTowns[i] = new TownInfo(townsNode[i]);

            XmlNode namesNode = rootNode.SelectSingleNode("Names");
            XmlNodeList nameNodeList = namesNode.ChildNodes;
            randomName = new string[nameNodeList.Count];
            for (int i = 0; i < nameNodeList.Count; i++)
                randomName[i] = nameNodeList[i].Attributes["content"].Value;
        }
        /// <summary>
        /// 根据城镇ID查找特殊城镇
        /// </summary>
        /// <param name="id">城镇ID</param>
        /// <returns></returns>
        public TownInfo FindSTownInfoByID(int id)
        {
            return specialTowns[id];
        }
        public string RandomTownName()
        {
            return randomName[MathTool.RandomInt(randomName.Length)];
        }
    }
}