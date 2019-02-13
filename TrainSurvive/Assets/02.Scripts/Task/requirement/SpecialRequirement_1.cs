/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/28 17:45:57
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRequirement_1 : TaskRequirement
{
    public SpecialRequirement_1()
    {
        description = "队伍中需要有智力>=25,敏捷>=15的女性";
        condition = description;
        isfinish = false;
    }
    public override bool achieveGoal()
    {
        foreach(Person p in World.getInstance().Persons)
        {
            if (p.ismale = false && p.intelligence >= 25 && p.agile >= 15)
            {
                isfinish = true;
                break;
            }
        }       
        return isfinish;
        throw new NotImplementedException();
    }

    public override void conditionChange(int numOrId)
    {
        throw new NotImplementedException();
    }
}
