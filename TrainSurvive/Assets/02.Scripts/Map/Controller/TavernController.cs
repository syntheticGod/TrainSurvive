/*
 * 描述：酒馆列表控制器
 * 作者：项叶盛
 * 创建时间：2018/11/25 1:14:15
 * 版本：v0.1
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using WorldMap.Model;
using WorldMap.UI;

namespace WorldMap.Controller
{
    public class TavernController : WindowsController, Observer
    {
        private const int UNSELECTED = -1;
        private static string[] chatBtnsStrs = { "大家好", "大家好", "大家好" };
        private static string[] personalChatBtnsStrs = { "今日发生了什么？", "关于你", "招募你" };
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
        {
            Train.Instance.Attach(obs: this);
            Team.Instance.Attach(obs: this);
        }
        protected override void OnDisable()
        {
            Train.Instance.Detach(obs: this);
            Team.Instance.Detach(obs: this);
        }
        protected override void CreateModel()
        {
            m_titleString = "酒馆";
            m_windowSizeType = WindowSizeType.MIDDLE14x12;
            base.CreateModel();
            SetBackground("tavern_bg_01");
            //左上角的头像框
            tavernNPCListView = new GameObject("HeroListView").AddComponent<TavernNPCListView>();
            tavernNPCListView.StartAxis = GridLayoutGroup.Axis.Horizontal;
            tavernNPCListView.ScrollDirection = ScrollType.Vertical;
            tavernNPCListView.onItemClick = OnItemClick;
            tavernNPCListView.onPersistentItemClick = OnPersistentClick;
            Utility.SetParent(tavernNPCListView, this);
            Utility.Anchor(tavernNPCListView, new Vector2(0.0417F, 0.2F), new Vector2(0.292F, 0.8F));
            //中间聊天窗
            townChatListView = new GameObject("TownChatListView").AddComponent<TownChatListView>();
            townChatListView.StartAxis = GridLayoutGroup.Axis.Vertical;
            townChatListView.ScrollDirection = ScrollType.Vertical;
            townChatListView.SetCellSize(new Vector2(-1, 40F));
            townChatListView.m_selectable = false;
            Utility.SetParent(townChatListView, this);
            Utility.Anchor(townChatListView, new Vector2(0.375F, 0.35F), new Vector2(0.792F, 0.8F));
            //中间选择按钮
            RectTransform chatBtnsRect = new GameObject("ChatBtns").AddComponent<RectTransform>();
            chatBtns = new Button[chatBtnsStrs.Length];
            Utility.SetParent(chatBtnsRect, this);
            Utility.Anchor(chatBtnsRect, new Vector2(0.375F, 0.2F), new Vector2(0.792F, 0.35F));
            for (int i = 0; i < chatBtns.Length; i++)
            {
                chatBtns[i] = Utility.CreateBtn("ChatBtn" + i, chatBtnsStrs[i]);
                Utility.SetParent(chatBtns[i], chatBtnsRect);
                Utility.Anchor(chatBtns[i], new Vector2(0, (float)i / chatBtns.Length), new Vector2(1, (float)(i + 1) / chatBtns.Length));
                BUTTON_ID bid = BUTTON_ID.TAVERN_NONE + i + 1;
                chatBtns[i].onClick.AddListener(delegate () { OnClick(bid); });
            }
            //右边待开发区
            nullListView = new GameObject("NullListView").AddComponent<NullListView>();
            Utility.SetParent(nullListView, this);
            Utility.Anchor(nullListView, new Vector2(0.875F, 0.2F), new Vector2(0.958F, 0.8F));
            
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
        protected override bool FocusBehaviour()
        {
            return true;
        }
        protected override void UnfocusBehaviour()
        { }
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
                    if(selectedIndex != UNSELECTED)
                    {
                        Debug.Log("玩家：招募指令");
                        if (selectedIndex == UNSELECTED)
                        {
                            Debug.Log("系统：未选择任何NPC");
                            break;
                        }
                        NPC currentNPC = currentTown.NPCs[selectedIndex];
                        if (!currentTown.RecruitNPC(currentNPC))
                        {
                            Debug.Log("系统：招募NPC失败");
                            break;
                        }
                        if (world.IfTeamOuting)
                            Team.Instance.CallBackRecruit(currentNPC.PersonInfo);
                        else
                            Train.Instance.CallBackRecruit(currentNPC.PersonInfo);
                        tavernNPCListView.RemoveItem(currentNPC.PersonInfo);
                        tavernNPCListView.ClickManually(0);
                        break;
                    }
                    else
                    {
                        Debug.Log("大家好");
                    }
                    break;
                case BUTTON_ID.TAVERN_BUTTON2:
                    if(selectedIndex != UNSELECTED)
                    {

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
        public void ObserverUpdate(int state, int echo)
        {
            if (state != (int)Train.STATE.STOP_TOWN || state != (int)Team.STATE.STOP_TOWN)
            {
                HideWindow();
            }
        }

        public string GetName()
        {
            return "TavernController";
        }

    }
}