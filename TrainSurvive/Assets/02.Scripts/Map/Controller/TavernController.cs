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
using TTT.UI;
using TTT.Controller
    ;
using WorldMap.Model;
using WorldMap.UI;

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
        private Model.Town currentTown;
        public void SetTown(Model.Town town)
        {
            currentTown = town;
        }
        protected override void OnEnable()
        {}
        protected override void OnDisable()
        {}
        protected override void CreateModel()
        {
            m_titleString = "酒馆";
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
        List<Person> heros;
        List<NPCChat> chats;
        List<int> nullData;
        protected override bool PrepareDataBeforeShowWindow()
        {
            heros = new List<Person>();
            for (int i = 0; i < currentTown.NPCs.Count; i++)
            {
                heros.Add(currentTown.NPCs[i].PersonInfo);
            }
            //FOR TEST测试
            chats = new List<NPCChat>();
            for (int i = 0; i < currentTown.NPCs.Count; i++)
            {
                NPCChat chat = new NPCChat();
                chat.Name = currentTown.NPCs[i].Name;
                chat.Content = "我的力量为" + currentTown.NPCs[i].Strength;
                chats.Add(chat);
            }
            nullData = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                nullData.Add(i);
            }
            Debug.Log("酒馆：我这里有" + currentTown.NPCCnt + "人");
            return true;
        }
        protected override void AfterShowWindow()
        {
            tavernNPCListView.Datas = heros;
            tavernNPCListView.ClickManually(0);
            townChatListView.Datas = chats;
            nullListView.Datas = nullData;
        }
        public void OnItemClick(ListViewItem item, Person person)
        {
            selectedIndex = heros.IndexOf(person);
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i].GetComponentInChildren<Text>().text = personalChatBtnsStrs[i];
            }
            Debug.Log("点击了" + currentTown.NPCs[selectedIndex].Name);
        }
        public void OnPersistentClick(ListViewItem item, int index)
        {
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i].GetComponentInChildren<Text>().text = chatBtnsStrs[i];
            }
            selectedIndex = UNSELECTED;
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TAVERN_BUTTON1:
                    NPCChat chat = new NPCChat();
                    if (selectedIndex != UNSELECTED)
                    {
                        //私聊：最近过的怎么样
                        NPC npc = currentTown.NPCs[selectedIndex];
                        chat.Name = npc.Name;
                        chat.Content = "挺好的";
                    }
                    else
                    {
                        //公聊选项1：大家好
                        List<NPC> npcs = currentTown.NPCs;
                        if (npcs.Count == 0)
                        {
                            chat.Name = "回响";
                            chat.Content = chatBtnsStrs[id- BUTTON_ID.TAVERN_NONE-1];
                        }
                        else
                        {
                            int randomIndex = MathTool.RandomInt(npcs.Count);
                            NPC randomNPC = npcs[randomIndex];
                            chat.Name = randomNPC.Name;
                            chat.Content = "你好";
                        }
                    }
                    townChatListView.AddItem(chat);
                    break;
                case BUTTON_ID.TAVERN_BUTTON2:
                    if(selectedIndex != UNSELECTED)
                    {
                        //私聊选项2：请加入我
                        Debug.Log("玩家：招募指令");
                        if(WorldForMap.Instance.PersonCount() >= WorldForMap.Instance.MaxPersonCount())
                        {
                            InfoDialog.Show("人物已满，无法招募更多的人");
                            return;
                        }
                        NPC currentNPC = currentTown.NPCs[selectedIndex];
                        if (!currentTown.RecruitNPC(currentNPC))
                        {
                            Debug.LogError("系统：招募NPC失败");
                            break;
                        }
                        if (WorldForMap.Instance.IfTeamOuting)
                            Team.Instance.CallBackRecruit(currentNPC.PersonInfo);
                        else
                            Train.Instance.CallBackRecruit(currentNPC.PersonInfo);
                        tavernNPCListView.RemoveData(currentNPC.PersonInfo);
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