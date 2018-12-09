/*
 * 描述：这是城镇类
 * 目前该类成员只保存城镇在地图的坐标
 * 作者：王安鑫
 * 创建时间：2018/11/8 20:03:26
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WorldMap {
    public class Town {
        //城镇在地图的坐标
        public Vector2Int position { get; private set; }

        //与这个城镇相连的城镇（以此点作为起点的相连城镇）
        public List<Town> connectTowns { get; private set; }

        //如果它和下一个城镇连接，则记录它的连接路径
        public List<RailPath> railPaths;

        //如果它和下一个城镇连接，则记录它的连接路径
        public class RailPath {
            //铁轨的路径
            public List<Vector2Int> railPath;
            //x轴铁轨的个数（横）
            public int xRailNum;
            //如果有转弯的铁轨，这是转弯铁轨的角度；如果没有，则为-1
            public int railTurnAngle;
        }

        //设置Town在Map的坐标
        public Town(Vector2Int position) {
            this.position = position;

            //初始化相连列表，最大应该只有上右的相连
            connectTowns = new List<Town>(2);
            //初始化相连的铁轨列表，和连接城镇对应
            railPaths = new List<RailPath>(2);
        }

        //设置Town在地图的position
        public void SetMapPosition(Vector2Int position) {
            this.position = position;
        }

        //添加下一个城镇
        public void AddConnectTown(Town nextTown) {
            connectTowns.Add(nextTown);
        }
    }
}
