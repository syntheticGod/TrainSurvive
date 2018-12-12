/*
 * 描述：职业
 * 作者：项叶盛
 * 创建时间：2018/12/12 17:33:38
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;

public enum EPersonAttribute
{
    NONE = -1,
    VITALITY,//体力
    STRENGTH,//力量
    AGILE,//敏捷
    TECHNIQUE,//技巧
    INTELLIGENCE,//智力
    NUM
}
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
public class Profession
{
    public class AbiReq
    {
        public EPersonAttribute Abi;//属性
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
    public Profession(AbiReq[] abiReqs, string name, EProfession eProfession, string iconFile)
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
    }
}
