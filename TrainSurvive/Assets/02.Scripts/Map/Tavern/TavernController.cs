/*
 * 描述：酒馆列表控制器
 * 作者：项叶盛
 * 创建时间：2018/11/25 1:14:15
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TTT.Utility;
using TTT.UI.ListView;
using TTT.Controller;

using WorldMap.Model;
using WorldMap.UI;
using Story.Communication;
using TTT.Xml;
using TTT.Team;
using System;
using TTT.UI;

namespace WorldMap.Controller
{
    public class TavernController : WindowsController, Observer
    {
        private TavernNPCListView tavernNPCListView;
        private TownChatListView townChatListView;
        private NullListView nullListView;

        private const int BUTTON_COUNT = 3;
        private Button[] chatBtns;
        public enum BUTTON_ACTION
        {
            NONE = -1,
            CONTINUE,
            OK,
            CANCEL,
            NUM
        }
        /// <summary>
        /// 按钮行为：{ 0 无 | 1 继续 | 2 确定 | 3 取消 }
        /// </summary>
        private BUTTON_ACTION[] actions;

        private TownData currentTown;
        private DialogueCursor dialogueCursor;
        private ChatRoom chatRoom;

        private List<List<ChatSentence>> sentences;
        private List<int> nullData;
        protected override void OnEnable()
        {
            base.OnEnable();
            World.getInstance().Persons.Attach(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            World.getInstance().Persons.Detach(this);
        }
        public void SetTown(TownData town)
        {
            currentTown = town;
            chatRoom = new ChatRoom(town.Npcs);
            sentences = new List<List<ChatSentence>>();
            for (int i = 0; i <= town.Npcs.Count; i++)
                sentences.Add(new List<ChatSentence>());
            //酒馆开场白
            DialogueCursor openTalkCursor = new DialogueCursor(town.Info.Dialogue);
            //判断是否满足城镇
            if (openTalkCursor.Dialogue.IfAllSatisfy())
                sentences[0].AddRange(openTalkCursor.Next());

            //酒馆人物间的对话
            List<KeyValuePair<int, string>> chatSentences = chatRoom.chat();
            foreach (KeyValuePair<int, string> sentence in chatSentences)
                sentences[0].Add(new ChatSentence(sentence.Key, sentence.Value));
        }
        protected override void CreateModel()
        {
            m_titleString = currentTown.Info.TavernName;
            m_windowSizeType = EWindowSizeType.MIDDLE14x12;
            base.CreateModel();
            SetBackground("tavern_bg_01");
            //左上角的头像框
            tavernNPCListView = new GameObject("HeroListView").AddComponent<TavernNPCListView>();
            tavernNPCListView.Context = this;
            ViewTool.SetParent(tavernNPCListView, this);
            ViewTool.Anchor(tavernNPCListView, new Vector2(0.0417F, 0.2F), new Vector2(0.292F, 0.8F));
            //中间聊天窗
            townChatListView = new GameObject("TownChatListView").AddComponent<TownChatListView>();
            townChatListView.GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            townChatListView.GridConstraintCount = 1;
            townChatListView.StartAxis = GridLayoutGroup.Axis.Vertical;
            townChatListView.ScrollDirection = ScrollType.Vertical;
            townChatListView.SetCellSize(new Vector2(-1, 40F));
            townChatListView.IfSelectable = false;
            ViewTool.SetParent(townChatListView, this);
            ViewTool.Anchor(townChatListView, new Vector2(0.375F, 0.35F), new Vector2(0.792F, 0.8F));
            //中间选择按钮
            RectTransform chatBtnsRect = new GameObject("ChatBtns").AddComponent<RectTransform>();
            chatBtns = new Button[BUTTON_COUNT];
            actions = new BUTTON_ACTION[chatBtns.Length];
            ViewTool.SetParent(chatBtnsRect, this);
            ViewTool.Anchor(chatBtnsRect, new Vector2(0.375F, 0.2F), new Vector2(0.792F, 0.35F));
            float deltaY = 1.0f / chatBtns.Length;
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i] = ViewTool.CreateBtn("ChatBtn" + i, "");
                ViewTool.SetParent(chatBtns[i], chatBtnsRect);
                ViewTool.Anchor(chatBtns[i], new Vector2(0, 1 - deltaY * (i + 1)), new Vector2(1, 1 - deltaY * i));
                BUTTON_ID bid = BUTTON_ID.TAVERN_BUTTON1 + i;
                chatBtns[i].onClick.AddListener(delegate () { OnClick(bid); });
            }
            //右边待开发区
            nullListView = new GameObject("NullListView").AddComponent<NullListView>();
            ViewTool.SetParent(nullListView, this);
            ViewTool.Anchor(nullListView, new Vector2(0.875F, 0.2F), new Vector2(0.958F, 0.8F));

        }
        protected override bool PrepareDataBeforeShowWindow()
        {
            nullData = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                nullData.Add(i);
            }
            //Debug.Log("酒馆：我这里有" + currentTown.NPCCnt + "人");
            return true;
        }
        protected override void AfterShowWindow()
        {
            tavernNPCListView.Datas = currentTown.Npcs;
            tavernNPCListView.ClickManually(0);
            townChatListView.Datas = sentences[0];
            nullListView.Datas = nullData;
        }
        public void SetBtnText(string[] texts)
        {
            for (int i = 0; i < chatBtns.Length; i++)
                chatBtns[i].GetComponentInChildren<Text>().text = (texts == null ? "" : texts[i]);
        }
        /// <summary>
        /// 匹配到下一句玩家的句子
        /// </summary>
        /// <returns>
        /// TRUE：成功
        /// FALSE：对话已经结束
        /// </returns>
        public bool ShowNextDialogue()
        {
            if (dialogueCursor == null) return false;
            List<ChatSentence> nextSentences = dialogueCursor.Next();
            if (nextSentences.Count == 0) return false;
            int i;
            for (i = 0; i < nextSentences.Count; i++)
            {
                townChatListView.AddItem(nextSentences[i]);
            }
            ChatSentence playerSentence = dialogueCursor.CurrentSentence;
            string[] texts = new string[3];
            //清除按钮行为
            for (i = 0; i < actions.Length; i++)
                actions[i] = BUTTON_ACTION.NONE;
            if (playerSentence != null && playerSentence.IsPlayer)
            {
                if (playerSentence.Type == 0)
                {
                    texts[0] = playerSentence.Content;
                    actions[0] = BUTTON_ACTION.CONTINUE;
                }
                else
                {
                    texts[0] = playerSentence.OkContent;
                    actions[0] = BUTTON_ACTION.OK;
                    texts[1] = playerSentence.CancelContent;
                    actions[1] = BUTTON_ACTION.CANCEL;
                }
            }
            else
            {
                //如果最后一句不是玩家的句子，则只有一个选择，返回
                texts[0] = "返回大厅";
                actions[0] = BUTTON_ACTION.CANCEL;
            }
            SetBtnText(texts);
            return true;
        }
        public void ShowSelectedNpc(int index)
        {
            //选择大厅
            if (index == 0)
            {
                string[] chatBtnsStrs = { "大家好", "", "" };
                SetBtnText(chatBtnsStrs);
                //显示大厅聊天记录
                townChatListView.Datas = sentences[0];
                dialogueCursor = null;
            }
            else
            {
                SetBtnText(null);
                sentences[index].Clear();
                townChatListView.Datas = sentences[index];
                int npc = tavernNPCListView[index - 1];
                List<NpcDialogueInfo> dialogues = NpcDialogueInfoLoader.Instance.FindSatisfy(npc);
                for (int i = 0; i < dialogues.Count; i++)
                {
                    //不是循环对话 同时 对话未结束
                    if (!dialogues[i].IsForever && !World.getInstance().Dialogues.IfTalked(dialogues[i].ID))
                    {
                        dialogueCursor = new DialogueCursor(dialogues[i]);
                        ShowNextDialogue();
                        break;
                    }
                }
            }

        }
        public void OnClick(BUTTON_ID id)
        {
            //公聊
            if (tavernNPCListView.IfInHill())
            {
                switch (id)
                {
                    case BUTTON_ID.TAVERN_BUTTON1:
                        {
                            ChatSentence chat;
                            List<int> npcs = currentTown.Npcs;
                            if (npcs.Count == 0)
                            {
                                chat = new ChatSentence("回响", "~~~~");
                            }
                            else
                            {
                                int randomIndex = MathTool.RandomInt(npcs.Count);
                                int randomNPC = npcs[randomIndex];
                                chat = new ChatSentence(randomNPC, "你好");
                            }
                            townChatListView.AddItem(chat);
                        }
                        break;
                    default: Debug.Log("大家好"); break;
                }
            }
            //私人对话
            else
            {
                int index = id - BUTTON_ID.TAVERN_BUTTON1;
                //按钮行为：{ 0 无 | 1 继续 | 2 确定 | 3 取消 }
                if(actions[index] == BUTTON_ACTION.CONTINUE || actions[index] == BUTTON_ACTION.OK)
                {
                    ChatSentence playerSentence = dialogueCursor.CurrentSentence;
                    Precondition failureCondition = null;
                    if (playerSentence.IfAllSatisfy(out failureCondition))
                    {
                        playerSentence.DoAllActions();
                        townChatListView.AddItem(playerSentence);
                        dialogueCursor.Ignore(); //跳过玩家的回答句子
                        if (!ShowNextDialogue())
                            ShowSelectedNpc(0); //前往大厅
                    }
                    else
                    {
                        FlowInfo.ShowInfo("失败", failureCondition.FailureMessage());
                    }
                }
                else if(actions[index] == BUTTON_ACTION.CANCEL)
                {
                    //前往大厅
                    ShowSelectedNpc(0);
                }
            }
        }
        public void ObserverUpdate(int state, int echo, object tag = null)
        {

            switch ((PersonSet.EAction)state)
            {
                case PersonSet.EAction.RECRUIT_PERSON:
                    int npcID = (int)tag;
                    int index = tavernNPCListView.IndexOf(npcID);
                    if (index == tavernNPCListView.SelectIndex)
                        ShowSelectedNpc(0);
                    tavernNPCListView.RemoveData(npcID);
                    FlowInfo.ShowInfo("招募新人", NpcInfoLoader.Instance.Find(npcID).Name + "加入小队！");
                    break;
            }
        }
    }
}