using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public int MAP_ORIGIN_X = 30;
    public int MAP_ORIGIN_Z = 30;

    private GameObject[] mapObject;
    public GameObject loadObject;
    public GameObject waterObject;
    public GameObject mountainObject;
    public GameObject treeObject;
    private GameObject[] mapParent;

    public int loadNum;
    public int waterNum = 3;
    public int mountainNum = 3;
    public int treeNum = 6;

    public int mountainSize = 3;
    public int waterSize = 3;
    public int treeSize = 3;

    // 出现点的种类
    public enum SPAWN_POINT_TYPE {
        NONE = -1,

        //玩家位置
        //BLOCK_SPAWN_POINT_PLAYER = 0,

        //道路
        BLOCK_SPAWN_POINT_LOAD,

        //水
        BLOCK_SPAWN_POINT_WATER,

        //山
        BLOCK_SPAWN_POINT_MOUNTAIN,

        //树
        BLOCK_SPAWN_POINT_TREE,

        //type数量
        NUM
    };

    // 地图数据的结构体
    struct MapData {
        public int width;
        public int length;
        public int offset_x;    // data[0][0]表示位置offset_x,offset_z的方块
        public int offset_z;
        public SPAWN_POINT_TYPE[,] data;
    };

    //构建地图
    private void Awake() {
        createModel();
    }

    //地图对象
    private MapData mapData;
    public void createModel() {
        mapData = new MapData();
        mapData.width = MAP_ORIGIN_X;
        mapData.length = MAP_ORIGIN_Z;
        mapData.data = new SPAWN_POINT_TYPE[MAP_ORIGIN_X, MAP_ORIGIN_Z];
        for (int i = 0; i < mapData.width; i++) {
            for (int j = 0; j < mapData.length; j++) {
                mapData.data[i, j] = SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_LOAD;
            }
        }
        mapObject = new GameObject[(int)SPAWN_POINT_TYPE.NUM];
        mapObject[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_LOAD] = loadObject;
        mapObject[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_WATER] = waterObject;
        mapObject[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_TREE] = treeObject;
        mapObject[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_MOUNTAIN] = mountainObject;

        mapParent = new GameObject[(int)SPAWN_POINT_TYPE.NUM];
        mapParent[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_LOAD] = new GameObject("loads");
        mapParent[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_WATER] = new GameObject("waters");
        mapParent[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_TREE] = new GameObject("trees");
        mapParent[(int)SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_MOUNTAIN] = new GameObject("mountains");

        buildTerrain();

        paintTerrain();
    }

    //生成地形
    private void buildTerrain() {
        //生成特殊地形
        buildSpecialTerrain(mountainSize, mountainNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_MOUNTAIN);
        buildSpecialTerrain(waterSize, waterNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_WATER);
        buildSpecialTerrain(treeSize, treeNum, SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_TREE);
    }

    //生成特定的地形
    private void buildSpecialTerrain(int terrainSize, int terrainNum, SPAWN_POINT_TYPE pointType) {


        //随机生成
        for (int i = 0; i < terrainNum; i++) {
            //按范围随机生成点
            int posx = Random.Range(0, mapData.width - terrainSize);
            int posy = Random.Range(0, mapData.length - terrainSize);

            //检查该点是否合法
            bool isLegal = true;
            for (int j = 0; j < terrainSize && isLegal; j++) {
                for (int k = 0; k < terrainSize; k++) {
                    if (mapData.data[posx + j, posy + k] != SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_LOAD) {
                        isLegal = false;
                        break;
                    }
                }
            }

            //如果合法将该点周围布置成该环境
            if (isLegal == true) {
                for (int j = 0; j < terrainSize && isLegal; j++) {
                    for (int k = 0; k < terrainSize; k++) {
                        mapData.data[posx + j, posy + k] = pointType;
                    }
                }
            } 
            //否则重新生成随机的点
            else {
                i--;
            }
            
        }
    }

    //绘制地形
    private void paintTerrain() {

        Vector3 orign = new Vector3(0, 0, 0);
        mapData.offset_x = 1;
        mapData.offset_z = 1;
        for (int i = 0; i < mapData.width; i++) {
            for (int j = 0; j < mapData.length; j++) {
                GameObject o = Instantiate(mapObject[(int)mapData.data[i, j]],
                    orign + new Vector3(mapData.offset_x * i, 0, mapData.offset_z * j),
                    Quaternion.identity);
                o.transform.Rotate(90, 0, 0);
                o.transform.parent = mapParent[(int)mapData.data[i, j]].transform;
                //switch (mapData.data[i, j]) {
                //    case SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_LOAD: {
                        
                //    }
                //    break;
                //}
            }
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
