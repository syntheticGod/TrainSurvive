/*
 * 描述：城镇控制器，控制城市界面的显示与用户操作
 * 作者：项叶盛
 * 创建时间：2018/11/20 23:14:19
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using WorldMap.Model;

namespace WorldMap
{
    public class TownController : MonoBehaviour, OnClickListener, Observer
    {

        private Text townInfoText;
        private Town currentTown;
        private Train train;
        private DataSerialization ds;
        public bool IfAccepted(BUTTON_ID id)
        {
            return Utility.Between((int)BUTTON_ID.TOWN_NONE, (int)BUTTON_ID.TOWN_NUM, (int)id);
        }

        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TOWN_TAVERN:
                    Debug.Log("酒馆");
                    break;
                case BUTTON_ID.TOWN_SCHOOL:
                    Debug.Log("学校");
                    break;
                case BUTTON_ID.TOWN_SHOP:
                    Debug.Log("商店");
                    break;
            }
        }

        void Awake()
        {
            Debug.Log("TownController Awake");
            townInfoText = transform.Find("TownInfo").GetComponentInChildren<Text>();
            Debug.Assert(townInfoText != null);
            ButtonHandler.Instance.AddListeners(this);
            train = Train.Instance;
            train.Attach(this);
            ds = DataSerialization.Instance;
        }
        void Start()
        {
        }

        void Update()
        {

        }
        public bool TryShowTown()
        {
            if (!Map.GetIntanstance().IfTown(Train.Instance.MapPosTrain))
            {
                Debug.Log("当前列车位置不在城镇");
                return false;
            }
            ShowTwon();
            return true;
        }
        private void ShowTwon()
        {
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("城镇界面已经显示");
                return;
            }
            gameObject.SetActive(true);
            Debug.Log("Town Show");
            Vector2Int currentMapPos = train.MapPosTrain;
            Model.Town town;
            if (!ds.Find(currentMapPos, out town))
                Debug.Log("未找到坐标为"+currentMapPos+"的城镇");
            townInfoText.text = town.Name;
        }
        private void Hide()
        {
            if (!gameObject.activeInHierarchy)
                return;
            gameObject.SetActive(false);
        }
        public void ObserverUpdate(int state)
        {
            switch ((Train.STATE)state)
            {
                case Train.STATE.ARRIVED:
                    Debug.Log("列车到达 城镇  通知显示城镇");
                    ShowTwon();
                    break;
                default:
                    Hide();
                    break;
            }
        }
    }
}