/*
 * 描述：数值计算公式
 * 作者：项叶盛
 * 创建时间：2019/1/26 18:07:03
 * 版本：v0.7
 */
using UnityEngine;

namespace TTT.Utility
{
    public static class NumberFormula
    {
        //组件部分

        /// <summary>
        /// 射程
        /// </summary>
        public static float Range(float baseNumber, float fRarity) { return baseNumber * (fRarity + 0.5F); }
        /// <summary>
        /// 击退距离
        /// </summary>
        public static float BBDist(float baseNumber, float fRarity) { return Mathf.Pow(baseNumber, fRarity); }
        /// <summary>
        /// 攻击力
        /// </summary>
        public static float Atk(float baseNumber, float fRarity) { return Mathf.Pow(baseNumber, fRarity); } 
        /// <summary>
        /// 攻速
        /// </summary>
        public static float Ats(float baseNumber, float fRarity) { return Mathf.Pow(baseNumber, fRarity); } 
        /// <summary>
        /// 移速
        /// </summary>
        public static float Spd(float baseNumber, float fRarity) { return Mathf.Pow(baseNumber, fRarity); } 
        /// <summary>
        /// 暴击
        /// </summary>
        public static float Crc(float baseNumber, float fRarity) { return Mathf.Pow((baseNumber + 1), fRarity) - 1F; } 
        /// <summary>
        /// 受伤系数
        /// </summary>
        public static float Hit(float baseNumber, float fRarity) { return Mathf.Pow(baseNumber, fRarity); } 
        /// <summary>
        /// 技能受伤系数
        /// </summary>
        public static float SDmg(float baseNumber, float fRarity) { return Mathf.Pow(baseNumber, fRarity); } 
    }
}
