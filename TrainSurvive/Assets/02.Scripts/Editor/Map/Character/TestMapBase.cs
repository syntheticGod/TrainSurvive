/*
 * 描述：地图有关的基础测试类，生成基础的测试地图。
 * 作者：项叶盛
 * 创建时间：2018/11/19 20:21:59
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;
using WorldMap;

namespace TestWorldMap
{
    public class TestMapBase
    {
        private enum BLOCK_TYPE
        {
            NONE = -1,
            RAIL,
            TOWN,
            NUM
        }
        private const int O = (int)BLOCK_TYPE.NONE;
        private const int R = (int)BLOCK_TYPE.RAIL;
        private const int T = (int)BLOCK_TYPE.TOWN;

        //正确答案（因为数组显示和实际显示相反，先x后y，重复删去）
        private static int[,] mapDataTorTest = new int[,]{
            { O,T,O,O,O,O,O,O},
            { O,R,R,R,R,T,O,O},
            { O,O,O,O,O,O,O,O},
            { O,O,O,O,O,O,O,O},
            { O,O,O,O,O,O,O,O},
            { O,O,O,O,O,O,O,O},
            { R,R,R,R,R,R,R,T},
            { T,O,O,O,O,O,O,O},
        };
        protected Map map;
        private static IMapForTrain iMapForTrain;
        private const int mapRowNum = 8;
        private const int mapColNum = 8;
        //城镇铁轨生成类
        private static TownsRailGenerate townsRailGenerate;
        //城镇类参数（将大地图分为4块）
        private int townRowNum = 2;
        private int townColNum = 2;
        //城镇坐标设置（满足在各个大块范围内2x2）
        private Vector2Int[,] town ={
            {
                new Vector2Int(0, 1),
                new Vector2Int(1, 5)
            },
            {
                new Vector2Int(7, 0),
                new Vector2Int(6, 7)
            } };

        //设置城镇相应的属性
        private void SetTown(int x, int z)
        {
            TownsRailGenerate.SetTownProperty(map, new Vector2Int(x, z), town[x, z]);
            Debug.Log(x + " " + z);
        }
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //初始化map
            map = Map.GetInstance();
            map.initMap(mapRowNum, mapColNum);
            iMapForTrain = map;

            //初始化城镇类
            map.towns = new Town[townRowNum, townColNum];

            //设置城镇铁轨生成类
            townsRailGenerate = GameObject.Find("townsRailGenerate").GetComponent<TownsRailGenerate>();
            townsRailGenerate.mapData = map;
            townsRailGenerate.towns = map.towns;

            //只设置城镇
            for (int x = 0; x < townRowNum; x++)
            {
                for (int z = 0; z < townColNum; z++)
                {
                    SetTown(x, z);
                    map.towns[x, z] = new Town(town[x, z]);
                }
            }

            //连接城镇，构造铁轨
            TownsRailGenerate.BuildRails(map);
        }
    }
}