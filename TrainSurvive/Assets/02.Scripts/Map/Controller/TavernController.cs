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
        private Text[] NPCTexts;
        private Model.Train train;
        private Model.Team team;
        private const int MaxCountOfNPC = 3;
        private void Awake()
        {
            NPCTexts = new Text[MaxCountOfNPC];
            Transform NPCsInfo = gameObject.transform.Find("NPCsInfo");
            for (int i = 0; i < MaxCountOfNPC; ++i)
                NPCTexts[i] = NPCsInfo.Find("NPCText" + (i + 1)).GetComponent<Text>();
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
            for (int i = 0; i < NPCTexts.Length && i < npcs.Count; ++i)
            {
                NPCTexts[i].text = npcs[i].Info;
            }
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