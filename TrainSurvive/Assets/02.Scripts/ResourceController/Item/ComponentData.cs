/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/26 17:24:51
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System;
using Assets._02.Scripts.zhxUIScripts;
using System.Runtime.Serialization;
using TTT.Utility;

namespace TTT.Item
{
    [Serializable]
    public class ComponentData : ItemData
    {
        /// <summary>
        /// 稀有度系数
        /// </summary>
        public float FRarity { get; private set; }
        public void SetRarity(float rarity)
        {
#if DEBUG
            if (Item.Type != PublicData.ItemType.Component)
                Debug.LogError("目前只有组件需要设置稀有度，是否出错了呢？");
#endif
            FRarity = rarity;
        }
        /// <summary>
        /// 物品稀有度
        /// </summary>
        public new PublicData.Rarity Rarity { get { return (PublicData.Rarity)Math.Round(FRarity); } }
        /// <summary>
        /// 射程
        /// </summary>
        public float Range { get { return NumberFormula.Range((Item as ComponentInfo).Range, FRarity); } }
        /// <summary>
        /// 击退距离
        /// </summary>
        public float BBDist { get { return NumberFormula.BBDist((Item as ComponentInfo).BBDist, FRarity); } }
        /// <summary>
        /// 攻击力系数
        /// </summary>
        public float FacAtk { get { return NumberFormula.Atk((Item as ComponentInfo).FacAtk, FRarity); } }
        /// <summary>
        /// 攻速系数
        /// </summary>
        public float FacAts { get { return NumberFormula.Ats((Item as ComponentInfo).FacAts, FRarity); } }
        /// <summary>
        /// 移速系数
        /// </summary>
        public float FacSpd { get { return NumberFormula.Spd((Item as ComponentInfo).FacSpd, FRarity); } }
        /// <summary>
        /// 暴击修正
        /// </summary>
        public float ModCrC { get { return NumberFormula.Crc(((Item as ComponentInfo).ModCrc + 1), FRarity) - 1F; } }
        /// <summary>
        /// 受伤系数
        /// </summary>
        public float ModHit { get { return NumberFormula.Hit((Item as ComponentInfo).FacHit, FRarity); } }
        /// <summary>
        /// 技能受伤系数
        /// </summary>
        public float FacSDmg { get { return NumberFormula.SDmg((Item as ComponentInfo).FacSDmg, FRarity); } }
        /// <summary>
        /// 需要保存稀有度的物品
        /// 例如组件
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <param name="number">数量</param>
        /// <param name="rarity">稀有度</param>
        public ComponentData(int itemID, int number, float rarity) : base(itemID, number)
        {
            SetRarity(rarity);
        }
        public override bool Equals(object obj)
        {
            ComponentData other = obj as ComponentData;
            if (other == null) return false;
            if (!base.Equals(other)) return false;
            return  Rarity == other.Rarity;
        }
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + Rarity.GetHashCode();
            return hashCode;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FRarity", FRarity, typeof(float));
        }
        public ComponentData(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            FRarity = (float)info.GetValue("FRarity", typeof(float));
        }
        public override ItemData Clone()
        {
            return new ComponentData(ID, Number, FRarity);
        }
    }
}
