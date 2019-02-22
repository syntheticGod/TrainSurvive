/*
 * 描述：对话游标，下一句 或 上一句
 * 作者：项叶盛
 * 创建时间：2019/2/22 13:41:59
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldMap.Model
{
    public class DialogueCursor
    {
        #region 变量
        public DialogueInfo Dialogue { get; private set; }
        int currentIndex;
        #endregion

        #region 属性
        /// <summary>
        /// 返回当前句，如果结束则返回null
        /// </summary>
        public ChatSentence CurrentSentence
        {
            get
            {
                if (currentIndex < Dialogue.Sentences.Length)
                    return Dialogue.Sentences[currentIndex];
                else
                    return null;
                    
            }
        }
        #endregion

        #region 构造函数
        public DialogueCursor(DialogueInfo dialogue)
        {
            Dialogue = dialogue;
            currentIndex = 0;
        }
        #endregion

        #region 操作
        /// <summary>
        /// 返回下一阶段的 和 满足条件的 句子（不包括玩家的句子，直到玩家对话为止，或对话结束）
        /// </summary>
        /// <example>
        /// 示例：
        ///     NPC：1
        ///     NPC：2
        ///     PLAYER：3
        ///     NPC：4
        /// 返回
        ///     {NPC1,NPC2}
        /// </example>
        /// <param name="context">上下文</param>
        /// <returns>
        /// 长度为0：无更对对话
        /// </returns>
        public List<ChatSentence> Next()
        {
            List<ChatSentence> ans = new List<ChatSentence>();
            ChatSentence[] sentences = Dialogue.Sentences;
            if (currentIndex >= sentences.Length) return ans;
            //添加所有 {NPC的} 和 {满足条件的} 句子
            while (currentIndex < sentences.Length && sentences[currentIndex].IsNPC)
            {
                if (sentences[currentIndex].IfAllSatisfy())
                    ans.Add(sentences[currentIndex++]);
            }
            return ans;
        }
        public ChatSentence Ignore()
        {
            if (currentIndex == Dialogue.Sentences.Length)
                return null;
            return Dialogue.Sentences[currentIndex++];
        }
        #endregion
    }
}
