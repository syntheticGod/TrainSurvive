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

            battleActor.maxHealthPoint = (float)monster.getHpMax();
            battleActor.maxActionPoint = (float)monster.getApMax();
            battleActor.hpRecovery = (float)monster.getHpRec();
            battleActor.apRecovery = (float)monster.getApRec();
            battleActor.atkNeedTime = (float)monster.getValAts();
            battleActor.moveSpeed = (float)monster.getValSpd();
            battleActor.atkDamage = (float)monster.getValAtk();
            battleActor.atkRange = (float)monster.getRange();
            battleActor.damageRate = (float)monster.getValHit();
            battleActor.critDamage = (float)monster.getValCrd();
            battleActor.critRate = (float)monster.getValCrc();
            battleActor.hitRate = (float)monster.getValHrate();
            battleActor.dodgeRate = (float)monster.getValErate();

            
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

            battleActor.maxHealthPoint = (float)monster.getHpMax();
            battleActor.maxActionPoint = (float)monster.getApMax();
            battleActor.hpRecovery = (float)monster.getHpRec();
            battleActor.apRecovery = (float)monster.getApRec();
            battleActor.atkNeedTime = (float)monster.getValAts();
            battleActor.moveSpeed = (float)monster.getValSpd();
            battleActor.atkDamage = (float)monster.getValAtk();
            battleActor.atkRange = (float)monster.getRange();
            battleActor.damageRate = (float)monster.getValHit();
            battleActor.critDamage = (float)monster.getValCrd();
            battleActor.critRate = (float)monster.getValCrc();
            battleActor.hitRate = (float)monster.getValHrate();
            battleActor.dodgeRate = (float)monster.getValErate();

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
    }
}