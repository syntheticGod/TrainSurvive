/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/11 15:03:59
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using TTT.Utility;

namespace WorldBattle
{
    public class MonsterInitializer
    {
        //0基，存放等级0~4的怪物id
        private List<int>[] rank_idList=new List<int>[5];
        private MonsterInitializer()
        {
            for (int i = 0; i < 5; i++)
                rank_idList[i] = new List<int>();
            string xmlString = Resources.Load("xml/MonsterData").ToString();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNode root = document.SelectSingleNode("monsterlist");
            XmlNodeList monsterList = root.ChildNodes;
            foreach (XmlElement mosterElement in monsterList)
            {
                XmlNode pro= mosterElement.SelectSingleNode("property");
                int rank = int.Parse(pro.Attributes["rank"].Value);
                rank_idList[rank].Add(int.Parse(mosterElement.Attributes["id"].Value));
            }
        }
        private static MonsterInitializer instance;
        public static MonsterInitializer getInstance()
        {
            if (instance == null)
                instance = new MonsterInitializer();
            return instance;
        }
        //小怪（非BOSS）的首末id，用于控制读取xml的范围，比如随机获取特定种类的小怪（比如野兽类在0~10）
        private static int monsterIdMin = 0;
        private static int monsterIdMax = 28;
        private BattleActor battleActor = null;
        /// <summary>
        /// 生成一个指定ID的怪物
        /// </summary>
        /// <param name="character"></param>
        /// <param name="monsterId">怪物ID</param>
        public void initializeMonster(ref GameObject character, int monsterId)
        {
            string xmlString = Resources.Load("xml/MonsterData").ToString();
            string XPath = string.Format("./monster[@id='{0:D}']", monsterId);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNode root = document.SelectSingleNode("monsterlist");
            XmlNode aimNode = root.SelectSingleNode(XPath);
            XmlNode propertyNode = aimNode.SelectSingleNode("./property");
            XmlNode aiNode = aimNode.SelectSingleNode("./AI");
            XmlNode skillListNode = aimNode.SelectSingleNode("./skillList");

            Monster monster = new Monster();
            monster.id = monsterId;
            monster.name = aimNode.Attributes["name"].Value;
            monster.vitality = int.Parse(propertyNode.Attributes["vitality"].Value);
            monster.strength = int.Parse(propertyNode.Attributes["strength"].Value);
            monster.agile = int.Parse(propertyNode.Attributes["agile"].Value);
            monster.technique = int.Parse(propertyNode.Attributes["technique"].Value);
            monster.intelligence = int.Parse(propertyNode.Attributes["intelligence"].Value);
            monster.model = int.Parse(propertyNode.Attributes["model"].Value);
            monster.rank = int.Parse(propertyNode.Attributes["rank"].Value);
            monster.size = int.Parse(propertyNode.Attributes["size"].Value);
            monster.exp = int.Parse(propertyNode.Attributes["exp"].Value);

            if (aiNode == null)
            {
                character.AddComponent<NoneAI>();
                battleActor = character.GetComponent<NoneAI>();
            }
            else
            {
                switch (aiNode.Attributes["type"].Value)
                {
                    case "1":
                        character.AddComponent<type1AI>();
                        battleActor = character.GetComponent<type1AI>();
                        break;
                    case "2":
                        character.AddComponent<type2AI>();
                        battleActor = character.GetComponent<type2AI>();
                        break;
                    case "3":
                        character.AddComponent<type3AI>();
                        battleActor = character.GetComponent<type3AI>();
                        break;
                    case "4":
                        character.AddComponent<type4AI>();
                        battleActor = character.GetComponent<type4AI>();
                        break;
                    case "BeatNearAI":
                        character.AddComponent<BeatNearAI>();
                        battleActor = character.GetComponent<BeatNearAI>();
                        break;
                    case "BeatFarAI":
                        character.AddComponent<BeatFarAI>();
                        battleActor = character.GetComponent<BeatFarAI>();
                        break;
                    default:
                        character.AddComponent<NoneAI>();
                        battleActor = character.GetComponent<NoneAI>();
                        break;
                }
            }

            if (skillListNode != null)
            {
                foreach (XmlElement skillElement in skillListNode.ChildNodes)
                {
                    battleActor.addSkill(int.Parse(skillElement.Attributes["id"].Value));                    
                }
            }
            battleActor.playerPrefab = character;
            MonsterAdapter.setMonsterBattleActor(ref battleActor, ref monster);      
            addTaskListener(ref battleActor, ref monster);
        }
        /// <summary>
        /// 根据level难度生成一个怪物
        /// </summary>
        /// <param name="charachter"></param>
        /// <param name="level">
        /// i. 	 怪物难度1：80%等级1，20%等级2
        /// ii.  怪物难度2：40%等级1，50%等级2，10%等级3
        /// iii. 怪物难度3：20%等级1，30%等级2，50%等级3
        /// </param>
        public void randomMonster(ref GameObject character, int level)
        {
            int rank = 1;
            int randomInt = MathTool.RandomInt(100);
            switch (level)
            {
                case 1: if (randomInt < 80) rank = 1; else rank = 2; break;
                case 2: if (randomInt < 40) rank = 1; else if (randomInt < 90) rank = 2; else rank = 3; break;
                case 3: if (randomInt < 20) rank = 1; else if (randomInt < 50) rank = 2; else rank = 3; break;
                default: Debug.LogError("不支持该怪物等级，默认地将等级设为1"); break;
            }          
            int index_random = Random.Range(0, rank_idList[rank].Count);
            initializeMonster(ref character, rank_idList[rank][index_random]);
            Debug.Log("随机生成一个难度级别为" + level + "，等级为" + rank + "，ID为" + randomInt + "的怪物");
        }
        public BattleActor getBattleActor()
        {
            return battleActor;
        }

        /// <summary>
        /// 返回对应id怪物的名字
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getMonsterName(int id)
        {
            string result = "";
            string xmlString = Resources.Load("xml/MonsterData").ToString();
            string XPath = string.Format("./monster[@id='{0:D}']", id);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNode root = document.SelectSingleNode("monsterlist");
            XmlNode aimNode = root.SelectSingleNode(XPath);
            if (aimNode != null)
                result = aimNode.Attributes["name"].Value;
            else
                result = "不存在的怪物";
            return result;
        }

 

        private void addTaskListener(ref BattleActor actor, ref Monster monster)
        {
            actor.task_monsterId = monster.id;
            for(int i=0;i< TaskController.taskMaxIndex; i++)
            {
                Task t = TaskController.getInstance().TaskList[i];
                if ( t!= null)
                {
                        foreach (TaskRequirement req in t.reqList)
                        {
                            if (req.GetType() == typeof(KillRequirement))
                            {
                                actor.task_kill_handler += req.conditionChange;
                            }
                        }
                }
                else
                    continue;
            }
           
        }
    }
}