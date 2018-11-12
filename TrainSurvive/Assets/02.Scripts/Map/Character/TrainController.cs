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
        /// <summary>
        /// 一些变量设置
        /// </summary>
        /// <param name="iMapT"></param>
        /// <param name="iMap"></param>
        /// <param name="initPosition">列车初始点</param>
        public void init(IMapForTrainTemp iMapT, IMapForTrain iMap, Vector2Int initIndex, Train train)
        {
            iMapForTrainTemp = iMapT;
            iMapForTrain = iMap;
            InitIndex = initIndex;
            blockSize = iMapForTrainTemp.GetBlockSize();
            Debug.Assert(blockSize.x > 0.1 && blockSize.y > 0.1, "块大小设置的过小");
            mapOrigin = iMapForTrainTemp.GetMapOrigin();
            mapOriginUnit = mapOrigin / blockSize + new Vector2(0.5F, 0.5F);
            train.MinDeltaStep = 0.01F * blockSize.x;
            this.train = train;
            
            ButtonHandler.Instance.AddListeners(this);
        }
        void Start()
        {
            //初始化变量
            mainCamera = Camera.main;
            Debug.Assert(null != mainCamera, "需要将主摄像机的Tag改为MainCamera");
            cameraFocus = mainCamera.GetComponent<ICameraFocus>();
            Vector2 position = BlockCenter(InitIndex);
            transform.position = new Vector3(InitIndex.x, levelOfTrain, InitIndex.y);
            //驱散迷雾
            iMapForTrain.MoveToThisSpawn(InitIndex);
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

                Vector2 clickedPosition = WorldPosToMapPos(ray.origin);
                Vector2 currentPosition = WorldPosToMapPos(transform.position);
                Vector2Int clickIndex = BlockIndex(clickedPosition);
                Vector2Int currentIndex = BlockIndex(currentPosition);
                Vector2 startOfRail = new Vector2();
                Vector2 endOfRail = new Vector2();
                bool movePositive = true;
                //判断点击处是否是铁轨
                if (CanReachableAndSet(ref currentIndex, ref clickIndex, ref startOfRail, ref endOfRail, ref movePositive))
                {
                    train.StartRun(startOfRail, endOfRail, movePositive);
                }
            }
            //列车允许移动 或者 列车没有停止（列车不一定只有 停止和运行 两个状态） 才能进入
            if (train.IsMovable && !train.IsStoped)
            {
                Vector2 mapPosition = WorldPosToMapPos(transform.position);
                Vector2 blockCenter = FormatBlock(mapPosition);
                if (train.Run(ref mapPosition, blockCenter))
                {
                    transform.position = MapPosToWorldPos(mapPosition);
                    Vector2Int trainMapIndex = BlockIndex(mapPosition);
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
            return Utility.IgnoreY(worldPosition);
        }
        /// <summary>
        /// 地图平面的2D世界坐标转化为3D世界坐标
        /// </summary>
        /// <param name="mapPosition">地图平面的2D世界坐标</param>
        /// <returns></returns>
        private Vector3 MapPosToWorldPos(Vector2 mapPosition)
        {
            return Utility.AcceptY(mapPosition, levelOfTrain);
        }
        /// <summary>
        /// 根据世界坐标计算地图坐标
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <returns>地图坐标</returns>
        private Vector2Int BlockIndex(Vector2 position)
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
        private Vector2 BlockCenter(Vector2Int index2d)
        {
            //索引 * 块大小 = 原点到块中心的向量
            //再加上 原块的中心坐标 就是世界坐标
            return index2d * blockSize + mapOrigin;
        }
        /// <summary>
        /// 获取当前坐标所在块的中心坐标
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <returns></returns>
        private Vector2 FormatBlock(Vector2 position)
        {
            return BlockCenter(BlockIndex(position));
        }
        /// <summary>
        /// 判断current和click之间是否连通
        /// </summary>
        /// <param name="currentIndex">点1</param>
        /// <param name="clickIndex">点2</param>
        /// <param name="startOfRail">铁轨起点</param>
        /// <param name="endOfRail">铁轨重点</param>
        /// <param name="movePositive">是否正向移动</param>
        /// <returns></returns>
        private bool CanReachableAndSet(ref Vector2Int currentIndex, ref Vector2Int clickIndex, ref Vector2 startOfRail, ref Vector2 endOfRail, ref bool movePositive)
        {
            Vector2Int clickedStart, clickedEnd, currentStart, currentEnd;
            bool trainOnTheTown = iMapForTrain.IfTown(currentIndex);
            bool clickOnTheTown = iMapForTrain.IfTown(clickIndex);
            //列车位于城镇上，点击处也是城镇
            if (trainOnTheTown && clickOnTheTown)
            {
                //TODO : 最好需要一个接口用来判断两座城市之间有没有铁轨，并且返回起点和终点。
                Debug.Log("待实现");
                return false;
            }
            //列车位于城镇上，点击处不是城镇
            else if (trainOnTheTown)
            {
                if (!iMapForTrain.GetEachEndsOfRail(clickIndex, out clickedStart, out clickedEnd))
                {
                    Debug.Log("点击处不是铁轨也不是城镇");
                    return false;
                }
                //点击处铁轨的两端不包括当前列车所在的城市
                if (currentIndex != clickedStart && currentIndex != clickedEnd)
                {
                    Debug.Log("列车所在的城市：" + currentIndex
                           + " 目标铁轨的城市：" + clickedStart + " => " + clickedEnd + "无法到达");
                    return false;
                }
                currentStart = clickedStart;
                currentEnd = clickedEnd;
            }
            //列车不在城镇上，点击处是城镇
            else if (clickOnTheTown)
            {
                if (!iMapForTrain.GetEachEndsOfRail(currentIndex, out currentStart, out currentEnd))
                {
                    Debug.Log("列车不在铁轨上也不在城镇上");
                    return false;
                }
                if (clickIndex != currentStart && clickIndex != currentEnd)
                {
                    Debug.Log("列车所在的铁轨：" + currentIndex
                           + " 目标铁轨的城市：" + currentStart + " => " + currentEnd + "无法到达");
                    return false;
                }
                clickedStart = currentStart;
                clickedEnd = currentEnd;
            }
            //列车不在城镇上，点击处也不是城镇
            else
            {
                if (!iMapForTrain.GetEachEndsOfRail(clickIndex, out clickedStart, out clickedEnd))
                {
                    Debug.Log("点击处不是铁轨也不是城镇");
                    return false;
                }
                if (!iMapForTrain.GetEachEndsOfRail(currentIndex, out currentStart, out currentEnd))
                {
                    Debug.Log("列车不在铁轨上也不在城镇上");
                    return false;
                }
                if (clickedStart != currentStart || clickedEnd != currentEnd)
                    return false;
            }
            Debug.Log("点击处：" + clickIndex + "点击处铁轨的城市：" + clickedStart + "," + clickedEnd);
            //允许移动
            startOfRail = BlockCenter(currentStart);
            endOfRail = BlockCenter(currentEnd);
            Debug.Log("目前位置：" + currentIndex + "点击处:" + clickIndex);
            //判断铁轨结尾方向与前往方向是否一致
            //先判断x轴方向是否一致，再判断z轴方向
            movePositive = ((currentEnd.x - currentIndex.x) * (clickIndex.x - currentIndex.x) > 0 || (currentEnd.y - currentIndex.y) * (clickIndex.y - currentIndex.y) > 0);
            return true;
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
                    train.ContinueRun();
                    break;
                case BUTTON_ID.TRAIN_STOP:
                    Debug.Log("停车指令");
                    train.StopTemporarily();
                    break;
            }
        }
    }
}
