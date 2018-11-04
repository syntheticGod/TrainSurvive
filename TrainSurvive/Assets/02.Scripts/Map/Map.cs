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

public class Map {

    //大地图的宽高（x轴和z轴地块的个数）
    public int rowNum { get; private set; }
    public int colNum { get; private set; }

    //地图中的每个块
    public SpawnPoint[,] data;

    //各个城镇的位置
    public Vector2[,] townsPos;

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
        int rowNum = townsPos.GetLength(0);
        int colNum = townsPos.GetLength(1);
        sw.WriteLine(rowNum + "," + colNum);
        //写入城镇的位置信息
        for (int i = 0; i < rowNum; i++) {
            string row = townsPos[i, 0].x + "," + townsPos[i, 0].y;
            for (int j = 1; j < colNum; j++) {
                row += "," + townsPos[i, j].x + "," + townsPos[i, j].y;
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
        townsPos = new Vector2[townsRowNum, townsRowNum];

        //获取城镇数据
        for (int lineCnt = 0; lineCnt < townsRowNum; lineCnt++) {
            // 用“,”将每个字符分割开
            string[] curLineData = lines[lineIndex++].Split(spliter, option);
            for (int col = 0; col < townsColNum; col++) {
                int posx = int.Parse(curLineData[col * 2]);
                int posz = int.Parse(curLineData[col * 2 + 1]);
                townsPos[lineCnt, col] = new Vector2(posx, posz);
            }
        }
    }
}
