/*
 * 描述：人物属性有关的工具
 * 作者：项叶盛
 * 创建时间：2019/2/26 16:01:55
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
namespace TTT.Common
{
    /// <summary>
    /// 人物基础属性
    /// </summary>
    public enum EAttribute
    {
        NONE = -1,
        VITALITY,//体力
        STRENGTH,//力量
        AGILE,//敏捷
        TECHNIQUE,//技巧
        INTELLIGENCE,//智力
        NUM
    }
    public class AttriTool
    {
        /// <summary>
        /// 属性中文名字
        /// </summary>
        public static string[] NameC { get; } = { "体力", "力量", "敏捷", "技巧", "智力" };
        /// <summary>
        /// 属性英文名
        /// </summary>
        public static string[] NameE { get; } = { "vitality", "strength", "agile", "technique", "intelligence" };
        public static string Chinese(EAttribute attribute)
        {
#if DEBUG
            if (attribute < EAttribute.NONE || attribute >= EAttribute.NUM)
                throw new System.IndexOutOfRangeException("不合法的属性枚举 " + attribute.ToString());
#endif
            if (attribute == EAttribute.NONE) return "无";
            return NameC[(int)attribute];
        }
        public static string English(EAttribute attribute)
        {
#if DEBUG
            if (attribute <= EAttribute.NONE || attribute >= EAttribute.NUM)
                throw new System.IndexOutOfRangeException("不合法的属性枚举");
#endif
            return NameE[(int)attribute];
        }
        /// <summary>
        /// 将属性的英文名转化为索引
        /// </summary>
        /// <example>
        /// 0："vitality"
        /// 1："strength"
        /// 2："agile"
        /// 3："technique"
        /// 4："intelligence"
        /// </example>
        /// <param name="englishName">英文名</param>
        /// <returns></returns>
        public static int Compile(string englishName)
        {
            for(int i = 0; i < NameE.Length;i++)
            {
                if (NameE[i] == englishName)
                    return i;
            }
            return -1;
        }
    }
}