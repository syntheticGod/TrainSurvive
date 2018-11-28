/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/19 17:07:46
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;

using WorldMap.Model;

namespace TestWorldMap
{
    [TestFixture]
    public class TestRail
    {
        //第一版本的铁轨生成算法只会出现8种形状。
        private static Rail horizontalRailRight;
        private static Rail horizontalRailLeft;
        private static Rail verticalRailTop;
        private static Rail verticalRailBottom;
        private static Rail rightTopRail;
        private static Rail rightBottomRail;
        private static Rail leftTopRail;
        private static Rail leftBottomRail;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            horizontalRailRight = new Rail(new Vector2(1.0F, 2.0F), new Vector2(10.0F, 2.0F));
            horizontalRailLeft = new Rail(new Vector2(1.0F, 2.0F), new Vector2(-8.0F, 2.0F));
            verticalRailTop = new Rail(new Vector2(1.0F, 2.0F), new Vector2(1.0F, 11.0F));
            verticalRailBottom = new Rail(new Vector2(1.0F, 2.0F), new Vector2(1.0F, -7.0F));
            rightTopRail = new Rail(new Vector2(1.0F, 2.0F), new Vector2(10.0F, 11.0F));
            rightBottomRail = new Rail(new Vector2(1.0F, 2.0F), new Vector2(10.0F, -7.0F));
            leftTopRail = new Rail(new Vector2(1.0F, 2.0F), new Vector2(-8.0F, 11.0F));
            leftBottomRail = new Rail(new Vector2(1.0F, 2.0F), new Vector2(-8.0F, -7.0F));
        }
        [Test]
        public void TestCallTotalRoad()
        {
            Assert.IsTrue(Mathf.Approximately(9.0F, horizontalRailRight.CalTotalRoad()));
            Assert.IsTrue(Mathf.Approximately(9.0F, horizontalRailLeft.CalTotalRoad()));
            Assert.IsTrue(Mathf.Approximately(9.0F, verticalRailTop.CalTotalRoad()));
            Assert.IsTrue(Mathf.Approximately(9.0F, verticalRailBottom.CalTotalRoad()));
            Assert.IsTrue(Mathf.Approximately(18.0F, rightTopRail.CalTotalRoad()));
            Assert.IsTrue(Mathf.Approximately(18.0F, rightBottomRail.CalTotalRoad()));
            Assert.IsTrue(Mathf.Approximately(18.0F, leftTopRail.CalTotalRoad()));
            Assert.IsTrue(Mathf.Approximately(18.0F, leftBottomRail.CalTotalRoad()));
        }
        [Test]
        public void TestCalRemanentRoad()
        {
            TestCalRemanentRoadEach(horizontalRailRight, "horizontal right");
            TestCalRemanentRoadEach(horizontalRailLeft, "horizontal left");
            TestCalRemanentRoadEach(verticalRailTop, "vertical top");
            TestCalRemanentRoadEach(verticalRailBottom, "vertical bottom");
            TestCalRemanentRoadEach(rightTopRail, "right top");
            TestCalRemanentRoadEach(rightBottomRail, "right bottom");
            TestCalRemanentRoadEach(leftTopRail, "left top");
            TestCalRemanentRoadEach(leftBottomRail, "left bottom");
        }
        /// <summary>
        /// 测试铁轨的剩余路程
        /// 测试思路：
        /// 讲铁轨的每段分隔成若干部分（10部分），
        /// 再取每个节点，计算比较剩余路程。
        /// </summary>
        /// <param name="rail">铁轨</param>
        private void TestCalRemanentRoadEach(Rail rail, string info)
        {
            int maxLoopPerSegment = 10;
            float totalLoop = maxLoopPerSegment * (rail.Count - 1);
            float totalRoad = rail.CalTotalRoad();
            //取铁轨的每段
            for (int index = 0; index < rail.Count - 1; index++)
            {
                Vector2 startOfSegment = rail.GetInflection(index);
                Vector2 endOfSegment = rail.GetInflection(index + 1);
                Vector2 interatorDeltaDir = (1.0F / maxLoopPerSegment) * (endOfSegment - startOfSegment);
                Vector2 interator = startOfSegment;
                float remanentRoad = 0.0F;
                float expectedRoad = 0.0F;
                //将每段铁轨分成10份
                for (int loop = 0; loop < maxLoopPerSegment - 1; loop++, interator += interatorDeltaDir)
                {
                    //Debug.Log("index:" + index + " loop:" + loop);
                    expectedRoad = totalRoad * (1 - (maxLoopPerSegment * index + loop) / totalLoop);
                    Assert.IsTrue(rail.CalRemanentRoad(interator, true, ref remanentRoad), info + " positive " +
                        "expected:" + expectedRoad + " but:" + remanentRoad);
                    Assert.IsTrue(Mathf.Approximately(expectedRoad, remanentRoad), info + " positive " + 
                        "expected:" + expectedRoad + " but:" + remanentRoad);

                    expectedRoad = totalRoad * (maxLoopPerSegment * index + loop) / totalLoop;
                    Assert.IsTrue(rail.CalRemanentRoad(interator, false, ref remanentRoad), info + " positive " +
                        "expected:" + expectedRoad + " but:" + remanentRoad);
                    Assert.IsTrue(Mathf.Approximately(expectedRoad, remanentRoad), info + " negtive " +
                        "expected:"+ expectedRoad +" but:"+ remanentRoad);
                    ;
                }
            }
        }
        [Test]
        public void TestCalNextPosition()
        {

        }
        private void TestCalNextPositionEach(Rail rail, string info)
        {

        }
    }
}