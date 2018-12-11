/*
 * 描述：
 * 作者：王安鑫
 * 创建时间：2018/11/19 15:08:51
 * 版本：v0.1
 */
/*
 * 描述：IMapForTrain的测试代码
 * 作者：项叶盛
 * 创建时间：2018/11/9 10:58:35
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;
using WorldMap;

namespace TestWorldMap {
    [TestFixture]
    public class TestTownRailGenerate {

        private enum BLOCK_TYPE {
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

        //地图类
        private static Map map;
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

        //private Vector2Int rail1 = new Vector2Int(1, 2);

        private Vector2Int outerPoint1 = new Vector2Int(-1, 5);
        private Vector2Int outerPoint2 = new Vector2Int(mapRowNum, 5);
        private Vector2Int outerPoint3 = new Vector2Int(1, -1);
        private Vector2Int outerPoint4 = new Vector2Int(1, mapColNum);

        //设置城镇相应的属性
        private void SetTown(int x, int z) {
            TownsRailGenerate.SetTownProperty(map, new Vector2Int(x, z), town[x, z]);
            Debug.Log(x + " " + z);
        }
        private void SetRail(int x, int z) {
        }

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            //初始化map
            map = Map.GetIntanstance();
            map.initMap(mapRowNum, mapColNum);
            iMapForTrain = map;

            //初始化城镇类
            map.towns = new Town[townRowNum, townColNum];

            //设置城镇铁轨生成类
            //townsRailGenerate = new TownsRailGenerate();
            townsRailGenerate = GameObject.Find("TownsRailGenerate").GetComponent<TownsRailGenerate>();
            townsRailGenerate.mapData = map;
            townsRailGenerate.towns = map.towns;

            //只设置城镇
            for (int x = 0; x < townRowNum; x++) {
                for (int z = 0; z < townColNum; z++) {
                    SetTown(x, z);
                    map.towns[x, z] = new Town(town[x, z]);
                }
            }

            //连接城镇，构造铁轨
            TownsRailGenerate.BuildRails(map);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
        }


        [SetUp]
        public void SetUp() {
        }


        [TearDown]
        public void TearDown() {
        }


        //测试城镇铁轨是否一致，铁轨生成算法是不是符合预期
        [Test]
        public void TestIfTownRail() {

            for (int i = 0; i < mapRowNum; i++) {
                for (int j = 0; j < mapColNum; j++) {
                    switch ((BLOCK_TYPE)mapDataTorTest[i, j]) {
                        case BLOCK_TYPE.TOWN:
                            Assert.IsTrue(iMapForTrain.IfTown(new Vector2Int(i, j)));
                            break;

                        case BLOCK_TYPE.RAIL:
                            Assert.IsTrue(iMapForTrain.IfRail(new Vector2Int(i, j)));
                            break;

                        case BLOCK_TYPE.NONE:
                            Assert.IsFalse(iMapForTrain.IfTown(new Vector2Int(i, j)));
                            Assert.IsFalse(iMapForTrain.IfRail(new Vector2Int(i, j)));
                            break;
                    }
                }
            }

        }

        [Test]
        public void TestGetEachEndsOfRail() {
            for (int i = 0; i < mapRowNum; i++) {
                for (int j = 0; j < mapColNum; j++) {
                    Vector2Int town1 = new Vector2Int();
                    Vector2Int town2 = new Vector2Int();
                    switch ((BLOCK_TYPE)mapDataTorTest[i, j]) {
                        case BLOCK_TYPE.RAIL:
                            Assert.IsTrue(iMapForTrain.GetEachEndsOfRail(new Vector2Int(i, j), out town1, out town2));
                            if (i < 4) {
                                Assert.IsTrue(town1 == town[0, 0] && town2 == town[0, 1]);
                            } else {
                                Assert.IsTrue(town1 == town[1, 0] && town2 == town[1, 1]);
                            }
                            break;
                    }
                }
            }
        }

        //测试城镇铁轨是否连接，能否转换起始和终止点
        [Test]
        public void TestConnectedetweenTowns() {
            Vector2Int town1 = new Vector2Int();
            Vector2Int town2 = new Vector2Int();
            for (int i = 0; i < townRowNum; i++) {
                for (int j = 0; j < townColNum; j++) {

                    for (int k = 0; k < townColNum; k++) {
                        for (int l = 0; l < townColNum; l++) {
                            town1 = town[i, j];
                            town2 = town[k, l];
                            if ((i == 0 && j == 0 && k == 0 && l == 1) ||
                                (i == 1 && j == 0 && k == 1 && l == 1)) {
                                Assert.IsTrue(iMapForTrain.IfConnectedBetweenTowns(ref town1, ref town2));
                            } else if ((i == 0 && j == 1 && k == 0 && l == 0) ||
                                        (i == 1 && j == 1 && k == 1 && l == 0)) {
                                Assert.IsTrue(iMapForTrain.IfConnectedBetweenTowns(ref town1, ref town2));
                                Assert.IsTrue(town1 == town[k, l] && town2 == town[i, j]);
                            } else {
                                Assert.IsFalse(iMapForTrain.IfConnectedBetweenTowns(ref town1, ref town2));
                            }
                        }
                    }

                }
            }
        }
    }
}
