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
        private float velocity = 0.0F;

        private STATE state;
        private Rail railStandingOn;
        private Vector2 blockCenterUnderTrain;
        private Vector2 nextCityPosition;
        private Vector2 currentPosition;
        /// <summary>
        /// 是否暂时停止
        /// </summary>
        private bool ifTemporarilyStop;
        public Train(bool movable, float maxSpeed)
        {
            railStandingOn = new Rail(Vector2Int.zero, Vector2Int.zero);
            IsMovable = movable;
            state = STATE.STOPED;
            MaxSpeed = maxSpeed;
            ifTemporarilyStop = false;
        }
        /// <summary>
        /// 列车在运行时，下一帧的坐标。
        /// </summary>
        /// <param name="current">会被修改的世界坐标</param>
        /// <returns>
        /// TRUE：列车移动
        /// FALSE：列车没有移动
        /// </returns>
        public bool Run(ref Vector2 current, Vector2 blockCenter)
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
            if (MathUtilsByXYS.ApproximatelyInView(roadVecter, Vector2.zero))
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
            Debug.Log("direction:" + roadVecter);
            Debug.Log("remainedRoad:" + remainedRoad + "delta:" + deltaRoad+" smooth:"+ smoothDelta);
            //Debug.Log("roadVecter.x:" + roadVecter.x + " and 0:" + MathUtilsByXYS.ApproximatelyInView(roadVecter.x, 0));
            //Debug.Log("IsMovePositive:" + IsMovePositive);
            if (IsMovePositive)
            {
                //如果沿着轨道正方向移动，则x轴的优先度要高于y轴的优先度
                if (!MathUtilsByXYS.ApproximatelyInView(roadVecter.x, 0))
                    Approch(ref currentNext.x, Mathf.Min(deltaRoad, roadVecter.x), positiveX, blockCenter.x);
                else
                    Approch(ref currentNext.y, Mathf.Min(deltaRoad, roadVecter.y), positiveY, blockCenter.y);
            }
            else
            {
                //如果沿着轨道反方向移动，则y轴的优先度要高于x轴的优先度
                if (!MathUtilsByXYS.ApproximatelyInView(roadVecter.y, 0))
                    Approch(ref currentNext.y, Mathf.Min(deltaRoad, roadVecter.y), positiveY, blockCenter.y);
                else
                    Approch(ref currentNext.x, Mathf.Min(deltaRoad, roadVecter.x), positiveX, blockCenter.x);
            }
            current = currentPosition = currentNext;
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
            if (ifTemporarilyStop && MathUtilsByXYS.IfBetweenInclude(current, currentNext, blockCenter))
            {
                Debug.Log("暂时停止");
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
        public void StartRun(Vector2 startOfRail, Vector2 endOfRail, bool movePositive)
        {
            //如果列车正在运行则忽略
            if (IsRunning) return;
            railStandingOn.Start = startOfRail;
            railStandingOn.End = endOfRail;
            IsMovePositive = movePositive;
            nextCityPosition = movePositive ? railStandingOn.End : railStandingOn.Start;
            velocity = 0.0F;
            state = STATE.RUNNING;
            ifTemporarilyStop = false;
        }
        /// <summary>
        /// 列车继续运行
        /// </summary>
        public void ContinueRun()
        {
            //必须暂停才能继续运行
            if (!IsStoped) return;
            state = STATE.RUNNING;
            ifTemporarilyStop = false;
        }
        /// <summary>
        /// 暂时性停止在一个坐标点
        /// </summary>
        /// <param name="end">世界坐标</param>
        public void StopTemporarily()
        {
            Debug.Log("暂时性停止开始");
            state = STATE.STOPING;
            ifTemporarilyStop = true;
        }
        /// <summary>
        /// 马上暂停列车运行
        /// </summary>
        public void Stop()
        {
            state = STATE.STOPED;
            ifTemporarilyStop = false;
        }
        //
        //private float calRemainedRoad(ref Vector2 directionOfNext)
        //{
        //    Vector2 roadVecter = currentPosition - nextCityPosition;
        //    if (IsMovePositive)
        //    {
        //        //如果沿着轨道正方向移动，则x轴的优先度要高于y轴的优先度
        //        //
        //        if (!MathUtilsByXYS.ApproximatelyInView(roadVecter.x, 0))
        //        {

        //        }
        //        else
        //        {

        //        }

        //    }
        //        direction.x = (roadVecter.x <= 0) ? -1 : 1;

        //    if (positiveX = (roadVecter.x <= 0)) roadVecter.x = -roadVecter.x;
        //    if (positiveY = (roadVecter.y <= 0)) roadVecter.y = -roadVecter.y;
        //    return Mathf.Abs(roadVecter.x) + Mathf.Abs(roadVecter.y);
        //}
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
        public bool IsMovable { get; }
        public bool IsMovePositive { private set; get; }
        public enum STATE
        {
            STOPED,
            RUNNING,
            STOPING//正在停止
        }
    }
}