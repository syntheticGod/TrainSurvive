/*
 * 描述：玩家控制列车移动的控制器，
 *          该脚本应该绑定到列车对象上。
 * 作者：项叶盛
 * 创建时间：2018/10/31 0:29:36
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System;

using WorldMap.Model;
using WorldMap.UI;
using UnityEngine.UI;

namespace WorldMap.Controller
{
    public class TrainController : BaseController, OnClickListener, DialogCallBack, Observer
    {
        private const int levelOfTrain = -1;
        //列车状态映射显示信息
        private static string[] trainActionBtnStrs = { "开车","开车", "停车中..." ,"停车"};
        private const int ECHO_FROM_TRAIN = 1;
        //主摄像机
        private Camera mainCamera;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        //外部引用
        private IMapForTrain map;
        private GameObject trainModeBTs;
        private Text trainActionBtn;
        private TownController townController;
        private TeamOutPrepareDialog teamOutDialog;
        public void init()
        {
            Debug.Log("TrainController init");
            map = Map.GetIntanstance();
            //监听列车的所有状态
            ButtonHandler.Instance.AddListeners(this);
            Train.Instance.Attach(this, ECHO_FROM_TRAIN);
        }
        protected override void CreateModel()
        {
            Debug.Log("TrainController CreateModel");
            Transform canvas = GameObject.Find("/Canvas").transform;
            trainModeBTs = canvas.Find("TrainMode").gameObject;
            trainActionBtn = trainModeBTs.transform.Find("TrainRunOrStopBtn").Find("Text").GetComponent<Text>();

            townController = canvas.Find("TownViewer").GetComponent<TownController>();
            townController.Init();

            teamOutDialog = canvas.Find("TeamSelectDialog").GetComponent<TeamOutPrepareDialog>();
            teamOutDialog.CallBack = this;
            teamOutDialog.Init();
            
            cameraFocus = Camera.main.GetComponent<ICameraFocus>();
            cameraFocus.focusLock(transform);
        }
        protected override void Start()
        {
            Debug.Log("TrainController Start");
            base.Start();
            Train train = Train.Instance;
            transform.position = StaticResource.MapPosToWorldPos(train.PosTrain, levelOfTrain);
            map.MoveToThisSpawn(train.MapPosTrain);
        }
        protected override void Update()
        {
            base.Update();
            MouseEventDetecter();
            Train train = Train.Instance;
            if (train.IsMovable == false) return;
            //点击事件处理
            if (LeftMouseState == MouseState.Click)
            {
                //如果检测到是UI层，则不处理。
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                Ray ray = Camera.main.GetComponent<Camera>()
                    .ScreenPointToRay(Input.mousePosition);
                //因为摄像机的Projection 为 Orthographic，所以Ray的方向都是平行的
                Debug.Log("origin of ray:" + ray.origin + " dire:" + ray.direction);
                Debug.Log("mouse position " + Input.mousePosition);
                Vector2 clickedPosition = StaticResource.WorldPosToMapPos(ray.origin);
                if (map.isSpawnVisible(StaticResource.BlockIndex(clickedPosition)))
                {
                    if (!train.StartRun(clickedPosition))
                    {
                        Debug.Log("列车行动失败");
                        return;
                    }
                }
                else
                {
                    Debug.Log("点击处被迷雾环绕");
                    return;
                }
            }
            Vector2 current = StaticResource.WorldPosToMapPos(transform.position);
            if (train.Run(ref current))
                transform.position = StaticResource.MapPosToWorldPos(current, levelOfTrain);
        }
        public bool IfAccepted(BUTTON_ID id)
        {
            return Utility.Between((int)BUTTON_ID.TRAIN_NONE, (int)BUTTON_ID.TRAIN_NUM, (int)id);
        }
        /// <summary>
        /// UI按钮的点击事件，地图的鼠标事件在Update中处理。
        /// </summary>
        /// <param name="id">按钮的ID</param>
        public void OnClick(BUTTON_ID id)
        {
            Train train = Train.Instance;
            switch (id)
            {
                case BUTTON_ID.TRAIN_RUN_OR_STOP:
                    if (train.IsRunning)
                    {
                        Debug.Log("停车指令");
                        if (!train.StopTemporarily())
                        {
                            Debug.Log("开车指令失败");
                            break;
                        }
                        //trainActionBtn.text = trainActionBtnStrs[0];
                    }
                    else
                    {
                        Debug.Log("开车指令");
                        if (!train.ContinueRun())
                        {
                            Debug.Log("开车指令失败");
                            break;
                        }
                        //trainActionBtn.text = trainActionBtnStrs[1];
                    }
                    break;
                case BUTTON_ID.TRAIN_ENTRY_AREA:
                    Debug.Log("进入区域指令");
                    //TODO：目前只有城镇
                    if(townController!=null)
                        townController.TryShowTown(train.MapPosTrain);
                    else
                        Debug.Log("TeamOutDialog is null");
                    break;
                case BUTTON_ID.TRAIN_TEAM_ACTION:
                    Debug.Log("探险队行动");
                    //弹出框之后不能再操作列车
                    if (teamOutDialog != null)
                        teamOutDialog.Show();
                    else
                        Debug.Log("TeamOutDialog is null");
                    break;
                case BUTTON_ID.TRAIN_CHANGE:
                    SceneManager.LoadScene("TrainScene");
                    break;
            }
        }

        public void OK(TeamOutPrepareDialog dialog)
        {
            //险队准备
            ActiveTrain(false);
            ActiveBTs(false);
            Team.Instance.OutPrepare(Train.Instance.PosTrain, dialog.GetSelectedFood(), dialog.GetSelectedPerson());
            ControllerManager.FocusController("Team", "Character");
        }

        public void Cancel()
        { }
        private void ActiveTrain(bool active)
        {
            Train.Instance.SetMovable(active);
        }
        private bool ActiveBTs(bool active)
        {
            if (trainModeBTs.activeSelf == active)
            {
                Debug.Log("重复激活按钮，无效");
                return false;
            }
            trainModeBTs.SetActive(active);
            return true;
        }
        protected override bool FocusBehaviour()
        {
            ActiveBTs(true);
            ActiveTrain(true);
            cameraFocus.focusLock(transform);
            return true;
        }

        protected override void UnfocusBehaviour()
        {
        }

        public void ObserverUpdate(int state, int echo)
        {
            switch (echo)
            {
                case ECHO_FROM_TRAIN:
                    if(state < trainActionBtnStrs.Length)
                        trainActionBtn.text = trainActionBtnStrs[state];
                    else
                        Debug.LogError("系统：该列车状态无对应的显示信息");
                    break;
            }
        }
    }
}
