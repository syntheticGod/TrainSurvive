/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 21:44:52
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MoneyRequirement:TaskRequirement  {
    /// <summary>
    /// 需要的钱（钱必须一次付清）
    /// </summary>
    public int needMoney;

    /// <param name="moneyNums">需要的金钱</param>
    public MoneyRequirement(int moneyNums)
    {
        needMoney = moneyNums;
        condition = "需付清金钱："+ needMoney;
        _description = "我需要" + needMoney + "元，你带来了吗？";//以后也可以只在npc那展示金钱数字，其他话在npc数据里补充
        finish = false;
    }

    public override void achieveGoal(int nums)
    {
        //待补充（展示在界面上什么的）

        
            finish = true;
            condition = "已付清：" + needMoney;          
        throw new NotImplementedException();
    }
}
