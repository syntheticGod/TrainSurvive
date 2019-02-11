/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/9 20:41:12
 * 版本：v0.1
 */

using UnityEngine;
using System.Collections.Generic;

using TTT.Utility;
using TTT.Resource;

namespace WorldMap.Model
{
    public class Rail
    {
        public Vector2 Start { get { return inflectionPoints[0]; } }
        public Vector2 End { get { return inflectionPoints[inflectionPoints.Count - 1]; } }
        /// <summary>
        /// 铁轨拐点的个数，包括起点和终点
        /// </summary>
        public int Count { get { return inflectionPoints.Count; } }
        //拐点，从起点开始
        private List<Vector2> inflectionPoints;
        public Vector2 GetInflection(int index)
        {
            return inflectionPoints[index];
        }
        public Rail(Vector2 start, Vector2 end)
        {
            inflectionPoints = new List<Vector2>();
            //当前铁轨生成算法的默认拐点
            inflectionPoints.Add(start);
            //start和end在一条直线上
            if (!(Mathf.Approximately(start.x - end.x, 0) || Mathf.Approximately(start.y - end.y, 0)))
                inflectionPoints.Add(new Vector2(end.x, start.y));
            inflectionPoints.Add(end);
        }
        public float CalTotalRoad()
        {
            float sum = 0.0F;
            for (int i = 0; i < inflectionPoints.Count - 1; i++)
                sum += CalRailSegmentRoad(inflectionPoints[i + 1], inflectionPoints[i]);
            return sum;
        }
        /// <summary>
        /// 精确计算点position到铁轨终点的距离
        /// </summary>
        /// <param name="position">坐标点</param>
        /// <param name="movePositive">正向移动</param>
        /// <param name="remanentRoad">剩余路程</param>
        /// <returns>
        /// TRUE：点在铁轨上
        /// FALSE：点不在铁轨上
        /// </returns>
        public bool CalRemanentRoad(Vector2 position, bool movePositive, ref float remanentRoad)
        {
            remanentRoad = 0.0F;
            if (MathTool.Approximately(position, movePositive ? End : Start))
                return true;
            int start = 0, end = 0;
            if(!FindRailByPos(position, movePositive, ref start, ref end))
                return false;
            if (movePositive)
            {
                remanentRoad += CalRailSegmentRoad(position, inflectionPoints[start + 1]);
                int nloop = inflectionPoints.Count - 1;
                for (int i = start + 1; i < nloop; ++i)
                    remanentRoad += CalRailSegmentRoad(inflectionPoints[i], inflectionPoints[i + 1]);
            }
            else
            {
                remanentRoad += CalRailSegmentRoad(position, inflectionPoints[start - 1]);
                for (int i = start - 1; i > 0; --i)
                    remanentRoad += CalRailSegmentRoad(inflectionPoints[i], inflectionPoints[i - 1]);
            }
            return true;
        }
        /// <summary>
        /// 获得铁轨上距离position的路程为delta的点，positive选择正方向或者负方向
        /// </summary>
        /// <param name="position">当前位置</param>
        /// <param name="delta">移动距离，返回实际的距离</param>
        /// <param name="positive">是否是铁轨正向</param>
        /// <param name="passCenterOfBlock">返回是否经过块的中心点</param>
        /// <param name="arrived">返回是否到达终点</param>
        /// <returns>
        /// 如果position在铁轨上，则返回距离position路程为delta的方向为positive的点。
        /// 吐过position不在铁轨上，则返回position。
        /// </returns>
        public Vector2 CalNextPosition(Vector2 position, ref float delta, bool positive, out bool passCenterOfBlock, out bool arrived)
        {
            passCenterOfBlock = false;
            //寻找指定轨道。（定义：一个节点是一条铁轨的起点。）
            int start = 0, end = 0;
            //未找到处理（position不在铁轨上，即未找到方向。）
            if (!FindRailByPos(position, positive, ref start, ref end))
            {
                delta = 0;
                arrived = true;
                return position;
            }
            float remanentRoad = Mathf.Abs(inflectionPoints[end].x - position.x) + Mathf.Abs(inflectionPoints[end].y - position.y);
            //错误警告：remainRoad等于0时，会导致无限递归。
            if (Mathf.Approximately(remanentRoad, 0F))
            {
                Debug.LogError("remainRoad 不应该等于0，" +
                    "轨路段：" + inflectionPoints[start] + " => " + inflectionPoints[end] +
                    "当前位置：" + position);
            }
            else
            {
                //刚好抵达终点处理
                if (MathTool.ApproximatelyInView(delta, remanentRoad))
                {
                    Debug.Log("到达拐点或终点——刚好");
                    passCenterOfBlock = true;
                    delta = remanentRoad;
                    arrived = true;
                    return inflectionPoints[end];
                }
                //遇到特殊点处理（超过终点、超过拐点）
                if (remanentRoad < delta)
                {
                    //如果已经到达最后则直接返回。
                    if (end == 0 || end == inflectionPoints.Count - 1)
                    {
                        Debug.Log("达到终点——阻止超过终点");
                        passCenterOfBlock = true;
                        delta = remanentRoad;
                        arrived = true;
                        return inflectionPoints[end];
                    }
                    //NOTE：递归实现，可能发生StackOverflow，或卡死。
                    float remainDelta = delta - remanentRoad;
                    Vector2 nextPosition = CalNextPosition(inflectionPoints[end], ref remainDelta, positive, out passCenterOfBlock, out arrived);
                    //delta等于当前所走的路程 加上 拐弯后所走的路程
                    delta = remanentRoad + remainDelta;
                    Debug.Log("遇到拐点：" + inflectionPoints[end]);
                    //经过拐点
                    passCenterOfBlock = true;
                    return nextPosition;
                }
            }
            //经过中间节点（不包括 起点、终点、拐点）
            Vector2 direction = inflectionPoints[end] - inflectionPoints[start];
            Vector2 nextStep = position + direction.normalized * delta;
            passCenterOfBlock = IfPosOnTheRail(position, nextStep, StaticResource.FormatBlock(position));
            arrived = false;
            return nextStep;
        }
        /// <summary>
        /// 精确判断position是否在Start与End组成
        /// 不包括终点
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IfPosOnTheRail(Vector2 start, Vector2 end, Vector2 position)
        {
            return MathTool.IfBetweenLeft(start.x, end.x, position.x) &&
                    MathTool.IfBetweenLeft(start.y, end.y, position.y);
        }
        /// <summary>
        /// 通过坐标点查找铁轨路段
        /// 起点是一条路段的开始，不包括终点
        /// </summary>
        /// <param name="position">判别点</param>
        /// <param name="positive">判别方向</param>
        /// <param name="start">铁轨路段的列车运行起点</param>
        /// <param name="end">铁轨路段的列车运行终点</param>
        /// <returns>
        /// TRUE：铁轨路段的起点索引
        /// FALSE：点不在所有铁轨路段上
        /// </returns>
        private bool FindRailByPos(Vector2 position, bool positive, ref int start, ref int end)
        {
            int nloop = inflectionPoints.Count - 1;
            for (int i = 0; i < nloop; i++)
            {
                start = i;//一条铁轨路段的起点
                end = i + 1;//一条铁轨路段的终点
                if (!positive)
                {
                    start = i + 1;
                    end = i;
                }
                if (IfPosOnTheRail(inflectionPoints[start], inflectionPoints[end], position))
                    return true;
            }
            Debug.LogWarning("未在铁轨："+Start+"=>"+End+" 上找到坐标" + position);
            return false;
        }
        /// <summary>
        /// 计算铁轨一段的长度
        /// 注意：两个点必须才一条水平线上，或垂直先上。
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns>
        /// 路段长度
        /// </returns>
        private float CalRailSegmentRoad(Vector2 pos1, Vector2 pos2)
        {
            return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
        }
    }
}