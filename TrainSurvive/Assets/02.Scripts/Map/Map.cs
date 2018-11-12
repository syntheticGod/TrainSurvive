﻿/*
 * 描述：这是一个地图类
 * 用来存放地图的地形，地图的宽高
 * 负责对地图数据进行存储和读取
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

        //大地图的宽高（rowNum为x轴的个数，colNum为z轴地块的个数）
        public int rowNum { get; private set; }
        public int colNum { get; private set; }

        //地图中的每个块
        public SpawnPoint[,] spowns;

        //城镇类
        public Town[,] towns;

        //对地图属性做初始化
        public Map(int rowNum, int colNum) {
            this.rowNum = rowNum;
            this.colNum = colNum;
        }

        //将地图数据保存成一系列字符串
        public void SaveMap(StreamWriter sw) {
            if (this.rowNum == 0 || this.colNum == 0) {
                Debug.Log("地图保存失败！width或length为0");
                return;
            }

            //先写大地图的宽高
            sw.WriteLine(this.rowNum + "," + this.colNum);

            //写入普通地形的数据
            for (int i = 0; i < this.rowNum; i++) {
                string row = ((int)spowns[i, 0].terrainType).ToString();
                for (int j = 1; j < this.colNum; j++) {
                    row += "," + (int)spowns[i, j].terrainType;
                }
                sw.WriteLine(row);
            }

            //写入城镇的宽高
            int rowNum = towns.GetLength(0);
            int colNum = towns.GetLength(1);
            sw.WriteLine(rowNum + "," + colNum);
            //写入城镇的位置信息
            for (int i = 0; i < rowNum; i++) {
                string row = towns[i, 0].position.x + "," + towns[i, 0].position.y;
                for (int j = 1; j < colNum; j++) {
                    row += "," + towns[i, j].position.x + "," + towns[i, j].position.y;
                }
                sw.WriteLine(row);
            }
        }

        //读取地图数据
        public void ReadMap(string mapData) {
            // 将空元素删除的选项
            System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;

            // 用换行符将每行切割开
            string[] lines = mapData.Split(new char[] { '\r', '\n' }, option);

            // 用“,”将每个字符分割开
            char[] spliter = new char[1] { ',' };

            // 第一行地图的尺寸
            int lineIndex = 0;
            string[] sizewh = lines[lineIndex++].Split(spliter, option);
            this.rowNum = int.Parse(sizewh[0]);
            this.colNum = int.Parse(sizewh[1]);

            //获取地形数据
            for (int lineCnt = 0; lineCnt < this.rowNum; lineCnt++) {
                // 用“,”将每个字符分割开
                string[] curLineData = lines[lineIndex++].Split(spliter, option);
                for (int col = 0; col < this.colNum; col++) {
                    spowns[lineCnt, col].SetTerrainEnum((SpawnPoint.TerrainEnum)int.Parse(curLineData[col]));
                }
            }

            // 读取城镇的尺寸
            sizewh = lines[lineIndex++].Split(spliter, option);
            int townsRowNum = int.Parse(sizewh[0]);
            int townsColNum = int.Parse(sizewh[1]);

            //初始化城镇位置数组
            towns = new Town[townsRowNum, townsColNum];

            //获取城镇数据
            for (int lineCnt = 0; lineCnt < townsRowNum; lineCnt++) {
                // 用“,”将每个字符分割开
                string[] curLineData = lines[lineIndex++].Split(spliter, option);
                for (int col = 0; col < townsColNum; col++) {
                    int posx = int.Parse(curLineData[col * 2]);
                    int posz = int.Parse(curLineData[col * 2 + 1]);
                    //创建城镇类
                    towns[lineCnt, col]= new Town(new Vector2Int(posx, posz));
                }
            }
        }

        //更新地图地块的移动显示
        public bool MoveToThisSpawn(Vector2Int position) {
            if (IfInter(position) == false) {
                return false;
            }

            //更新周围8个格子内的视野
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
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
        /// <returns></returns>
        public bool IfRail(Vector2Int position) {
            return IfInter(position) &&
                spowns[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.RAIL;
        }

        /// <summary>
        /// 判断地图坐标是否是城镇
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns></returns>
        public bool IfTown(Vector2Int position) {
            return IfInter(position) &&
                spowns[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.TOWN;
        }

        /// <summary>
        /// 判断地图坐标是不是处于可见状态
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
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

