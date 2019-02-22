/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/22 17:55:26
 * 版本：v0.7
 */
using UnityEngine;
using System;

namespace WorldMap.Model
{
    [Serializable]
    public class PlayerMarker
    {
        private int posTeamX;
        private int posTeamY;
        private int posTrainX;
        private int posTrainY;
        public Vector2Int TrainMapPos
        {
            get { return new Vector2Int(posTrainX, posTrainY); }
            set { posTrainX = value.x; posTrainY = value.y; }
        }
        public Vector2Int TeamMapPos
        {
            get { return new Vector2Int(posTeamX, posTeamY); }
            set { posTeamX = value.x; posTeamY = value.y; }
        }
        /// <summary>
        /// 当前位置
        /// 如果是小队模式，则返回小队坐标
        /// 如果是列车模式，则返回列车坐标
        /// </summary>
        public Vector2Int MapPos
        {
            get { if (World.getInstance().ifTeamOuting) return TeamMapPos; else return TrainMapPos; }
        }
        /// <summary>
        /// 获取当前位置的城镇
        /// </summary>
        /// <returns>
        /// NULL：当前位置不存在城镇
        /// NOT NULL：当前城镇
        /// </returns>
        public TownData GetCurrentTown()
        {
            TownData town;
            World.getInstance().Towns.Find(MapPos, out town);
            return town;
        }
    }
}
