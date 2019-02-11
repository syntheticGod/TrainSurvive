/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/11 22:29:19
 * 版本：v0.7
 */
using UnityEngine;
using System;
using TTT.Xml;

namespace WorldMap.Model
{
    [Serializable]
    public class DialogueDataSet
    {
        public enum EDialogueState
        {
            NONE = -1,
            UNTALK,//未交谈过
            TALKING,//已交谈过，但是中途退出过。
            TALKED,//交谈完毕
            NUM
        }
        [SerializeField]
        private EDialogueState[] State;
        /// <summary>
        /// 建档时的初始化函数
        /// </summary>
        public void InitOnce()
        {
            State = new EDialogueState[DialogueInfoLoader.Instance.DialoguesCount()];
        }
        /// <summary>
        /// 根据对话ID获得对话的状态
        /// </summary>
        /// <param name="dialogueID">对话ID</param>
        /// <returns></returns>
        public EDialogueState GetState(int dialogueID)
        {
#if DEBUG
            if(dialogueID >= State.Length)
                Debug.LogError("不存在该ID的对话：" + dialogueID);
#endif
            return State[dialogueID];
        }
        /// <summary>
        /// 根据对话ID判断，对话的状态是否为已交谈
        /// </summary>
        /// <param name="ids">对话ID数组</param>
        /// <returns>
        /// TRUE：都交谈完毕
        /// FLASE：其中一个的状态不为交谈完毕
        /// </returns>
        public bool IfTalked(int[] ids)
        {
            foreach(int id in ids)
            {
                if (GetState(id) != EDialogueState.TALKED)
                    return false;
            }
            return true;
        }
    }
}