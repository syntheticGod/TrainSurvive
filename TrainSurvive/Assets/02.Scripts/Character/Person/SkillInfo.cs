/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/22 14:35:42
 * 版本：v0.1
 */

using System.Xml;
using TTT.Common;
using TTT.Resource;
using UnityEngine;
public enum ESkillComeFrom
{
    NONE = -1,
    NULL,//未携带技能的模板
    SCHOOL,//学校习得
    MONSTER//怪物的技能
}
public class SkillInfo
{
    public struct AbiReq
    {
        public EAttribute Abi;
        public int Number;
    }
    /// <summary>
    /// 技能ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 技能名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 技能详细类型
    /// </summary>
    public string TypeInfo { get; private set; }
    /// <summary>
    /// 技能所需要的AP描述
    /// 示例：
    /// 1、10
    /// 2、20/秒
    /// </summary>
    public string AP { get; private set; }
    /// <summary>
    /// 技能的来源
    /// </summary>
    public ESkillComeFrom ComeFrom { get; private set; }
    /// <summary>
    /// 技能描述
    /// </summary>
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
    public SkillInfo(XmlNode skillsNode)
    {
        ID = int.Parse(skillsNode.Attributes["id"].Value);
        Name = skillsNode.Attributes["name"].Value;
        TypeInfo = skillsNode.Attributes["type"].Value;
        AP = skillsNode.Attributes["AP"].Value;
        Description = skillsNode.Attributes["description"].Value;
        string comeFrom = skillsNode.Attributes["ComeFrom"].Value.ToLower();
        switch (comeFrom)
        {
            case "null":
                ComeFrom = ESkillComeFrom.NULL;
                break;
            case "school":
                ComeFrom = ESkillComeFrom.SCHOOL;
                break;
            case "monster":
                ComeFrom = ESkillComeFrom.MONSTER;
                break;
            default:
                ComeFrom = ESkillComeFrom.NONE;
                break;
        }
        XmlNodeList attributeRequires = skillsNode.SelectNodes("Precondition/Attributes/Attribute");
        AbiReqs = null;
        //无条件获得
        if (attributeRequires.Count != 0)
        {
            AbiReqs = new AbiReq[attributeRequires.Count];
            for (int y = 0; y < attributeRequires.Count; y++)
            {
                AbiReqs[y] = new SkillInfo.AbiReq();
                AbiReqs[y].Abi = EAttribute.NONE + 1 + int.Parse(attributeRequires[y].Attributes["Abi"].Value);
                AbiReqs[y].Number = int.Parse(attributeRequires[y].Attributes["Number"].Value);
            }
        }
        BigSprite = StaticResource.GetSprite("Commen/developing_icon_01_big");
        SmallSprite = StaticResource.GetSprite("Commen/developing_icon_01_small");
    }
    /// <summary>
    /// 判断该技能是否达到传入的属性的要求
    /// </summary>
    /// <param name="attribute">传入属性要求</param>
    /// <returns></returns>
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
