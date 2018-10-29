/*
 * 描述：用于TestHelloWorld的测试使用。
 * 作者：项叶盛
 * 创建时间：10/29/2018 4:13:48 PM
 * 版本：v0.1
 */


public class HelloWorld {
    private int money = 0;
    public int Money { set { money = value; } get { return money; } }
    public HelloWorld() { }
    public void addMoneyRight(int value) {
        money += value;
    }
    public void addMoneyWrong(int value) {
        money += value + 1;
    }

}
