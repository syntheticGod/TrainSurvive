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
    public int vitality { get { return attriNumber[(int)EAttribute.VITALITY]; } set { attriNumber[(int)EAttribute.VITALITY] = value; } }
    /// <summary>
    /// 力量
    /// </summary>
    public int strength { get { return attriNumber[(int)EAttribute.STRENGTH]; } set { attriNumber[(int)EAttribute.STRENGTH] = value; } }
    /// <summary>
    /// 敏捷
    /// </summary>
    public int agile { get { return attriNumber[(int)EAttribute.AGILE]; } set { attriNumber[(int)EAttribute.AGILE] = value; } }
    /// <summary>
    /// 技巧
    /// </summary>
    public int technique { get { return attriNumber[(int)EAttribute.TECHNIQUE]; } set { attriNumber[(int)EAttribute.TECHNIQUE] = value; } }
    /// <summary>
    /// 智力
    /// </summary>
    public int intelligence { get { return attriNumber[(int)EAttribute.INTELLIGENCE]; } set { attriNumber[(int)EAttribute.INTELLIGENCE] = value; } }
    private int[] attriNumber;
    public int GetAttriNumber(EAttribute eAttribute)
    {
        return attriNumber[(int)eAttribute];
    }
    public void AddAttriNumber(EAttribute eAttribute, int delta)
    {
        attriNumber[(int)eAttribute] += delta;
    }
    private int[] attriMaxNumber;
    public int GetAttriMaxNumber(EAttribute eAttribute)
    {
        return attriMaxNumber[(int)eAttribute];
    }
    /// <summary>
    /// 已训练次数
    /// </summary>
    public int trainCnt = 0;
    public bool hasWeapon = false;
    public int weaponId = 0;
    /// <summary>
    /// 人物是否出战
    /// </summary>
    public bool ifReadyForFighting = false;
    public string name = "";
    /// <summary>
    /// 性别用ismale代替
    /// </summary>
    public bool ismale = true;
    /// <summary>
    /// 小数属性保留的位数
    /// </summary>
    private const int numsLeft = 3;
    /// <summary>
    /// 人物所持有的武器对象
    /// </summary>
    public Weapon weapon = null;
    //----------技能----------↓
    private List<int> skillsGot = new List<int>();
    /// <summary>
    /// 添加人物学习到的技能
    /// 如果已经存在则不添加
    /// </summary>
    /// <param name="skillID"></param>
    public void AddGotSkill(int skillID)
    {
        ContainerTool.InsertSortUnique(skillsGot, skillID);
    }
    public List<int> GetSkillsGot()
    {
        return skillsGot;
    }
    public bool IfHaveGotTheSkill(int skillID)
    {
        return ContainerTool.IfContainByBinarySearching(skillsGot, skillID);
    }
    public bool IfHaveGotTheSkill(SkillInfo skill)
    {
        return ContainerTool.IfContainByBinarySearching(skillsGot, skill.ID);
    }
    //----------技能----------↑

    //----------专精----------↓
    /// <summary>
    /// 三个专精槽位
    /// </summary>
    private int[] professions;
    /// <summary>
    /// 允许的槽位
    /// </summary>
    private int professionAvaliable;
    /// <summary>
    /// 获取第index级专精
    /// </summary>
    /// <param name="index">[0,1,2] => 第一级 第二级 第三级</param>
    /// <returns>
    /// NULL：未专精
    /// NOT NULL：专精对象
    /// </returns>
    public Profession getProfession(int index)
    {
        if (professions[index] == -1)
            return null;
        return StaticResource.GetProfessionByID(professions[index]);
    }
    /// <summary>
    /// 判断专精槽是否足够
    /// </summary>
    /// <returns></returns>
    public bool IfProfessionAvailable()
    {
        return professions[professionAvaliable - 1] == -1;
    }
    /// <summary>
    /// 判断指定属性是否已专精
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool IfProfession(EAttribute attribute)
    {
        for(int i = 0; i < professions.Length; i++)
        {
            Profession profession = getProfession(i);
            if (profession == null) continue;
            foreach(Profession.AbiReq req in profession.AbiReqs)
            {
                //如果已学的专精的属性要求中包含该属性，则返回真
                if(req.Abi == attribute)
                    return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 获取最高级的专精
    /// </summary>
    /// <returns>
    /// NOT NULL：最高级专精
    /// NULL：没有修过专精
    /// </returns>
    public Profession getTopProfession()
    {
        for (int i = professions.Length - 1; i >= 0; i--)
        {
            if (professions[i] != -1)
                return StaticResource.GetProfessionByID(professions[i]);
        }
        return null;
    }
    /// <summary>
    /// 根据专精的Level绑定专精
    /// </summary>
    /// <param name="profession"></param>
    public void setProfession(Profession profession)
    {
        if (profession.Level == EProfessionLevel.NONE)
        {
            Debug.LogError("专精错误");
            return;
        }
        professions[(int)profession.Level] = profession.ID;
    }
    
    //----------专精----------↑

    [NonSerialized]
    private int lastWeaponId = -1;
    private Person()
    {
        //保留以后用
        professions = new int[3] { -1, -1, -1 };
        attriNumber = new int[(int)EAttribute.NUM] { 0, 0, 0, 0, 0 };
        //默认最大属性为10
        attriMaxNumber = new int[(int)EAttribute.NUM] { 10, 10, 10, 10, 10 };
        //初始专精槽数为1
        professionAvaliable = 1;
        ifReadyForFighting = false;
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
        for (EAttribute itr = EAttribute.NONE + 1; itr < EAttribute.NUM; itr++)
        {
            p.attriNumber[(int)itr] = MathTool.RandomRange(0, p.attriMaxNumber[(int)itr]);
        }
        //获得无条件获得的技能
        SkillInfo[] avaliableSkills = StaticResource.GetAvailableSkills(p.attriNumber);
        if (avaliableSkills != null)
        {
            foreach (SkillInfo skill in avaliableSkills)
            {
                p.AddGotSkill(skill.ID);
            }
        }
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
            atk = atk * weapon.facAtk;
        }
        return Math.Round(atk, numsLeft);
    }
    public double getValAts()
    {
        double ats = 1 * (1 + 0.03 * agile);
        if (hasWeapon)
        {
            ats = ats * weapon.facAts;
        }
        return Math.Round(ats, numsLeft);
    }
    public double getValSpd()
    {
        double spd = 1 * (1 + 0.02 * agile);
        if (hasWeapon)
        {
            spd = spd * weapon.facSpd;
        }
        return Math.Round(spd, numsLeft);
    }
    public double getValCrc()
    {
        double crc = 0.02 * technique;
        if (hasWeapon)
        {
            crc = crc + weapon.modCrc;
        }
        return Math.Round(crc, numsLeft);
    }
    public double getValCrd()
    {
        double crd = 1.6 + 0.03 * technique;
        if (hasWeapon)
        {
            crd = crd + weapon.modCrd;
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
        double num = 0.02 * agile;
        return Math.Round(num, numsLeft);
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
    public double getValHit()
    {
        double num = 1;
        if (hasWeapon)
        {
            num = weapon.modHit;
        }
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
