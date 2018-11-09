/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/9 20:41:12
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap
{
    public class Rail
    {
        public Vector2 Start { set; get; }
        public Vector2 End { set; get; }
        //拐点，从起点开始
        public List<Vector2> inflectionPoints;
        public Rail(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            inflectionPoints = new List<Vector2>();
            //TODO：默认拐点
        }
        public float CalTotalRoad()
        {
            //TODO：计算总路程
            return 0.0f;
        }
    }
}