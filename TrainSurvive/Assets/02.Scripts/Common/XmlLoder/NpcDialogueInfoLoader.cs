/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/1 13:11:43
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Xml;
using WorldMap.Model;
using System.Collections.Generic;

namespace TTT.Xml
{
    public class NpcDialogueInfoLoader : BaseXmlLoader
    {
        private static NpcDialogueInfoLoader loader;
        public static NpcDialogueInfoLoader Instance { get { if (loader == null) loader = new NpcDialogueInfoLoader(); return loader; } }
        private NpcDialogueInfo[] dialogues;
        private NpcDialogueInfoLoader() : base("Dialogue")
        { }

        protected override void LoadFromXml(XmlDocument document)
        {
            XmlNodeList dialoguesNodeList = document.SelectSingleNode("dialogues").SelectNodes("dialogue");
            dialogues = new NpcDialogueInfo[dialoguesNodeList.Count];
            for(int i = 0; i < dialogues.Length; i++)
                dialogues[i] = new NpcDialogueInfo(dialoguesNodeList[i]);
        }
        /// <summary>
        /// 返回满足当前状态的指定NPC的所有对话
        /// </summary>
        /// <param name="npcID">NPC的ID</param>
        /// <returns>
        /// 对话列表，以xml中的顺序为序
        /// </returns>
        public List<NpcDialogueInfo> FindSatisfy(int npcID)
        {
            List<NpcDialogueInfo> ans = new List<NpcDialogueInfo>();
            foreach(NpcDialogueInfo dialogue in dialogues)
            {
                if (dialogue.NpcID != npcID) continue;
                if(dialogue.IfAllSatisfy())
                    ans.Add(dialogue);
            }
            return ans;
        }
        public int DialoguesCount()
        {
            return dialogues.Length;
        }
    }
}
