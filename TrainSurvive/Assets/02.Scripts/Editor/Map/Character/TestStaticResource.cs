/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/19 17:09:40
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;

using WorldMap;

namespace TestWorldMap
{
    [TestFixture]
    public class TestStaticResource
    {
        [Test]
        public void TestRandomInt()
        {
            int maxLoop = 100;
            while (maxLoop-- > 0)
            {
                int random = StaticResource.RandomInt(10);
                Assert.IsTrue(random < 10, "随机数大于10了");
                Assert.IsTrue(random >= 0, "随机数小于0了");
            }
        }
    }
}
