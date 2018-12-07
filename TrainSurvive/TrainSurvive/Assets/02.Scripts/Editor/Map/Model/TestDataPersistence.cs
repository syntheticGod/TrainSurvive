/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/24 1:13:31
 * 版本：v0.1
 */
using UnityEngine;
using NUnit.Framework;
using WorldMap.Model;

namespace TestWorldMap
{
    [TestFixture]
    public class TestDataPersistence
    {
        DataSerialization ds;
        DataPersistence dp;
        string fileName = "TEST_DATA_TOWN.dat";
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ds = DataSerialization.Instance;
            WorldMap.Town[,] towns =
                {
                { new WorldMap.Town(new Vector2Int(1,2)), new WorldMap.Town(new Vector2Int(5,7))},
                { new WorldMap.Town(new Vector2Int(1,10)), new WorldMap.Town(new Vector2Int(7,15))}
                };
            ds.Init(towns);
            dp = DataPersistence.Instance;
            dp.SetFileName(fileName);
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            dp.Clean();
        }
        [SetUp]
        public void SetUp()
        {
        }
        [Test]
        public void TestLoadData()
        {
            dp.Save();
            ds.Clean();
            Assert.IsTrue(dp.LoadData());
            Town town;
            Assert.IsTrue(ds.Find(new Vector2Int(1, 2), out town));
            Assert.AreEqual(town.PosIndexX, 1);
            Assert.AreEqual(town.PosIndexY, 2);
            Assert.IsTrue(ds.Find(new Vector2Int(5, 7), out town));
            Assert.AreEqual(town.PosIndexX, 5);
            Assert.AreEqual(town.PosIndexY, 7);
            Assert.IsTrue(ds.Find(new Vector2Int(1, 10), out town));
            Assert.AreEqual(town.PosIndexX, 1);
            Assert.AreEqual(town.PosIndexY, 10);
            Assert.IsTrue(ds.Find(new Vector2Int(7, 15), out town));
            Assert.AreEqual(town.PosIndexX, 7);
            Assert.AreEqual(town.PosIndexY, 15);
        }
    }
}
