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
    public class DialogueInfoLoader : BaseXmlLoader
    {
        private static DialogueInfoLoader loader;
        public static DialogueInfoLoader Instance { get { if (loader == null) loader = new DialogueInfoLoader(); return loader; } }
        private DialogueInfo[] dialogues;
        private DialogueInfoLoader() : base("Dialogue")
        { }

        protected override void LoadFromXml(XmlDocument document)
        {
            XmlNodeList dialoguesNodeList = document.SelectSingleNode("dialogues").SelectNodes("dialogue");
            dialogues = new DialogueInfo[dialoguesNodeList.Count];
            for(int i = 0; i < dialogues.Length; i++)
                dialogues[i] = new DialogueInfo(dialoguesNodeList[i]);
        }
        /// <summary>
        /// 返回满足当前状态的指定NPC的所有对话
        /// </summary>
        /// <param name="npcID">NPC的ID</param>
        /// <returns>
        /// 对话列表，以xml中的顺序为序
        /// </returns>
        public List<DialogueInfo> FindSatisfy(int npcID)
        {
            List<DialogueInfo> ans = new List<DialogueInfo>();
            foreach(DialogueInfo dialogue in dialogues)
            {
                if (dialogue.NpcID != npcID) continue;
                bool satisfy = true;
                foreach(DialogueCondition condition in dialogue.preconditions)
                {
                    satisfy = condition.IfSatisfy();
                    if (!satisfy) break;
                }
                if (satisfy)
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
