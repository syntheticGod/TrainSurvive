/*
 * 描述：
 * 作者：王安鑫
 * 创建时间：2018/11/26 19:46:16
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldBattle {
    public class PersonAI : BattleActor {
        //撤退所需的等待时间
        private const float retreatNeedTime = 5.0f;
        //设置控制射程的范围参数
        private const float controlRangePara = 0.8f;
        //绑定两个技能按钮
        public Button []skillBtn;

        //玩家当前的策略状态
        public StrategyStateEnum strategyState;

        //玩家的策略状态（由玩家控制）
        public enum StrategyStateEnum {
            NONE = -1,

            //进攻
            ATTACK,

            //后退
            BACK_UP,

            //控制射程
            CONTROL_RANGE,

            //休息
            REST,

            NUM
        }

        //玩家角色所需要的初始化方法
        protected override void otherInit() {
            //初始化玩家游戏状态开始为不动
            strategyState = StrategyStateEnum.NONE;
        }

        //玩家角色战斗的AI（通过指令来控制）
        protected override void AIStrategy() {
            //先执行玩家行动的策略
            switch (strategyState) {
                //进攻状态
                case StrategyStateEnum.ATTACK:
                    attackState();
                    break;

                //后退状态
                case StrategyStateEnum.BACK_UP:
                    backUpState();
                    break;

                //控制射程
                case StrategyStateEnum.CONTROL_RANGE:
                    controlRangeState();
                    break;

                //休息状态（进入休息子状态）
                case StrategyStateEnum.REST:
                    changeSubState(ActionStateEnum.REST);
                    break;
            }

            //调用是否可以撤退
            checkCanRetreat();
        }

        /// <summary>
        ///进攻状态
        ///寻找最近的目标，距离相等时则找序号最小的
        ///如果目标小于攻击范围，则进入攻击状态
        ///否则靠近目标，进入移动状态
        /// </summary>
        private void attackState() {
            //如果当前处于攻击状态，等这次攻击完
            if (subStateController.curActionState == ActionStateEnum.ATTACK) {
                return;
            }

            //选中最近的敌人
            selectNearestEnemy();

            //如果没找到（没有敌人），返回
            if (atkTarget == -1) {
                return;
            }

            //先朝向敌人，若朝向不对，开始转向
            //获取当前角色应该的朝向（朝向敌人）
            curMotionDir = enemyActors[atkTarget].pos - pos > 0.0f ? 1 : -1;

            //如果目标小于攻击范围，则进入攻击状态
            if (Mathf.Abs(enemyActors[atkTarget].pos - pos) <= atkRange) {
                changeSubState(ActionStateEnum.ATTACK);
            } else {
                //进入移动状态
                changeSubState(ActionStateEnum.MOTION);
            }

            return;
        }

        /// <summary>
        /// 后退状态
        /// 如果当前处于前进状态，更改方向
        /// 进入移动状态
        /// </summary>
        private void backUpState() {
            //设置当前的前进方向应该为后退
            curMotionDir = -forwardDir;
            //curMotionDir = -getMotionDir();

            //进入移动状态
            changeSubState(ActionStateEnum.MOTION);

            return;
        }

        /// <summary>
        /// 控制范围状态
        ///寻找最近的目标，距离相等时则找序号最小的
        ///如果目标在控制距离范围内，进入攻击状态
        ///如果目标在攻击范围之外，靠近目标
        ///如果目标离自己太近，远离目标
        /// </summary>
        private void controlRangeState() {
            //如果当前处于攻击状态，等这次攻击完
            if (subStateController.curActionState == ActionStateEnum.ATTACK) {
                return;
            }

            //选中最近的敌人
            selectNearestEnemy();

            //如果没找到（没有敌人），返回
            if (atkTarget == -1) {
                return;
            }

            //当前敌人与自己的距离
            float distance = Mathf.Abs(enemyActors[atkTarget].pos - pos);
            //获取当前角色应该的朝向（朝向敌人）
            curMotionDir = enemyActors[atkTarget].pos - pos > 0.0f ? 1 : -1;

            //如果当前距离超过攻击距离，则向目标靠近
            if (distance > atkRange) {
                //进入移动状态
                changeSubState(ActionStateEnum.MOTION);
            }
            //如果当前距离小于控制范围，则远离敌人
            else if (distance < atkRange * controlRangePara) {
                //远离敌人
                curMotionDir *= -1;
                //进入移动状态
                changeSubState(ActionStateEnum.MOTION);
            }
            //如果距离在控制范围之内，进行攻击
            else {
                //进入攻击状态
                changeSubState(ActionStateEnum.ATTACK);
            }

            return;
        }

        //改变成攻击大状态
        public void changeAttackState(bool isOn) {
            if (isOn == false) {
                return;
            }
            Debug.Log(myId + "改变成攻击大状态");
            strategyState = StrategyStateEnum.ATTACK;
        }
        //改变成后退大状态
        public void changeBackUpState(bool isOn) {
            if (isOn == false) {
                return;
            }
            Debug.Log(myId + "改变成后退大状态");
            strategyState = StrategyStateEnum.BACK_UP;
        }
        //改变成控制射程大状态
        public void changeControlRangeState(bool isOn) {
            if (isOn == false) {
                return;
            }
            Debug.Log(myId + "改变成控制射程大状态");
            strategyState = StrategyStateEnum.CONTROL_RANGE;
        }
        //改变成休息大状态
        public void changeRestState(bool isOn) {
            if (isOn == false) {
                return;
            }
            Debug.Log(myId + "改变成休息大状态");
            strategyState = StrategyStateEnum.REST;
        }

        //切换目标
        public void changeSelectTarget(int num) {
            Debug.Log("切换所选的目标" + num);
            selectedAtkTarget = num - 1;
            //如果当前目标已经死亡，这次选择目标不生效
            if (enemyActors[selectedAtkTarget].isAlive == false) {
                selectedAtkTarget = -1;
            }
        }

        /// <summary>
        /// 检查当前是否撤退
        /// </summary>
        private void checkCanRetreat() {
            //如果当前是攻击状态或技能状态，撤退时间清空
            if (subStateController.curActionState == ActionStateEnum.ATTACK
                || subStateController.curActionState == ActionStateEnum.SKILL) {
                curRetreatPassTime = 0.0f;
            }

            //当前撤退等待时间增加
            curRetreatPassTime += Time.deltaTime;

            //Debug.Log(curRetreatWaitTime);

            //如果当前撤退等待时间大于撤退所需时间
            if (curRetreatPassTime > retreatNeedTime) {
                //显示可以撤退
                retreatBtn.interactable = true;
            } else {
                //如果不是显示不能撤退
                retreatBtn.interactable = false;
            }
        }

        /// <summary>
        /// 开始撤退状态
        /// </summary>
        public void startRetreat() {
            changeSubState(ActionStateEnum.RETREAT);
        }

        /// <summary>
        /// 开始释放技能
        /// </summary>
        /// <param name="skillIndex">技能的编号</param>
        public void startSkillRelease(int skillIndex) {
            Debug.Log("释放技能" + skillIndex);
            //释放技能
            releaseSkill(skillIndex);
        }

        /// <summary>
        /// 当ap改变时技能按钮激活
        /// </summary>
        protected override void changeSkillBtn() {
            for (int i = 0; i < skillList.Count; i++) {
                //如果当前技能不是被动技能且能释放，则该按钮可用
                if (skillList[i].isPassive == false && skillList[i].canReleaseSkill()) {
                    skillBtn[i].interactable = true;
                } else {
                    //否则设置其不可用
                    skillBtn[i].interactable = false;
                }
            }
        }

        /// <summary>
        /// 选中最近的敌人
        /// 如果玩家已经选中了敌人，则不变
        /// 否则找最近的敌人
        /// </summary>
        protected void selectNearestEnemy() {
            //如果当前已经存在玩家选中的目标了，朝着目标行动
            if (selectedAtkTarget != -1 && enemyActors[selectedAtkTarget].isAlive == true) {
                atkTarget = selectedAtkTarget;
            } else {
                //获取距离最近的目标
                atkTarget = HelpSelectTarget.getNearestEnemy(this);
            }
        }
    }
}

