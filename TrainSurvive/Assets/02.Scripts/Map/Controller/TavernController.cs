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

namespace WorldMap
{
    public class TavernController : MonoBehaviour, OnClickListener, Observer
    {
        private GameObject[] NPCItems;
        private Model.Train train;
        private Model.Team team;
        private const int MaxCountOfNPC = 5;
        private int selected = -1;
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
                imageButtons[i].ID = i;
                imageButtons[i].onClick = delegate (int id)
                {
                    if(selected != -1)
                        imageButtons[selected].normal();
                    selected = id;
                };
            }
            team = Model.Team.Instance;
            train = Model.Train.Instance;
            train.Attach(obs: this);
            team.Attach(obs: this);
        }
        void Start()
        {
        }
        void Update()
        {
        }
        public void ShowTavern(List<NPC> npcs)
        {
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("酒馆页面已经显示");
                return;
            }
            gameObject.SetActive(true);
            for (int i = 0; i < NPCItems.Length && i < npcs.Count; ++i)
            {
                NPCItems[i].GetComponentInChildren<Text>().text = npcs[i].Info;
            }
            Debug.Log("酒馆：我这里有"+npcs.Count+"人");
        }
        public void Hide()
        {
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TAVERN_RECRUIT:
                    Debug.Log("招募");
                    break;
            }
        }

        public bool IfAccepted(BUTTON_ID id)
        {
            return Utility.IfBetweenBoth((int)BUTTON_ID.TAVERN_NONE, (int)BUTTON_ID.TAVERN_NUM, (int)id);
        }

        public void ObserverUpdate(int state, int echo)
        {
            if(state != (int)Train.STATE.STOP_TOWN || state != (int)Team.STATE.STOP_TOWN)
            {
                Hide();
            }
        }
    }
}