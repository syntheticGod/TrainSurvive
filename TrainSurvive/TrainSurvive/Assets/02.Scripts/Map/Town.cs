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

        //设置Town在Map的坐标
        public Town(Vector2Int position) {
            this.position = position;

            //初始化相连列表，最大应该只有上右的相连
            connectTowns = new List<Town>(2);
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
