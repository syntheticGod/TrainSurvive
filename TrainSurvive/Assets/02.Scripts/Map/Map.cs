/*
 * 描述：这是一个地图类
 * 用来存放地图的地形，地图的宽高
 * 负责对地图地形各种属性读取判断的操作
 * 
 * 作者：王安鑫
 * 创建时间：2018/11/1 18:19:50
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using TTT.Utility;
using WorldBattle;
using System.Collections.Generic;
using static WorldMap.SpawnPoint;

namespace WorldMap {
    public class Map : IMapForTrain {

        public static Map map = null;

        //获取Map类的单例
        public static Map GetInstance() {
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
        private Map() { }

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
        /// 根据地图坐标获取区块坐标
        /// 条件是：假设区块的划分是根据同一纬度中城镇的个数等距划分
        /// </summary>
        /// <param name="position">地图坐标</param>
        /// <returns>
        /// 区块坐标
        /// </returns>
        public Vector2Int CalArea(Vector2Int position)
        {
            int x = (position.x * towns.GetLength(0)) / colNum;
            int y = (position.y * towns.GetLength(1)) / rowNum;
            return new Vector2Int(x, y);
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
        /// 判断是否是怪物区域
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns>
        /// TRUE：是怪物区域
        /// FALSE：不是怪物区域
        /// </returns>
        public bool IfMonsterArea(Vector2Int position) {
            return IfInter(position) &&
                spowns[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.MONSTER;
        }
        /// <summary>
        /// 判断是否是特殊战斗区域
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns>
        /// TRUE：是怪物区域
        /// FALSE：不是怪物区域
        /// </returns>
        public bool IfSpecialBattleArea(Vector2Int position)
        {
            return IfInter(position) &&
                spowns[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.SPECIAL_AREA;
        }
        /// <summary>
        /// 获取指定位置怪物级别
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns>
        /// 1、2、3：怪物的三个级别
        /// -1：该区块不是怪物区块
        /// </returns>
        public int GetMonsterLevel(Vector2Int position)
        {
            if (!IfMonsterArea(position))
                return -1;
            return spowns[position.x, position.y].monsterId;
        }
        /// <summary>
        /// 判断地块是否可以采集
        /// </summary>
        /// <param name="position"></param>
        /// <returns>
        /// TRUE：可采集
        /// FALSE：不可采集
        /// </returns>
        public bool IfCanGathered(Vector2Int position) {
            return IfInter(position) && !spowns[position.x, position.y].isGathered;
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
                MathTool.Swap<Vector2Int>(ref startTownPos, ref endTownPos);
                MathTool.Swap<Vector2Int>(ref townStart, ref townEnd);
            }

            //返回这两个城镇是否相连
            return towns[startTownPos.x, startTownPos.y].connectTowns.Contains(towns[endTownPos.x, endTownPos.y]);
        }

        /// <summary>
        /// 清除当前位置上的怪物或者是特殊区域的标记
        /// 并且清除上面的gameObject
        /// </summary>
        /// <param name="pos"></param>
        public void clearBattle(Vector2Int pos) {
            if (IfInter(pos) == false) {
                return;
            }

            switch (spowns[pos.x, pos.y].specialTerrainType) {
                case SpecialTerrainEnum.SPECIAL_AREA:
                case SpecialTerrainEnum.MONSTER: {
                    //清空标记
                    spowns[pos.x, pos.y].SetSpecialTerrain(SpecialTerrainEnum.NONE);
                    //获取对应的gameObject
                    GameObject curObject = spowns[pos.x, pos.y].getGameObject(SpawnObjectEnum.MONSTER_LEVEL);
                    //将其位置移到下面（等同于删除）
                    Vector3 curPos = curObject.transform.position;
                    curPos.z = 1;
                    curObject.transform.position = curPos;
                }
                break;
            }
        }

        /// <summary>
        /// 当前资源已经采集
        /// 标上已经采集的图标，设置标记位
        /// </summary>
        /// <param name="pos"></param>
        public void setGathered(Vector2Int pos) {
            //在地图内且当前资源未被采集
            if (IfInter(pos) == false && spowns[pos.x, pos.y].isGathered == false) {
                return;
            }

            //设置当前资源已被采集
            spowns[pos.x, pos.y].SetIsGathered(true);

            //生成对应的gameObject
            ObjectGenerate.paintIsGather(pos);
        }

        /// <summary>
        /// 生成特殊战斗
        /// 根据区块坐标，在地块中随机选取一个位置生成特殊战斗
        /// </summary>
        /// <param name="arePos">区块坐标，不是位置坐标</param>
        /// <param name="specialId">特殊战斗id</param>
        /// <returns>
        /// TRUE：生成了一个特殊地区
        /// FALSE：无法生成特殊区域，该地块没有任何的空位
        /// </returns>
        public bool generateSpecialArea(Vector2Int arePos, int specialId) {
            //获取城镇的行数和列数
            int townRowNum = towns.GetLength(0);
            int townColNum = towns.GetLength(1);

            //获取大块横向和竖向的大小
            int townRowSize = rowNum / townRowNum;
            int townColSize = colNum / townColNum;

            //获取当前大块范围内可被作为特殊战斗的坐标
            List<Vector2Int> candidatePos = new List<Vector2Int>();

            //获取当前大块的范围
            int starti = arePos.x * townRowSize;
            int endi = (arePos.x + 1) * townRowSize;
            int startj = arePos.y * townRowSize;
            int endj = (arePos.y + 1) * townRowSize;
            //遍历大块的每个元素
            for (int i = starti; i < endi; i++) {
                for (int j = startj; j < endj; j++) {
                    //如果不为空，跳过
                    if (map.spowns[i, j].specialTerrainType != SpecialTerrainEnum.NONE) {
                        continue;
                    }

                    //将候选地块放入队列中
                    candidatePos.Add(new Vector2Int(i, j));
                }
            }

            //如果没有空余位置生成特殊地块（基本不可能）
            if (candidatePos.Count == 0) {
                return false;
            }

            //选取其中一个点作为特殊地区
            int randIndex = Random.Range(0, candidatePos.Count);
            Vector2Int battleMapPos = candidatePos[randIndex];
            //设置特殊区域和区域id
            map.spowns[battleMapPos.x, battleMapPos.y].SetSpecialTerrain(SpecialTerrainEnum.SPECIAL_AREA);
            map.spowns[battleMapPos.x, battleMapPos.y].SetMonsterId(specialId);

            //生成对应的gameObject
            ObjectGenerate.paintSpecialArea(battleMapPos);

            return true;
        }
    }
}

