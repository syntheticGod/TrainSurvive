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

namespace WorldMap.Controller
{
    public class TavernController : WindowsController
    {
        private const int UNSELECTED = -1;
        private static string[] chatBtnsStrs = { "", "", "大家好" };
        private static string[] personalChatBtnsStrs = { "", "请加入我", "最近过的怎么样" };
        private int selectedIndex = UNSELECTED;

        private TavernNPCListView tavernNPCListView;
        private TownChatListView townChatListView;
        private NullListView nullListView;
        private Button[] chatBtns;
        private TownData currentTown;
        private ChatRoom chatRoom;
        private List<KeyValuePair<int, string>> chatSentences;
        private List<ChatSentence>[] sentences;
        List<int> nullData;
        public void SetTown(TownData town)
        {
            currentTown = town;
            chatRoom = new ChatRoom(town.Npcs);
            chatSentences = chatRoom.chat();
            sentences = new List<ChatSentence>[town.Npcs.Count + 1];
            for (int i = 0; i < sentences.Length; i++)
                sentences[i] = new List<ChatSentence>();
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
            ViewTool.SetParent(chatBtnsRect, this);
            ViewTool.Anchor(chatBtnsRect, new Vector2(0.375F, 0.2F), new Vector2(0.792F, 0.35F));
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i] = ViewTool.CreateBtn("ChatBtn" + i, chatBtnsStrs[i]);
                ViewTool.SetParent(chatBtns[i], chatBtnsRect);
                ViewTool.Anchor(chatBtns[i], new Vector2(0, (float)i / chatBtns.Length), new Vector2(1, (float)(i + 1) / chatBtns.Length));
                BUTTON_ID bid = BUTTON_ID.TAVERN_NONE + i + 1;
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
        public void OnItemClick(ListViewItem item, int npc)
        {
            selectedIndex = currentTown.Npcs.IndexOf(npc);
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i].GetComponentInChildren<Text>().text = personalChatBtnsStrs[i];
            }
            //Debug.Log("点击了" + currentTown.NPCs[selectedIndex].Name);
            townChatListView.Datas = sentences[selectedIndex + 1];
        }
        public void OnPersistentClick(ListViewItem item, int index)
        {
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i].GetComponentInChildren<Text>().text = chatBtnsStrs[i];
            }
            selectedIndex = UNSELECTED;
            //显示大厅聊天记录
            townChatListView.Datas = sentences[0];
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TAVERN_BUTTON1:
                    ChatSentence chat;
                    if (selectedIndex != UNSELECTED)
                    {
                        //私聊：最近过的怎么样
                        int npc = currentTown.Npcs[selectedIndex];
                        chat = new ChatSentence(npc, "挺好的");
                    }
                    else
                    {
                        //公聊选项1：大家好
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
                    }
                    townChatListView.AddItem(chat);
                    break;
                case BUTTON_ID.TAVERN_BUTTON2:
                    if (selectedIndex != UNSELECTED)
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
                            break;
                        }
                        //if (WorldForMap.Instance.IfTeamOuting)
                        //    Team.Instance.CallBackRecruit(currentNPC.PersonInfo);
                        //else
                        //    Train.Instance.CallBackRecruit(currentNPC.PersonInfo);
                        tavernNPCListView.RemoveData(currentNPC);
                        tavernNPCListView.ClickManually(0);
                        break;
                    }
                    else
                    {
                        Debug.Log("大家好");
                    }
                    break;
                case BUTTON_ID.TAVERN_BUTTON3:
                    break;
            }
        }
    }
}