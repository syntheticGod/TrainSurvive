/*
 * 描述：NPC对话的句子
 * 作者：项叶盛
 * 创建时间：2018/12/7 19:21:41
 * 版本：v0.1
 */
namespace WorldMap.Model
{
    public class ChatSentence
    {
        public ChatSentence(NpcData speaker, string content)
        {
            Name = speaker.Name;
            Content = content;
        }
        /// <summary>
        /// 讲话者不存在，需要手动输入一个。如：“回响：大家好！”
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public ChatSentence(string name, string content)
        {
            Name = name;
            Content = content;
        }
        public string Name { get; private set; }
        public string Content { get; private set; }
    }

}