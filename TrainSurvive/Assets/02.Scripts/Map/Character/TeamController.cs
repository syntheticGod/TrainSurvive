/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 16:06:56
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldMap
{
    public class TeamController : MonoBehaviour, OnClickListener
    {
        private int levelOfTeam = 2;
        //小队数据结构
        private Team team;
        private GameObject teamModeBTs;
        private TrainController trainController;
        //主摄像机
        private Camera mainCamera;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        public void Init(Team team,Train train, TrainController trainController)
        {
            ButtonHandler.Instance.AddListeners(this);
            this.team = team;
            this.team.Init(train);
            this.trainController = trainController;
            trainController.SetTeamController(this);
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
                if (!team.GoTrain())
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
        public void Active(Vector2 position)
        {
            team.SetPosition(position);
            ActiveTeam(true);
            ActiveBTs(true);
            cameraFocus.focusLock(transform);
        }
        public void OnClick(BUTTON_ID id)
        {
            if (!ButtonHandler.IsTeam(id))
                return;
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
            }
        }
        private bool KeyEventDetecter()
        {
            if (Input.GetKeyUp(KeyCode.W))
            {
                team.MoveTop();
                return true;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                team.MoveBottom();
                return true;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                team.MoveLeft();
                return true;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                team.MoveRight();
                return true;
            }
            return false;
        }
    }
}