/*
 * 描述：职业
 * 作者：项叶盛
 * 创建时间：2018/12/12 17:33:38
 * 版本：v0.1
 */
using UnityEngine;

using TTT.Utility;
using TTT.Resource;

public enum EProfession
{
    NONE = -1,
    KNIGHT,//骑士
    FIGHTER,//战士
    ASSASSIN,//刺客
    SHOOTER,//枪手
    ENCHANTER,//法师
    RAMPART,//壁垒
    BARBARIAN,//野蛮人
    PORTER,//后勤，搬运工
    BLACKSMITH,//铁匠
    CLERIC,//医师（牧师）
    WARRIOR,//勇士
    DUAL_SWORDSMAN,//双剑士Dual Swordsman
    ACROBAT,//游侠
    MAGIC_SWORDSMAN,//魔剑士Magic Swordsman
    FASTER_SHOOTER,//快攻手（快枪手）Faster Shooter
    SHADOW,//潜行者（暗影？）
    WINDRUNNER,//风行者
    SNIPER,//鹰隼（狙击手）
    ENGINEER,//工程师
    FORCE_USER,//魔导师
    NUM
}
public enum EProfessionLevel
{
    NONE = -1,
    LEVEL1,
    LEVEL2,
    LEVEL3,
    NUM
}
public class Profession
{
    public class AbiReq
    {
        public EAttribute Abi;//属性
        public int Number;//数值
        public float costFix;//折扣
    }
    public AbiReq[] AbiReqs { get; }
    public string Name { get; }
    public EProfession Type { get; }
    /// <summary>
    /// 60*60 像素
    /// </summary>
    public Sprite IconSmall { get; }
    /// <summary>
    /// 120*120像素
    /// </summary>
    public Sprite IconBig { get; }
    /// <summary>
    /// -1：无专精
    /// 0：一级专精
    /// 1：二级专精
    /// 2：三级专精
    /// </summary>
    public EProfessionLevel Level { get; }
    public string Info { get; }
    public Profession(AbiReq[] abiReqs, string name, EProfession eProfession, string iconFile, string info)
    {
        AbiReqs = abiReqs;
        Name = name;
        Type = eProfession;
        IconSmall = Resources.Load<Sprite>(iconFile + "_small");
        IconBig = Resources.Load<Sprite>(iconFile + "_big");
        if (IconSmall == null || IconBig == null)
        {
            throw new System.Exception("文件" + iconFile + "未找到");
        }
        Info = info;
        if (Type == EProfession.NONE)
            Level = EProfessionLevel.NONE;
        if (MathTool.IfBetweenBoth((int)EProfession.KNIGHT, (int)EProfession.ENCHANTER, (int)Type))
            Level = EProfessionLevel.LEVEL1;
        else
            Level = EProfessionLevel.LEVEL2;
    }
    /// <summary>
    /// 判断一个属性是否满足专精要求
    /// 注意：如果传入专精不要求的属性，返回TRUE。也被认为满足。
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool CheckRequires(EAttribute attribute, int number, ref int requireNumber)
    {
        requireNumber = 0;
        foreach (AbiReq abiReq in AbiReqs)
        {
            if (abiReq.Abi == attribute && abiReq.Number > number)
            {
                requireNumber = abiReq.Number;
                return false;
            }
        }
        return true;
    }
    public float GetCostFixByAttribute(EAttribute attribute)
    {
        foreach (AbiReq abiReq in AbiReqs)
        {
            if (abiReq.Abi == attribute)
                return abiReq.costFix;
        }
        return 1.0F;
    }
}
