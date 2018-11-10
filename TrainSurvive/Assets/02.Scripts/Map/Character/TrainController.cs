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
        private Vector2Int InitIndex;
        private Vector2 blockSize;            //readonly
        private Vector2 mapOrigin;          //readonly
        private Vector2 mapOriginUnit;    //readonly

        //列车
        private Train train;
        //主摄像机
        private Camera mainCamera;
        //地图脚本，用于获取地图信息
        private IMapForTrain iMapForTrain;
        private IMapForTrainTemp iMapForTrainTemp;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        private EventSystem eventSystem;
        private GraphicRaycaster graphicRaycaster;
        /// <summary>
        /// 一些变量设置
        /// </summary>
        /// <param name="iMapT"></param>
        /// <param name="iMap"></param>
        /// <param name="initPosition">列车初始点</param>
        public void init(IMapForTrainTemp iMapT, IMapForTrain iMap, Vector2Int initIndex)
        {
            iMapForTrainTemp = iMapT;
            iMapForTrain = iMap;
            InitIndex = initIndex;
            blockSize = iMapForTrainTemp.GetBlockSize();
            Debug.Assert(blockSize.x > 0.1 && blockSize.y > 0.1, "块大小设置的过小");
            mapOrigin = iMapForTrainTemp.GetMapOrigin();
            mapOriginUnit = mapOrigin / blockSize + new Vector2(0.5F, 0.5F);
            train = new Train(true);
            ButtonHandler.Instance.AddListeners(this);
        }
        void Start()
        {
            //初始化变量
            mainCamera = Camera.main;
            Debug.Assert(null != mainCamera, "需要将主摄像机的Tag改为MainCamera");
            cameraFocus = mainCamera.GetComponent<ICameraFocus>();
            Vector2 position = CalBlockCenterByIndex(InitIndex);
            transform.position = new Vector3(InitIndex.x, levelOfTrain, InitIndex.y);
            //驱散迷雾
            iMapForTrain.MoveToThisSpawn(InitIndex);
            //焦距自己
            cameraFocus.focusLock(transform);
            //
            GameObject canvas = GameObject.Find("Canvas") ;
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            eventSystem = canvas.GetComponent<EventSystem>();
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

                Vector2 clickedPosition = WorldPosToMapPos(ray.origin);
                Vector2 currentPosition = WorldPosToMapPos(transform.position);
                Vector2Int clickIndex = WhereTheBlockByIndex(ref clickedPosition);
                Vector2Int currentIndex = WhereTheBlockByIndex(ref currentPosition);
                Vector2 startOfRail = new Vector2();
                Vector2 endOfRail = new Vector2();
                bool movePositive = true;
                //判断点击处是否是铁轨
                if (CanReachableAndSet(ref currentIndex, ref clickIndex, ref startOfRail, ref endOfRail, ref movePositive))
                {
                    train.StartRun(startOfRail, endOfRail, movePositive);
                }
            }

            //列车移动判断
            if (train.IsMovable && train.IsRunning)
            {
                Vector2 formatPosition = WorldPosToMapPos(transform.position);
                if (train.Run(ref formatPosition))
                {
                    transform.position = MapPosToWorldPos(formatPosition);
                    Vector2Int trainMapIndex = WhereTheBlockByIndex(ref formatPosition);
                    //驱散迷雾
                    iMapForTrain.MoveToThisSpawn(trainMapIndex);
                }
            }
        }
        /// <summary>
        /// 3D世界坐标转化为地图平面的2D世界坐标
        /// </summary>
        /// <param name="worldPosition">3D世界坐标</param>
        /// <returns></returns>
        private Vector2 WorldPosToMapPos(Vector3 worldPosition)
        {
            return MathUtilsByXYS.IgnoreY(worldPosition);
        }
        /// <summary>
        /// 地图平面的2D世界坐标转化为3D世界坐标
        /// </summary>
        /// <param name="mapPosition">地图平面的2D世界坐标</param>
        /// <returns></returns>
        private Vector3 MapPosToWorldPos(Vector2 mapPosition)
        {
            return MathUtilsByXYS.AcceptY(mapPosition, levelOfTrain);
        }
        /// <summary>
        /// 根据世界坐标计算地图坐标
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <returns>地图坐标</returns>
        private Vector2Int WhereTheBlockByIndex(ref Vector2 position)
        {
            //因为块中心位于原点坐标处，所以要减去blockSize/2
            //(clickedPosition - worldMap.getMapOrigin() - blockSize / 2) / blockSize;
            //公式优化如下
            //clicckedPosition/blockSize - ((mapOrigin + blockSize/2)/blockSize)
            Vector2 index2F = position / blockSize - mapOriginUnit;
            return new Vector2Int(Mathf.CeilToInt(index2F.x), Mathf.CeilToInt(index2F.y));
        }
        /// <summary>
        /// 计算指定块中心的世界坐标
        /// </summary>
        /// <param name="index2d">地图坐标</param>
        /// <returns>世界坐标</returns>
        private Vector2 CalBlockCenterByIndex(Vector2Int index2d)
        {
            //索引 * 块大小 = 原点到块中心的向量
            //再加上 原块的中心坐标 就是世界坐标
            return index2d * blockSize + mapOrigin;
        }
        private bool CanReachableAndSet(ref Vector2Int currentIndex, ref Vector2Int clickIndex, ref Vector2 startOfRail, ref Vector2 endOfRail, ref bool movePositive)
        {
            Vector2Int clickedStart, clickedEnd, currentStart, currentEnd;
            if (!iMapForTrain.GetEachEndsOfRail(clickIndex, out clickedStart, out clickedEnd))
            {
                Debug.Log("点击处不是铁轨");
                return false;
            }
            Debug.Log("点击处：" + clickIndex + "点击处铁轨的城市：" + clickedStart + "," + clickedEnd);

            //列车于点击的轨道是否是在一条轨道上。
            bool ifTrainOnTheLine = true;
            if (iMapForTrain.IfTown(currentIndex))
            {
                currentStart = clickedStart;
                currentEnd = clickedEnd;
                //如果在列车在城镇上，则判断点击的铁轨是否包含该城镇。
                if (clickedStart != currentIndex && clickedEnd != currentIndex)
                    ifTrainOnTheLine = false;
                Debug.Log("列车所在的城市：" + currentIndex
                       + " 目标铁轨的城市：" + clickedStart + "," + clickedEnd + (ifTrainOnTheLine ? " 在" : "不在") + "指定铁轨");
            }
            else
            {
                if (!iMapForTrain.GetEachEndsOfRail(currentIndex, out currentStart, out currentEnd))
                {
                    Debug.Log("列车不在铁轨上");
                    return false;
                }
                Debug.Log("当前铁轨的城市：" + currentStart + "," + clickedEnd
                       + " 目标铁轨的城市：" + clickedStart + "," + clickedEnd);
                if (currentStart != clickedStart)
                    ifTrainOnTheLine = false;
            }
            //如果点击点和当前位置不在同一条铁轨上，则失败。
            if (!ifTrainOnTheLine)
            {
                Debug.Log("不能跨铁轨移动");
                return false;
            }
            //允许移动
            startOfRail = CalBlockCenterByIndex(currentStart);
            endOfRail = CalBlockCenterByIndex(currentEnd);
            Debug.Log("目前位置：" + currentIndex + "点击处:" + clickIndex);
            //判断铁轨结尾方向与前往方向是否一致
            //先判断x轴方向是否一致，再判断z轴方向
            movePositive = ((currentEnd.x - currentIndex.x) * (clickIndex.x - currentIndex.x) > 0 || (currentEnd.y - currentIndex.y) * (clickIndex.y - currentIndex.y) > 0);
            return true;
        }
        public void OnClick(BUTTON_ID id)
        {
            if (!ButtonHandler.IsTrain(id))
                return;
            switch (id)
            {
                case BUTTON_ID.TRAIN_RUN:
                    Debug.Log("开车指令");
                    train.ContinueRun();
                    break;
                case BUTTON_ID.TRAIN_STOP:
                    Debug.Log("停车指令");
                    train.Stop();
                    break;
            }
        }
    }
}
