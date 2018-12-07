/*
 * 描述：使用NUnit进行单元测试示例。
 *           写好测试代码之后在Unity中开启Test Runner窗口
 *           进行测试，
 * 作者：项叶盛
 * 创建时间：10/29/2018 4:13:38 PM
 * 版本：v0.1
 */
using NUnit.Framework;
using UnityEngine;
[TestFixture]
public class TestHelloWorld {
    private HelloWorld world;
    [OneTimeSetUp]
    public void OneTimeSetUp() { }
    [OneTimeTearDown]
    public void OneTimeTearDown() { }
    [SetUp]
    public void setUp() {
        world = new HelloWorld();
    }
    [TearDown]
    public void tearDown() { }
    [Test]
    public void testAddMoneyRight() {
        world.addMoneyRight(100);
        Assert.AreEqual(world.Money, 100);
    }
    [Test]
    public void testAddMoneyFail() {
        world.addMoneyWrong(100);
        Assert.AreEqual(world.Money, 100);
    }

    [Test]
    public void testWord() {
        Vector3 c = new Vector3(1, 1, 1);
        MyClass d = new MyClass(2);
        a(c, d);
        //Debug.Log(c.x + "my:" + d.a);
        b(ref c);
        //Debug.Log(c.x);
    }
    private void a(Vector3 c, MyClass d)
    {
        c.x = 22;
        d.a = 22;
    }  
    private void b(ref Vector3 c)
    {
        c.x = 33;
    }
}
public class MyClass
{
    public MyClass(int a)
    {
        this.a = a;
    }
    public int a;
}