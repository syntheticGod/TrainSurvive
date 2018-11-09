/*
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
using UnityEngine;

namespace WorldMap {
    public class Map : IMapForTrain{

        //大地图的宽高（rowNum为x轴的个数，colNum为z轴地块的个数）
        public int rowNum { get; private set; }
        public int colNum { get; private set; }

        //地图中的每个块
        public SpawnPoint[,] data;

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
                string row = ((int)data[i, 0].terrainType).ToString();
                for (int j = 1; j < this.colNum; j++) {
                    row += "," + (int)data[i, j].terrainType;
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
                    data[lineCnt, col].SetTerrainEnum((SpawnPoint.TerrainEnum)int.Parse(curLineData[col]));
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
                    towns[lineCnt, col]= new Town(new Vector2Int(posx, posz));
                }
            }
        }
        
        /// <summary>
        /// 判断点是否在地图内部
        /// </summary>
        /// <param name="position">地图坐标</param>
        /// <returns>在内部返回真</returns>
        public bool IfInter(Vector2Int position)
        {
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
                data[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.RAIL;
        }
        /// <summary>
        /// 判断地图坐标是否是城镇
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns></returns>
        public bool IfTown(Vector2Int position) {
            return IfInter(position) &&
                data[position.x, position.y].specialTerrainType == SpawnPoint.SpecialTerrainEnum.TOWN;
        }
        /// <summary>
        /// 获取轨道的两端。
        /// 如果铁轨只有一个点时，两个端点会重合。
        /// </summary>
        /// <param name="railPosition">传入铁轨的地图坐标。</param>
        /// <param name="start">传出一端的地图坐标</param>
        /// <param name="end">传出另一端的地图坐标</param>
        /// <returns>false：如果指定点不是铁轨则</returns>
        public bool GetEachEndsOfRail(Vector2Int railPosition, out Vector2Int start, out Vector2Int end)
        {
            start = new Vector2Int();
            end = new Vector2Int();
            if (IfRail(railPosition) == false) {
                return false;
            } else {
                start = data[railPosition.x, railPosition.y].startTownPos;
                end = data[railPosition.x, railPosition.y].townPos;
                return true;
            }
        }
    }
}

