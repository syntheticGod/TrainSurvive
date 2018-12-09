/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 16:06:56
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldMap.Model;

namespace WorldMap.Controller
{
    public class TeamController : BaseController, Observer
    {
        private int levelOfTeam = -2;
        
        private RectTransform teamModeBTs;
        private TrainController trainController;
        private static string[] bottomBtnsStrs = { "进入区域", "上车", "采集", "背包","人物" };
        private Button[] bottomBtns;
        private static string[] teamActionBtnStrs = { "采集", "停止采集" };
        private Text teamActionBtn;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        private float lastSize = 0;
        protected override void OnEnable()
        {
            Team.Instance.Attach(this);
        }
        protected override void CreateModel()
        {
            //Buttons
            Transform canvas = GameObject.Find("/Canvas").transform;
            teamModeBTs = new GameObject("TrainMode").AddComponent<RectTransform>();
            Utility.SetParent(teamModeBTs, canvas);
            Utility.RightBottom(teamModeBTs, new Vector2(1, 0), Vector2.zero, Vector2.zero);
            Vector2 btnPivot = new Vector2(1, 0);
            Vector2 btnSize = new Vector2(120, 120);
            float btnSpace = 6;
            bottomBtns = new Button[bottomBtnsStrs.Length];
            for (int i = 0; i < bottomBtnsStrs.Length; i++)
            {
                bottomBtns[i] = Utility.CreateBtn("Btn" + i, bottomBtnsStrs[i]);
                Utility.SetParent(bottomBtns[i], teamModeBTs);
                Utility.RightBottom(bottomBtns[i], btnPivot, btnSize, new Vector2((btnSize.x + btnSpace) * -i, 0));
                BUTTON_ID btnID = BUTTON_ID.TEAM_NONE + i + 1;
                bottomBtns[i].onClick.AddListener(delegate () { OnClick(btnID); });
            }
            teamActionBtn = bottomBtns[2].transform.Find("Text").GetComponent<Text>();
            
            cameraFocus = Camera.main.GetComponent<ICameraFocus>();
        }
        protected override void Start()
        {
            base.Start();
            Debug.Log("TeamController Start");
            if (world.IfTeamOuting)
            {
                Debug.Log("FOCUS TEAM");
                transform.position = StaticResource.MapPosToWorldPos(Team.Instance.MapPosTeam, levelOfTeam);
                cameraFocus.focusLock(transform);
            }
        }
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
            //FOR TEST
            if(!Mathf.Approximately(lastSize, team.Inventory.GetWeight()))
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
                        TownController townController = ControllerManager.GetWindow<TownController>("TownViewer");
                        townController.SetTown(town);
                        townController.ShowWindow();
                    }
                    else
                    {
                        Debug.Log("该区域不可触发");
                    }
                    break;
                case BUTTON_ID.TEAM_RETRUN:
                    Debug.Log("回车指令");
                    if (!ActiveTeam(false))
                    {
                        Debug.Log("回车指令执行失败");
                        return;
                    }
                    ActiveBTs(false);
                    ControllerManager.FocusController("Train", "Character");
                    break;
                case BUTTON_ID.TEAM_GATHER:
                    if (world.IfTeamGathering)
                    {
                        Debug.Log("停止采集指令");
                        world.StopGather();
                        teamActionBtn.text = teamActionBtnStrs[0];
                    }
                    else
                    {
                        Debug.Log("采集指令");
                        world.DoGather();
                        teamActionBtn.text = teamActionBtnStrs[1];
                    }
                    
                    break;
                case BUTTON_ID.TEAM_PACK:
                    Debug.Log("背包指令");
                    ControllerManager.ShowWindow<PackController>("PackViewer");
                    break;
                case BUTTON_ID.TEAM_CHARACTER:

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
        private bool ActiveTeam(bool active)
        {
            if (gameObject.activeSelf == active)
            {
                Debug.Log("重复操作，无效");
                return false;
            }
            Team team = Team.Instance;
            if (active)
            {
                transform.position = StaticResource.MapPosToWorldPos(team.PosTeam, levelOfTeam);
            }
            else
            {
                if (!team.CanTeamGoBack())
                    return false;
                if (!team.GoBackToTrain())
                    return false;
            }
            gameObject.SetActive(active);
            return true;
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
            ActiveTeam(true);
            ActiveBTs(true);
            cameraFocus.focusLock(transform);
            return true;
        }
        protected override void UnfocusBehaviour()
        {
        }

        public string GetName()
        {
            return "TeamController";
        }

        public void ObserverUpdate(int state, int echo)
        {
            Team.STATE tState = (Team.STATE)state;
            if(tState != Team.STATE.GATHERING)
            {
                Debug.Log("探险队：采集被打断，停止采集");
                world.StopGather();
                teamActionBtn.text = teamActionBtnStrs[0];
            }
        }
    }
}