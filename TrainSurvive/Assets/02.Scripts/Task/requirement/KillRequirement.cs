/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 21:43:56
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBattle;
[System.Serializable]
public class KillRequirement : TaskRequirement
{
    public int monsterId
    {
        get; private set;
    }
    public string monsterName
    {
        get; private set;
    }
    public int needKillNums
    {
        get;private set;
    }
    public int hasKillNums
    {
        get; private set;
    }
    private KillRequirement(){ }
    public KillRequirement(int monsterid, int nums)
    {
        monsterId = monsterid;
        needKillNums = nums;
        hasKillNums = 0;
        MonsterInitializer ini = MonsterInitializer.getInstance();
        monsterName = ini.getMonsterName(monsterid);
        condition = "击杀：" + monsterName + "  "+ hasKillNums+"/" + needKillNums;
        description = "你还需要击杀" + monsterName + (needKillNums- hasKillNums)+"个！";//以后也可以只在npc那展示金钱数字，其他话在npc数据里补充
        isfinish = false;
    }


    public override bool achieveGoal()
    {
        if (isfinish)
            return true;

        isfinish = hasKillNums >= needKillNums;
        if (isfinish)
        {
            condition = monsterName + "击杀完毕";
            description = condition;
        }
        return isfinish;
        throw new NotImplementedException();
    }

    private void killMonster(int targetId)
    {
        if(targetId== monsterId)
        {
            hasKillNums++;
            condition = "已击杀：" + monsterName + "  " + hasKillNums + "/" + needKillNums;
            description = "你还需要击杀" + monsterName + (needKillNums - hasKillNums) + "个！";//以后也可以只在npc那展示金钱数字，其他话在npc数据里补充
        }
    }

    public override void conditionChange(int numOrId)
    {
        killMonster(numOrId);
    }
}
