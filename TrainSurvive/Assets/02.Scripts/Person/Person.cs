/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/11/20 10:41:45
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Assets._02.Scripts.zhxUIScripts;
using TTT.Resource;
using TTT.Utility;

[System.Serializable]
public class Person
{
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
    /// <summary>
    /// 已训练次数
    /// </summary>
    public int trainCnt = 0;
    public bool hasWeapon = false;
    public int weaponId = 0;
    /// <summary>
    /// 是否外出，即是否在探险队里（探险队不一定处于出动状态）
    /// </summary>
    public bool ifOuting = false;
    public string name = "张三";
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
    /// 人物所持有的武器对象
    /// </summary>
    public Weapon weapon = null;
    private EProfession[] professions;
    public Profession getProfession(int index)
    {
        if (professions[index] == EProfession.NONE)
            return null;
        return StaticResource.GetProfession(professions[index]);
    }
    [NonSerialized]
    private int lastWeaponId = -1;
    private Person()
    {
        //保留以后用
        professions = new EProfession[3] { EProfession.NONE, EProfession.NONE, EProfession.NONE };
    }
    /// <summary>
    /// 生成一个随机属性的人物（未持有武器）
    /// </summary>
    /// <returns></returns>
    public static Person RandomPerson()
    {
        Person p = new Person();
        p.ismale = MathTool.RandomInt(2) == 0;
        p.name = StaticResource.RandomNPCName(p.ismale);
        p.vitality = MathTool.RandomRange(0, 10);
        p.strength = MathTool.RandomRange(0, 10);
        p.agile = MathTool.RandomRange(0, 10);
        p.technique = MathTool.RandomRange(0, 10);
        p.intelligence = MathTool.RandomRange(0, 10);
        p.ifOuting = false;
        return p;
    }
    //以下获取的属性均保留numsLeft位小数
    public double getHpMax()
    {
        double hpMax = 100 * (1 + 0.05 * vitality);
        return Math.Round(hpMax, numsLeft);
    }
    public double getApMax()
    {
        double apMax = 100 * (1 + 0.05 * intelligence);
        return Math.Round(apMax, numsLeft);
    }
    public double getHpRec()
    {
        return Math.Round(5.000, numsLeft);
    }
    public double getApRec()
    {
        double apRec = 5 * (1 + 0.05 * intelligence);
        if (hasWeapon)
        {
            apRec = apRec * weapon.facArec;
        }
        return Math.Round(apRec, numsLeft);
    }
    public double getValAtk()
    {
        double atk = 10 * (1 + 0.05 * strength);
        if (hasWeapon)
        {

        }
        return Math.Round(atk, numsLeft);
    }
    public double getValAts()
    {
        double ats = 1 * (1 + 0.03 * agile);
        if (hasWeapon)
        {

        }
        return Math.Round(ats, numsLeft);
    }
    public double getValSpd()
    {
        double spd = 1 * (1 + 0.02 * agile);
        if (hasWeapon)
        {

        }
        return Math.Round(spd, numsLeft);
    }
    public double getValCrc()
    {
        double crc = 0.02 * technique;
        if (hasWeapon)
        {

        }
        return Math.Round(crc, numsLeft);
    }
    public double getValCrd()
    {
        double crd = 1.6 * (1 + 0.03 * technique);
        if (hasWeapon)
        {

        }
        return Math.Round(crd, numsLeft);
    }
    public double getValHrate()
    {
        double num = 1 * (1 + 0.025 * technique);
        return Math.Round(num, numsLeft);
    }
    public double getValErate()
    {
        double num = 1 * (0 + 0.02 * agile);
        return Math.Round(num, numsLeft);
    }
    public double getRange()
    {
        double num = 1;
        if (hasWeapon)
        {
            //num=..
        }
        num = num * (1 + 0.03 * technique);
        return Math.Round(num, numsLeft);
    }
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
}
