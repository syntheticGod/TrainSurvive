/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/3/5 13:15:02
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;

namespace TTT.Interpreter
{
    public class ItemNode : ObjectNode
    {
        public int[] IDs { get; private set; }
        public int[] Numbers { get; private set; }
        public ItemNode(string words)
        {
            if(words[0] == '{' && words[words.Length-1] == '}')
                words = words.Remove(words.Length - 1, 1).Remove(0, 1);
            string[] items = words.Split(',');
            IDs = new int[items.Length];
            Numbers = new int[items.Length];
            for (int i = 0; i < IDs.Length; i++)
            {
                string[] item = items[i].Split(':');
                IDs[i] = int.Parse(item[0]);
                Numbers[i] = int.Parse(item[1]);
            }
        }
    }
}