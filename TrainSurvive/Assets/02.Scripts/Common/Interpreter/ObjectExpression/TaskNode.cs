/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/3/5 13:18:46
 * 版本：v0.7
 */

namespace TTT.Interpreter
{
    public class TaskNode : ObjectNode
    {
        public int[] IDs { get; private set; }
        public TaskNode(string words)
        {
            if (words[0] == '{' && words[words.Length - 1] == '}')
                words = words.Remove(words.Length - 1, 1).Remove(0, 1);
            string[] items = words.Split(',');
            IDs = new int[items.Length];
            for (int i = 0; i < IDs.Length; i++)
            {
                IDs[i] = int.Parse(items[i]);
            }
        }
    }
}