/*
 * 描述：NPC对话的句子
 * 作者：项叶盛
 * 创建时间：2018/12/7 19:21:41
 * 版本：v0.1
 */
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
        public string Name { get; private set; }
        public string Content { get; private set; }
    }

}