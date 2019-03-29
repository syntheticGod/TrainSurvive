/*
 * 描述：这是气候和地块种类生成算法
 * 作者：王安鑫
 * 创建时间：2018/11/27 21:56:16
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap {
    public class ClimateTerrainGenerate : GenerateBase {

        private int mapHeight;
        private int mapWidth;

        //气候地块数量
        public int climateBlockNum = 100;
        //地形地块数量
        public int terrainBlockNum = 100;
        //grid地块数量
        private int blockNum = 100;

        //根据地图尺寸和地块数量自适应地块尺寸的参数
        private float sizeFac = 1;
        //气候地块的尺寸
        public float climateSizeFac = 3;
        //地形地块的尺寸
        public float terrainSizeFac = 1;

        //地块数量加x，x最后会加到主地块上
        private int randTypeNum;
        //地块类型数量
        private int realTypeNum;

        //地图类
        private Map mapData;

        //生成的暂时的逻辑地图
        private int[,] grid;

        //温带气候的颜色
        public GameObject temperateGameObject;
        //热带气候的颜色
        public GameObject tropicGameObject;
        //冻原气候的颜色
        public GameObject tundraGameObject;
        //炎热气候的颜色
        public GameObject heatGameObject;
        //极寒气候的颜色
        public GameObject coldGameObject;

        //四种地形的图标
        public GameObject plainObject;
        public GameObject hillObject;
        public GameObject mountainObject;
        public GameObject forestObject;
        //将地形存放到同一object下
        private GameObject[] terrainObject;
        //地形在气候上方多大位置（改为图层显示）
        //public Vector3 terrainPos = new Vector3(0, 0, -0.01f)；
        public Vector3 terrainPos = new Vector3(0, 0, 0f);

        //将气候存放到同一object下
        private GameObject[] climateObject;

        //将气候图标放到同一GameObject下
        private GameObject climateParentObject;
        //将地形图标放到同一GameObject下
        private GameObject terrainParentObject;

        //初始化grid的值
        private int gridInit = -1;

        public override void otherInit() {
            climateParentObject = new GameObject("climates");
            terrainParentObject = new GameObject("terrains");

            //获得地图类
            mapData = mapGenerate.mapData;

            //获得map的长宽
            mapWidth = mapData.rowNum;
            mapHeight = mapData.colNum;
        }

        public override void generate() {
            //生成大地图的气候
            generateClimate();

            //生成大地图的地块属性
            generateSpawnType();
        }

        public override void paint() {
            //对气候进行绘画
            paintClimate();

            //对地块类型进行绘画
            paintTerrain();
        }

        //生成大地图的气候
        private void generateClimate() {
            //真实气候的数量
            realTypeNum = (int)SpawnPoint.ClimateEnum.NUM;
            //额外多余气候的数量（温带占2/(n+1)，其余的占1/(n+1)）
            randTypeNum = realTypeNum + 1;
            //设置气候地块的大小
            sizeFac = climateSizeFac;
            //设置气候地块的数量
            blockNum = climateBlockNum;

            //初始化grid
            grid = new int[mapWidth, mapHeight];

            //设置初始化为0，默认其余的为平原
            gridInit = 0;

            //生成大地图的气候
            init();
            randMap(blockNum, randTypeNum);
            typeFix(realTypeNum);
            aloneFix(realTypeNum);

            //对大地图气候进行赋值
            for (int i = 0; i < mapWidth; i++) {
                for (int j = 0; j < mapHeight; j++) {
                    mapData.spowns[i, j].SetClimateEnum((SpawnPoint.ClimateEnum)grid[i, j]);
                }
            }
        }

        //生成大地图的地块
        private void generateSpawnType() {
            //真实地块类型的数量
            realTypeNum = (int)SpawnPoint.TerrainEnum.NUM;
            //额外多余类型的数量（平原占2/(n+1)，其余的占1/(n+1)）
            randTypeNum = realTypeNum;
            //设置地形地块的大小
            sizeFac = terrainSizeFac;
            //设置地形地块的数量
            blockNum = terrainBlockNum;

            //设置初始化为0，默认平原
            gridInit = 0;

            //生成大地图的地块（取消修正）
            init();
            randMap(blockNum, randTypeNum);
            typeFix(realTypeNum);

            //对大地图气候进行赋值
            for (int i = 0; i < mapWidth; i++) {
                for (int j = 0; j < mapHeight; j++) {
                    mapData.spowns[i, j].SetTerrainEnum((SpawnPoint.TerrainEnum)grid[i, j]);
                }
            }
        }

        //对气候进行绘画
        private void paintClimate() {
            //将气候模型绑定
            climateObject = new GameObject[(int)SpawnPoint.ClimateEnum.NUM];
            climateObject[(int)SpawnPoint.ClimateEnum.TEMPERATE] = temperateGameObject;
            climateObject[(int)SpawnPoint.ClimateEnum.TROPIC] = tropicGameObject;
            climateObject[(int)SpawnPoint.ClimateEnum.TUNDRA] = tundraGameObject;
            climateObject[(int)SpawnPoint.ClimateEnum.HEAT] = heatGameObject;
            climateObject[(int)SpawnPoint.ClimateEnum.COLD] = coldGameObject;

            for (int i = 0; i < mapData.rowNum; i++) {
                for (int j = 0; j < mapData.colNum; j++) {
                    if ((int)mapData.spowns[i, j].climateType < 0) {
                        continue;
                    }
                    //生成指定气候的类型
                    GameObject o = Instantiate(climateObject[(int)mapData.spowns[i,j].climateType],
                        MapGenerate.orign + new Vector3(MapGenerate.spawnOffsetX * i, MapGenerate.spawnOffsetY * j, 0),
                        Quaternion.identity);
                    //o.transform.Rotate(90, 0, 0);
                    o.transform.parent = climateParentObject.transform;

                    //绑定气候的gameObject
                    mapData.spowns[i, j].SetSpawnObject(SpawnPoint.SpawnObjectEnum.CLIMATE, o);
                }
            }
        }

        //对地形进行绘画
        private void paintTerrain() {
            //将气候模型绑定
            terrainObject = new GameObject[(int)SpawnPoint.TerrainEnum.NUM];
            terrainObject[(int)SpawnPoint.TerrainEnum.PLAIN] = plainObject;
            terrainObject[(int)SpawnPoint.TerrainEnum.HILL] = hillObject;
            terrainObject[(int)SpawnPoint.TerrainEnum.FOREST] = forestObject;
            terrainObject[(int)SpawnPoint.TerrainEnum.MOUNTAIN] = mountainObject;

            for (int i = 0; i < mapData.rowNum; i++) {
                for (int j = 0; j < mapData.colNum; j++) {
                    //平原没有图标
                    if ((int)mapData.spowns[i, j].terrainType < 1) {
                        continue;
                    }
                    //生成指定气候的类型
                    GameObject o = Instantiate(terrainObject[(int)mapData.spowns[i, j].terrainType],
                        MapGenerate.orign + new Vector3(MapGenerate.spawnOffsetX * i, MapGenerate.spawnOffsetY * j, 0) + terrainPos,
                        Quaternion.identity);
                    //o.transform.Rotate(90, 0, 0);
                    o.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    o.transform.parent = terrainParentObject.transform;

                    //绑定气候的gameObject
                    mapData.spowns[i, j].SetSpawnObject(SpawnPoint.SpawnObjectEnum.TERRAIN, o);
                }
            }
        }

        //初始化 赋值0
        void init() {
            for (int i = 0; i < mapWidth; i++) { 
                for (int j = 0; j < mapHeight; j++) {
                    grid[i, j] = gridInit;
                }
            }
        }

        private bool isValid(int x, int y) {
            if (x >= 0 && y >= 0 && x < mapWidth && y < mapHeight)
                return true;
            else
                return false;
        }

        //邻近区块检测算法
        private int checkNearBlock(int x, int y, int type) {
            int cnt = 0;
            for (int i = x - 1; i <= x + 1; i++) {
                for (int j = y - 1; j <= y + 1; j++) {
                    if (isValid(i, j) && grid[i, j] == type)
                        cnt++;
                }
            }
            //need表示需要和几个同色相邻
            return cnt;
        }

        // 由分子/分母组成的概率
        private bool randFrac(int num, int den) {
            if (Random.Range(0, den) < num)
                return true;
            else
                return false;
        }

        //生成单个不规则块 xy为中心点坐标 type为地块类型 width height为大致半径
        private void block(int x, int y, int type, int width, int height) {
            width = (int)(width * Random.Range(5, 15) / 10.0f);
            height = (int)(height * Random.Range(5, 15) / 10.0f);

            //基础半径
            float fac1 = 2;
            //空地上生成半径  >fac1
            float fac2 = 3;
            //侵占其他type的半径  >fac2
            float fac3 = 4;
            //最大扩展半径
            float expand = (width - width / fac1 + height - height / fac1) / 2;

            //铺设基础和一轮扩展
            int endI = (int)(x + width / fac1);
            int endJ = (int)(y + height / fac1);
            for (int i = x - (int)(width / fac1); i < endI; i++) {
                for (int j = y - (int)(height / fac1); j < endJ; j++) {
                    if (isValid(i, j)) {
                        if (grid[i, j] == 0) {
                            //吃空地
                            if (Mathf.Abs(i - x) + Mathf.Abs(j - y) <= width / fac2 + height / fac2)
                                grid[i, j] = type;
                        } else {
                            //吃别的地
                            if (Mathf.Abs(i - x) + Mathf.Abs(j - y) <= width / fac3 + height / fac3)
                                grid[i, j] = type;
                        }
                    }
                }
            }

            for (int k = 1; k <= expand; k++) {
                for (int i = x - width; i < x + width; i++) {
                    for (int j = y - height; j < y + height; j++) {
                        //与3个同色快相邻
                        if (isValid(i, j) && grid[i, j] != type
                            && checkNearBlock(i, j, type) > 3
                            && randFrac(2, 5)) {
                            grid[i, j] = type;
                        }
                    }
                }
            }
        }

        //随机生成数个色块，根据地图尺寸和地块密度自适应尺寸
        private void randMap(int time, int numType) {
            int sqrtTime = (int)Mathf.Sqrt(time);
            for (int i = 0; i < time; i++) {
                block(Random.Range(0, mapHeight),
                    Random.Range(0, mapWidth),
                    Random.Range(1, numType),
                    (int)(sizeFac * mapHeight / sqrtTime),
                    (int)(sizeFac * mapWidth / sqrtTime));
            }
        }

        //这个函数很重要 用来保证平原分布比例 比如在生成时randType=5，但实际上除了平原（0）外只用得到1~4，因此可以把5归并到0，这样0（平原）的数量才能比其他地形占据绝对优势。
        //通过修改randType和realType的比例，可以修改平原分布比例。例如目前randType=5,realType=4,则最终平原分布（0和5都归并成平原）可以略大于20%，而其他4种地形平均小于20%
        //如果不太理解 把randType调大看看效果就明白了
        private void typeFix(int reType) {
            for (int i = 0; i < mapWidth; i++) {
                for (int j = 0; j < mapHeight; j++) {
                    if (grid[i, j] >= reType)
                        grid[i, j] = 0;
                }
            }
        }

        //将块数修正
        private void aloneFix(int realType) {
            int flag = 3;
            while (flag-- > 0) {
                for (int i = 0; i < mapWidth; i++) {
                    for (int j = 0; j < mapHeight; j++) {
                        //如果落单
                        if (checkNearBlock(i, j, grid[i, j]) < 3) {
                            //投靠强势色
                            grid[i, j] = findMaxTypeNearBlock(i, j);
                        }
                    }
                }
            }

            return;
        }

        //找出邻近区块，相同type区块数最多的
        //返回最多的type
        private int findMaxTypeNearBlock(int x, int y) {
            int maxType = -1;
            int count = 0;
            int[] typeCount = new int[realTypeNum];
            for (int i = x - 1; i <= x + 1; i++) {
                for (int j = y - 1; j <= y + 1; j++) {
                    if (isValid(i, j) && grid[i, j] != -1) {
                        typeCount[grid[i, j]]++;
                    }
                }
            }
            for (int i = 0; i < realTypeNum; i++) {
                if (count < typeCount[i]) {
                    count = typeCount[i];
                    maxType = i;
                }
            }
            //返回相同块数最多的类型
            return maxType;
        }
    }
}

