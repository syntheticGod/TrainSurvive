/*
 * 描述：
 * 作者：王安鑫
 * 创建时间：2018/12/8 16:41:56
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class EnemyAI : BattleActor {

        //玩家角色所需要的初始化方法
        protected override void otherInit() {
            
        }

        //玩家角色战斗的AI（通过指令来控制）
        protected override void AIStrategy() {
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
                atkTarget = getNearestEnemy();
            }
        }

        /// <summary>
        /// 查找最近的目标
        /// 距离相等时则找序号最小的
        /// </summary>
        /// <returns>返回目标的id</returns>
        private int getNearestEnemy() {
            int nearestId = -1;

            //寻找距离最近相应目标(距离最近，序号最前)
            foreach (BattleActor enemyActor in enemyActors) {
                //如果当前敌人存活
                if (enemyActor.isAlive) {
                    if (nearestId == -1 || Mathf.Abs(enemyActor.pos - pos) > Mathf.Abs(enemyActors[nearestId].pos - pos)) {
                        nearestId = enemyActor.myId;
                    }
                }
            }

            return nearestId;
        }
    }
}

