/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/1 13:16:35
 * 版本：v0.7
 */
using System.Xml;
using TTT.Xml;

namespace WorldMap.Model
{
    public abstract class DialogueInfo
    {
        /// <summary>
        /// 对话句子列表
        /// </summary>
        public ChatSentence[] Sentences { get; private set; }
        /// <summary>
        /// 触发该对话的前提条件列表
        /// </summary>
        public Precondition[] Preconditions { get; private set; }
        public DialogueInfo(XmlNode root)
        {
            Preconditions = Precondition.Compile(root.Attributes["precondition"]?.Value);
        }
        protected void InitSentence(string oppositeName, XmlNode root)
        {
            XmlNodeList sentenceNodeList = root.SelectNodes("sentence");
            Sentences = new ChatSentence[sentenceNodeList.Count];
            for (int i = 0; i < Sentences.Length; i++)
                Sentences[i] = new ChatSentence(oppositeName, sentenceNodeList[i]);
        }
        /// <summary>
        /// 判断该对话是否满足所有条件
        /// </summary>
        /// <returns></returns>
        public bool IfAllSatisfy(object context = null)
        {
            foreach (Precondition condition in Preconditions)
            {
                if (!condition.IfSatisfy()) return false;
            }
            return true;
        }
    }
    public class NpcDialogueInfo : DialogueInfo
    {
        /// <summary>
        /// 对话ID
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// 对话人
        /// </summary>
        public int NpcID { get; private set; }
        /// <summary>
        /// 对话是否可以无限触发
        /// </summary>
        public bool IsForever { get; private set; }
        public NpcDialogueInfo(XmlNode root) : base(root)
        {
            ID = int.Parse(root.Attributes["id"].Value);
            NpcID = int.Parse(root.Attributes["npcId"].Value);
            IsForever = root.Attributes["type"]?.Value == "forever";
            string name = NpcInfoLoader.Instance.Find(NpcID).Name;
            InitSentence(name, root);
        }
    }
    public class TavernDialogueInfo : DialogueInfo
    {
        public TavernDialogueInfo(string tavernName, XmlNode root) : base(root)
        {
            InitSentence(tavernName, root);
        }
    }
}