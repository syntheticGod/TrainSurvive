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
    private void sort(char[] ws) {
        for(int i = 0; i < ws.Length-1; i++) {
            if(ws[i]>ws[i+1]) {
                char temp = ws[i];
                ws[i] = ws[i + 1];
                ws[i + 1] = temp;
            }
        }
    }
    private string print(char [] ws) {
        string text = "";
        foreach(char w in ws){
            text += w;
        }
        return text;
    }
    [Test]
    public void testWord() {
        char[] w = { 'j','l','x','w','g','z' };
        Debug.Log(print(w));
        sort(w);
        Debug.Log(print(w));
    }
}
