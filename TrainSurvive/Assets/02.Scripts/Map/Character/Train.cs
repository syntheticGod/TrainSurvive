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

        private StateType state;
        private Rail railStandingOn;
        private Vector2 nextStopPosition;
        public Train(bool movable)
        {
            railStandingOn = new Rail(Vector2Int.zero, Vector2Int.zero);
            IsMovable = movable;
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
                Stop();
                return false;
            }
            float remainedRoad = roadVecter.x + roadVecter.y;
            //remainedRoad一定是大于0的
            float deltaRoad = remainedRoad - Mathf.SmoothDamp(remainedRoad, 0, ref velocity, smoothTime);
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
        public void Stop()
        {
            Debug.Log("到达目的地");
            state = StateType.STOP;
        }
        private void Approch(ref float current, float delta, bool positive)
        {
            current += positive ? delta : -delta;
        }
        public void StartRun()
        {
            nextStopPosition = NextStation;
            velocity = 0.0F;
            state = StateType.RUNNING;
        }
        public void SetStartStationOfRail(Vector2 station)
        {
            railStandingOn.Start = station;
        }
        public void SetEndStationOfRail(Vector2 station)
        {
            railStandingOn.End = station;
        }
        public bool IsRunning { get { return state == StateType.RUNNING; } }
        public bool IsStop { get { return state == StateType.STOP; } }
        public bool IsMovable { get; }
        public bool IsMovePositive { set; get; }
        public Vector2 NextStation {
            get {
                return IsMovePositive ? railStandingOn.End : railStandingOn.Start;
            }
        }
        public enum StateType
        {
            STOP,
            RUNNING
        }
    }
}