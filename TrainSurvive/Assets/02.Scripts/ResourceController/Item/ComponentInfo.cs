/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/26 15:41:28
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Xml;
using Assets._02.Scripts.zhxUIScripts;

namespace TTT.Item
{
    public class ComponentInfo : ItemInfo
    {
        /// <summary>
        /// 初始价格
        /// </summary>
        public int Price { get; }
        /// <summary>
        /// 部位
        /// </summary>
        public int Position { get; }
        /// <summary>
        /// 射程
        /// </summary>
        public float Range { get; }
        /// <summary>
        /// 击退距离
        /// </summary>
        public float BBDist { get; }
        /// <summary>
        /// 攻击系数
        /// </summary>
        public float FacAtk { get; }
        /// <summary>
        /// 攻速系数
        /// </summary>
        public float FacAts { get; }
        /// <summary>
        /// 移速系数
        /// </summary>
        public float FacSpd { get; }
        /// <summary>
        /// 暴击修正
        /// </summary>
        public float ModCrc { get; }
        /// <summary>
        /// 受伤系数
        /// </summary>
        public float FacHit { get; }
        /// <summary>
        /// 技能伤害系数
        /// </summary>
        public float FacSDmg { get; }

        public ComponentInfo(XmlNode node) : base(node)
        {
            Type = PublicData.ItemType.Component;
            Position = int.Parse(node.Attributes["position"].Value);
            Range = float.Parse(node.Attributes["range"].Value);
            BBDist = float.Parse(node.Attributes["bbdist"].Value);
            FacAtk = float.Parse(node.Attributes["facAtk"].Value);
            FacSpd = float.Parse(node.Attributes["facSpd"].Value);
            ModCrc = float.Parse(node.Attributes["modCrc"].Value);
            FacHit = float.Parse(node.Attributes["facHit"].Value);
            FacSDmg = float.Parse(node.Attributes["facSDmg"].Value);
        }
        public void SetRarity(float rarity)
        {
            Rarity = (PublicData.Rarity)Mathf.RoundToInt(rarity);

        }
        public override int GetOriginPrice()
        {
            return Price;
        }

        public override int GetSellPrice()
        {
            return (int)(Price * SellRatio);
        }
    }
}
