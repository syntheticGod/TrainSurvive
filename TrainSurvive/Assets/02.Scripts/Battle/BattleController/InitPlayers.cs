/*
 * 描述：战斗开始时初始化玩家
 * 作者：王安鑫
 * 创建时间：2018/12/14 19:27:28
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using TTT.Resource;
using UnityEngine;
using UnityEngine.UI;

namespace WorldBattle {
    public class InitPlayers : MonoBehaviour {
        //技能1，2（测试）
        public static int skill1 = 35;
        public static int skill2 = 27;

        /// <summary>
        /// 初始化玩家操作的角色
        /// 创建GameObject
        /// 创建AI
        /// 绑定panel
        /// </summary>
        public static void initPlayers() {
            //获取battleController
            BattleController battleController = BattleController.getInstance();

            //初始化相应列表
            battleController.playerActors = new List<BattleActor>();
            battleController.playerPanels = new List<GameObject>();

            //如果当前处于测试状态
            if (battleController.isTest) {
                //和玩家生成的统一
                for (int i = 0; i < battleController.personNum; i++) {
                    //生成指定的玩家对象，绑定脚本和指令框
                    BattleActor battleActor = generatePlayer(battleController.player, "玩家" + (i + 1), i);

                    //初始化人物属性
                    initBattlePerson(ref battleActor, i, null);

                    //添加到玩家列表中
                    battleController.playerActors.Add(battleActor);
                }
            } else {
                //初始化当前玩家角色人物为0
                battleController.personNum = 0;

                //遍历world中的人物列表，将小队人物参战
                foreach (Person person in World.getInstance().Persons) {
                    //如果当前人不处于出战状态，返回false
                    if (person.ifReadyForFighting == false) {
                        continue;
                    }

                    //无法改变绑定和初始化人物的属性，所以只能暂时用静态skill读取
                    skill1 = person.getSkillCarryed(1);
                    skill2 = person.getSkillCarryed(2);

                    //生成指定的玩家对象，绑定脚本和指令框
                    BattleActor battleActor = generatePlayer(battleController.player, person.name, battleController.personNum);

                    //初始化人物属性
                    initBattlePerson(ref battleActor, battleController.personNum, person);

                    //添加到玩家列表中
                    battleController.playerActors.Add(battleActor);

                    //增加人的数量
                    battleController.personNum++;
                }
            }
        }

        /// <summary>
        /// 生成指定的玩家对象，绑定脚本和指令框
        /// </summary>
        /// <param name="gameObject">玩家角色的prefab</param>
        /// <param name="personName">玩家名</param>
        /// <returns>返回绑定的脚本</returns>
        private static BattleActor generatePlayer(GameObject gameObject, string personName, int index) {
            //获取battleController
            BattleController battleController = BattleController.getInstance();

            //生成玩家
            GameObject curPlayer = Instantiate(gameObject,
                battleController.orign,
                Quaternion.identity);

            //绑定gameObject
            curPlayer.AddComponent<PersonAI>();

            //将生成的脚本和person数组绑定
            BattleActor battleActor = curPlayer.GetComponent<PersonAI>();
            //获取绑定的游戏对象
            battleActor.playerPrefab = curPlayer;

            //创建一个新的panel
            GameObject curPanel = Instantiate(battleController.playerPanel);
            //将panel增加到panel列表中
            battleController.playerPanels.Add(curPanel);
            //将当前的panel绑定到canvas中
            curPanel.transform.parent = battleController.curCanvas.transform;
            //改变rect的位置
            curPanel.transform.position +=
                new Vector3(curPanel.GetComponent<RectTransform>().rect.width * index, 0, 0);
            //绑定战斗角色和操作表
            PanelBind.bindPlayerPanel((PersonAI)battleActor, battleController.playerPanels[index].transform);

            //绑定玩家的姓名
            battleActor.nameText.text = personName;

            //返回战斗角色类
            return battleActor;
        }

        /// <summary>
        /// 初始化玩家出战角色的参数
        /// </summary>
        /// <param name="battleActor"></param>
        /// <param name="i"></param>
        private static void initBattlePerson(ref BattleActor battleActor, int i, Person person) {
            if (BattleController.getInstance().isTest) {
                //初始化人物各属性值(随机)，只做测试使用
                initPersonPara(ref battleActor, 1.0f);
            } else {
                //如果当前不处于测试状态，按照person初始化battleActor
                initTrueBattlePerson(ref battleActor, person);
            }
            
            //初始化人物的id
            battleActor.myId = i;
            //初始化人物的位置
            battleActor.pos = 0.0f;
            //初始化人物是否为玩家角色
            battleActor.isPlayer = true;
        }

        /// <summary>
        /// 根据person获取对应的属性
        /// </summary>
        /// <param name="battleActor"></param>
        /// <param name="person"></param>
        private static void initTrueBattlePerson(ref BattleActor battleActor, Person person) {
            //初始化人物各属性值(随机)，只做测试使用
            battleActor.maxHealthPoint = (float)person.getHpMax();
            battleActor.maxActionPoint = (float)person.getApMax();
            battleActor.hpRecovery = (float)person.getHpRec();
            battleActor.apRecovery = (float)person.getApRec();
            battleActor.atkNeedTime = 1 / (float)person.getValAts();
            battleActor.moveSpeed = (float)person.getValSpd();
            battleActor.atkDamage = (float)person.getValAtk();
            battleActor.atkRange = (float)person.getRange();
            battleActor.damageRate = (float)person.getValHit();
            battleActor.critDamage = (float)person.getValCrd();
            battleActor.critRate = (float)person.getValCrc();
            battleActor.hitRate = (float)person.getValHrate();
            battleActor.dodgeRate = (float)person.getValErate();
            battleActor.skillPara = 1.0f;

            //初始化五个属性
            battleActor.vitality = person.vitality;
            battleActor.strength = person.strength;
            battleActor.agility = person.agile;
            battleActor.technical = person.technique;
            battleActor.intelligence = person.intelligence;

            //初始化两个技能
            battleActor.addSkill(person.getSkillCarryed(1));
            battleActor.addSkill(person.getSkillCarryed(2));
        }

        /// <summary>
        /// 对出战角色进行随机参数的初始化
        /// </summary>
        /// <param name="person">当前角色</param>
        /// <param name="para">相应参数的弱化</param>
        public static void initPersonPara(ref BattleActor person, float para) {
            //初始化人物各属性值(随机)，只做测试使用
            person.maxHealthPoint = 2000.0f * para;
            person.maxActionPoint = 2000.0f * para;
            person.hpRecovery = 5;
            if (BattleController.getInstance().isTest == true) {
                person.apRecovery = 100;
            } else {
                person.apRecovery = 5;
            }
            person.atkNeedTime = 1 / para;
            person.moveSpeed = 3.0f;
            person.atkDamage = 10f * para;
            person.atkRange = Random.Range(1f, 5f) * para;
            person.damageRate = 1.0f;
            person.critDamage = 1.6f;
            person.critRate = 0.2f;
            person.hitRate = 1.0f;
            person.dodgeRate = 0.0f;
            person.skillPara = 1.0f;

            //初始化五个属性
            person.vitality = 50;
            person.strength = 50;
            person.agility = 50;
            person.technical = 50;
            person.intelligence = 50;

            //初始化两个技能
            person.addSkill(skill1);
            person.addSkill(skill2);
        }
    }
}

