/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/19 17:10:55
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;

using TTT.Utility;

namespace TestWorldMap
{
    [TestFixture]
    public class TestUtility
    {
        [Test]
        public void TestApproximatelyInView()
        {
            //float类型参数
            float a = 1.0F, b = 1.1F, c = 0.9F;
            float d = 1.00001F, e = 0.99999F;
            Assert.IsFalse(MathTool.ApproximatelyInView(a, b));
            Assert.IsFalse(MathTool.ApproximatelyInView(a, c));
            Assert.IsTrue(MathTool.ApproximatelyInView(a, d));
            Assert.IsTrue(MathTool.ApproximatelyInView(a, e));
            //向量类型参数
            Vector2 va = new Vector2(a, a);
            Vector2 vaYF = new Vector2(a, b);
            Vector2 vaYN = new Vector2(a, d);
            Vector2 vaXF = new Vector2(b, a);
            Vector2 vaXN = new Vector2(d, a);
            Assert.IsFalse(MathTool.ApproximatelyInView(va, vaYF));
            Assert.IsTrue(MathTool.ApproximatelyInView(va, vaYN));
            Assert.IsFalse(MathTool.ApproximatelyInView(va, vaXF));
            Assert.IsTrue(MathTool.ApproximatelyInView(va, vaXN));
        }
        [Test]
        public void TestIgnoreAndAcceptY()
        {
            Vector3 a = new Vector3(1.0F, 1.0F, 1.0F);
            Vector2 a2 = MathTool.IgnoreY(a);
            Assert.AreEqual(a.x, a2.x);
            Assert.AreEqual(a.z, a2.y);
            Assert.AreEqual(a, MathTool.AcceptY(a2, a.y));
        }
        [Test]
        public void TestIfBetween()
        {
            float edge1 = -1.0F, edge2 = 2.0F;
            float value1 = edge1, value2 = edge2;
            float value3 = 1.0F, value4 = 3.0F, value5 = -2.0F;
            //IfBetweenBoth
            Assert.IsTrue(MathTool.IfBetweenBoth(edge1, edge2, value1));
            Assert.IsTrue(MathTool.IfBetweenBoth(edge1, edge2, value2));
            Assert.IsTrue(MathTool.IfBetweenBoth(edge1, edge2, value3));
            Assert.IsFalse(MathTool.IfBetweenBoth(edge1, edge2, value4));
            Assert.IsFalse(MathTool.IfBetweenBoth(edge1, edge2, value5));
            //IfBetweenLeft
            Assert.IsTrue(MathTool.IfBetweenLeft(edge1, edge2, value1));
            Assert.IsFalse(MathTool.IfBetweenLeft(edge1, edge2, value2));
            Assert.IsTrue(MathTool.IfBetweenLeft(edge1, edge2, value3));
            Assert.IsFalse(MathTool.IfBetweenLeft(edge1, edge2, value4));
            Assert.IsFalse(MathTool.IfBetweenLeft(edge1, edge2, value5));
        }
    }
    [TestFixture]
    public class TestMatrix2x2Int
    {
        [Test]
        public void TestRoate()
        {
            Vector2Int va = new Vector2Int(2, 3);
            Assert.AreEqual(Matrix2x2Int.Roate90Clockwise(va), new Vector2Int(3, -2));
            Assert.AreEqual(Matrix2x2Int.Roate90Anticlockwise(va), new Vector2Int(-3, 2));
            Assert.AreEqual(Matrix2x2Int.Roate180(va), new Vector2Int(-2, -3));
            Matrix2x2Int.Roate90Clockwise(ref va);
            Assert.AreEqual(va, new Vector2Int(3, -2));
            va = new Vector2Int(2, 3);
            Matrix2x2Int.Roate90Anticlockwise(ref va);
            Assert.AreEqual(va, new Vector2Int(-3, 2));
            va = new Vector2Int(2, 3);
            Matrix2x2Int.Roate180(ref va);
            Assert.AreEqual(va, new Vector2Int(-2, -3));
        }
    }
}

