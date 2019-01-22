/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/11 15:58:47
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System;
using Assets._02.Scripts.zhxUIScripts;
using UnityEngine;

public class Monster {
    public int id;
    /// <summary>
    /// 体力
    /// </summary>
    public int vitality = 0;
    /// <summary>
    /// 力量
    /// </summary>
    public int strength = 0;
    /// <summary>
    /// 敏捷
    /// </summary>
    public int agile = 0;
    /// <summary>
    /// 技巧
    /// </summary>
    public int technique = 0;
    /// <summary>
    /// 智力
    /// </summary>
    public int intelligence = 0;
    public int rank = 1;
    public bool hasWeapon = false;
    public int weaponId = 0;

    public string name = "";
    /// <summary>
    /// 性别用ismale代替
    /// </summary>
    public bool ismale = true;
    //技能属性待添加

    /// <summary>
    /// 小数属性保留的位数
    /// </summary>
    private const int numsLeft = 3;

    /// <summary>
    /// 怪物所持有的武器对象
    /// </summary>
    public Weapon weapon = null;


    public Monster()
    {
        //保留以后用
    }

    //以下获取的属性均保留numsLeft位小数
    public double getHpMax()
    {
        double hpMax = 100 * (1 + 0.05 * vitality);
        return Math.Round(hpMax, numsLeft) * rank;
    }
    public double getApMax()
    {
        double apMax = 100 * (1 + 0.05 * intelligence);
        return Math.Round(apMax, numsLeft) * rank;
    }
    public double getHpRec()
    {
        return Math.Round(5.000, numsLeft) * rank;
    }
    public double getApRec()
    {
        double apRec = 5 * (1 + 0.05 * intelligence);
        if (hasWeapon)
        {
            apRec = apRec * weapon.facArec;
        }
        return Math.Round(apRec, numsLeft) * rank;
    }
    public double getValAtk()
    {
        double atk = 10 * (1 + 0.05 * strength);
        if (hasWeapon)
        {
            atk = atk * weapon.facAtk;
        }
        return Math.Round(atk, numsLeft) * rank;
    }
    public double getValAts()
    {
        double ats = 1 * (1 + 0.03 * agile);
        if (hasWeapon)
        {
            ats = ats * weapon.facAts;
        }
        return Math.Round(ats, numsLeft) * rank;
    }
    public double getValSpd()
    {
        double spd = 1 * (1 + 0.02 * agile);
        if (hasWeapon)
        {
            spd = spd * weapon.facSpd;
        }
        return Math.Round(spd, numsLeft) * rank;
    }
    public double getValCrc()
    {
        double crc = 0.02 * technique;
        if (hasWeapon)
        {
            crc = crc + weapon.modCrc;
        }
        return Math.Round(crc, numsLeft) * rank;
    }
    public double getValCrd()
    {
        double crd = 1.6 + 0.03 * technique;
        if (hasWeapon)
        {
            crd = crd + weapon.modCrd;
        }
        return Math.Round(crd, numsLeft) * rank;
    }
    public double getValHrate()
    {
        double num = 1 * (1 + 0.025 * technique);
        return Math.Round(num, numsLeft) * rank;
    }
    public double getValErate()
    {
        double num = 0.02 * agile;
        return Math.Round(num, numsLeft) * rank;
    }
    public double getValHit()
    {
        double num = 1;
        if (hasWeapon)
        {
            num = weapon.modHit;
        }
        return Math.Round(num, numsLeft) * rank;
    }
    public double getRange()
    {
        double num = 1;
        if (hasWeapon)
        {
            num = weapon.range;
        }
        num = num * (1 + 0.03 * technique);
        return Math.Round(num, numsLeft);
    }
    /*
    public void equipWeapon(Weapon weapon)
    {
        this.weapon = (Weapon)weapon.Clone();
        weaponId = weapon.id;
        hasWeapon = true;
    }

    public void unequipWeapon()
    {
        weapon = null;
        weaponId = -1;
        hasWeapon = false;
    }
    */
}
