/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/11 14:23:21
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一直攻击最靠近玩家的AI，怪物AI里selectedAtkTarget字段无效
namespace WorldBattle
{
    public class BeatNearAI : BattleActor
{
    protected override void AIStrategy()
    {
            //如果当前处于攻击状态，等这次攻击完
            if (subStateController.curActionState == ActionStateEnum.ATTACK)
            {
                return;
            }

            int targetId = -1;
            //寻找距离最近相应目标(距离最近，序号最前)
            foreach (BattleActor enemyActor in enemyActors)
            {
                //如果当前敌人存活
                if (enemyActor.isAlive)
                {
                    if (targetId == -1 || Mathf.Abs(enemyActor.pos - pos) < Mathf.Abs(enemyActors[targetId].pos - pos))
                    {
                        targetId = enemyActor.myId;
                    }
                }
            }
            atkTarget = targetId;
            
            //如果没找到（没有敌人），返回
            if (atkTarget == -1)
            {
                return;
            }

            //先朝向敌人，若朝向不对，开始转向
            //获取当前角色应该的朝向（朝向敌人）
            curMotionDir = enemyActors[atkTarget].pos - pos > 0.0f ? 1 : -1;

            //如果目标小于攻击范围，则进入攻击状态
            if (Mathf.Abs(enemyActors[atkTarget].pos - pos) <= atkRange)
            {
                changeSubState(ActionStateEnum.ATTACK);
            }
            else
            {
                //进入移动状态
                changeSubState(ActionStateEnum.MOTION);
            }

            return;
        }

    protected override void otherInit()
    {

    }
}
}
