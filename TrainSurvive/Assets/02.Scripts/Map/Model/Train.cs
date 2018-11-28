/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/9 20:42:50
 * 版本：v0.1
 */
using System.Collections.Generic;
using UnityEngine;
using WorldMap;

namespace WorldMap.Model
{
    public class Train : SubjectBase
    {
        //列车的最大移动速度
        public float MaxSpeed { set; get; }
        //列车的最小移动距离
        public float MinDeltaStep { set; get; } = 0.01F;
        public float SmoothTime { set; get; } = 0.3F;
        //列车所在的世界坐标
        public Vector2 PosTrain { private set; get; }
        //列车所在的地图坐标
        public Vector2Int MapPosTrain
        {
            get
            {
                return StaticResource.BlockIndex(PosTrain);
            }
        }
        //列车状态
        private STATE state;
        private STATE State
        {
            get { return state; }
            set
            {
                state = value;
                this.Notify((int)state);
            }
        }

        //列车运行时下一目的地（城市位置）
        private Vector2 nextCityPosition;
        //列车运行时所在的铁轨
        private Rail railStandingOn;
        //列车脚下的方块的中心位置
        private Vector2 blockCenterUnderTrain;
        // 是否暂时停止
        private bool ifTemporarilyStop;
        //列车移动速度
        private float velocity = 0.0F;

        //外部引用
        private IMapForTrain map;
        private Team team;
        public TrainController Controller { private set; get; }
        public override int MaxState()
        {
            return (int)STATE.NUM;
        }
        public static Train Instance { get; } = new Train();
        private Train() : base()
        {
        }
        public void Init(bool movable, float maxSpeed, Vector2Int initPosition, TrainController controller)
        {
            map = Map.GetIntanstance();
            team = Team.Instance;
            PosTrain = StaticResource.BlockCenter(initPosition);
            IsMovable = movable;
            MaxSpeed = maxSpeed;
            ifTemporarilyStop = false;
            Controller = controller;
            Controller.init(this);
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
            railStandingOn = new Rail(StaticResource.BlockCenter(currentStart), StaticResource.BlockCenter(currentEnd));
            Debug.Log("目前位置：" + currentIndex + "点击处:" + clickIndex);
            //判断铁轨结尾方向与前往方向是否一致
            //先判断x轴方向是否一致，再判断z轴方向
            IsMovePositive = ((currentEnd.x - currentIndex.x) * (clickIndex.x - currentIndex.x) > 0 || (currentEnd.y - currentIndex.y) * (clickIndex.y - currentIndex.y) > 0);
            return true;
        }
        /// <summary>
        /// 列车沿着铁轨运动，下一帧的坐标。
        /// </summary>
        /// <param name="current">会被修改的世界坐标</param>
        /// <returns>
        /// TRUE：列车移动
        /// FALSE：列车没有移动
        /// </returns>
        public bool Run(ref Vector2 current)
        {
            //列车不允许移动 或者 列车停止了
            if (!IsMovable || IsStoped)
                return false;
            Vector2 currentNext = current;
            float remanentRoad = 0.0F;
            railStandingOn.CalRemanentRoad(current, IsMovePositive, ref remanentRoad);
            float smoothDelta = Mathf.SmoothDamp(remanentRoad, 0, ref velocity, SmoothTime, MaxSpeed, Time.deltaTime);
            //限制列车的最小移动距离和最大移动距离
            //最小移动距离（MinDeltaStep） <= deltaRoad  <= 剩余路程（remainRoad）
            float deltaRoad = 0.0F;
            if (!Mathf.Approximately(smoothDelta, remanentRoad))
                deltaRoad = Mathf.Max(remanentRoad - smoothDelta, MinDeltaStep);
            bool passCenterOfBlock = false;
            bool arrived = false;
            currentNext = railStandingOn.CalNextPosition(currentNext, ref deltaRoad, IsMovePositive, out passCenterOfBlock, out arrived);
            //Debug.Log("运行日记：" + current + " => " + currentNext + " 剩余路程：" + remanentRoad);
            //放在前面的原因是保证，如果达到终点时最后一定的状态一定时ARRIVED
            if (passCenterOfBlock)
                PassCenterCallBack(StaticResource.BlockIndex(current));
            if (arrived)
            {
                Debug.Log("到达目的地：" + current);
                //到达目的地
                State = STATE.STOP_TOWN;
            }
            current = PosTrain = currentNext;
            return true;
        }
        /// <summary>
        /// 列车经过一个方块的中心点时的回调函数，不包括起点。
        /// 不会被重复调用。
        /// </summary>
        /// <param name="center">块的地图坐标</param>
        public void PassCenterCallBack(Vector2Int center)
        {
            map.MoveToThisSpawn(center);
            //暂时性停车
            if (ifTemporarilyStop)
            {
                Debug.Log("暂时性停车__成功");
                State = STATE.STOP_RAIL;
                ifTemporarilyStop = false;
            }
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
            Debug.Log("列车开始前往 城市坐标：" + nextCityPosition + " 总路程：" + railStandingOn.CalTotalRoad());
            velocity = 0.0F;
            State = STATE.RUNNING;
            return true;
        }
        /// <summary>
        /// 列车继续运行
        /// </summary>
        public bool ContinueRun()
        {
            //必须暂停才能继续运行
            if (!IsStoped)
            {
                Debug.Log("列车正在运行");
                return false;
            }
            //只有中途停车后才能继续开车
            if (!IsStopedTemporarily)
            {
                Debug.Log("只能中途停车，才能开车");
                return false;
            }
            State = STATE.RUNNING;
            return true;
        }
        /// <summary>
        /// 暂时性停止在一个坐标点
        /// </summary>
        /// <param name="end">世界坐标</param>
        public bool StopTemporarily()
        {
            if (IsStoped)
            {
                Debug.Log("列车已经停止");
                return true;
            }
            //TODO:暂时性停车失效了
            //TODO:添加列车经过每个方块的回调函数
            Debug.Log("暂时性停止__开始");
            State = STATE.STOPING;
            ifTemporarilyStop = true;
            return true;
        }
        //列车属性判断
        public bool IsRunning
        {
            get { return State == STATE.RUNNING; }
        }
        //列车是否已经停稳
        public bool IsStoped
        {
            get { return State == STATE.STOP_TOWN || State == STATE.STOP_RAIL; }
        }
        public bool IsArrived
        {
            get { return State == STATE.STOP_TOWN; }
        }
        private bool IsStopedTemporarily
        {
            get { return State == STATE.STOP_RAIL; }
        }
        public bool IsStoping
        {
            get { return State == STATE.STOPING; }
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
        private bool IsMovePositive { set; get; }
        public enum STATE
        {
            NONE,
            //已经停止
            STOP_TOWN = 0,//列车停在城镇中
            STOP_RAIL,//中途暂时性停车，停在铁轨上
            //还在移动中
            STOPING,//列车正在暂时性停车，但是还未停止
            RUNNING,//列车正在运行
            NUM
        }
    }
}