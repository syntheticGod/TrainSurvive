/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/3/5 13:32:03
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TTT.Interpreter
{
    public class InstructionHandler
    {
        private int currentIndex;
        public string[] Words {get;private set;}
        
        public string CurrentWord { get { return Words[currentIndex]; } }
        public string NextWord { get { return Words[++currentIndex]; } }
        public bool End { get { return currentIndex >= Words.Length; } }
        public void IgnoreWord() { currentIndex++; }
        
        public Node Compile(string instruction)
        {
            Words = instruction.Split(' ');
            Stack<string> stack = new Stack<string>();
            for (int i = 0; i < Words.Length; i++)
            {

            }
            Node root = new ObjectNode();
            return root;
        }
    }
}