/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/26 16:49:23
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldBattle
{//【稳健型】：就近攻击，如果（当前攻击目标与自己的距离）<（自己射程的一半），就后退，直至距离拉开至（>=射程的一半/接近地图边界） //类似玩家的控制射程，测试后视情况考虑与控制射程合并


    public class type4AI : BattleActor
{

    private List<int> active_skill_indexList = new List<int>();
    private bool has_active_skill;
    private int skill_released_index = 0;//技能列表中的下标，不超过count
    private float mapLength;
    public type4AI()
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
                if (targetId == -1 || enemyActor.pos> enemyActors[targetId].pos)
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

         //优先释放技能
            if (has_active_skill)
            {
                if (releaseSkill(active_skill_indexList[skill_released_index]))
                {
                    skill_released_index = skill_released_index == active_skill_indexList.Count - 1 ? 0 : (skill_released_index + 1);
                    return;//释放技能成功，状态已切换
                }
            }

            float pos_distance = pos - enemyActors[atkTarget].pos;
            //射程外往朝向左，超过1/2射程朝向右
            if (pos_distance > atkRange)
            {
                curMotionDir = -1;
                changeSubState(ActionStateEnum.MOTION);
            }
            else if(pos_distance <= atkRange&& pos_distance >= atkRange/2)
            {
                curMotionDir = -1;
                changeSubState(ActionStateEnum.ATTACK);
            }
            else
            {
                if (pos >= mapLength)//退至边界
                {
                    curMotionDir = -1;
                    changeSubState(ActionStateEnum.ATTACK);
                }
                else
                {
                    curMotionDir = 1;
                    changeSubState(ActionStateEnum.MOTION);
                }               
            }
       

        return;
        //throw new NotImplementedException();
    }

    protected override void otherInit()
    {
            mapLength = BattleController.getInstance().battleMapLen;
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


}
}

