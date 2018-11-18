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

namespace WorldMap {
    public class MapGenerate : MonoBehaviour, IMapForTrainTemp {

        //大地图的宽高（x轴和z轴地块的个数）
        public int mapWidth = 100;
        public int mapHeight = 100;

        //四种地形的图标
        public GameObject plainObject;
        public GameObject hillObject;
        public GameObject mountainObject;
        public GameObject forestObject;

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
        private TownsRailGenerate townsRailGenerate;

        //判定这是新游戏（生成地图）还是读取地图
        public bool isCreateMap = true;

        //设置起始状态为迷雾状态还是明亮状态(测试)
        public static bool isFogState = true;

        //提供给外界Map类
        public Map GetMap() {
            return mapData;
        }

        //构建地图
        private void Awake() {
            CreateModel();

            //测试，设置人物出生地点为第一个城镇
            CharacterBuild characterBuild = GameObject.Find("CharacterBuild").GetComponent<CharacterBuild>();
            characterBuild.Init(mapData.towns[0, 0].position);
        }

        //保存时间的时间间隔
        public float SaveTimeInterval = 3.0f;
        //当前的时间
        private float curTime = 5.0f;
        //测试，3s保存一次动态数据
        private void Update() {
            curTime -= Time.deltaTime;
            if (curTime < 0) {
                curTime = SaveTimeInterval;
                SaveReadMap.SaveDynamicMapInfo(ref mapData);
            }
        }

        //创建地图
        public void CreateModel() {
            //获取城镇铁轨脚本
            townsRailGenerate = GameObject.Find("townsRailGenerate").GetComponent<TownsRailGenerate>();

            //对地图进行初始化处理
            mapData = new Map(mapWidth, mapHeight);
            mapData.spowns = new SpawnPoint[mapWidth, mapHeight];
            for (int i = 0; i < mapWidth; i++) {
                for (int j = 0; j < mapHeight; j++) {
                    mapData.spowns[i, j] = new SpawnPoint();
                }
            }

            //为每种地形赋予一个图标
            mapObject = new GameObject[(int)SpawnPoint.TerrainEnum.NUM];
            mapObject[(int)SpawnPoint.TerrainEnum.PLAIN] = plainObject;
            mapObject[(int)SpawnPoint.TerrainEnum.HILL] = hillObject;
            mapObject[(int)SpawnPoint.TerrainEnum.FOREST] = forestObject;
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

            //判断是不是第一次生成地形
            if (isCreateMap) {
                //生成特殊地形的算法
                BuildTerrain();
            } else {
                //读取地图的信息
                SaveReadMap.ReadMapInfo(ref mapData);
            }

            //对地形进行绘画
            PaintTerrain();

            //生成城镇，并绘画出城镇和铁轨
            townsRailGenerate.StartGenerate();

            //如果是第一次生成地图的静态数据，要将其保存
            if (isCreateMap) {
                SaveReadMap.SaveStaticMapInfo(ref mapData);
            }
        }

        ////生成地形
        private void BuildTerrain() { }
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
            for (int i = 0; i < mapData.rowNum; i++) {
                for (int j = 0; j < mapData.colNum; j++) {
                    GameObject o = Instantiate(mapObject[(int)mapData.spowns[i, j].terrainType],
                        orign + new Vector3(spawnOffsetX * i, 0, spawnOffsetZ * j),
                        Quaternion.identity);
                    o.transform.Rotate(90, 0, 0);
                    o.transform.parent = mapParent[(int)mapData.spowns[i, j].terrainType].transform;

                    //绑定地块的gameObject
                    mapData.spowns[i, j].SetSpawnObject(SpawnPoint.SpawnObjectEnum.TERRAIN, o);
                }
            }
        }

        public Vector2Int GetMapSize() {
            return new Vector2Int(mapWidth, mapHeight);
        }

        public Vector2 GetMapOrigin() {
            return new Vector2(orign.x, orign.z);
        }

        public Vector2 GetBlockSize() {
            return new Vector2(spawnOffsetX, spawnOffsetZ);
        }
    }
}

