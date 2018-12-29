/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 16:06:56
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Resource;
using TTT.Utility;
using WorldMap.Model;
using WorldMap.UI;

namespace WorldMap.Controller
{
    public class TeamController : BaseController, Observer
    {
        //探险队icon最多显示个数
        public const int MAX_NUM_OF_TEAM_INVIEW = 5;
        private int levelOfTeam = -2;
        //视图
        private RectTransform teamModeBTs;
        private SpriteRenderer[] teamPersons;
        //数据
        private static string[] bottomBtnsStrs = { "进入区域", "上车", "采集", "背包", "探险队信息" };
        private Button[] bottomBtns;
        private Text teamActionBtn;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        private float lastSize = 0;
        protected override void OnEnable()
        {
            Team.Instance.Attach(this);
        }
        protected override void OnDisable()
        {
            Team.Instance.Detach(this);
        }
        protected override void CreateModel()
        {
            //Buttons
            Transform canvas = GameObject.Find("/Canvas").transform;
            teamModeBTs = new GameObject("TeamMode").AddComponent<RectTransform>();
            ViewTool.SetParent(teamModeBTs, canvas);
            ViewTool.RightBottom(teamModeBTs, new Vector2(1, 0), Vector2.zero, Vector2.zero);
            Vector2 btnPivot = new Vector2(1, 0);
            Vector2 btnSize = new Vector2(120, 120);
            float btnSpace = 6;
            bottomBtns = new Button[bottomBtnsStrs.Length];
            for (int i = 0; i < bottomBtnsStrs.Length; i++)
            {
                bottomBtns[i] = ViewTool.CreateBtn("Btn" + i, bottomBtnsStrs[i]);
                ViewTool.SetParent(bottomBtns[i], teamModeBTs);
                ViewTool.RightBottom(bottomBtns[i], btnPivot, btnSize, new Vector2((btnSize.x + btnSpace) * -i, 0));
                BUTTON_ID btnID = BUTTON_ID.TEAM_NONE + i + 1;
                bottomBtns[i].onClick.AddListener(delegate () { OnClick(btnID); });
            }
            ActiveBTs(false);
            teamActionBtn = bottomBtns[2].transform.Find("Text").GetComponent<Text>();
            //根据探险队数量，放至探险队图标
            teamPersons = new SpriteRenderer[MAX_NUM_OF_TEAM_INVIEW];
            for (int i = 0; i < MAX_NUM_OF_TEAM_INVIEW; i++)
            {
                teamPersons[i] = GOTool.CreateSpriteRenderer("Person" + i, transform, false);
            }
            teamPersons[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            teamPersons[1].transform.localPosition = new Vector3(-0.3f, -0.3f, 0f);
            teamPersons[2].transform.localPosition = new Vector3(0.3f, -0.3f, 0f);
            teamPersons[3].transform.localPosition = new Vector3(-0.3f, 0.3f, 0f);
            teamPersons[4].transform.localPosition = new Vector3(0.3f, 0.3f, 0f);

            cameraFocus = Camera.main.GetComponent<ICameraFocus>();
        }
        //protected override void Start()
        //{
        //    base.Start();
        //    Debug.Log("TeamController Start");
        //    cameraFocus.focusLock(transform);
        //}
        protected override void Update()
        {
            base.Update();
            KeyEventDetecter();
            Vector2 current = StaticResource.WorldPosToMapPos(transform.position);
            Team team = Team.Instance;
            if (team.Run(ref current))
            {
                transform.position = StaticResource.MapPosToWorldPos(current, levelOfTeam);
            }
            //FOR TEST：检测背包重量测试
            if (!Mathf.Approximately(lastSize, team.Inventory.GetWeight()))
            {
                Debug.Log("探险队背包变化：" + team.Inventory.GetWeight());
                lastSize = team.Inventory.GetWeight();
            }
            //---
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TEAM_ENTRY_AREA:
                    Debug.Log("进入区域指令");
                    //TODO：目前只有城镇
                    Model.Town town;
                    if (world.FindTown(Team.Instance.MapPosTeam, out town))
                    {
                        //进入城镇后不能操作小队
                        Team.Instance.IsMovable = false;
                        TownController townController = ControllerManager.Instance.GetWindow<TownController>("TownViewer");
                        townController.SetTown(town);
                        townController.Show(this);
                    }
                    else
                    {
                        Debug.Log("该区域不可触发");
                    }
                    break;
                case BUTTON_ID.TEAM_RETRUN:
                    Debug.Log("回车指令");
                    if (!CanTeamGoBack())
                    {
                        Debug.Log("回车指令执行失败");
                        return;
                    }
                    ControllerManager.Instance.FocusController("Train", "Character");
                    Hide();
                    break;
                case BUTTON_ID.TEAM_GATHER:
                    Debug.Log("采集指令");
                    if (world.DoGather())
                    {
                        //TODO 将地块改为已采集状态
                    }
                    break;
                case BUTTON_ID.TEAM_PACK:
                    Debug.Log("背包指令");
                    ControllerManager.Instance.GetWindow<PackController>("PackViewer").Show();
                    break;
                case BUTTON_ID.TEAM_CHARACTER:
                    Debug.Log("查看小队指令");
                    ControllerManager.Instance.GetWindow<TeamInfoController>("TeamInfoController").Show();
                    break;
            }
        }
        private bool KeyEventDetecter()
        {
            Team team = Team.Instance;
            if (Input.GetKeyUp(KeyCode.W))
            {
                Debug.Log("探险队向上移动指令");
                team.MoveTop();
                return true;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                Debug.Log("探险队向下移动指令");
                team.MoveBottom();
                return true;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                Debug.Log("探险队向左移动指令");
                team.MoveLeft();
                return true;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                Debug.Log("探险队向右移动指令");
                team.MoveRight();
                return true;
            }
            return false;
        }
        public void RefreshView()
        {
            int i;
            for (i = 0; i < teamPersons.Length && i < world.TeamNumber(); i++)
            {
                teamPersons[i].gameObject.SetActive(true);
            }
            for (; i < teamPersons.Length; i++)
            {
                teamPersons[i].gameObject.SetActive(false);
            }
            FaceTowardsBottom();
        }
        private bool CanTeamGoBack()
        {
            Team team = Team.Instance;
            if (!team.CanTeamGoBack())
                return false;
            if (!team.GoBackToTrain())
                return false;
            return true;
        }
        private void FaceTowardsBottom()
        {
            int delta = ESprite.PERSON1_T - ESprite.PERSON1_B + 1;
            ESprite type = ESprite.PERSON1_B;
            for (int i = 0; i < teamPersons.Length && i < world.TeamNumber(); i++)
            {
                teamPersons[i].sprite = StaticResource.GetSprite(type);
                teamPersons[i].transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                type += delta;
            }
        }
        private void FaceTowardsTop()
        {
            int delta = ESprite.PERSON1_T - ESprite.PERSON1_B + 1;
            ESprite type = ESprite.PERSON1_T;
            for (int i = 0; i < teamPersons.Length && i < world.TeamNumber(); i++)
            {
                teamPersons[i].sprite = StaticResource.GetSprite(type);
                teamPersons[i].transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                type += delta;
            }
        }
        private void FaceTowardsLeft()
        {
            int delta = ESprite.PERSON1_T - ESprite.PERSON1_B + 1;
            ESprite type = ESprite.PERSON1_L;
            for (int i = 0; i < teamPersons.Length && i < world.TeamNumber(); i++)
            {
                teamPersons[i].sprite = StaticResource.GetSprite(type);
                teamPersons[i].transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                type += delta;
            }
        }
        private void FaceTowardsRight()
        {
            int delta = ESprite.PERSON1_T - ESprite.PERSON1_B + 1;
            ESprite type = ESprite.PERSON1_L;
            for (int i = 0; i < teamPersons.Length && i < world.TeamNumber(); i++)
            {
                teamPersons[i].sprite = StaticResource.GetSprite(type);
                teamPersons[i].transform.localScale = new Vector3(-3.0f, 3.0f, 3.0f);
                type += delta;
            }
        }
        private bool ActiveBTs(bool active)
        {
            if (teamModeBTs.gameObject.activeInHierarchy == active)
            {
                Debug.Log("重复激活按钮，无效");
                return false;
            }
            teamModeBTs.gameObject.SetActive(active);
            return true;
        }
        protected override bool FocusBehaviour()
        {
            Team.Instance.IsMovable = true;
            return true;
        }

        protected override bool UnfocusBehaviour()
        {
            Team.Instance.IsMovable = false;
            return true;
        }

        protected override bool ShowBehaviour()
        {
            ActiveBTs(true);
            cameraFocus.focusLock(transform);
            transform.position = StaticResource.MapPosToWorldPos(Team.Instance.MapPosTeam, levelOfTeam);
            RefreshView();
            return true;
        }

        protected override bool HideBehaviour()
        {
            ActiveBTs(false);
            if (gameObject.activeSelf == true)
                gameObject.SetActive(false);
            return true;
        }
        public void ObserverUpdate(int state, int echo)
        {
            Team.STATE tState = (Team.STATE)state;
            switch (tState)
            {
                case Team.STATE.MOVING_TOP:
                    FaceTowardsTop();
                    break;
                case Team.STATE.MOVING_RIGHT:
                    FaceTowardsRight();
                    break;
                case Team.STATE.MOVING_BOTTOM:
                    FaceTowardsBottom();
                    break;
                case Team.STATE.MOVING_LEFT:
                    FaceTowardsLeft();
                    break;
            }
        }
    }
}