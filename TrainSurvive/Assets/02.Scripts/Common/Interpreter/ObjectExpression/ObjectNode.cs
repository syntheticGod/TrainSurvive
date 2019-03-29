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
    public class ObjectNode : Node
    {
        public static ObjectNode Compile(InstructionHandler handler)
        {
            ObjectNode root = null;
            switch (handler.CurrentWord)
            {
                case "item":
                    root = new ItemNode(handler.NextWord);
                    break;
                case "money":
                    root = new MoneyNode(handler.NextWord);
                    break;
                case "task":
                    root = new TaskNode(handler.NextWord);
                    break;
                case "npc":
                    root = new NPCNode(handler.NextWord);
                    break;
                default:
                    return null;
            }
            handler.IgnoreWord();
            return root;
        }
    }
}
