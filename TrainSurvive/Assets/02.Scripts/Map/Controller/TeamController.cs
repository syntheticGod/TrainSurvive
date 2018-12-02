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

namespace WorldMap
{
    public class TeamController : MonoBehaviour, OnClickListener
    {
        private int levelOfTeam = -2;
        
        private Team team;
        private Train train;
        private GameObject teamModeBTs;
        private TrainController trainController;
        //主摄像机
        private Camera mainCamera;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        private WorldForMap world;
        public void Init(Team team)
        {
            ButtonHandler.Instance.AddListeners(this);
            this.team = team;
            this.trainController = Train.Instance.Controller;
            train = Train.Instance;
            world = WorldForMap.Instance;
        }
        void Awake()
        {
            Debug.Log("TeamController Awake");
            teamModeBTs = GameObject.Find("/Canvas").transform.Find("TeamMode").gameObject;
            mainCamera = Camera.main;
            Debug.Assert(null != mainCamera, "需要将主摄像机的Tag改为MainCamera");
            cameraFocus = mainCamera.GetComponent<ICameraFocus>();
        }
        void Start()
        {
            Debug.Log("TeamController Start");
        }
        void Update()
        {
            KeyEventDetecter();
            Vector2 current = StaticResource.WorldPosToMapPos(transform.position);
            if (team.Run(ref current))
            {
                transform.position = StaticResource.MapPosToWorldPos(current, levelOfTeam);
            }
        }
        private bool ActiveTeam(bool active)
        {
            if (gameObject.activeSelf == active)
            {
                Debug.Log("重复操作，无效");
                return false;
            }
            if (active)
            {
                transform.position = StaticResource.MapPosToWorldPos(team.PosTeam, levelOfTeam);
            }
            else
            {
                if (!team.CanTeamGoBack())
                    return false;
                if (!train.TeamComeBack())
                    return false;
                if (!team.GoBackToTrain())
                    return false;
            }
            gameObject.SetActive(active);
            return true;
        }
        private bool ActiveBTs(bool active)
        {
            if (teamModeBTs.activeSelf == active)
            {
                Debug.Log("重复操作，无效");
                return false;
            }
            teamModeBTs.SetActive(active);
            return true;
        }
        public void Active()
        {
            ActiveTeam(true);
            ActiveBTs(true);
            cameraFocus.focusLock(transform);
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
                    trainController.Active();
                    break;
                case BUTTON_ID.TEAM_GATHER:
                    world.DoGather();
                    break;
            }
        }
        private bool KeyEventDetecter()
        {
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
    }
}