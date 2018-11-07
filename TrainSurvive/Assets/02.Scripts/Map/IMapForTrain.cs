/*
 * 描述：列车需要的地图接口，该接口涉及到的坐标都是以方块为单位。
 *          例如，getMapSize返回2X2方块个数（长，宽）。
 * 作者：项叶盛
 * 创建时间：2018/10/31 0:43:33
 * 版本：v0.1
 */
using UnityEngine;

namespace WorldMap
{
    public interface IMapForTrain
    {
        /// <summary>
        /// 获取地图的X轴方块个数，和Z轴的方块个数
        /// </summary>
        /// <returns>整数向量为（X轴个数, Z轴个数）</returns>
        Vector2Int getMapSize();
        /// <summary>
        /// 获取第一块方块中心的世界坐标
        /// </summary>
        /// <returns>浮点向量为（X轴世界坐标，Z轴世界坐标）</returns>
        Vector2 getMapOrigin();
        /// <summary>
        /// 获取一块的长和宽
        /// </summary>
        /// <returns>浮点向量为（X轴长度，Z轴长度）</returns>
        Vector2 getBlockSize();
        /// <summary>
        /// 判断地图坐标是否是轨道
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns></returns>
        bool ifRail(Vector2Int position);
    }
    //测试类
    public class MapForTest : IMapForTrain
    {
        public GameObject block;
        public GameObject mapBuilder;
        public GameObject worldMap;
        private Vector2 blockSize;
        private Vector2 mapOrigin;
        private Vector2Int mapSize;
        public MapForTest()
        {
            mapBuilder = GameObject.Find("MapBuild");
            block = GameObject.Find("load(Clone)");
            worldMap = GameObject.Find("worldmap");
            //MapGenerate mapComponent = mapBuilder.GetComponent<MapGenerate>();
            mapSize = new Vector2Int
            {
                x = 300,
                y = 300
            };
            blockSize = new Vector2
            {
                x = block.GetComponent<SpriteRenderer>()
                        .sprite.bounds.size.x,
                y = block.GetComponent<SpriteRenderer>()
                        .sprite.bounds.size.y,
            };
            mapOrigin = new Vector2
            {
                x = worldMap.transform.position.x,
                y = worldMap.transform.position.z
            };
            Debug.Log("map size:" + mapSize);
            Debug.Log("map origin:" + mapOrigin);
            Debug.Log("block size:" + blockSize);
        }

        public Vector2 getBlockSize()
        {
            return blockSize;
        }

        public Vector2 getMapOrigin()
        {
            return mapOrigin;
        }

        public Vector2Int getMapSize()
        {
            return mapSize;
        }

        public bool ifRail(Vector2Int position)
        {
            return true;
        }
    }
}
