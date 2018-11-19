/*
 * 描述：这是用来保存和读取地图的类
 * 作者：王安鑫
 * 创建时间：2018/11/17 9:15:59
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WorldMap {
    public class SaveReadMap : MonoBehaviour {

        //保存地图数据的文件
        private const string MAP_STATIC_FILE_NAME = "MAP_STATIC_INFO.txt";
        //保存地图动态数据的文件
        private const string MAP_DYNAMIC_FILE_NAME = "MAP_DYNAMIC_INFO.txt";

        //保存地图信息的委托函数
        public delegate void SaveMapDelegate(StreamWriter sw, ref Map map);
        //读取地图信息的委托函数
        public delegate void ReadMapDelegate(String mapData, ref Map map);

        /** 保存地图的静态信息
         * 地图的地形信息
         * 地图的城镇位置信息
         * 
         * 地图的动态信息
         * 地图的迷雾状态
         */
        public static void SaveStaticMapInfo(ref Map map) {
            //保存地图的静态信息
            SaveMapInfo(MAP_STATIC_FILE_NAME, ref map, new SaveMapDelegate(SaveStaticMap));
        }

        /**地图的动态信息
         * 地图的迷雾状态
         */
        public static void SaveDynamicMapInfo(ref Map map) {
            //保存地图的动态信息
            SaveMapInfo(MAP_DYNAMIC_FILE_NAME, ref map, new SaveMapDelegate(SaveDynamicMap));
        } 

        /** 读取地图的静态和动态信息
         */
        public static void ReadMapInfo(ref Map map) {
            //保存地图的静态信息
            ReadMapInfo(MAP_STATIC_FILE_NAME, ref map, new ReadMapDelegate(ReadStaticMap));
            //保存地图的动态信息
            ReadMapInfo(MAP_DYNAMIC_FILE_NAME, ref map, new ReadMapDelegate(ReadDynamicMap));
        }

        /// <summary>
        /// 保存地图的信息
        /// 通过pathName找到对应的文件，调用委托函数，保存静态或动态的信息
        /// </summary>
        /// <param name="pathName">保存的文件名</param>
        /// <param name="map">地图</param>
        /// <param name="saveMap">保存静态数据或动态数据</param>
        private static void SaveMapInfo(string pathName, ref Map map, SaveMapDelegate saveMap) {
            //文件流信息
            StreamWriter sw;
            FileInfo t = new FileInfo(Application.persistentDataPath + "//" + pathName);
            if (!t.Exists) {
                //如果此文件不存在则创建
                sw = t.CreateText();
            } else {
                //如果此文件存在则打开
                t.Delete();
                sw = t.CreateText();
            }

            //通过map提供的方法存入地图数据
            saveMap(sw, ref map);

            //关闭流
            sw.Close();
            //销毁流
            sw.Dispose();
        }

        //将地图数据保存成一系列字符串
        private static void SaveStaticMap(StreamWriter sw, ref Map map) {
            if (map.rowNum == 0 || map.colNum == 0) {
                Debug.Log("地图保存失败！width或length为0");
                return;
            }

            //先写大地图的宽高
            sw.WriteLine(map.rowNum + "," + map.colNum);

            //写入普通地形的数据
            for (int i = 0; i < map.rowNum; i++) {
                string row = ((int)map.spowns[i, 0].terrainType).ToString();
                for (int j = 1; j < map.colNum; j++) {
                    row += "," + (int)map.spowns[i, j].terrainType;
                }
                sw.WriteLine(row);
            }

            //写入城镇的宽高
            int rowNum = map.towns.GetLength(0);
            int colNum = map.towns.GetLength(1);
            sw.WriteLine(rowNum + "," + colNum);
            //写入城镇的位置信息
            for (int i = 0; i < rowNum; i++) {
                string row = map.towns[i, 0].position.x + "," + map.towns[i, 0].position.y;
                for (int j = 1; j < colNum; j++) {
                    row += "," + map.towns[i, j].position.x + "," + map.towns[i, j].position.y;
                }
                sw.WriteLine(row);
            }
        }

        //保存地图的动态信息
        //迷雾信息
        private static void SaveDynamicMap(StreamWriter sw, ref Map map) {
            if (map.rowNum == 0 || map.colNum == 0) {
                Debug.Log("地图保存失败！width或length为0");
                return;
            }

            //先写大地图的宽高
            sw.WriteLine(map.rowNum + "," + map.colNum);

            //写入普通地形的数据
            for (int i = 0; i < map.rowNum; i++) {
                string row = ((int)map.spowns[i, 0].viewState).ToString();
                for (int j = 1; j < map.colNum; j++) {
                    row += "," + (int)map.spowns[i, j].viewState;
                }
                sw.WriteLine(row);
            }
        }

        /** 读取地图的静态信息
         */
        private static void ReadMapInfo(string pathName, ref Map map, ReadMapDelegate readMap) {
            //使用流的形式读取
            StreamReader sr = null;
            try {
                sr = File.OpenText(Application.persistentDataPath + "//" + pathName);
            } catch (Exception e) {
                //路径与名称未找到文件则直接返回空
                Debug.Log(e.ToString());
                return;
            }

            //读取map的数据
            readMap(sr.ReadToEnd(), ref map);

            //关闭流
            sr.Close();
            //销毁流
            sr.Dispose();
        }

        //读取地图的静态数据
        private static void ReadStaticMap(string mapData, ref Map map) {
            // 将空元素删除的选项
            System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;

            // 用换行符将每行切割开
            string[] lines = mapData.Split(new char[] { '\r', '\n' }, option);

            // 用“,”将每个字符分割开
            char[] spliter = new char[1] { ',' };

            // 第一行地图的尺寸
            int lineIndex = 0;
            string[] sizewh = lines[lineIndex++].Split(spliter, option);
            //map.initMap(int.Parse(sizewh[0]), int.Parse(sizewh[1]));

            //获取地形数据
            for (int lineCnt = 0; lineCnt < map.rowNum; lineCnt++) {
                // 用“,”将每个字符分割开
                string[] curLineData = lines[lineIndex++].Split(spliter, option);
                for (int col = 0; col < map.colNum; col++) {
                    map.spowns[lineCnt, col].SetTerrainEnum((SpawnPoint.TerrainEnum)int.Parse(curLineData[col]));
                }
            }

            // 读取城镇的尺寸
            sizewh = lines[lineIndex++].Split(spliter, option);
            int townsRowNum = int.Parse(sizewh[0]);
            int townsColNum = int.Parse(sizewh[1]);

            //初始化城镇位置数组
            map.towns = new Town[townsRowNum, townsColNum];

            //获取城镇数据
            for (int lineCnt = 0; lineCnt < townsRowNum; lineCnt++) {
                // 用“,”将每个字符分割开
                string[] curLineData = lines[lineIndex++].Split(spliter, option);
                for (int col = 0; col < townsColNum; col++) {
                    int posx = int.Parse(curLineData[col * 2]);
                    int posz = int.Parse(curLineData[col * 2 + 1]);
                    //创建城镇类
                    map.towns[lineCnt, col] = new Town(new Vector2Int(posx, posz));
                }
            }
        }

        //读取地图的动态数据
        private static void ReadDynamicMap(string mapData, ref Map map) {
            // 将空元素删除的选项
            System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;

            // 用换行符将每行切割开
            string[] lines = mapData.Split(new char[] { '\r', '\n' }, option);

            // 用“,”将每个字符分割开
            char[] spliter = new char[1] { ',' };

            // 第一行地图的尺寸
            int lineIndex = 0;
            string[] sizewh = lines[lineIndex++].Split(spliter, option);
            //map.initMap(int.Parse(sizewh[0]), int.Parse(sizewh[1]));

            //获取迷雾的数据
            for (int lineCnt = 0; lineCnt < map.rowNum; lineCnt++) {
                // 用“,”将每个字符分割开
                string[] curLineData = lines[lineIndex++].Split(spliter, option);
                for (int col = 0; col < map.colNum; col++) {
                    map.spowns[lineCnt, col].SetViewState((SpawnPoint.SpawnViewStateEnum)int.Parse(curLineData[col]));

                    //Debug.Log(map.spowns[lineCnt, col].viewState);
                }
            }
        }
    }
}

