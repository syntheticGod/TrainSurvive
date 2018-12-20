/*
 * 描述：职业
 * 作者：项叶盛
 * 创建时间：2018/12/12 17:33:38
 * 版本：v0.1
 */
using UnityEngine;

using TTT.Utility;
using TTT.Resource;

public enum EProfessionLevel
{
    NONE = -1,
    LEVEL1,
    LEVEL2,
    LEVEL3,
    NUM
}
public enum EProfessionState
{
    NONE = -1,
    DEVELOPING,//待开放
    OPENING,//正常
    EMPTY,//空
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
    /// <summary>
    /// 专精名字
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 专精ID
    /// </summary>
    public int ID { get; }
    public EProfessionState State { get; }
    /// <summary>
    /// 60*60 像素
    /// </summary>
    public Sprite IconSmall { get; }
    /// <summary>
    /// 120*120像素
    /// </summary>
    public Sprite IconBig { get; }
    /// <summary>
    /// 专精级别
    /// </summary>
    public EProfessionLevel Level { get; }
    /// <summary>
    /// 简介
    /// </summary>
    public string Info { get; }
    public Profession(int id, AbiReq[] abiReqs, EProfessionState state, string name, string info)
    {
        AbiReqs = abiReqs;
        State = state;
        Name = name;
        ID = id;
        IconSmall = StaticResource.GetSprite(ESprite.PROFESSION0_BIG + id);
        IconBig = StaticResource.GetSprite(ESprite.PROFESSION0_SMALL + id);
        Info = info;
        if (MathTool.IfBetweenBoth(0, 4, id))
            Level = EProfessionLevel.LEVEL1;
        else if (MathTool.IfBetweenBoth(5, 19, id))
            Level = EProfessionLevel.LEVEL2;
        else
            Level = EProfessionLevel.LEVEL3;
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
    public float GetDiscountByAttri(EAttribute attribute)
    {
        foreach (AbiReq abiReq in AbiReqs)
        {
            if (abiReq.Abi == attribute)
                return abiReq.costFix;
        }
        return 1.0F;
    }
}
