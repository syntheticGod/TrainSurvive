/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/26 18:02:01
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace TTT.Item
{
    public class WeaponData : ItemData
    {
        /// <summary>
        /// 射程
        /// </summary>
        public float Range { get { return component1.Range + component2.Range; } }
        /// <summary>
        /// 击退距离
        /// </summary>
        public float BBDist { get { return component1.BBDist + component2.BBDist; } }
        /// <summary>
        /// 攻击力系数
        /// </summary>
        public float FacAtk { get { return component1.FacAtk * component2.FacAtk; } }
        /// <summary>
        /// 攻速系数
        /// </summary>
        public float FacAts { get { return component1.FacAts * component2.FacAts; } }
        /// <summary>
        /// 移速系数
        /// </summary>
        public float FacSpd { get { return component1.FacSpd * component2.FacSpd; } }
        /// <summary>
        /// 暴击修正
        /// </summary>
        public float ModCrC { get { return (1.0f + component1.ModCrC) * (1.0f + component2.ModCrC) - 1.0f; } }
        /// <summary>
        /// 受伤系数
        /// </summary>
        public float ModHit { get { return component1.ModHit * component2.ModHit; } }
        /// <summary>
        /// 技能受伤系数
        /// </summary>
        public float FacSDmg { get { return component1.FacAts * component2.FacAts; } }
        /// <summary>
        /// 组件1
        /// </summary>
        protected ComponentData component1;
        /// <summary>
        /// 组件2
        /// </summary>
        protected ComponentData component2;
        /// <summary>
        /// 由不同的组件合成
        /// </summary>
        /// <param name="id">武器ID</param>
        /// <param name="component1">组件1</param>
        /// <param name="component2">组件2</param>
        public WeaponData(int id, ComponentData component1, ComponentData component2, bool randomRarity = false) : base(id, 1, randomRarity)
        {
            this.component1 = component1;
            this.component2 = component2;
        }
        private WeaponData(int id, int number, bool randomRarity = false) : base(id, number, randomRarity)
        {
            component1 = null;
            component2 = null;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Component1ID", component1.ID, typeof(int));
            info.AddValue("Component1Rarity", component1.FRarity, typeof(float));
            info.AddValue("Component2ID", component2.ID, typeof(int));
            info.AddValue("Component2Rarity", component2.FRarity, typeof(float));
        }
        public WeaponData(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            int id1 = info.GetInt32("Component1ID");
            float rarity1 = (float)info.GetValue("Component1Rarity", typeof(float));
            int id2 = info.GetInt32("Component2ID");
            float rarity2 = (float)info.GetValue("Component2Rarity", typeof(float));
        }
        public override ItemData Clone()
        {
            WeaponData clone = new WeaponData(ID, Number, false);
            clone.component1 = component1.Clone() as ComponentData;
            clone.component2 = component2.Clone() as ComponentData;
            return clone;
        }
        public override bool Equals(object obj)
        {
            WeaponData other = obj as WeaponData;
            if (other == null) return false;
            return base.Equals(obj) &&
                component1.Equals(other.component1) &&
                component2.Equals(other.component2);
        }
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + component1.GetHashCode();
            hashCode = hashCode * -1521134295 + component2.GetHashCode();
            return hashCode;
        }
    }
}