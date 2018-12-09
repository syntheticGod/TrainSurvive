/*
 * 描述：这是一个地图生成类
 * 做地图的生成和绘画，目前只做最基本的的地形生成和绘画
 * 会通过调用子object的函数做一些特殊地形的生成和绘画
 * 
 * 负责对地图信息的读取和保存
 * isCreatMap为ture是创建一个新的地图，最后保存到文本中
 * isCreatMap为false是读取文本中的地图，并将其绘制
 * 
 * 作者：王安鑫
 * 创建时间：2018/11/1 18:19:50
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WorldMap {
    public class MapGenerate : MonoBehaviour, IMapForTrainTemp {

        //大地图的宽高（x轴和z轴地块的个数）
        public int mapWidth = 100;
        public int mapHeight = 100;

        //三种特殊地形的图标
        //湖泊
        public GameObject lakeObject;
        //火山
        public GameObject volcanoObject;
        //密林
        public GameObject forestObject;

        //将地形存放到同一object下
        private GameObject[] mapObject;
        private GameObject[] mapParent;
        public GameObject mapRootObject;

        //特殊地形的数目
        public int lakeNum = 0;
        public int volcanoNum = 0;
        public int forestNum = 0;

        public const int specialTerrainSize = 2;

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
        private TownsRailGenerate townsRailGenerate;
        //获取气候地形生成类
        private ClimateTerrainGenerate climateTerrainGenerate;

        //判定这是新游戏（生成地图）还是读取地图
        public bool isCreateMap;

        //设置起始状态为迷雾状态还是明亮状态(测试)
        public static bool isFogState = false;

        //构建地图
        private void Awake() {
            CreateModel();

            //测试，设置人物出生地点为第一个城镇
            CharacterBuild characterBuild = GameObject.Find("CharacterBuild").GetComponent<CharacterBuild>();
            characterBuild.Init(mapData.towns[0, 0].position);
        }

        //创建地图
        public void CreateModel() {
            //获取城镇铁轨脚本
            townsRailGenerate = GameObject.Find("TownsRailGenerate").GetComponent<TownsRailGenerate>();
            //获取气候地形生成脚本
            climateTerrainGenerate = GameObject.Find("ClimateTerrainGenerate").GetComponent<ClimateTerrainGenerate>();

            //对地图进行初始化处理
            mapData = Map.GetIntanstance();
            mapData.initMap(mapWidth, mapHeight);

            ////为每种地形赋予一个图标
            //mapObject = new GameObject[(int)SpawnPoint.TerrainEnum.NUM];
            //mapObject[(int)SpawnPoint.TerrainEnum.HILL] = lakeObject;
            //mapObject[(int)SpawnPoint.TerrainEnum.FOREST] = forestObject;
            //mapObject[(int)SpawnPoint.TerrainEnum.MOUNTAIN] = volcanoObject;

            ////将地图素材存放到同一object中
            //mapRootObject = new GameObject("map");
            //mapParent = new GameObject[(int)SpawnPoint.TerrainEnum.NUM];
            //mapParent[(int)SpawnPoint.TerrainEnum.HILL] = new GameObject("hills");
            //mapParent[(int)SpawnPoint.TerrainEnum.FOREST] = new GameObject("froests");
            //mapParent[(int)SpawnPoint.TerrainEnum.MOUNTAIN] = new GameObject("mountains");
            //for (int i = 0; i < (int)SpawnPoint.TerrainEnum.NUM; i++) {
            //    mapParent[i].transform.parent = mapRootObject.transform;
            //}

            //先读取地图的信息
            SaveReadMap.ReadMapInfo();
            //获取是否保存地图
            isCreateMap = SaveReadMap.isCreateMap;

            if (SaveReadMap.isCreateMap) {
                //生成特殊地形的算法
                BuildTerrain();
            }

            ////判断是不是第一次生成地形
            //if (isCreateMap) {
            //    //生成特殊地形的算法
            //    BuildTerrain();
            //} else {
            //    //读取地图的信息
            //    SaveReadMap.ReadMapInfo();
            //}

            //对地形进行绘画
            //PaintTerrain();

            //生成气候和地形
            climateTerrainGenerate.StartGenerate();

            //生成城镇，并绘画出城镇和铁轨
            townsRailGenerate.StartGenerate();

            //如果是第一次生成地图的静态数据，要将其保存
            //if (isCreateMap) {
            //    SaveReadMap.SaveStaticMapInfo();
            //}
        }

        ////生成地形
        private void BuildTerrain() { }
        private void buildTerrain() {
            //生成特殊地形
            //buildSpecialTerrain(specialTerrainSize, lakeNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_MOUNTAIN);
            //buildSpecialTerrain(specialTerrainSize, volcanoNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_WATER);
            //buildSpecialTerrain(specialTerrainSize, forestNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_TREE);
        }

        //生成特定的地形
        //private void buildSpecialTerrain(int terrainSize, int terrainNum) {
        //    Random random = new Random();

        //    //随机生成
        //    for (int i = 0; i < terrainNum; i++) {
        //        //按范围随机生成点
        //        int posx = Random.Range(0, mapData.rowNum - terrainSize);
        //        int posy = Random.Range(0, mapData.colNum - terrainSize);

        //        //检查该点是否合法
        //        bool isLegal = true;
        //        for (int j = 0; j < terrainSize && isLegal; j++) {
        //            for (int k = 0; k < terrainSize; k++) {
        //                if (mapData.spowns[posx + j, posy + k] != SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_LOAD) {
        //                    isLegal = false;
        //                    break;
        //                }
        //            }
        //        }

        //        //如果合法将该点周围布置成该环境
        //        if (isLegal == true) {
        //            for (int j = 0; j < terrainSize && isLegal; j++) {
        //                for (int k = 0; k < terrainSize; k++) {
        //                    mapData.spowns[posx + j, posy + k] = pointType;
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
            for (int i = 0; i < mapData.rowNum; i++) {
                for (int j = 0; j < mapData.colNum; j++) {
                    GameObject o = Instantiate(mapObject[(int)mapData.spowns[i, j].terrainType],
                        orign + new Vector3(spawnOffsetX * i, spawnOffsetZ * j, 0),
                        Quaternion.identity);
                    //o.transform.Rotate(90, 0, 0);
                    o.transform.parent = mapParent[(int)mapData.spowns[i, j].terrainType].transform;

                    //绑定地块的gameObject
                    mapData.spowns[i, j].SetSpawnObject(SpawnPoint.SpawnObjectEnum.TERRAIN, o);
                }
            }
        }

        /// <summary>
        /// 获取地图的X轴方块个数，和Z轴的方块个数
        /// </summary>
        /// <returns>整数向量为（X轴个数, Z轴个数）</returns>
        public Vector2Int GetMapSize() {
            return new Vector2Int(mapWidth, mapHeight);
        }

        /// <summary>
        /// 获取第一块方块中心的世界坐标
        /// </summary>
        /// <returns>浮点向量为（X轴世界坐标，Z轴世界坐标）</returns>
        public Vector2 GetMapOrigin() {
            return new Vector2(orign.x, orign.z);
        }

        /// <summary>
        /// 获取一块的长和宽
        /// </summary>
        /// <returns>浮点向量为（X轴长度，Z轴长度）</returns>
        public Vector2 GetBlockSize() {
            return new Vector2(spawnOffsetX, spawnOffsetZ);
        }
    }
}

