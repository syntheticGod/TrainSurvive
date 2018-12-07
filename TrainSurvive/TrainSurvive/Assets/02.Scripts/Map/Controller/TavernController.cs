/*
 * 描述：酒馆列表控制器
 * 作者：项叶盛
 * 创建时间：2018/11/25 1:14:15
 * 版本：v0.1
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldMap
{
    public class TavernController : MonoBehaviour, OnClickListener, Observer
    {
        private const int UNSELECTED = -1;
        private GameObject[] NPCItems;
        private Model.Train train;
        private Model.Team team;
        private Model.Town town;
        private WorldForMap world;
        private const int MaxCountOfNPC = 5;
        private int selectedIndex = UNSELECTED;
        private Model.NPC selectedNPC = null;
        private ImageButton[] imageButtons;
        private void Awake()
        {
            NPCItems = new GameObject[MaxCountOfNPC];
            imageButtons = new ImageButton[MaxCountOfNPC];
            Transform NPCsInfo = gameObject.transform.Find("NPCsInfo");
            for (int i = 0; i < MaxCountOfNPC; ++i)
            {
                NPCItems[i] = NPCsInfo.Find("InfoItem" + (i + 1)).gameObject;
                imageButtons[i] = NPCItems[i].GetComponentInChildren<ImageButton>();
                imageButtons[i].onClick = delegate (int id, object tag)
                {
                    if (selectedIndex != UNSELECTED)
                        imageButtons[selectedIndex].normal();
                    selectedIndex = id;
                    selectedNPC = tag as Model.NPC;
                };
            }
        }
        public void Init()
        {
            team = Model.Team.Instance;
            train = Model.Train.Instance;
            world = WorldForMap.Instance;
            train.Attach(obs: this);
            team.Attach(obs: this);
            ButtonHandler.Instance.AddListeners(this);
        }
        void Start()
        {
        }
        void Update()
        {
        }
        public void ShowTavern(Model.Town town)
        {
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("酒馆页面已经显示");
                return;
            }
            gameObject.SetActive(true);
            //TODO：待优化，list点击列表
            for (int i = 0; i < NPCItems.Length; ++i)
            {
                imageButtons[i].Clean();
            }
            for (int i = 0; i < town.NPCCnt; ++i)
            {
                imageButtons[i].ShowText(town.NPCs[i].Info);
                imageButtons[i].Tag = town.NPCs[i];
                imageButtons[i].ID = i;
            }
            this.town = town;
            Debug.Log("酒馆：我这里有" + town.NPCCnt + "人");
        }
        private void Hide()
        {
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TAVERN_RECRUIT:
                    Debug.Log("玩家：招募指令");
                    if (selectedIndex == UNSELECTED)
                    {
                        Debug.Log("系统：未选择任何NPC");
                        break;
                    }
                    if (!town.RecruitNPC(selectedNPC))
                    {
                        Debug.Log("系统：招募NPC失败");
                        break;
                    }
                    if (world.IfTeamOuting)
                        team.CallBackRecruit(selectedNPC.PersonInfo);
                    else
                        train.CallBackRecruit(selectedNPC.PersonInfo);
                    imageButtons[selectedIndex].Clean();
                    break;
                case BUTTON_ID.TAVERN_CANCEL:
                    Hide();
                    break;
            }
        }

        public bool IfAccepted(BUTTON_ID id)
        {
            return Utility.IfBetweenBoth((int)BUTTON_ID.TAVERN_NONE, (int)BUTTON_ID.TAVERN_NUM, (int)id);
        }

        public void ObserverUpdate(int state, int echo)
        {
            if (state != (int)Model.Train.STATE.STOP_TOWN || state != (int)Model.Team.STATE.STOP_TOWN)
            {
                Hide();
            }
        }
    }
}