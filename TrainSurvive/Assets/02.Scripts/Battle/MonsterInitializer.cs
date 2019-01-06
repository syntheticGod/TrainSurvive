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

namespace WorldBattle
{
public class MonsterInitializer 
    {
        //小怪（非BOSS）的首末id，用于控制读取xml的范围，比如随机获取特定种类的小怪（比如野兽类在0~10）
        private static int monsterIdMin = 0;
        private static int monsterIdMax = 2;
        private BattleActor battleActor=null;

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
            XmlNode weaponNode = aimNode.SelectSingleNode("./weapon");

            Monster monster = new Monster();
            monster.id = monsterId;
            monster.name = aimNode.Attributes["name"].Value;
            monster.vitality = int.Parse(propertyNode.Attributes["vitality"].Value);
            monster.strength = int.Parse(propertyNode.Attributes["strength"].Value);
            monster.agile = int.Parse(propertyNode.Attributes["agile"].Value);
            monster.technique = int.Parse(propertyNode.Attributes["technique"].Value);
            monster.intelligence = int.Parse(propertyNode.Attributes["intelligence"].Value);

            if (weaponNode != null)
            {
                monster.hasWeapon = true;
                monster.weaponId = int.Parse(weaponNode.Value);
                //要补上武器信息的更新
            }


            if (aiNode == null)
            {
                character.AddComponent<NoneAI>();
                battleActor = character.GetComponent<NoneAI>();
            }
            else
            {
                switch (aiNode.Attributes["type"].Value)
                {
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

            battleActor.playerPrefab = character;
            setProperty(ref battleActor,ref monster);
            addTaskListener(ref battleActor, ref monster);
        }
        /// <summary>
        /// 随机生成一个小怪(monster)
        /// </summary>
        /// <param name="gameob"></param>
        public void initializeMonster(ref GameObject character)
        {
            string xmlString = Resources.Load("xml/MonsterData").ToString();
            int targerID=Random.Range(monsterIdMin, monsterIdMax+1);
            string XPath = string.Format("./monster[@id='{0:D}']", targerID);          
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNode root = document.SelectSingleNode("monsterlist");
            XmlNode aimNode = root.SelectSingleNode(XPath);
            XmlNode propertyNode = aimNode.SelectSingleNode("./property");
            XmlNode aiNode = aimNode.SelectSingleNode("./AI");
            XmlNode weaponNode = aimNode.SelectSingleNode("./weapon");

            Monster monster = new Monster();
            monster.id = targerID;
            monster.name= aimNode.Attributes["name"].Value;
            monster.vitality = int.Parse(propertyNode.Attributes["vitality"].Value);
            monster.strength = int.Parse(propertyNode.Attributes["strength"].Value);
            monster.agile = int.Parse(propertyNode.Attributes["agile"].Value);
            monster.technique = int.Parse(propertyNode.Attributes["technique"].Value);
            monster.intelligence = int.Parse(propertyNode.Attributes["intelligence"].Value);

            if (weaponNode != null)
            {
                monster.hasWeapon = true;
                monster.weaponId = int.Parse(weaponNode.Value);
                //要补上武器信息的更新
            }

            if (aiNode == null)
            {
                character.AddComponent<NoneAI>();
                battleActor = character.GetComponent<NoneAI>();
            }
            else
            {
                switch (aiNode.Value)
                {
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

            battleActor.playerPrefab = character;
            setProperty(ref battleActor, ref monster);
            addTaskListener(ref battleActor, ref monster);
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

        private void setProperty(ref BattleActor actor,ref Monster monster)
        {
            actor.task_monsterId = monster.id;
            actor.maxHealthPoint = (float)monster.getHpMax();
            actor.maxActionPoint = (float)monster.getApMax();
            actor.hpRecovery = (float)monster.getHpRec();
            actor.apRecovery = (float)monster.getApRec();
            actor.atkNeedTime = (float)monster.getValAts();
            actor.moveSpeed = (float)monster.getValSpd();
            actor.atkDamage = (float)monster.getValAtk();
            actor.atkRange = (float)monster.getRange();
            actor.damageRate = (float)monster.getValHit();
            actor.critDamage = (float)monster.getValCrd();
            actor.critRate = (float)monster.getValCrc();
            actor.hitRate = (float)monster.getValHrate();
            actor.dodgeRate = (float)monster.getValErate();
        }

        private void addTaskListener(ref BattleActor actor, ref Monster monster)
        {
            actor.task_monsterId = monster.id;
            foreach(Task t in TaskController.getInstance().Task_doing.Values)
            {
                foreach(TaskRequirement req in t.reqList)
                {
                    if(req.GetType()== typeof(KillRequirement))
                    {
                        actor.task_kill_handler += req.conditionChange;
                    }
                }
            }
        }
    }
}