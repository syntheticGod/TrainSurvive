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
namespace WorldMap
{
    public class TrainController : MonoBehaviour, OnClickListener, DialogCallBack
    {
        private const int levelOfTrain = -1;

        //私有信息
        private StaticResource staticResource;
        //列车
        private Train train;
        //主摄像机
        private Camera mainCamera;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        //外部引用
        private IMapForTrain map;
        private Team team;
        private TownController townController;
        private GameObject trainModeBTs;
        private TeamOutPrepareDialog teamOutDialog;
        private World world;
        public void init(Train train)
        {
            staticResource = StaticResource.Instance();
            map = Map.GetIntanstance();
            this.train = train;
            team = Team.Instance;
            train.MinDeltaStep = 0.01F * staticResource.BlockSize.x;
            world = World.getInstance();
            //监听列车的所有状态
            ButtonHandler.Instance.AddListeners(this);
        }
        void Awake()
        {
            Debug.Log("TrainController Awake");
            //因为TrainMode和TownViewer在启动时不一定是enable状态。所以通过Transform寻找
            Transform canvas = GameObject.Find("/Canvas").transform;
            trainModeBTs = canvas.Find("TrainMode").gameObject;
            townController = canvas.Find("TownViewer").GetComponent<TownController>();
            townController.Init();
            teamOutDialog = canvas.Find("TeamSelectDialog").GetComponent<TeamOutPrepareDialog>();
            teamOutDialog.CallBack = this;
            teamOutDialog.Init();
        }
        void Start()
        {
            Debug.Log("TrainController Start");
            //初始化变量
            mainCamera = Camera.main;
            cameraFocus = Camera.main.GetComponent<ICameraFocus>();
            transform.position = StaticResource.MapPosToWorldPos(train.PosTrain, levelOfTrain);
            //焦距自己
            cameraFocus.focusLock(transform);
            //驱散迷雾
            map.MoveToThisSpawn(train.MapPosTrain);
            townController.TryShowTown(train.MapPosTrain);
        }
        void Update()
        {
            //点击事件处理
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                //如果检测到是UI层，则不处理。
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                Ray ray = mainCamera.GetComponent<Camera>()
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
        private bool ActiveTrain(bool active)
        {
            if (train.IsMovable == active)
            {
                Debug.Log("重复操作，无效");
                return false;
            }
            return train.SetMovable(active);
        }
        private bool ActiveBTs(bool active)
        {
            if (trainModeBTs.activeSelf == active)
            {
                Debug.Log("重复操作，无效");
                return false;
            }
            trainModeBTs.SetActive(active);
            return true;
        }
        public void Active()
        {
            ActiveTrain(true);
            ActiveBTs(true);
            cameraFocus.focusLock(transform);
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
            switch (id)
            {
                case BUTTON_ID.TRAIN_RUN:
                    Debug.Log("开车指令");
                    if (!train.ContinueRun())
                    {
                        Debug.Log("开车指令失败");
                        break;
                    }
                    break;
                case BUTTON_ID.TRAIN_STOP:
                    Debug.Log("停车指令");
                    if (!train.StopTemporarily())
                    {
                        Debug.Log("开车指令失败");
                        break;
                    }
                    break;
                case BUTTON_ID.TRAIN_TEAM_ACTION:
                    Debug.Log("探险队行动");
                    //弹出框之后不能再操作列车
                    if (!ActiveTrain(false))
                    {
                        Debug.Log("探险队行动失败");
                        break;
                    }
                    teamOutDialog.Show();
                    break;
                case BUTTON_ID.TRAIN_CHANGE:
                    SceneManager.LoadScene("GCTestScene1");
                    break;
            }
        }

        public void OK(TeamOutPrepareDialog dialog)
        {
            //先列车准备
            train.TeamOutPrepare(dialog.GetSelectedFood(), dialog.GetSelectedPerson());
            //再探险队准备
            team.OutPrepare(train.PosTrain, dialog.GetSelectedFood(), dialog.GetSelectedPerson());
            team.Controller.Active();
            ActiveBTs(false);
        }

        public void Cancel()
        {
            ActiveTrain(true);
        }
    }
}
