/*
 * 描述：玩家控制列车移动的控制器，
 *          该脚本应该绑定到列车对象上。
 * 作者：项叶盛
 * 创建时间：2018/10/31 0:29:36
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WorldMap
{
    public class TrainController : MonoBehaviour, OnClickListener
    {
        private const int levelOfTrain = 1;

        //私有信息
        private Vector2 initPosition;
        private StaticResource staticResource;
        //列车
        private Train train;
        //主摄像机
        private Camera mainCamera;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        //外部引用
        private IMapForTrain map;
        private TeamController teamController;
        /// <summary>
        /// 给TraiController设置TeamController
        /// </summary>
        /// <param name="tc"></param>
        public void SetTeamController(TeamController tc)
        {
            teamController = tc;
        }
        private GameObject trainModeBTs;
        /// <summary>
        /// 一些变量设置
        /// </summary>
        /// <param name="iMapT"></param>
        /// <param name="iMap"></param>
        /// <param name="initPosition">列车初始点</param>
        public void init(IMapForTrain iMap, Vector2Int initIndex, Train train)
        {
            staticResource = StaticResource.Instance;
            map = iMap;
            initPosition = StaticResource.BlockCenter(initIndex);
            train.MinDeltaStep = 0.01F * staticResource.BlockSize.x;
            this.train = train;
            this.train.Init(initPosition);
            ButtonHandler.Instance.AddListeners(this);
        }
        void Awake()
        {
            Debug.Log("TrainController Awake");
            trainModeBTs = GameObject.Find("/Canvas").transform.Find("TrainMode").gameObject;
        }
        void Start()
        {
            //初始化变量
            mainCamera = Camera.main;
            Debug.Assert(null != mainCamera, "需要将主摄像机的Tag改为MainCamera");
            cameraFocus = mainCamera.GetComponent<ICameraFocus>();
            transform.position = StaticResource.MapPosToWorldPos(initPosition, levelOfTrain);
            //驱散迷雾
            map.MoveToThisSpawn(StaticResource.BlockIndex(initPosition));
            //焦距自己
            cameraFocus.focusLock(transform);
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
                if (!train.StartRun(StaticResource.WorldPosToMapPos(ray.origin)))
                {
                    Debug.Log("列车行动失败");
                }
            }
            //列车允许移动 或者 列车没有停止（列车不一定只有 停止和运行 两个状态） 才能进入
            if (train.IsMovable && !train.IsStoped)
            {
                Vector2 mapPosition = StaticResource.WorldPosToMapPos(transform.position);
                if (train.Run(ref mapPosition))
                {
                    transform.position = StaticResource.MapPosToWorldPos(mapPosition, levelOfTrain);
                    Vector2Int trainMapIndex = StaticResource.BlockIndex(mapPosition);
                    //驱散迷雾
                    map.MoveToThisSpawn(trainMapIndex);
                }
            }
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
        /// <summary>
        /// UI按钮的点击事件，地图的鼠标事件在Update中处理。
        /// </summary>
        /// <param name="id">按钮的ID</param>
        public void OnClick(BUTTON_ID id)
        {
            if (!ButtonHandler.IsTrain(id))
                return;
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
                case BUTTON_ID.TEAM_ACTION:
                    Debug.Log("探险队行动");
                    if (!ActiveTrain(false))
                    {
                        Debug.Log("探险队行动失败");
                        break;
                    }
                    ActiveBTs(false);
                    teamController.Active(train.PosTrain);
                    break;
            }
        }
    }
}
