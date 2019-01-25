/*
 * 描述：
 * 作者：李巡
 * 创建时间：2019/1/21 15:34:52
 * 版本：v0.7
 */
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Story.Communication;
using Story.MyTools;
using UnityEngine;

namespace Story.Scripts{

    public class Scripts{
        private Dictionary<string,List<string>> texts;
        private List<List<List<scriptType>>> templates;

        public enum scriptType
        {
            routine,
            insult,
            counter,
            satire,
            agree,
            selfChat,

        }

        public Scripts(XmlNodeList contents)
        {
            texts = new Dictionary<string, List<string>>();
            foreach (XmlNode node in contents)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;
                XmlNodeList sentencesNode = node.ChildNodes;
                List<string> sentences = new List<string>();
                foreach(XmlNode sentenceNode in sentencesNode)
                    sentences.Add(sentenceNode.InnerText);
                Debug.Log("name:" + node.Name);
                texts.Add(node.Name, sentences);
            }
                
            //对应的脚本类型
            templates = new List<List<List<scriptType>>>{
            
             //t00
             new List<List<scriptType>>{
                 new List<scriptType>{ scriptType.routine, scriptType.routine},
             },
            
             //t01
             new List<List<scriptType>>{
                 new List<scriptType>{ scriptType.routine, scriptType.agree},
                 new List<scriptType>{ scriptType.insult, scriptType.counter},
             },
            
             //t000
             new List<List<scriptType>>{
                 new List<scriptType>{ scriptType.routine, scriptType.routine, scriptType.routine},
                 new List<scriptType>{ scriptType.routine, scriptType.routine, scriptType.satire},
                 
             },
            
             //t001
             new List<List<scriptType>>{
                 new List<scriptType>{ scriptType.routine, scriptType.agree, scriptType.routine},
                 new List<scriptType>{ scriptType.routine, scriptType.routine, scriptType.agree},
                 new List<scriptType>{ scriptType.selfChat, scriptType.insult, scriptType.counter},

             },
            
             //t010
             new List<List<scriptType>>{
                 new List<scriptType>{ scriptType.routine, scriptType.agree, scriptType.routine},
                 new List<scriptType>{ scriptType.insult, scriptType.counter, scriptType.counter},
             },
            
             //t011
             new List<List<scriptType>>{
                 new List<scriptType>{ scriptType.routine, scriptType.agree, scriptType.agree},
                 new List<scriptType>{ scriptType.routine, scriptType.insult, scriptType.counter},
             },
            
             //t012
             new List<List<scriptType>>{
                 new List<scriptType>{ scriptType.routine, scriptType.agree, scriptType.routine},
                 new List<scriptType>{ scriptType.routine, scriptType.agree, scriptType.agree},

             },
            
             };

        }

        /// <summary>
        /// 返回相应的对话字符串
        /// </summary>
        /// <param name="stage"></param>
        /// <returns>
        /// 1       “123”
        /// 2       “234”
        /// 1       “111”
        /// </returns>
        public List<KeyValuePair<int,string>> getString(KeyValuePair<ChatRoom.chatType,List<int>> stage){

            List<KeyValuePair<int,string>> result = new List<KeyValuePair<int, string>>();

            int chatLen = stage.Value.Count;
            List<scriptType> types = getMatchScriptType(stage.Key);


            int pre = stage.Value[0];
            for(int i = 0; i < chatLen; i++){
                int textLen = texts[types[i]+""].Count;
                string reference = "";
                if (i >= 1 && stage.Value[i]!=pre){
                    reference += "@" + stage.Value[i - 1];
                }
                KeyValuePair<int,string> pair = new KeyValuePair<int, string>(stage.Value[i],reference + texts[types[i]+""][Tools.random(textLen)]);
                result.Add(pair);
            }
            //result.Add(new KeyValuePair<int, string>(-1,"----------------------------"));

            return result;
        }

        /// <summary>
        /// 根据对话的形式得到对话的类型
        /// </summary>
        /// <param name="chatType"></param>
        /// <returns></returns>
        private List<scriptType> getMatchScriptType(ChatRoom.chatType chatType){
            List<List<scriptType>> allMatch = templates[(int)chatType];

            return allMatch[Tools.random(allMatch.Count)];
        }
    }
}