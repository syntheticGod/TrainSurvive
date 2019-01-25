/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/22 15:25:40
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBattle;

public class MonsterAdapter {

    public static void setMonsterBattleActor(ref BattleActor actor, ref Monster monster)
    {       
        
        actor.task_monsterId = monster.id;
        actor.maxHealthPoint = (float)monster.getHpMax();
        actor.maxActionPoint = (float)monster.getApMax();
        actor.hpRecovery = (float)monster.getHpRec();
        actor.apRecovery = (float)monster.getApRec();
        actor.atkNeedTime = (float)monster.getValAts();
        actor.moveSpeed = (float)monster.getValSpd();
        actor.atkDamage = (float)monster.getValAtk();
        actor.atkRange = (float)monster.getRange();
        actor.damageRate = (float)monster.getValHit();
        actor.critDamage = (float)monster.getValCrd();
        actor.critRate = (float)monster.getValCrc();
        actor.hitRate = (float)monster.getValHrate();
        actor.dodgeRate = (float)monster.getValErate();

        actor.name_str = monster.name;
        actor.exp = monster.exp;
        actor.size = monster.size;
        actor.rank = monster.rank;
        actor.model = monster.model;
    }
}
