/*
 * 描述：这是一个地图生成算法
 * 
 * 作者：王安鑫
 * 创建时间：2018/11/1 18:19:50
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerate : MonoBehaviour {

    //大地图的宽高（x轴和z轴地块的个数）
    public int mapWidth = 100;
    public int mapHeight = 100;

    //四种地形的图标
    public GameObject plainObject;
    public GameObject hillObject;
    public GameObject mountainObject;
    public GameObject froestObject;

    //将地形存放到同一object下
    private GameObject[] mapObject;
    private GameObject[] mapParent;
    public GameObject mapRootObject;

    public int loadNum;
    public int waterNum = 0;
    public int mountainNum = 0;
    public int treeNum = 0;

    public int mountainSize = 3;
    public int waterSize = 3;
    public int treeSize = 3;

    //地块大小
    [System.NonSerialized]
    public int spawnOffsetX = 1;
    [System.NonSerialized]
    public int spawnOffsetZ = 1;
    //地图的起始点
    [System.NonSerialized]
    public Vector3 orign = new Vector3();

    //地图类
    public Map mapData;

    //获取城镇生成类
    TownsRailGenerate townsRailGenerate;

    //构建地图
    private void Awake() {
        CreateModel();
    }

    //创建地图
    public void CreateModel() {
        //获取城镇铁轨脚本
        townsRailGenerate = GameObject.Find("townsRailGenerate").GetComponent<TownsRailGenerate>();

        //对地图进行初始化处理
        mapData = new Map(mapWidth, mapHeight, spawnOffsetX, spawnOffsetZ);
        mapData.data = new SpawnPoint[mapWidth, mapHeight];
        for (int i = 0; i < mapData.width; i++) {
            for (int j = 0; j < mapData.length; j++) {
                mapData.data[i, j] = new SpawnPoint();
            }
        }

        //为每种地形赋予一个图标
        mapObject = new GameObject[(int)SpawnPoint.TerrainEnum.NUM];
        mapObject[(int)SpawnPoint.TerrainEnum.PLAIN] = plainObject;
        mapObject[(int)SpawnPoint.TerrainEnum.HILL] = hillObject;
        mapObject[(int)SpawnPoint.TerrainEnum.FOREST] = froestObject;
        mapObject[(int)SpawnPoint.TerrainEnum.MOUNTAIN] = mountainObject;

        //将地图素材存放到同一object中
        mapRootObject = new GameObject("map");
        mapParent = new GameObject[(int)SpawnPoint.TerrainEnum.NUM];
        mapParent[(int)SpawnPoint.TerrainEnum.PLAIN] = new GameObject("plains");
        mapParent[(int)SpawnPoint.TerrainEnum.HILL] = new GameObject("hills");
        mapParent[(int)SpawnPoint.TerrainEnum.FOREST] = new GameObject("froests");
        mapParent[(int)SpawnPoint.TerrainEnum.MOUNTAIN] = new GameObject("mountains");
        for (int i = 0; i < (int)SpawnPoint.TerrainEnum.NUM; i++) {
            mapParent[i].transform.parent = mapRootObject.transform;
        }

        //生成特殊地形的算法
        //buildTerrain();

        //生成城镇
        townsRailGenerate.StartGenerate();

        PaintTerrain();
    }

    ////生成地形
    //private void buildTerrain() {
    //    //生成特殊地形
    //    buildSpecialTerrain(mountainSize, mountainNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_MOUNTAIN);
    //    buildSpecialTerrain(waterSize, waterNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_WATER);
    //    buildSpecialTerrain(treeSize, treeNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_TREE);
    //}

    ////生成特定的地形
    //private void buildSpecialTerrain(int terrainSize, int terrainNum, SPAWN_POINT_TYPE pointType) {
    //    Random random = new Random();

    //    //随机生成
    //    for (int i = 0; i < terrainNum; i++) {
    //        //按范围随机生成点
    //        int posx = Random.Range(0, mapData.width - terrainSize);
    //        int posy = Random.Range(0, mapData.length - terrainSize);

    //        //检查该点是否合法
    //        bool isLegal = true;
    //        for (int j = 0; j < terrainSize && isLegal; j++) {
    //            for (int k = 0; k < terrainSize; k++) {
    //                if (mapData.data[posx + j, posy + k] != SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_LOAD) {
    //                    isLegal = false;
    //                    break;
    //                }
    //            }
    //        }

    //        //如果合法将该点周围布置成该环境
    //        if (isLegal == true) {
    //            for (int j = 0; j < terrainSize && isLegal; j++) {
    //                for (int k = 0; k < terrainSize; k++) {
    //                    mapData.data[posx + j, posy + k] = pointType;
    //                }
    //            }
    //        }
    //        //否则重新生成随机的点
    //        else {
    //            i--;
    //        }

    //    }
    //}

    //绘制四种地形图标
    private void PaintTerrain() {
        for (int i = 0; i < mapData.width; i++) {
            for (int j = 0; j < mapData.length; j++) {
                GameObject o = Instantiate(mapObject[(int)mapData.data[i, j].terrainType],
                    orign + new Vector3(mapData.offsetX * i, 0, mapData.offsetZ * j),
                    Quaternion.identity);
                o.transform.Rotate(90, 0, 0);
                o.transform.parent = mapParent[(int)mapData.data[i, j].terrainType].transform;
            }
        }
    }
}
