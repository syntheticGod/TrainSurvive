/*
 * 描述：武器信息
 * 作者：项叶盛
 * 创建时间：2019/1/3 2:28:18
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Xml;
using TTT.Resource;

namespace TTT.Item
{
    public class DecorationsInfo : ItemInfo
    {

        /// <summary>
        /// 初始价格
        /// </summary>
        public int Price { get; }
        /// <summary>
        /// 武器类型
        /// </summary>
        public PublicData.WeaponType WeaponType { get; }
        /// <summary>
        /// 攻击距离
        /// </summary>
        public float Range { get; }
        /// <summary>
        /// 攻击系数
        /// </summary>
        public float FacAtk { get; }
        /// <summary>
        /// 攻击速度系数
        /// </summary>
        public float FacAts { get; }
        /// <summary>
        /// 移速系数
        /// </summary>
        public float FacSpd { get; }
        /// <summary>
        /// 受伤系数
        /// </summary>
        public float FacHit { get; }
        /// <summary>
        /// AP恢复系数
        /// </summary>
        public float FacAPR { get; }
        /// <summary>
        /// 技能伤害系数
        /// </summary>
        public float FacSDmg { get; }
        /// <summary>
        /// 暴击修正
        /// </summary>
        public float ModCrC { get; }
        /// <summary>
        /// 暴击伤害修正
        /// </summary>
        public float ModCrD { get; }
        /// <summary>
        /// 击退距离
        /// </summary>
        public float BBDist { get; }
        /// <summary>
        /// HP上限系数
        /// </summary>
        public float HpMax { get; }
        /// <summary>
        /// AP上限系数
        /// </summary>
        public float ApMax { get; }
        /// <summary>
        /// 命中修正
        /// </summary>
        public float HRate { get; }
        /// <summary>
        /// 闪避修正
        /// </summary>
        public float ERate { get; }
        public DecorationsInfo(XmlNode node)
            : base(node)
        {
            Type = PublicData.ItemType.Decorations;
            Price = int.Parse(node.Attributes["price"].Value);
            Range   = float.Parse(node.Attributes["range"].Value);
            FacAtk   = float.Parse(node.Attributes["atk"].Value);
            FacAts    = float.Parse(node.Attributes["ats"].Value);
            FacSpd   = float.Parse(node.Attributes["spd"].Value);
            FacHit     = float.Parse(node.Attributes["hit"].Value);
            FacAPR   = float.Parse(node.Attributes["arec"].Value);
            FacSDmg  = float.Parse(node.Attributes["sdmg"].Value);
            ModCrC   = float.Parse(node.Attributes["crc"].Value);
            ModCrD   = float.Parse(node.Attributes["crd"].Value);
            BBDist = float.Parse(node.Attributes["bbdist"].Value);
            HpMax = float.Parse(node.Attributes["hpMax"].Value);
            ApMax = float.Parse(node.Attributes["apMax"].Value);
            HRate = float.Parse(node.Attributes["hRate"].Value);
            ERate = float.Parse(node.Attributes["eRate"].Value);
        }

        public override int GetOriginPrice()
        {
            return Price;
        }

        public override int GetSellPrice()
        {
            return UnityEngine.Mathf.RoundToInt(Price * SellRatio);
        }
    }
}