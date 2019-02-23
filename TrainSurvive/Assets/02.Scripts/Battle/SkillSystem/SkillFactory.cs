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
using static WorldBattle.BaseSkill;

namespace WorldBattle {
    public class SkillFactory : MonoBehaviour {

        //通过id返回对应的技能
        private static Dictionary<int, BaseSkill> skillMap = null;

        /// <summary>
        /// 通过id返回对应的技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseSkill getSkill(int id, BattleActor battleActor) {
            //如果当前没初始化，将其初始化
            if (skillMap == null) {
                initSkillFactory();
            }

            //根据id获取指定技能（-1则证明该技能为空）
            BaseSkill noneSkill = new NoneSkill(battleActor);
            if (id != -1 && skillMap.TryGetValue(id, out noneSkill) == false) {
                Debug.Log("正在获取未知的技能？" + id + battleActor);
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
            skillMap = new Dictionary<int, BaseSkill>();

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
                BaseSkill curSkill = new NoneSkill(null);
                //获取比例（某些技能需要）
                float rate, rangeRate, attackRate, timeRate, moveRate, atkSpeedRate, atkDamageRate, injuryedRate;
                int monsterId, monsterNum;

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
                    case 12:
                        //获取迅击技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new FastAttackSkill(null, needAp, skillType, attackRate);
                        break;
                    case 13:
                        //增加狂化技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        atkDamageRate = float.Parse(curSkillNode.Attributes["atkDamageRate"].Value);
                        injuryedRate = float.Parse(curSkillNode.Attributes["injuryedRate"].Value);
                        curSkill = new CrazySkill(null, needAp, skillType, attackRate, atkDamageRate, injuryedRate);
                        break;
                    case 15:
                        //获取勾爪技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        timeRate = float.Parse(curSkillNode.Attributes["timeRate"].Value);
                        curSkill = new HookClawSkill(null, needAp, skillType, rangeRate, timeRate);
                        break;
                    case 16:
                        //获取眩晕击技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        timeRate = float.Parse(curSkillNode.Attributes["timeRate"].Value);
                        curSkill = new DizzinessStrikeSkill(null, needAp, skillType, attackRate, timeRate);
                        break;
                    case 17:
                        //获取空袭技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new AirStrikeSkill(null, needAp, skillType, attackRate);
                        break;
                    case 18:
                        //获取武器涂毒技能
                        curSkill = new WeaponPosionSkill(null, needAp, skillType);
                        break;
                    case 19:
                        //获取圣光闪现技能
                        rate = float.Parse(curSkillNode.Attributes["rate"].Value);
                        curSkill = new BackStepSkill(null, needAp, skillType, rate);
                        break;
                    case 20:
                        //获取寒冰守护技能
                        timeRate = float.Parse(curSkillNode.Attributes["timeRate"].Value);
                        curSkill = new IceProtectSkill(null, needAp, skillType, timeRate);
                        break;
                    case 21:
                        //获取火焰爆轰技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new FlameExplosionSkill(null, needAp, skillType, attackRate);
                        break;
                    case 22:
                        //获取御风技能
                        moveRate = float.Parse(curSkillNode.Attributes["moveRate"].Value);
                        atkSpeedRate = float.Parse(curSkillNode.Attributes["atkSpeedRate"].Value);
                        curSkill = new WindSkill(null, needAp, skillType, moveRate, atkSpeedRate);
                        break;
                    case 23:
                        //获取闪电链技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new ChainLighntingSkill(null, needAp, skillType, attackRate);
                        break;
                    case 24:
                        //获取圣光术技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new HolyLightSkill(null, needAp, skillType, attackRate);
                        break;
                    case 25:
                        //获取寒冰禁锢技能
                        timeRate = float.Parse(curSkillNode.Attributes["timeRate"].Value);
                        curSkill = new IceImprisonSkill(null, needAp, skillType, timeRate);
                        break;
                    case 26:
                        //获取陨石术技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new MeteoriteSkill(null, needAp, skillType, rangeRate, attackRate);
                        break;
                    case 27:
                        //获取飓风术
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        curSkill = new TornadoSkill(null, needAp, skillType, rangeRate);
                        break;
                    case 28:
                        //获取雷击术技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new LightningStrikeSkill(null, needAp, skillType, attackRate);
                        break;
                    case 29:
                        //获取钢铁意志技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new IronWillSkill(null, needAp, skillType, attackRate);
                        break;
                    case 30:
                        //获取魔能之力技能
                        attackRate = float.Parse(curSkillNode.Attributes["attackRate"].Value);
                        curSkill = new MagicPowerSkill(null, needAp, skillType, attackRate);
                        break;
                    case 32:
                        //获取背刺技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        atkSpeedRate = float.Parse(curSkillNode.Attributes["atkSpeedRate"].Value);
                        timeRate = float.Parse(curSkillNode.Attributes["timeRate"].Value);
                        curSkill = new BackStabSkill(null, needAp, skillType, rangeRate, atkSpeedRate, timeRate);
                        break;
                    case 33:
                        //获取召唤狼群技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        monsterId = int.Parse(curSkillNode.Attributes["monsterId"].Value);
                        curSkill = new SummonWolvesSkill(null, needAp, skillType, rangeRate, monsterId);
                        break;
                    case 34:
                        //获取呼朋引伴技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        monsterId = int.Parse(curSkillNode.Attributes["monsterId"].Value);
                        curSkill = new SummonWolvesSkill(null, needAp, skillType, rangeRate, monsterId);
                        break;
                    case 35:
                        //获取黑暗将至技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        monsterId = int.Parse(curSkillNode.Attributes["monsterId"].Value);
                        monsterNum = int.Parse(curSkillNode.Attributes["monsterNum"].Value);
                        curSkill = new DarkComeSkill(null, needAp, skillType, rangeRate, monsterId, monsterNum);
                        break;
                    case 36:
                        //获取枪林弹雨技能
                        curSkill = new BulletSkill(null, needAp, skillType);
                        break;
                    case 37:
                        //获取诅咒技能
                        curSkill = new CurseSkill(null, needAp, skillType);
                        break;
                    case 38:
                        //获取强力击技能
                        rangeRate = float.Parse(curSkillNode.Attributes["rangeRate"].Value);
                        curSkill = new PowerBlowSkill(null, needAp, skillType, rangeRate);
                        break;
                }

                //放入当前技能
                skillMap.Add(skillId, curSkill);
            }
        }
    }
}

