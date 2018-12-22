/*
 * 描述：这是技能工厂类
 * 作者：王安鑫
 * 创建时间：2018/12/15 9:36:33
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static WorldBattle.Skill;

namespace WorldBattle {
    public class SkillFactory : MonoBehaviour {

        //通过id返回对应的技能
        private static Dictionary<int, Skill> skillMap = null;

        /// <summary>
        /// 通过id返回对应的技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Skill getSkill(int id, BattleActor battleActor) {
            //如果当前没初始化，将其初始化
            if (skillMap == null) {
                initSkillFactory();
            }

            //根据id获取指定技能
            Skill noneSkill = new NoneSkill(battleActor);
            if (skillMap.TryGetValue(id, out noneSkill) == false) {
                Debug.Log("正在获取未知的buff？" + id + battleActor);
                Debug.Break();
            }

            //绑定到当前的对象
            noneSkill.battleActor = battleActor;
            return noneSkill.Clone(battleActor);
        }

        /// <summary>
        /// 初始化技能工厂
        /// </summary>
        private static void initSkillFactory() {
            //初始化技能映射表
            skillMap = new Dictionary<int, Skill>();

            //获取Skill库的xml文件
            string xmlString = Resources.Load("xml/Skill").ToString();

            //解析xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            //查找<BuffPool>
            XmlNode rootNode = xmlDoc.SelectSingleNode("SkillPool");
            //遍历所有子节点
            foreach (XmlNode curSkillNode in rootNode.ChildNodes) {
                //如果当前行是注释，跳过
                if (curSkillNode.NodeType == XmlNodeType.Comment) {
                    continue;
                }

                //先获取skill的id
                int skillId = int.Parse(curSkillNode.Attributes["id"].Value);
                //获取needAp
                int needAp = int.Parse(curSkillNode.Attributes["needAp"].Value);
                //获取技能的类型
                string skillTypeString = curSkillNode.Attributes["skillType"].Value;
                SkillType skillType = (SkillType)Enum.Parse(typeof(SkillType), skillTypeString);

                //初始化一个空的技能
                Skill curSkill = new NoneSkill(null);
                //获取比例（某些技能需要）
                float rate, rangeRate, attackRate, timeRate;

                //获取指定id
                switch (skillId) {
                    case 1:
                        //获取重击技能
                        curSkill = new ThumpSkill(null, needAp, skillType);
                        break;
                    case 2:
                        //获取加速技能
                        curSkill = new SpeedUpSkill(null, needAp, skillType);
                        break;
                    case 3:
                        //获取毒素技能
                        curSkill = new PosionSkill(null, needAp, skillType);
                        break;
                    case 4:
                        //获取自愈技能
                        rate = float.Parse(curSkillNode.Attributes["rate"].Value);
                        curSkill = new SelfHealingSkill(null, needAp, skillType, rate);
                        break;
                    case 5:
                        //获取怒意打击技能
                        rate = float.Parse(curSkillNode.Attributes["rate"].Value);
                        curSkill = new AngerAgainstSkill(null, needAp, skillType, rate);
                        break;
                    case 6:
                        //获取鲜血渴望技能
                        rate = float.Parse(curSkillNode.Attributes["rate"].Value);
                        curSkill = new BloodThirstySkill(null, needAp, skillType, rate);
                        break;
                    case 7:
                        //获取裂地击技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        timeRate = float.Parse(curSkillNode.Attributes["timeRate"].Value);
                        curSkill = new CrackStrikeSkill(null, needAp, skillType, rangeRate, attackRate, timeRate);
                        break;
                    case 8:
                        //获取致命一击技能
                        rate = float.Parse(curSkillNode.Attributes["rate"].Value);
                        curSkill = new CritStrikeSkill(null, needAp, skillType, rate);
                        break;
                    case 9:
                        //获取后撤步技能
                        rate = float.Parse(curSkillNode.Attributes["rate"].Value);
                        curSkill = new BackStepSkill(null, needAp, skillType, rate);
                        break;
                    case 10:
                        //获取二连击技能
                        curSkill = new MultipleAttackSkill(null, needAp, skillType, 2);
                        break;
                    case 11:
                        //获取三连击技能
                        curSkill = new MultipleAttackSkill(null, needAp, skillType, 3);
                        break;
                }

                //放入当前技能
                skillMap.Add(skillId, curSkill);
            }
        }
    }
}

