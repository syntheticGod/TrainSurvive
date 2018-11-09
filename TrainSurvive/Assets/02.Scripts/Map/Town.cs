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

        //设置Town在Map的坐标
        public Town(Vector2Int position) {
            this.position = position;
        }

        //设置Town在地图的position
        public void SetMapPosition(Vector2Int position) {
            this.position = position;
        }
    }
}
