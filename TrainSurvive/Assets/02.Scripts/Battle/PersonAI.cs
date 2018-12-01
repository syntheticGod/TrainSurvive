/*
 * 描述：
 * 作者：王安鑫
 * 创建时间：2018/11/26 19:46:16
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class PersonAI : BattleAI {
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
                    startRestState();
                    break;
            }

            //调用是否可以撤退
            checkCanRetreat();
        }

        // Use this for initialization
        //void Start() {

        //}

        /// <summary>
        /// 开始撤退
        /// </summary>
        public void startRetreat() {
            //播放撤退动画

            //设置撤退状态
            actionState = ActionStateEnum.RETREAT;

            //该角色本场不行动
            isAlive = false;

            //每次有角色死亡或撤退调用battleConroller判断本场战斗是否结束
            battleController.isBattleEnd(isPlayer);
        }

        /// <summary>
        ///进攻状态
        ///寻找最近的目标，距离相等时则找序号最小的
        ///如果目标小于攻击范围，则进入攻击状态
        ///否则靠近目标，进入移动状态
        /// </summary>
        private void attackState() {
            //如果当前处于攻击状态，等这次攻击完
            if (actionState == ActionStateEnum.ATTACK) {
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
            curMotionDir = enemy[atkTarget].pos - pos > 0.0f ? 1 : -1;

            //如果目标小于攻击范围，则进入攻击状态
            if (Mathf.Abs(enemy[atkTarget].pos - pos) <= atkRange) {
                startAttackState();
            } else {
                //进入移动状态
                actionState = ActionStateEnum.MOTION;
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
            actionState = ActionStateEnum.MOTION;

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
            if (actionState == ActionStateEnum.ATTACK) {
                return;
            }

            //选中最近的敌人
            selectNearestEnemy();

            //如果没找到（没有敌人），返回
            if (atkTarget == -1) {
                return;
            }

            //当前敌人与自己的距离
            float distance = Mathf.Abs(enemy[atkTarget].pos - pos);
            //获取当前角色应该的朝向（朝向敌人）
            curMotionDir = enemy[atkTarget].pos - pos > 0.0f ? 1 : -1;

            //如果当前距离超过攻击距离，则向目标靠近
            if (distance > atkRange) {
                //进入移动状态
                actionState = ActionStateEnum.MOTION;
            }
            //如果当前距离小于控制范围，则远离敌人
            else if (distance < atkRange * controlRangePara) {
                //远离敌人
                curMotionDir *= -1;
                //进入移动状态
                actionState = ActionStateEnum.MOTION;
            }
            //如果距离在控制范围之内，进行攻击
            else {
                //进入攻击状态
                startAttackState();
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

        /// <summary>
        /// 检查当前是否撤退
        /// </summary>
        private void checkCanRetreat() {
            //如果当前是攻击状态或技能状态，撤退时间清空
            if (actionState == ActionStateEnum.ATTACK || actionState == ActionStateEnum.SKILL) {
                curRetreatWaitTime = 0.0f;
            }

            //当前撤退等待时间增加
            curRetreatWaitTime += Time.deltaTime;

            //Debug.Log(curRetreatWaitTime);

            //如果当前撤退等待时间大于撤退所需时间
            if (curRetreatWaitTime > retreatNeedTime) {
                //显示可以撤退
                retreatBtn.interactable = true;
            } else {
                //如果不是显示不能撤退
                retreatBtn.interactable = false;
            }
        }
    }
}

