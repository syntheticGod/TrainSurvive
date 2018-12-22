/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/22 15:18:48
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;
using System.Collections;

namespace TestCharacter
{

    [TestFixture]
    public class TestPerson
    {
        Person person;
        [SetUp]
        public void SetUp()
        {
            person = Person.RandomPerson();
        }
        [TestCase]
        public void TestAddSkill()
        {
            int[] skills = { 5, 3, 2, 1 };
            int[] skillsRight = { 1, 2, 3, 5 };
            for (int i = 0; i < skills.Length; i++)
            {
                person.AddGotSkill(skills[i]);
            }
            for (int i = 0; i < skills.Length; i++)
            {
                Assert.AreEqual(person.GetSkillsGot()[i], skillsRight[i]);
            }
        }
    }
}
