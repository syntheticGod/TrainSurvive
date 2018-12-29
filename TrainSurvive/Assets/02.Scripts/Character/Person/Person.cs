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
using WorldMap.UI;
using System.Text;

[System.Serializable]
public class Person
{
    //----------个人信息----------↓
    /// <summary>
    /// 人物姓名
    /// </summary>
    public string name = "";
    /// <summary>
    /// 性别用ismale代替
    /// </summary>
    public bool ismale = true;
    /// <summary>
    /// 人物的小头像 60*60像素
    /// </summary>
    public Sprite IconSmall { get { return StaticResource.GetSprite(ESprite.DEVELOPING_SMALL); } }
    /// <summary>
    /// 人物的大头像 120*120像素
    /// </summary>
    public Sprite IconBig { get { return StaticResource.GetSprite(ESprite.DEVELOPING_BIG); } }
    public string SimpleInfo
    {
        get
        {
            Profession topProfession = getTopProfession();
            if (topProfession == null)
                return string.Format("{0}", name);
            else
                return string.Format("{0},{1}", name, topProfession.Name);
        }
    }
    public string ProfessionInfo
    {
        get
        {
            return string.Format("{0}/{1}/{2}", StaticResource.GetAttributeName(proAttributes[0]), StaticResource.GetAttributeName(proAttributes[1]), StaticResource.GetAttributeName(proAttributes[2]));
        }
    }
    public string BackgroundStoreInfo
    {
        get
        {
            return "背景故事正在路上。。。";
        }
    }
    //----------个人信息----------↑----------人物状态----------↓
    /// <summary>
    /// 人物是否出战
    /// </summary>
    public bool ifReadyForFighting = false;
    //----------人物状态----------↑----------属性----------↓
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
    /// <summary>
    /// 小数属性保留的位数
    /// </summary>
    private const int numsLeft = 3;
    private int[] attriNumber;
    private int[] attriMaxNumber;
    public int[] GetAttriNumbers()
    {
        return attriNumber;
    }
    /// <summary>
    /// 获得相应属性值
    /// </summary>
    /// <param name="eAttribute">属性</param>
    /// <returns>属性值</returns>
    public int GetAttriNumber(EAttribute eAttribute)
    {
        return attriNumber[(int)eAttribute];
    }
    /// <summary>
    /// 获得相应最大属性值
    /// </summary>
    /// <param name="eAttribute">属性</param>
    /// <returns>最大属性值</returns>
    public int GetAttriMaxNumber(EAttribute eAttribute)
    {
        return attriMaxNumber[(int)eAttribute];
    }
    /// <summary>
    /// 已训练次数
    /// </summary>
    public int trainCnt = 0;
    /// 增加人物属性，同时获得相应技能
    /// </summary>
    /// <param name="eAttribute">属性</param>
    /// <param name="delta">增值</param>
    public void AddAttriNumber(EAttribute eAttribute, int delta)
    {
        attriNumber[(int)eAttribute] += delta;
        trainCnt++;
        SkillInfo[] skills = StaticResource.GetAvailableSkills(attriNumber);
        List<SkillInfo> newSkills = new List<SkillInfo>();
        for (int i = 0; i < skills.Length; i++)
        {
            if (IfHaveGotTheSkill(skills[i]) == false)
            {
                newSkills.Add(skills[i]);
                AddGotSkill(skills[i].ID);
            }
        }
        if (newSkills.Count != 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("学习到新技能：");
            foreach (SkillInfo skill in newSkills)
            {
                sb.AppendFormat("{0},", skill.Name);
            }
            sb.Remove(sb.Length - 1, 1);
            InfoDialog.Show(sb.ToString());
        }
    }
    /// <summary>
    /// 先支付金额或策略点后加属性
    /// </summary>
    /// <param name="attribute">属性</param>
    /// <param name="delta">增加差值</param>
    /// <param name="payByWhat">0：金钱 1：策略点</param>
    /// <returns>
    /// 1：添加属性成功
    /// -1：失败，金钱/策略点 不足
    /// -2：失败，属性达到上限
    /// -3：失败，扣款/扣策略点 失败（检测时金钱足够，扣款时金钱不足）
    /// </returns>
    public int AddAttributeWithPay(EAttribute attribute, int delta, int payByWhat)
    {
        World world = World.getInstance();
        //计算金币
        int cost;
        if (payByWhat == 0)
        {
            cost = CalMoneyByAttribute(attribute, delta);
            if (!world.IfMoneyEnough(cost))
                return -1;
        }
        else
        {
            cost = CallStrategyByAttribute(attribute, delta);
            if (!world.IfStrategyEnough(cost))
                return -1;
        }
        //检查属性上限
        int maxAttributeNumber;
        //专精可以解锁属性上限
        if (IfProfession(attribute))
            maxAttributeNumber = 999;
        else
            maxAttributeNumber = GetAttriMaxNumber(attribute);
        if (GetAttriNumber(attribute) + delta > maxAttributeNumber)
            return -2;
        //扣款 加属性
        if (payByWhat == 0)
        {
            if (!world.PayByMoney(cost))
                return -3;
        }
        else
        {
            if (!world.PayByStrategy(cost))
                return -3;
        }
        AddAttriNumber(attribute, delta);
        return 1;
    }
    /// <summary>
    /// 根据训练次数计算金钱花费增值百分比
    /// </summary>
    /// <returns>
    /// 单位为 %
    /// </returns>
    public int CallMoneyIncreaseByTrainCnt()
    {
        return trainCnt * 5;
    }
    /// <summary>
    /// 计算增加delta点属性时所需要的金钱
    /// </summary>
    /// <param name="attribute">属性</param>
    /// <param name="deltaNumber">属性差值</param>
    /// <returns>
    /// 所需的金钱数额
    /// </returns>
    public int CalMoneyByAttribute(EAttribute attribute, int deltaNumber)
    {
        float money = deltaNumber * 1000F * (1 + GetAttriNumber(attribute) * 0.05F) * (1 + CallMoneyIncreaseByTrainCnt() / 100.0f);
        if (IfProfession(attribute))
            money *= getTopProfession().GetDiscountByAttri(attribute);
        return Mathf.RoundToInt(money);
    }
    /// <summary>
    /// 计算增加delta点属性时所需要的策略点
    /// </summary>
    /// <param name="attribute">属性</param>
    /// <param name="deltaNumber">策略点</param>
    /// <returns>
    /// 所需的策略点数
    /// </returns>
    public int CallStrategyByAttribute(EAttribute attribute, int deltaNumber)
    {
        float money = CalMoneyByAttribute(attribute, deltaNumber);
        return Mathf.RoundToInt(money * 0.1f);
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
    //----------属性----------↑----------武器----------↓
    public bool hasWeapon = false;
    public int weaponId = 0;
    /// <summary>
    /// 人物所持有的武器对象
    /// </summary>
    public Weapon weapon = null;
    [NonSerialized]
    private int lastWeaponId = -1;
    public void equipWeapon(Weapon weapon)
    {
        this.weapon = (Weapon)weapon.Clone();
        weaponId = weapon.id;
        hasWeapon = true;
    }
    public void unequipWeapon()
    {
        weapon = null;
        lastWeaponId = weaponId;
        weaponId = -1;
        hasWeapon = false;
    }
    //----------武器----------↑----------技能----------↓
    private List<int> skillsCarryed = new List<int>();
    private int skill_carryed_maxNum = 2;
    /// <summary>
    /// 获得已携带的技能id，技能下标超过最大值（当前为2,即取值范围1~2）或者未携带技能则返回-1
    /// </summary>
    /// <param name="index">技能下标，1基</param>
    /// <returns></returns>
    public int getSkillCarryed(int index)
    {
        if (index > skill_carryed_maxNum)
            return -1;
        return skillsCarryed[index - 1];
    }
    /// <summary>
    /// 已经获得的技能
    /// </summary>
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
    //----------技能----------↑----------专精----------↓
    /// <summary>
    /// 三个专精槽位
    /// 存放的是专精的ID
    /// </summary>
    private int[] professions;
    /// <summary>
    /// 允许的槽位
    /// </summary>
    private int professionAvaliable;
    /// <summary>
    /// 玩家选择的专精属性次序
    /// </summary>
    private EAttribute[] proAttributes;
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
        for (int i = 0; i < proAttributes.Length; i++)
        {
            if (proAttributes[i] == attribute)
                return true;
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
    public void setProfession(Profession profession, EAttribute attribute)
    {
        if (profession.Level == EProfessionLevel.NONE)
        {
            Debug.LogError("专精错误");
            return;
        }
        professions[(int)profession.Level] = profession.ID;
        proAttributes[(int)profession.Level] = attribute;
    }
    //----------专精----------↑
    private Person()
    {
        //保留以后用
        professions = new int[3] { -1, -1, -1 };
        proAttributes = new EAttribute[3] { EAttribute.NONE, EAttribute.NONE, EAttribute.NONE };
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
}
