/*
 * 描述：
 * 作者：李巡
 * 创建时间：2019/1/21 15:34:52
 * 版本：v0.7
 */
using System;
using System.Collections.Generic;
using Story.Faker;
using Story.MyTools;
using Story.Scripts;

namespace Story.Communication{

    public class ChatRoom{

        List<Person> persons;
        public ChatRoom(ref List<Person> members){
            persons = members;
        }

        public List<KeyValuePair<int, string>> chat(){

            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();

            //todo test output
            //---------------------------------------------------

            var shuffled = shuffle(persons.Count);
            var divided = divide(shuffled);
            for (int i = 0; i < shuffled.Count; i++){
                Console.WriteLine(shuffled[i]);
            }
            List<KeyValuePair<chatType,List<int>>> chatPattern = matchChatType(divided);
            for (int i = 0; i < chatPattern.Count; i++){
                Console.WriteLine("----"+chatPattern[i].Key.ToString());
                List<int> g = chatPattern[i].Value;
                for (int j = 0; j < g.Count; j++){
                    Console.WriteLine(g[j]);
                }
            }

            //---------------------------------------------------


            //todo for test
            //List<KeyValuePair<chatType,List<int>>> chatPattern = matchChatType(divide(shuffle(persons.Count)));



            for (int i = 0; i < chatPattern.Count; i++){
                KeyValuePair<chatType,List<int>> stage = chatPattern[i];
                result.AddRange(Scripts.Scripts.instance.getString(stage));
            }

            return result;
        }

        /// <summary>
        /// n个人，每个人4-6句话，乱序排列
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public List<int> shuffle(int n){
            List<int> candidate = new List<int>();

            int total = 0;
            for(int i=0; i<n; i++){
                int num = Tools.random(3,5);
                candidate.Add(num);
                total += num;
            }

            List<int> result = new List<int>();
            for(int i=0; i<total; i++){
                int index = Tools.random(n);
                if(candidate[index]-- != 0) {
                    result.Add(index);
                }
            }

            for(int i = 0; i<n; i++){
                if(candidate[i] != 0 ){
                    for(int j = 0; j<candidate[i]; j++){
                        result.Add(i);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将乱序后的顺序分为几种对话段,2和3两种组
        /// </summary>
        /// <param name="shuffled"></param>
        /// <returns></returns>
        public List<List<int>> divide(List<int> shuffled){
            List<List<int>> result = new List<List<int>>();
            List<int> group = new List<int>();
            int len = shuffled.Count;
            while(len!=0){
                if(len<4){
                    if(len>1){
                        group.Add(len);
                    }
                    break;
                }
                int divide = Tools.random(2,4);
                group.Add(divide);
                len -= divide;
            }
            int start = 0;
            for(int i = 0; i < group.Count; i++){
                result.Add(shuffled.GetRange(start,group[i]));
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
        public List<KeyValuePair<chatType,List<int>>> matchChatType(List<List<int>> grouped){
            List<KeyValuePair<chatType,List<int>>> result = new List<KeyValuePair<chatType,List<int>>>();

            for(int i = 0; i<grouped.Count; i++){
                List<int> seq = grouped[i];
                string pattern = "t0";
                switch(seq.Count){
                    case 2:
                        pattern += seq[1] == seq[0] ? "0" : "1";
                        break;
                    case 3:
                        pattern += seq[1] == seq[0] ? "0" : "1";
                        if(pattern == "t00"){
                            pattern += seq[2] == seq[0] ? "0" : "1";
                        }else{
                            pattern += seq[2] == seq[0] ? "0" : (seq[2] == seq[1] ? "1" : "2");
                        }
                        break;
                    default:
                        break;
                }
                KeyValuePair<chatType,List<int>> pair = new KeyValuePair<chatType, List<int>>((chatType)Enum.Parse(typeof(chatType),pattern),seq);
                result.Add(pair);
            }

            return result;
        }
    }
}