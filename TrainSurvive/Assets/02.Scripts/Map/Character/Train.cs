/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/9 20:42:50
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldMap
{
    public class Train
    {
        //列车的最大移动速度
        public float MaxSpeed { set; get; }
        //列车的最小移动距离
        public float MinDeltaStep { set; get; } = 0.01F;
        public float SmoothTime { set; get; } = 0.3F;
        //列车移动速度
        private float velocity = 0.0F;
        //列车状态
        private STATE state;
        //列车运行时下一目的地（城市位置）
        private Vector2 nextCityPosition;
        //列车运行时所在的铁轨
        private Rail railStandingOn;
        public Vector2 PosTrain { private set;  get; }
        //列车脚下的方块的中心位置
        private Vector2 blockCenterUnderTrain;
        // 是否暂时停止
        private bool ifTemporarilyStop;
        //外部引用
        private IMapForTrain map;
        public Train(IMapForTrain map, bool movable, float maxSpeed)
        {
            this.map = map;
            railStandingOn = new Rail(Vector2Int.zero, Vector2Int.zero);
            IsMovable = movable;
            state = STATE.STOPED;
            MaxSpeed = maxSpeed;
            ifTemporarilyStop = false;
        }
        public void Init(Vector2 initPosition)
        {
            PosTrain = initPosition;
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
        private bool CanReachableAndSet(Vector2 clickPosition)
        {
            Vector2Int currentIndex = StaticResource.BlockIndex(PosTrain);
            Vector2Int clickIndex = StaticResource.BlockIndex(clickPosition);
            Vector2Int clickedStart, clickedEnd, currentStart, currentEnd;
            bool trainOnTheTown = map.IfTown(currentIndex);
            bool clickOnTheTown = map.IfTown(clickIndex);
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
                if (!map.GetEachEndsOfRail(clickIndex, out clickedStart, out clickedEnd))
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
                if (!map.GetEachEndsOfRail(currentIndex, out currentStart, out currentEnd))
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
                if (!map.GetEachEndsOfRail(clickIndex, out clickedStart, out clickedEnd))
                {
                    Debug.Log("点击处不是铁轨也不是城镇");
                    return false;
                }
                if (!map.GetEachEndsOfRail(currentIndex, out currentStart, out currentEnd))
                {
                    Debug.Log("列车不在铁轨上也不在城镇上");
                    return false;
                }
                if (clickedStart != currentStart || clickedEnd != currentEnd)
                    return false;
            }
            Debug.Log("点击处：" + clickIndex + "点击处铁轨的城市：" + clickedStart + "," + clickedEnd);
            //允许移动
            railStandingOn.Start = StaticResource.BlockCenter(currentStart);
            railStandingOn.End = StaticResource.BlockCenter(currentEnd);
            Debug.Log("目前位置：" + currentIndex + "点击处:" + clickIndex);
            //判断铁轨结尾方向与前往方向是否一致
            //先判断x轴方向是否一致，再判断z轴方向
            IsMovePositive = ((currentEnd.x - currentIndex.x) * (clickIndex.x - currentIndex.x) > 0 || (currentEnd.y - currentIndex.y) * (clickIndex.y - currentIndex.y) > 0);
            return true;
        }
        /// <summary>
        /// 列车在运行时，下一帧的坐标。
        /// </summary>
        /// <param name="current">会被修改的世界坐标</param>
        /// <returns>
        /// TRUE：列车移动
        /// FALSE：列车没有移动
        /// </returns>
        public bool Run(ref Vector2 current)
        {
            Vector2 currentNext = current;
            //positive是x轴的正方向
            bool positiveX = true;
            bool positiveY = true;
            //原点 - 目标
            Vector2 roadVecter = currentNext - nextCityPosition;
            //如果 (原点 - 目标) <= 0 为真 则 positive = True (目标 - 原点 > 0)
            //如果 (原点 - 目标) <= 0 为假 则 positive = False (目标 - 原点 < 0)
            //等于0时 positive = True
            if (positiveX = (roadVecter.x <= 0)) roadVecter.x = -roadVecter.x;
            if (positiveY = (roadVecter.y <= 0)) roadVecter.y = -roadVecter.y;
            if (Utility.ApproximatelyInView(roadVecter, Vector2.zero))
            {
                //到达目的地
                Debug.Log("到达目的地");
                Stop();
                return false;
            }
            float remainedRoad = roadVecter.x + roadVecter.y;
            //remainedRoad一定是大于0的
            float smoothDelta = Mathf.SmoothDamp(remainedRoad, 0, ref velocity, SmoothTime, MaxSpeed);
            //限制列车的最小移动距离
            float deltaRoad = Mathf.Max(remainedRoad - smoothDelta, MinDeltaStep);
            //Debug.Log("direction:" + roadVecter);
            //Debug.Log("remainedRoad:" + remainedRoad + "delta:" + deltaRoad+" smooth:"+ smoothDelta);
            Vector2 blockCenter = StaticResource.FormatBlock(current);
            if (IsMovePositive)
            {
                //如果沿着轨道正方向移动，则x轴的优先度要高于y轴的优先度
                if (!Utility.ApproximatelyInView(roadVecter.x, 0))
                    Approch(ref currentNext.x, Mathf.Min(deltaRoad, roadVecter.x), positiveX, blockCenter.x);
                else
                    Approch(ref currentNext.y, Mathf.Min(deltaRoad, roadVecter.y), positiveY, blockCenter.y);
            }
            else
            {
                //如果沿着轨道反方向移动，则y轴的优先度要高于x轴的优先度
                if (!Utility.ApproximatelyInView(roadVecter.y, 0))
                    Approch(ref currentNext.y, Mathf.Min(deltaRoad, roadVecter.y), positiveY, blockCenter.y);
                else
                    Approch(ref currentNext.x, Mathf.Min(deltaRoad, roadVecter.x), positiveX, blockCenter.x);
            }
            current = PosTrain = currentNext;
            return true;
        }
        /// <summary>
        /// 修改current的值
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="delta">正差值</param>
        /// <param name="positive">增加还是减小</param>
        private void Approch(ref float current, float delta, bool positive, float blockCenter)
        {
            float currentNext = current + (positive ? delta : -delta);
            if (ifTemporarilyStop && Utility.IfBetweenInclude(current, currentNext, blockCenter))
            {
                Debug.Log("暂时性停止__结束");
                current = blockCenter;
                Stop();
                return;
            }
            current = currentNext;
        }
        /// <summary>
        /// 启动列车，重新计算起止点。
        /// 会在用户成功点击目的地的时候被调用
        /// </summary>
        /// <param name="startOfRail">铁轨的</param>
        /// <param name="endOfRail"></param>
        public bool StartRun(Vector2 clickedPosition)
        {
            //列车启动判断
            //1. 是否行动
            //2. 是否正在运行
            //3. 目的点是否可到达
            if (!IsMovable || IsRunning || !CanReachableAndSet(clickedPosition))
                return false;
            nextCityPosition = IsMovePositive ? railStandingOn.End : railStandingOn.Start;
            velocity = 0.0F;
            state = STATE.RUNNING;
            ifTemporarilyStop = false;
            return true;
        }
        /// <summary>
        /// 列车继续运行
        /// </summary>
        public bool ContinueRun()
        {
            //必须暂停才能继续运行
            //只有中途停车后才能继续开车
            if (!IsStoped)
            {
                Debug.Log("列车正在运行");
                return false;
            }
            if (!ifTemporarilyStop)
            {
                Debug.Log("只用中途停车，才能开车");
                return false;
            }
            state = STATE.RUNNING;
            ifTemporarilyStop = false;
            return true;
        }
        /// <summary>
        /// 暂时性停止在一个坐标点
        /// </summary>
        /// <param name="end">世界坐标</param>
        public bool StopTemporarily()
        {
            Debug.Log("暂时性停止__开始");
            state = STATE.STOPING;
            ifTemporarilyStop = true;
            return true;
        }
        /// <summary>
        /// 马上暂停列车运行
        /// </summary>
        public void Stop()
        {
            state = STATE.STOPED;
            ifTemporarilyStop = false;
        }
        //列车属性判断
        public bool IsRunning {
            get { return state == STATE.RUNNING; }
        }
        public bool IsStoped {
            get { return state == STATE.STOPED; }
        }
        public bool IsStoping
        {
            get { return state == STATE.STOPING; }
        }
        public bool IsMovable { private set; get; }
        public bool SetMovable(bool movable)
        {
            if (!movable && !IsStoped)
            {
                Debug.Log("列车没有停下，不能出队");
                return false;
            }
            IsMovable = movable;
            return true;
        }
        public bool IsMovePositive { private set; get; }
        public enum STATE
        {
            STOPED,
            RUNNING,
            STOPING//正在停止
        }
    }
}