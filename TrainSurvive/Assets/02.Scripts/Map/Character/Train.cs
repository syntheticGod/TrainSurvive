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
        private float velocity = 0.0F;
        private float smoothTime = 0.3F;
        private float maxSpeed = 1F;
        private STATE state;
        private Rail railStandingOn;
        private Vector2 nextStopPosition;
        public Train(bool movable)
        {
            railStandingOn = new Rail(Vector2Int.zero, Vector2Int.zero);
            IsMovable = movable;
            state = STATE.ARRIVE;
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
            //TODO：将
            bool positiveX = true;
            bool positiveY = true;
            //原点 - 目标
            Vector2 roadVecter = current - nextStopPosition;
            //如果 (原点 - 目标) <= 0 为真 则 positive = True (目标 - 原点 > 0)
            //如果 (原点 - 目标) <= 0 为假 则 positive = False (目标 - 原点 < 0)
            //等于0时 positive = True
            if (positiveX = (roadVecter.x <= 0)) roadVecter.x = -roadVecter.x;
            if (positiveY = (roadVecter.y <= 0)) roadVecter.y = -roadVecter.y;
            if (MathUtilsByXYS.ApproximatelyInView(roadVecter, Vector2.zero))
            {
                //到达目的地
                Debug.Log("到达目的地");
                Arrive();
                return false;
            }
            float remainedRoad = roadVecter.x + roadVecter.y;
            //remainedRoad一定是大于0的
            float deltaRoad = remainedRoad - Mathf.SmoothDamp(remainedRoad, 0, ref velocity, smoothTime, maxSpeed);
            //Debug.Log("remainedRoad:" + remainedRoad + "delta road is:" + deltaRoad);
            //Debug.Log("roadVecter.x:" + roadVecter.x + " and 0:" + MathUtilsByXYS.ApproximatelyInView(roadVecter.x, 0));
            //Debug.Log("IsMovePositive:" + IsMovePositive);
            if (IsMovePositive)
            {
                //如果沿着轨道正方向移动，则x轴的优先度要高于y轴的优先度
                if (!MathUtilsByXYS.ApproximatelyInView(roadVecter.x, 0))
                    Approch(ref current.x, Mathf.Min(deltaRoad, roadVecter.x), positiveX);
                else
                    Approch(ref current.y, Mathf.Min(deltaRoad, roadVecter.y), positiveY);
            }
            else
            {
                //如果沿着轨道反方向移动，则y轴的优先度要高于x轴的优先度
                if (!MathUtilsByXYS.ApproximatelyInView(roadVecter.y, 0))
                    Approch(ref current.y, Mathf.Min(deltaRoad, roadVecter.y), positiveY);
                else
                    Approch(ref current.x, Mathf.Min(deltaRoad, roadVecter.x), positiveX);
            }
            return true;
        }
        /// <summary>
        /// 修改current的值
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="delta">正差值</param>
        /// <param name="positive">增加还是减小</param>
        private void Approch(ref float current, float delta, bool positive)
        {
            current += positive ? delta : -delta;
        }
        /// <summary>
        /// 启动列车，重新计算起止点。
        /// </summary>
        /// <param name="startOfRail">铁轨的</param>
        /// <param name="endOfRail"></param>
        public void StartRun(Vector2 startOfRail, Vector2 endOfRail, bool movePositive)
        {
            Debug.Log("列车是否到站:"+IsArrived);
            //如果未到站则不能启动列车
            if (!IsArrived) return;
            railStandingOn.Start = startOfRail;
            railStandingOn.End = endOfRail;
            IsMovePositive = movePositive;
            nextStopPosition = NextStation;
            velocity = 0.0F;
            state = STATE.RUNNING;
        }
        /// <summary>
        /// 到站了
        /// </summary>
        public void Arrive()
        {
            state = STATE.ARRIVE;
        }
        /// <summary>
        /// 列车继续运行
        /// </summary>
        public void ContinueRun()
        {
            //必须暂停才能继续运行
            if (!IsStop) return;
            state = STATE.RUNNING;
        }
        /// <summary>
        /// 暂时暂停列车运行
        /// </summary>
        public void Stop()
        {
            state = STATE.STOP;
        }
        public bool IsRunning {
            get { return state == STATE.RUNNING; }
        }
        public bool IsStop {
            get { return state == STATE.STOP; }
        }
        public bool IsArrived
        {
            get { return state == STATE.ARRIVE; }
        }
        public bool IsMovable { get; }
        public bool IsMovePositive { private set; get; }
        public Vector2 NextStation {
            get {
                return IsMovePositive ? railStandingOn.End : railStandingOn.Start;
            }
        }
        public enum STATE
        {
            ARRIVE,
            RUNNING,
            STOP
        }
    }
}