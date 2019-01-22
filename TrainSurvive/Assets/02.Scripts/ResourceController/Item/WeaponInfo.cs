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
    public class WeaponInfo : ItemInfo
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
        public float FacAttack { get; }
        /// <summary>
        /// 攻击速度系数
        /// </summary>
        public float FacAttackSpeed { get; }
        /// <summary>
        /// 移速系数
        /// </summary>
        public float FacMoveSpeed { get; }
        /// <summary>
        /// 受伤系数
        /// </summary>
        public float FacHit { get; }
        /// <summary>
        /// AP恢复系数
        /// </summary>
        public float FacAPRecover { get; }
        /// <summary>
        /// 技能伤害系数
        /// </summary>
        public float FacSkillDamage { get; }
        /// <summary>
        /// 暴击修正
        /// </summary>
        public float ModCritCarray { get; }
        /// <summary>
        /// 暴击伤害修正
        /// </summary>
        public float ModCritDamage { get; }
        /// <summary>
        /// 击退距离
        /// </summary>
        public float BeatBackDistance { get; }
        public WeaponInfo(XmlNode node)
            : base(node)
        {
            Type = PublicData.ItemType.Weapon;
            Price = int.Parse(node.Attributes["price"].Value);
            WeaponType = (PublicData.WeaponType)int.Parse(node.Attributes["type"].Value);
            Range       = float.Parse(node.Attributes["range"].Value);
            FacAttack  = float.Parse(node.Attributes["atk"].Value);
            FacAttackSpeed = float.Parse(node.Attributes["ats"].Value);
            FacMoveSpeed = float.Parse(node.Attributes["spd"].Value);
            FacHit            = float.Parse(node.Attributes["hit"].Value);
            FacAPRecover = float.Parse(node.Attributes["arec"].Value);
            FacSkillDamage  = float.Parse(node.Attributes["sdmg"].Value);
            ModCritCarray   = float.Parse(node.Attributes["crc"].Value);
            ModCritDamage   = float.Parse(node.Attributes["crd"].Value);
            BeatBackDistance = float.Parse(node.Attributes["bbdist"].Value);
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