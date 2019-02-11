/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/1 13:16:35
 * 版本：v0.7
 */
using System.Xml;

namespace WorldMap.Model
{
    public class DialogueInfo
    {
        /// <summary>
        /// 对话句子列表
        /// </summary>
        public ChatSentence[] Sentences { get; private set; }
        /// <summary>
        /// 对话ID
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// 对话人
        /// </summary>
        public int NpcID { get; private set; }
        /// <summary>
        /// 触发该对话的前提条件列表
        /// </summary>
        public DialogueCondition[] preconditions { get; private set; }
        public DialogueInfo(XmlNode root)
        {
            ID = int.Parse(root.Attributes["id"].Value);
            NpcID = int.Parse(root.Attributes["npcId"].Value);
            string condition = root.Attributes["precondition"].Value;
            if(condition.Length != 0)
            {
                string[] precon = condition.Split(';');
                preconditions = new DialogueCondition[precon.Length];
                for (int i = 0; i < precon.Length; i++)
                {
                    string[] words = precon[i].Split(' ');
                    switch (words[1])
                    {
                        case "task": preconditions[i] = new TaskCondition(words); break;
                        case "other": preconditions[i] = new OtherCondition(words); break;
                    }
                }
            }
            else
            {
                preconditions = new DialogueCondition[0];
            }
            XmlNodeList sentenceNodeList = root.SelectNodes("sentence");
            Sentences = new ChatSentence[sentenceNodeList.Count];
            for (int i = 0; i < Sentences.Length; i++)
                Sentences[i] = new ChatSentence(sentenceNodeList[i], NpcID);
        }
        /// <summary>
        /// 判断该对话是否满足所有条件
        /// </summary>
        /// <returns></returns>
        public bool IfAllSatisfy()
        {
            foreach (DialogueCondition condition in preconditions)
            {
                if (!condition.IfSatisfy()) return false;
            }
            return true;
        }
    }
}