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
        /// 返回下一阶段的 和 满足条件的 句子（直到满足条件的玩家对话为止，或对话结束）
        /// 会跳过不满足条件的
        /// </summary>
        /// <example>
        /// 示例：
        /// 条件 1发生， 条件2不发生
        ///     NPC：1 & 当 条件 1        √
        ///     NPC：2 & 当 条件 2        X，因为条件2没发生
        ///     PLAYER1 & 当 条件 2      X，因为条件2没发生
        ///     NPC4 & 当 条件1            √
        ///     PLAYER2 & 当 条件 1 ----- 指针停在这，因为条件1发生
        ///     NPC5 & 当 条件1
        ///     PLAYER3 & 当 条件 1
        /// 返回
        ///     {NPC1, NPC4} 
        /// </example>
        /// <returns>
        /// 长度为0：无更对对话
        /// </returns>
        public List<ChatSentence> Next()
        {
            List<ChatSentence> ans = new List<ChatSentence>();
            ChatSentence[] sentences = Dialogue.Sentences;
            if (currentIndex >= sentences.Length) return ans;
            //添加所有 {不是玩家的} 和 {满足条件的} 句子
            while (currentIndex < sentences.Length)
            {
                if (sentences[currentIndex].IsPlayer && sentences[currentIndex].IfAllSatisfy())
                    break;
                if (sentences[currentIndex].IfAllSatisfy())
                    ans.Add(sentences[currentIndex]);
                currentIndex++;
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
