/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/22 14:35:42
 * 版本：v0.1
 */

using TTT.Resource;
using UnityEngine;

public class SkillInfo
{
    public struct AbiReq
    {
        public EAttribute Abi;
        public int Number;
    }
    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    /// <summary>
    /// 技能所需要的AP描述
    /// 示例：
    /// 1、10
    /// 2、20/秒
    /// </summary>
    public string AP { get; private set; }
    public string Description { get; private set; }
    /// <summary>
    /// 获得该技能所需的前置属性
    /// </summary>
    public AbiReq[] AbiReqs { get; private set; }
    /// <summary>
    /// 120 x 120 像素
    /// </summary>
    public Sprite BigSprite { get; private set; }
    /// <summary>
    /// 60 x 60 像素
    /// </summary>
    public Sprite SmallSprite { get; private set; }
    public SkillInfo(int id, string name, string type, string ap, string description, AbiReq[] abiReqs)
    {
        ID = id;
        Name = name;
        Type = type;
        AP = ap;
        Description = description;
        AbiReqs = abiReqs;
        BigSprite = StaticResource.GetSprite(ESprite.DEVELOPING_BIG);
        SmallSprite = StaticResource.GetSprite(ESprite.DEVELOPING_SMALL);
    }
    public bool IfAvailable(int[] attribute)
    {
        //无条件获得的技能AbiReqs为NULL
        if (AbiReqs == null)
            return true;
        foreach(AbiReq req in AbiReqs)
        {
            if (req.Number > attribute[(int)req.Abi])
                return false;
        }
        return true;
    }
}
