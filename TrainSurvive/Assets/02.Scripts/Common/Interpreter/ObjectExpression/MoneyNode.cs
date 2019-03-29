/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/3/5 13:18:46
 * 版本：v0.7
 */

namespace TTT.Interpreter
{
    public class MoneyNode : ObjectNode
    {
        public int Money { get; private set; }
        public MoneyNode(string word)
        {
            Money = int.Parse(word);
        }
    }
}