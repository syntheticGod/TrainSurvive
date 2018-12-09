/*
 * 描述：这是一个地图类
 * 用来存放地图的地形，地图的宽高
 * 负责对地图地形各种属性读取判断的操作
 * 
 * 作者：王安鑫
 * 创建时间：2018/11/1 18:19:50
 * 版本：v0.1
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace WorldMap {
    public class Map : IMapForTrain{

        public static Map map = null;

        //获取Map类的单例
        public static Map GetIntanstance() {
            if (map == null) {
                map = new Map();
            }
            return map;
        }


        //大地图的宽高（rowNum为x轴地块的个数，colNum为z轴地块的个数）
        public int rowNum { get; private set; }
        public int colNum { get; private set; }

        //视野距离（默认为1，周围8个格子）
        public int viewDist = 1;

        //地图中的每个块
        public SpawnPoint[,] spowns;

        //地图上的城镇类
        public Town[,] towns;

        //设置单例模式
        private Map() {}

        //设置地块的rowNum和colNum数量
        public void initMap(int rowNum, int colNum) {
            this.rowNum = rowNum;
            this.colNum = colNum;
            spowns = new SpawnPoint[rowNum, colNum];
            for (int i = 0; i < rowNum; i++) {
                for (int j = 0; j < colNum; j++) {
                    spowns[i, j] = new SpawnPoint();
                }
            }
        }

        //移动到此地块，地块更新显示状态
        //此地块处于明亮状态，周围viewDist范围内变为已探索状态
        //如viewDist=1，周围8个格子变为已探索状态
        public bool MoveToThisSpawn(Vector2Int position) {
            if (IfInter(position) == false) {
                return false;
            }

            //更新周围8个格子内的视野
            for (int i = -viewDist; i <= viewDist; i++) {
                for (int j = -viewDist; j <= viewDist; j++) {
                    Vector2Int newPos = position + new Vector2Int(i, j);
                    if (IfInter(newPos)) {
                        //如果是玩家当前处于的位置，则它处于显示的状态，否则处于半显示状态
                        if (i == 0 && j == 0) {
                            spowns[newPos.x, newPos.y].SetViewState(SpawnPoint.SpawnViewStateEnum.VISBLE);
                        } else {
                            spowns[newPos.x, newPos.y].SetViewState(SpawnPoint.SpawnViewStateEnum.HALF_INVISIBLE);
                        }
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// 判断点是否在地图内部
        /// </summary>
        /// <param name="position">地图坐标</param>
        /// <returns>在内部返回真</returns>
        public bool IfInter(Vector2Int position) {
            if (position.x >= rowNum || position.x < 0) return false;
            if (position.y >= colNum || position.y < 0) return false;
            return true;
        }

        /// <summary>
        /// 判断地图坐标是否是轨道
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns>是铁轨返回真</returns>
        public bool IfRail(Vector2Int position) {
            return IfInter(position) &&
                spowns[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.RAIL;
        }

        /// <summary>
        /// 判断地图坐标是否是城镇
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns>是城镇返回真</returns>
        public bool IfTown(Vector2Int position) {
            return IfInter(position) &&
                spowns[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.TOWN;
        }

        /// <summary>
        /// 判断地图坐标是不是处于可见状态
        /// </summary>
        /// <param name="position"></param>
        /// <returns>如果处于可见状态</returns>
        public bool isSpawnVisible(Vector2Int position) {
            if (IfInter(position) == false) {
                return false;
            }
            return spowns[position.x, position.y].viewState != SpawnPoint.SpawnViewStateEnum.INVISIBLE;
        }

        /// <summary>
        /// 获取轨道的两端。
        /// 如果铁轨只有一个点时，两个端点会重合。
        /// </summary>
        /// <param name="railPosition">传入铁轨的地图坐标。</param>
        /// <param name="start">传出一端的地图坐标</param>
        /// <param name="end">传出另一端的地图坐标</param>
        /// <returns>false：如果指定点不是铁轨则</returns>
        public bool GetEachEndsOfRail(Vector2Int railPosition, out Vector2Int start, out Vector2Int end) {
            start = new Vector2Int();
            end = new Vector2Int();
            if (IfRail(railPosition) == false) {
                return false;
            } else {
                Vector2Int startTownPos = spowns[railPosition.x, railPosition.y].startTownPos;
                Vector2Int endTownPos = spowns[railPosition.x, railPosition.y].townPos;

                start = towns[startTownPos.x, startTownPos.y].position;
                end = towns[endTownPos.x, endTownPos.y].position;
                return true;
            }
        }

        /// <summary>
        /// 判断两座城市之间是否存在直接连通的铁轨。
        /// 并返回铁轨生成顺序
        /// </summary>
        /// <param name="townStart">城市1，如果连通则返回起点</param>
        /// <param name="townEnd">城市2，如果连通则返回终点</param>
        /// <returns>
        /// TRUE：两座城市之间存在直接连通的铁轨
        /// FALSE：(超出地图范围，任意一个不是城镇)两座城市之间不能直接连通
        /// </returns>
        public bool IfConnectedBetweenTowns(ref Vector2Int townStart, ref Vector2Int townEnd) {
            //如果这两个城镇有一个城镇不在地图内，返回false
            if (IfInter(townStart) == false || IfInter(townEnd) == false) {
                return false;
            }

            //如果这两个有一个不是城镇返回false
            if (spowns[townStart.x, townStart.y].specialTerrainType != SpawnPoint.SpecialTerrainEnum.TOWN
                || spowns[townEnd.x, townEnd.y].specialTerrainType != SpawnPoint.SpecialTerrainEnum.TOWN) {
                return false;
            }

            Vector2Int startTownPos = spowns[townStart.x, townStart.y].townPos;
            Vector2Int endTownPos = spowns[townEnd.x, townEnd.y].townPos;

            //如果起始的坐标和终点坐标相反，则将其交换
            if (startTownPos.x > endTownPos.x || startTownPos.y > endTownPos.y) {
                Utility.Swap<Vector2Int>(ref startTownPos, ref endTownPos);
                Utility.Swap<Vector2Int>(ref townStart, ref townEnd);
            }

            //返回这两个城镇是否相连
            return towns[startTownPos.x, startTownPos.y].connectTowns.Contains(towns[endTownPos.x, endTownPos.y]);
        }
    }
}

