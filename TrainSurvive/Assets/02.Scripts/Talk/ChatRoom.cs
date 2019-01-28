/*
 * 描述：
 * 作者：李巡
 * 创建时间：2019/1/21 15:34:52
 * 版本：v0.7
 */
using UnityEngine;

using System;
using System.Collections.Generic;

using Story.Faker;
using Story.MyTools;
using Story.Scripts;

using TTT.Resource;
using WorldMap.Model;

namespace Story.Communication
{

    public class ChatRoom
    {

        List<NpcData> persons;
        public ChatRoom(List<NpcData> members)
        {
            persons = members;
        }

        public List<KeyValuePair<int, string>> chat()
        {

            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();

            //todo test output
            //---------------------------------------------------

            var shuffled = shuffle(persons.Count);
            //将乱序后的顺序分为几种对话段,2和3两种组
            var divided = divide(shuffled);
#if DEBUG
            for (int i = 0; i < shuffled.Count; i++)
            {
                Debug.Log("人物排序 ：" + shuffled[i]);
            }
#endif
            List<KeyValuePair<chatType, List<int>>> chatPattern = matchChatType(divided);
            for (int i = 0; i < chatPattern.Count; i++)
            {
                Debug.Log("----" + chatPattern[i].Key.ToString());
                List<int> g = chatPattern[i].Value;
#if DEBUG
                for (int j = 0; j < g.Count; j++)
                {
                    Debug.Log(g[j]);
                }
#endif
            }

            //---------------------------------------------------


            //todo for test
            //List<KeyValuePair<chatType,List<int>>> chatPattern = matchChatType(divide(shuffle(persons.Count)));



            for (int i = 0; i < chatPattern.Count; i++)
            {
                KeyValuePair<chatType, List<int>> stage = chatPattern[i];
                result.AddRange(StaticResource.ChatScripts.getString(stage));
            }

            return result;
        }

        /// <summary>
        /// n个人，每个人4-6句话，乱序排列
        /// </summary>
        /// <param name="n">人数</param>
        /// <returns></returns>
        public List<int> shuffle(int n)
        {
            List<int> candidate = new List<int>();

            int total = 0;
            for (int i = 0; i < n; i++)
            {
                int num = Tools.random(3, 5);
                candidate.Add(num);
                total += num;
            }

            List<int> result = new List<int>();
            for (int i = 0; i < total; i++)
            {
                int index = Tools.random(n);
                if (candidate[index]-- != 0)
                {
                    result.Add(index);
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (candidate[i] != 0)
                {
                    for (int j = 0; j < candidate[i]; j++)
                    {
                        result.Add(i);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将输入的列表随机分为多个组，每组最多3人，最少2人
        /// 如果传入的列表长度为1，则返回内容为空的列表
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public List<List<int>> divide(List<int> inputList)
        {
            List<List<int>> result = new List<List<int>>();
            List<int> group = new List<int>();
            int len = inputList.Count;
            while (len != 0)
            {
                if (len < 4)
                {
                    if (len > 1)
                    {
                        group.Add(len);
                    }
                    break;
                }
                int divide = Tools.random(2, 4);
                group.Add(divide);
                len -= divide;
            }
            int start = 0;
            for (int i = 0; i < group.Count; i++)
            {
                result.Add(inputList.GetRange(start, group[i]));
                start += group[i];
            }
            return result;
        }

        /// <summary>
        /// 不同的对话模式
        /// </summary>
        public enum chatType
        {
            t00,
            t01,
            t000,
            t001,
            t010,
            t011,
            t012,
        }

        /// <summary>
        /// 针对不同的对话段,匹配相应的类型
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        public List<KeyValuePair<chatType, List<int>>> matchChatType(List<List<int>> grouped)
        {
            List<KeyValuePair<chatType, List<int>>> result = new List<KeyValuePair<chatType, List<int>>>();

            for (int i = 0; i < grouped.Count; i++)
            {
                List<int> seq = grouped[i];
                string pattern = "t0";
                switch (seq.Count)
                {
                    case 2:
                        pattern += seq[1] == seq[0] ? "0" : "1";
                        break;
                    case 3:
                        pattern += seq[1] == seq[0] ? "0" : "1";
                        if (pattern == "t00")
                        {
                            pattern += seq[2] == seq[0] ? "0" : "1";
                        }
                        else
                        {
                            pattern += seq[2] == seq[0] ? "0" : (seq[2] == seq[1] ? "1" : "2");
                        }
                        break;
                    default:
                        break;
                }
                KeyValuePair<chatType, List<int>> pair = new KeyValuePair<chatType, List<int>>((chatType)Enum.Parse(typeof(chatType), pattern), seq);
                result.Add(pair);
            }

            return result;
        }
    }
}