/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/29 13:26:22
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using WorldMap.Model;

namespace TTT.Xml
{
    public class NpcInfoLoader : BaseXmlLoader
    {
        static NpcInfoLoader loader;
        public static NpcInfoLoader Instance { get { if (loader == null) loader = new NpcInfoLoader(); return loader; } }
        private NpcInfo[] npcInfos;
        private Dictionary<int, NpcInfo> idToNpcInfo;
        public NpcInfoLoader() : base("NPC")
        { }

        protected override void LoadFromXml(XmlDocument document)
        {
            XmlNode root = document.SelectSingleNode("npcs");
            XmlNodeList npcs = root.SelectNodes("npc");
            npcInfos = new NpcInfo[npcs.Count];
            idToNpcInfo = new Dictionary<int, NpcInfo>();
            for (int i = 0; i < npcs.Count; i++)
            {
                npcInfos[i] = new NpcInfo(npcs[i]);
                idToNpcInfo.Add(npcInfos[i].ID, npcInfos[i]);
            }
        }
        public bool Find(int id, out NpcInfo npcInfo)
        {
            return idToNpcInfo.TryGetValue(id, out npcInfo);
        }
        public NpcInfo Find(int id)
        {
            NpcInfo npcInfo;
            idToNpcInfo.TryGetValue(id, out npcInfo);
            return npcInfo;
        }
        public List<int> AllNpcID()
        {
            List<int> ids = new List<int>();
            foreach(NpcInfo info in npcInfos)
            {
                ids.Add(info.ID);
            }
            return ids;
        }
    }
}
