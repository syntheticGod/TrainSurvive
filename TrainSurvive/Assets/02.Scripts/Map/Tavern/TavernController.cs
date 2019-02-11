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

namespace WorldMap.Controller
{
    public class TavernController : WindowsController
    {
        private const int UNSELECTED = -1;
        private static string[] chatBtnsStrs = { "大家好", "", "" };
        private int selectedIndex = UNSELECTED;

        private TavernNPCListView tavernNPCListView;
        private TownChatListView townChatListView;
        private NullListView nullListView;
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
        private DialogueInfo currentDialogue;
        private int sentenceIndex;
        private ChatRoom chatRoom;

        private List<ChatSentence>[] sentences;
        private List<int> nullData;

        public void SetTown(TownData town)
        {
            currentTown = town;
            chatRoom = new ChatRoom(town.Npcs);
            sentences = new List<ChatSentence>[town.Npcs.Count + 1];
            for (int i = 0; i < sentences.Length; i++)
                sentences[i] = new List<ChatSentence>();
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
            tavernNPCListView.GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            tavernNPCListView.GridConstraintCount = 2;
            tavernNPCListView.StartAxis = GridLayoutGroup.Axis.Horizontal;
            tavernNPCListView.ScrollDirection = ScrollType.Vertical;
            tavernNPCListView.onItemClick = OnItemClick;
            tavernNPCListView.onPersistentItemClick = OnPersistentClick;
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
            chatBtns = new Button[chatBtnsStrs.Length];
            actions = new BUTTON_ACTION[chatBtns.Length];
            ViewTool.SetParent(chatBtnsRect, this);
            ViewTool.Anchor(chatBtnsRect, new Vector2(0.375F, 0.2F), new Vector2(0.792F, 0.35F));
            float deltaY = 1.0f / chatBtns.Length;
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i] = ViewTool.CreateBtn("ChatBtn" + i, chatBtnsStrs[i]);
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
        public void SetDialogue(DialogueInfo dialogue)
        {
            currentDialogue = dialogue;
            sentenceIndex = 0;
            if (dialogue != null)
                NextSentence();
        }
        public void SetBtnText(string[] texts)
        {
            for (int i = 0; i < chatBtns.Length; i++)
                chatBtns[i].GetComponentInChildren<Text>().text = (texts == null ? "" : texts[i]);
        }
        public void ClearBtnActions()
        {
            for (int i = 0; i < actions.Length; i++)
                actions[i] = BUTTON_ACTION.NONE;
        }
        public bool NextSentence()
        {
            if (currentDialogue == null) return false;
            ChatSentence[] sentences = currentDialogue.Sentences;
            if (sentenceIndex >= sentences.Length) return false;
            //找到player的句子
            for (; sentenceIndex < sentences.Length && sentences[sentenceIndex].IsNPC; sentenceIndex++)
                townChatListView.AddItem(sentences[sentenceIndex]);
#if DEBUG
            //最后一句为NPC的句子
            if (sentenceIndex == sentences.Length)
            {
                Debug.LogError("最后一句必须是玩家的句子，对话xml配置错误。");
                return false;
            }
#endif
            ClearBtnActions();
            ChatSentence playerSentence = sentences[sentenceIndex];
            string[] texts = new string[3];
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
            SetBtnText(texts);
            return true;
        }
        public void OnItemClick(ListViewItem item, int npc)
        {
            selectedIndex = currentTown.Npcs.IndexOf(npc);
            //Debug.Log("点击了" + currentTown.NPCs[selectedIndex].Name);
            SetBtnText(null);
            townChatListView.Datas = sentences[selectedIndex + 1];
            List<DialogueInfo> dialogues = DialogueInfoLoader.Instance.FindSatisfy(npc);
            if (dialogues.Count != 0)
                SetDialogue(dialogues[0]);
        }
        public void OnPersistentClick(ListViewItem item, int index)
        {
            GoToHill();
        }
        /// <summary>
        /// 去往大厅
        /// </summary>
        public void GoToHill()
        {
            SetBtnText(chatBtnsStrs);
            selectedIndex = UNSELECTED;
            //显示大厅聊天记录
            townChatListView.Datas = sentences[0];
            SetDialogue(null);
        }
        public void OnClick(BUTTON_ID id)
        {
            //公聊
            if (selectedIndex == UNSELECTED)
            {
                switch (id)
                {
                    case BUTTON_ID.TAVERN_BUTTON1:
                        {
                            ChatSentence chat;
                            List<int> npcs = currentTown.Npcs;
                            if (npcs.Count == 0)
                            {
                                chat = new ChatSentence("回响", chatBtnsStrs[id - BUTTON_ID.TAVERN_NONE - 1]);
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
                bool gotoHill = false;
                //按钮行为：{ 0 无 | 1 继续 | 2 确定 | 3 取消 }
                switch (actions[index])
                {
                    case BUTTON_ACTION.CONTINUE:
                        currentDialogue.Sentences[sentenceIndex].DoAllActions();
                        //跳过玩家的回答句子
                        sentenceIndex++;
                        if (!NextSentence())
                            gotoHill = true;
                        break;
                    case BUTTON_ACTION.OK:
                        currentDialogue.Sentences[sentenceIndex].DoAllActions();
                        gotoHill = true;
                        break;
                    case BUTTON_ACTION.CANCEL: gotoHill = true; break;
                    default: break;
                }
                if (gotoHill) GoToHill();
            }
        }
        public void RecruitNpc(int selectedIndex)
        {
            //私聊选项2：请加入我
            Debug.Log("玩家：招募指令");
            if (WorldForMap.Instance.PersonCount() >= WorldForMap.Instance.MaxPersonCount())
            {
                InfoDialog.Show("人物已满，无法招募更多的人");
                return;
            }
            int currentNPC = currentTown.Npcs[selectedIndex];
            if (!currentTown.RecruitNPC(currentNPC))
            {
                Debug.LogError("系统：招募NPC失败");
                return;
            }
            //if (WorldForMap.Instance.IfTeamOuting)
            //    Team.Instance.CallBackRecruit(currentNPC.PersonInfo);
            //else
            //    Train.Instance.CallBackRecruit(currentNPC.PersonInfo);
            tavernNPCListView.RemoveData(currentNPC);
            tavernNPCListView.ClickManually(0);
        }
    }
}