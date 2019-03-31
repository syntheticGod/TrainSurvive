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
using WorldMap.Model;
using TTT.Common;

[System.Serializable]
public class Person
{
    #region 个人信息
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
    public Sprite IconSmall { get { return StaticResource.GetSprite("Commen/developing_icon_01_small"); } }
    /// <summary>
    /// 人物的大头像 120*120像素
    /// </summary>
    public Sprite IconBig { get { return StaticResource.GetSprite("Commen/developing_icon_01_big"); } }
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
            return string.Format("{0}/{1}/{2}", AttriTool.Chinese(profAttris[0]), AttriTool.Chinese(profAttris[1]), AttriTool.Chinese(profAttris[2]));
        }
    }
    public string BackgroundStoreInfo
    {
        get
        {
            return "背景故事正在路上。。。";
        }
    }
    #endregion 个人信息

    #region 人物状态
    /// <summary>
    /// 人物是否出战
    /// </summary>
    public bool ifReadyForFighting = false;
    #endregion 人物状态

    #region 属性
    /// <summary>
    /// 体力
    /// </summary>
    public int vitality { get { return AttriNumbers[(int)EAttribute.VITALITY]; } set { AttriNumbers[(int)EAttribute.VITALITY] = value; } }
    /// <summary>
    /// 力量
    /// </summary>
    public int strength { get { return AttriNumbers[(int)EAttribute.STRENGTH]; } set { AttriNumbers[(int)EAttribute.STRENGTH] = value; } }
    /// <summary>
    /// 敏捷
    /// </summary>
    public int agile { get { return AttriNumbers[(int)EAttribute.AGILE]; } set { AttriNumbers[(int)EAttribute.AGILE] = value; } }
    /// <summary>
    /// 技巧
    /// </summary>
    public int technique { get { return AttriNumbers[(int)EAttribute.TECHNIQUE]; } set { AttriNumbers[(int)EAttribute.TECHNIQUE] = value; } }
    /// <summary>
    /// 智力
    /// </summary>
    public int intelligence { get { return AttriNumbers[(int)EAttribute.INTELLIGENCE]; } set { AttriNumbers[(int)EAttribute.INTELLIGENCE] = value; } }

    public int[] AttriNumbers;
    private int[] AttriMaxNumbers;
    /// <summary>
    /// 已训练次数
    /// </summary>
    public int trainCnt = 0;
    /// <summary>
    ///  增加人物属性，同时获得相应技能
    /// </summary>
    /// <param name="eAttribute">属性</param>
    /// <param name="delta">增值</param>
    private void AddAttriNumber(EAttribute eAttribute, int delta)
    {
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
        //检查属性上限
        if (AttriNumbers[(int)attribute] + delta > AttriMaxNumbers[(int)attribute])
            return -2;
        //计算金币
        int cost;
        if (payByWhat == 0)
        {
            cost = CalMoneyByAttribute(attribute, delta);
            if (!World.getInstance().IfMoneyEnough(cost))
                return -1;
        }
        else
        {
            cost = CallStrategyByAttribute(attribute, delta);
            if (!World.getInstance().IfStrategyEnough(cost))
                return -1;
        }
        //扣款 加属性
        if (payByWhat == 0)
        {
            if (!World.getInstance().PayByMoney(cost))
                return -3;
        }
        else
        {
            if (!World.getInstance().PayByStrategy(cost))
                return -3;
        }
        //属性添加动作
        AttriNumbers[(int)attribute] += delta;
        trainCnt++;
        //获取技能
        List<SkillInfo> skills = StaticResource.GetAvailableSkills(AttriNumbers, ESkillComeFrom.SCHOOL);
        List<SkillInfo> newSkills = new List<SkillInfo>();
        for (int i = 0; i < skills.Count; i++)
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
        float money = deltaNumber * 1000F * (1 + AttriNumbers[(int)attribute] * 0.05F) * (1 + CallMoneyIncreaseByTrainCnt() / 100.0f);
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
    /// <summary>
    /// 小数属性保留的位数
    /// </summary>
    private const int numsLeft = 3;
    //以下获取的属性均保留numsLeft位小数
    public double getHpMax()
    {
        double hpMax = 500 * (1 + 0.05 * vitality);
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
        double atk = 50 * (1 + 0.05 * strength);
        if (hasWeapon)
        {
            atk = atk * weapon.facAtk;
        }
        return Math.Round(atk, numsLeft);
    }
    public double getValAts()
    {
        double ats = 2 * (1 + 0.03 * agile);
        if (hasWeapon)
        {
            ats = ats * weapon.facAts;
        }
        return Math.Round(ats, numsLeft);
    }
    public double getValSpd()
    {
        double spd = 2 * (1 + 0.02 * agile);
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
        double crd = 2 + 0.03 * technique;
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
    #endregion

    #region 武器
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
    #endregion 武器

    #region 技能
    private List<int> skillsCarryed = new List<int>();
    private int skill_carryed_maxNum = 2;
    /// <summary>
    /// 获得已携带的技能id，技能下标超过最大值（当前为2,即取值范围1~2）或者未携带技能则返回-1
    /// </summary>
    /// <param name="index">技能下标，1基</param>
    /// <returns></returns>
    public int getSkillCarryed(int index)
    {
        if (index > skill_carryed_maxNum || index > skillsCarryed.Count)
            return -1;
        return skillsCarryed[index - 1];
    }
    public void carry_skill(int skillId)
    {
        if (!skillsCarryed.Contains(skillId))
        {
            if (skillsCarryed.Count < skill_carryed_maxNum)
                skillsCarryed.Add(skillId);
        }
    }
    public void uncarry_skill(int skillId)
    {
        for (int i = 0; i < skill_carryed_maxNum && i < skillsCarryed.Count; i++)
        {
            if (skillsCarryed[i] == skillId)
            {
                skillsCarryed.RemoveAt(i);
                break;
            }
        }
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
    /// <summary>
    /// 根据初始属性获取技能
    /// </summary>
    private void InitSkill()
    {
        List<SkillInfo> avaliableSkills = StaticResource.GetAvailableSkills(AttriNumbers, ESkillComeFrom.SCHOOL);
        foreach (SkillInfo skill in avaliableSkills)
        {
            AddGotSkill(skill.ID);
        }
    }
    #endregion 技能

    #region 专精
    /// <summary>
    /// 三个专精槽位
    /// 存放的是专精的ID
    /// </summary>
    private int[] profIDs;
    /// <summary>
    /// 允许的槽位
    /// </summary>
    private int professionAvaliable;
    /// <summary>
    /// 玩家选择的专精属性次序
    /// </summary>
    private EAttribute[] profAttris;
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
        if (profIDs[index] == -1)
            return null;
        return StaticResource.GetProfessionByID(profIDs[index]);
    }
    /// <summary>
    /// 判断专精槽是否足够
    /// </summary>
    /// <returns></returns>
    public bool IfProfessionAvailable()
    {
        if (professionAvaliable == 0) return false;
        return profIDs[professionAvaliable - 1] == -1;
    }
    /// <summary>
    /// 判断指定属性是否已专精
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool IfProfession(EAttribute attribute)
    {
        for (int i = 0; i < profAttris.Length; i++)
        {
            if (profAttris[i] == attribute)
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
        for (int i = profIDs.Length - 1; i >= 0; i--)
        {
            if (profIDs[i] != -1)
                return StaticResource.GetProfessionByID(profIDs[i]);
        }
        return null;
    }
    /// <summary>
    /// 根据专精的Level绑定专精
    /// </summary>
    /// <param name="profession"></param>
    public void SetProfession(Profession profession, EAttribute attribute)
    {
        if (profession.Level == EProfessionLevel.NONE)
        {
            Debug.LogError("专精错误");
            return;
        }
        profIDs[(int)profession.Level] = profession.ID;
        profAttris[(int)profession.Level] = attribute;
        //解锁上限
        AttriMaxNumbers[(int)attribute] = 999;
    }
    #endregion 专精

    #region 构造函数
    private Person()
    {
        //保留以后用
        profIDs = new int[3] { -1, -1, -1 };
        profAttris = new EAttribute[3] { EAttribute.NONE, EAttribute.NONE, EAttribute.NONE };
        AttriNumbers = new int[(int)EAttribute.NUM] { 0, 0, 0, 0, 0 };
        //默认最大属性为10
        AttriMaxNumbers = new int[(int)EAttribute.NUM] { 10, 10, 10, 10, 10 };
        //初始专精槽数为1
        professionAvaliable = 1;
        ifReadyForFighting = false;
    }
    public Person(NpcInfo npc) : this()
    {
        ismale = npc.Gender;
        name = npc.Name;
        for (int i = 0; i < AttriNumbers.Length; i++)
        {
            AttriNumbers[i] = npc.AttriNumber[i];
        }
        professionAvaliable = 0;
        Profession lastProf = null;
        foreach (EAttribute currentAttri in npc.Professions)
        {
            if (currentAttri == EAttribute.NONE) break;
            Profession prof = StaticResource.GetNextProfessions(lastProf)[(int)currentAttri];
            SetProfession(prof, currentAttri);
            professionAvaliable++;
            lastProf = prof;
        }
        InitSkill();
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
            p.AttriNumbers[(int)itr] = MathTool.RandomRange(0, p.AttriMaxNumbers[(int)itr]);
        }
        p.InitSkill();
        return p;
    }
    #endregion
}
