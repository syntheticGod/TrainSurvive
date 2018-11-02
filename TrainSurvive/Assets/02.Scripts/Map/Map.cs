using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map {

    //大地图的宽高（x轴和z轴地块的个数）
    public int width { get;}
    public int length { get; }

    //每个地块的大小
    public int offsetX { get; }
    public int offsetZ { get; }

    //地图中的每个块
    public SpawnPoint[,] data;

    //各个城镇的位置
    public Vector2[,] townsPos;

    //对地图属性做初始化
    public Map(int width, int length, int offsetX, int offsetZ) {
        this.width = width;
        this.length = length;
        this.offsetX = offsetX;
        this.offsetZ = offsetZ;
    }
}
