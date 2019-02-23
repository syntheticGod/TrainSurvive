/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/26 16:22:21
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle
{//【暴怒型】：始终攻击（当前攻击力*当前攻击速度/最大生命值）最低的目标

    public class type2AI : BattleActor
    {

        private List<int> active_skill_indexList = new List<int>();
        private bool has_active_skill;
        private int skill_released_index = 0;//技能列表中的下标，不超过count
        public type2AI()
        {
        }

        protected override void AIStrategy()
        {

            if (subStateController.curActionState == ActionStateEnum.ATTACK || subStateController.curActionState == ActionStateEnum.SKILL)
            {
                return;
            }

            int targetId = -1;
      
            foreach (BattleActor enemyActor in enemyActors)
            {
                //如果当前敌人存活
                if (enemyActor.isAlive)
                {
                    if (targetId == -1 || cal(enemyActor) < cal(enemyActors[targetId]))
                    {
                        targetId = enemyActor.myId;
                    }
                }
            }
            atkTarget = targetId;
            selectedAtkTarget = atkTarget;

            //如果没找到（没有敌人），返回
            if (targetId == -1)
            {
                return;
            }
            //先朝向敌人，若朝向不对，开始转向
            //获取当前角色应该的朝向（朝向敌人）
            curMotionDir = enemyActors[atkTarget].pos - pos > 0.0f ? 1 : -1;

            //优先释放技能
            if (has_active_skill)
            {
                if (releaseSkill(active_skill_indexList[skill_released_index]))
                {
                    skill_released_index = skill_released_index == active_skill_indexList.Count - 1 ? 0 : (skill_released_index + 1);
                    return;//释放技能成功，状态已切换
                }
            }

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
            throw new NotImplementedException();
        }

        protected override void otherInit()
        {
            int index = 0;
            foreach (BaseSkill skill in skillList)
            {
                if (skill.skillType == BaseSkill.SkillType.ACTIVE)
                    active_skill_indexList.Add(index);
                index++;
            }
            has_active_skill = active_skill_indexList.Count == 0 ? false : true;

            //throw new NotImplementedException();
        }

        private float cal(BattleActor enemyActor)
        {
            return enemyActor.atkDamage / enemyActor.atkNeedTime / enemyActor.maxHealthPoint;
        }
    }
}