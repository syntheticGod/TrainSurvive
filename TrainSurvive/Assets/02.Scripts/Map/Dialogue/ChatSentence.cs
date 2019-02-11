/*
 * 描述：NPC对话的句子
 * 作者：项叶盛
 * 创建时间：2018/12/7 19:21:41
 * 版本：v0.1
 */
using System.Xml;
using TTT.Xml;
using UnityEngine;

namespace WorldMap.Model
{
    public class ChatSentence
    {
        public ChatSentence(int speakerID, string content)
        {
            Name = NpcInfoLoader.Instance.Find(speakerID).Name;
            Content = ReplaceIDToName(content);
        }
        /// <summary>
        /// 讲话者不存在，需要手动输入一个。如：“回响：大家好！”
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public ChatSentence(string name, string content)
        {
            Name = name;
            Content = ReplaceIDToName(content);
        }
        public string ReplaceIDToName(string content)
        {
            //替换@+id 为 @+NPC名字
            int atIndex = content.IndexOf('@');
            if (atIndex != -1)
            {
                string at = content;
                //去掉@前面的字符串
                at = at.Remove(0, atIndex);
                int endIndex = 1;
                while (endIndex < at.Length && at[endIndex] >= '0' && at[endIndex] <= '9')
                    endIndex++;
                at = at.Remove(endIndex, at.Length - endIndex);
                string id = at.Remove(0, 1);
                try
                {
                    string name = NpcInfoLoader.Instance.Find(int.Parse(id)).Name;
                    content = content.Replace(at, "@" + name + " ");
                }
                catch (System.FormatException e)
                {
                    Debug.LogError(id + "  " + e.ToString());
                }
            }
            return content;
        }
        /// <summary>
        /// 对话人的名字
        /// 因为对话人的名字 可能 为回响、空气等，所以需要一个变量。而不用NPC的ID。
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 讲话者类型。0：npc；1：player；
        /// </summary>
        public int SpeakerType { get; private set; }
        public bool IsNPC { get { return SpeakerType == 0; } }
        public bool IsPlayer { get { return SpeakerType == 1; } }
        /// <summary>
        /// 句子类型：0：普通句子；1：选择句（目前只有玩家才有选择句）
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        ///普通句子的内容
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// 确定选择内容
        /// </summary>
        public string OkContent { get; private set; }
        /// <summary>
        /// 取消选择内容
        /// </summary>
        public string CancelContent { get; private set; }
        /// <summary>
        /// 语句结束后的指令
        /// </summary>
        public SentenceAction[] Actions { get; private set; }
        public ChatSentence(XmlNode node, int NpcID)
        {
            Name = NpcInfoLoader.Instance.Find(NpcID).Name;
            if (node.Attributes["speakerType"].Value.Equals("player"))
                SpeakerType = 1;
            Type = int.Parse(node.Attributes["type"].Value);
            switch (Type)
            {
                case 0://普通句子
                    Content = node.Attributes["content"].Value;
                    break;
                case 1://选择性句子
                    OkContent = node.Attributes["ok"].Value;
                    CancelContent = node.Attributes["cancel"].Value;
                    break;
            }
            if (node.Attributes["action"] != null)
            {
                string[] actions = node.Attributes["action"].Value.Split(';');
                Actions = new SentenceAction[actions.Length];
                for (int i = 0; i < Actions.Length; i++)
                {
                    string[] words = actions[i].Split(' ');
                    switch (words[1])
                    {
                        case "item": Actions[i] = new ItemAction(words); break;
                        case "battle": Actions[i] = new BattleAction(words); break;
                        case "task": Actions[i] = new TaskAction(words); break;
                        case "npc": Actions[i] = new NpcAction(words); break;
                        case "money": Actions[i] = new MoneyAction(words); break;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DoAllActions()
        {
            if (Actions == null) return true;
            for (int i = 0; i < Actions.Length; i++)
            {
                if (!Actions[i].DoAction())
                {
                    //撤销之前做过的行为
                    while (i >= 0) Actions[i].UnDoAction();
                    return false;
                }
            }
            return true;
        }
    }
}