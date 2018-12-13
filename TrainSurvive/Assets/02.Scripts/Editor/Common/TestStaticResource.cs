/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/12 19:27:45
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;

using TTT.Resource;

namespace TTT.Test.Resource
{
    [TestFixture]
    public class TestStaticResource
    {
        [TestCase]
        public void TestGetProfession()
        {
            Profession profession = StaticResource.GetProfession(EProfession.KNIGHT);
            Assert.AreEqual("骑士", profession.Name);
            Assert.AreEqual(EProfession.KNIGHT, profession.Type);
            Profession.AbiReq abiReq = profession.AbiReqs[0];
            Assert.AreEqual(EAttribute.NONE + 1 + 0, abiReq.Abi);
            Assert.AreEqual(10, abiReq.Number);
            Assert.AreEqual(0.8F, abiReq.costFix);
            Debug.Log(profession.Info);
        }
    }
}
