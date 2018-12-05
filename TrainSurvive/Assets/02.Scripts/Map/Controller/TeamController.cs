/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 16:06:56
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WorldMap.Model;

namespace WorldMap.Controller
{
    public class TeamController : BaseController, OnClickListener
    {
        private int levelOfTeam = -2;
        
        private GameObject teamModeBTs;
        private TrainController trainController;
        //主摄像机
        private Camera mainCamera;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        private float lastSize = 0;
        public void Init()
        {
            ButtonHandler.Instance.AddListeners(this);
        }
        protected override void CreateModel()
        {
            Transform canvas = GameObject.Find("/Canvas").transform;
            teamModeBTs = canvas.Find("TeamMode").gameObject;
            mainCamera = Camera.main;
            Debug.Assert(null != mainCamera, "需要将主摄像机的Tag改为MainCamera");
            cameraFocus = mainCamera.GetComponent<ICameraFocus>();
        }
        protected override void Start()
        {
            base.Start();
            Debug.Log("TeamController Start");
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
            if(!Mathf.Approximately(lastSize, team.Inventory.currSize))
            {
                Debug.Log("探险队背包变化：" + team.Inventory.currSize);
                lastSize = team.Inventory.currSize;
            }
        }
        public bool IfAccepted(BUTTON_ID id)
        {
            return Utility.Between((int)BUTTON_ID.TEAM_NONE, (int)BUTTON_ID.TEAM_NUM, (int)id);
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
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
                    Debug.Log("采集指令");
                    world.DoGather();
                    break;
                case BUTTON_ID.TEAM_PACK:
                    Debug.Log("背包指令");
                    ControllerManager.ShowWindow<PackController>("PackViewer");
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
            if (teamModeBTs.activeInHierarchy == active)
                return false;
            teamModeBTs.SetActive(active);
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
    }
}