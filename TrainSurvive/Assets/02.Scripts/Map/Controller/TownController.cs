/*
 * 描述：城镇控制器，控制城市界面的显示与用户操作
 * 作者：项叶盛
 * 创建时间：2018/11/20 23:14:19
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

namespace WorldMap
{
    public class TownController : MonoBehaviour, OnClickListener, Observer
    {

        private Text townInfoText;
        private Model.Town currentTown;
        private Model.Train train;
        private Model.Team team;
        private WorldForMap world;
        private const int ECHO_CODE_TRAIN = 1;
        private const int ECHO_CODE_TEAM = 2;
        private TavernController tavernController;
        public TownController()
        {
            Debug.Log("TownController Construct");
        }
        public void Init()
        {
            world = WorldForMap.Instance;
            team = Model.Team.Instance;
            train = Model.Train.Instance;
        }
        void Awake()
        {
            Debug.Log("TownController Awake");
            townInfoText = transform.Find("TownInfo").GetComponentInChildren<Text>();
            tavernController = GameObject.Find("/Canvas").transform.Find("TavernViewer").GetComponent<TavernController>();
            tavernController.Init();
            Debug.Assert(townInfoText != null);
            ButtonHandler.Instance.AddListeners(this);
            train.Attach(obs: this, echo: ECHO_CODE_TRAIN);
            team.Attach(obs: this, echo: ECHO_CODE_TEAM);
        }
        void Start()
        {
            Debug.Log("TownController Start");
        }

        void Update()
        {

        }
        public bool TryShowTown(Vector2Int mapPos)
        {
            Model.Town town;
            if(!world.FindTown(mapPos, out town))
            {
                Debug.Log("当前列车位置不在城镇");
                return false;
            }
            ShowTwon(town);
            return true;
        }
        private void ShowTwon(Model.Town town)
        {
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("城镇界面已经显示");
                return;
            }
            gameObject.SetActive(true);
            Debug.Log("Town Show");
            currentTown = town;
            townInfoText.text = town.Name;
        }
        private void Hide()
        {
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);
        }
        public void ObserverUpdate(int state, int echo)
        {
            switch (echo)
            {
                case ECHO_CODE_TRAIN:
                    UpdateByTrain((Model.Train.STATE)state);
                    break;
                case ECHO_CODE_TEAM:
                    UpdateByTeam((Model.Team.STATE)state);
                    break;
            }
        }
        private void UpdateByTrain(Model.Train.STATE state)
        {
            switch (state)
            {
                case Model.Train.STATE.STOP_TOWN:
                    Debug.Log("列车到达 城镇  通知显示城镇");
                    Model.Town town;
                    if(!world.FindTown(train.MapPosTrain, out town))
                    {
                        Debug.LogWarning("列车所在位置不是城镇");
                        return;
                    }
                    ShowTwon(town);
                    break;
                default:
                    Hide();
                    break;
            }
        }
        private void UpdateByTeam(Model.Team.STATE state)
        {
            switch (state)
            {
                case Model.Team.STATE.STOP_TOWN:
                    Debug.Log("探险队到达 城镇  通知显示城镇");
                    Model.Town town;
                    if (!world.FindTown(team.MapPosTeam, out town))
                    {
                        Debug.LogWarning("列车所在位置不是城镇");
                        return;
                    }
                    ShowTwon(town);
                    break;
                default:
                    Hide();
                    break;
            }
        }
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
                    tavernController.ShowTavern(currentTown);
                    break;
                case BUTTON_ID.TOWN_SCHOOL:
                    Debug.Log("学校");
                    break;
                case BUTTON_ID.TOWN_SHOP:
                    Debug.Log("商店");
                    break;
            }
        }
    }
}