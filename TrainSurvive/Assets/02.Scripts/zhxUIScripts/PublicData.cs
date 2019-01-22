/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:40
 * 版本：v0.1
 */
/*
 * 描述：公共数据类
 * 作者：张皓翔
 * 创建时间：2018/10/31 18:56:03
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._02.Scripts.zhxUIScripts
{
    public static class PublicData
    {
        public enum Rarity { NONE = -1, Poor, Common, Rare, Epic, Legendary, NUM };         //劣质、普通、优良、精巧、传奇
        public enum ItemType { NONE = -1, Weapon, Material, SpecialItem, NUM };               //武器、材料、特殊物品
        public enum WeaponType
        {
            /// <summary>
            /// 盾
            /// </summary>
            Shield = 0,
            /// <summary>
            /// 剑
            /// </summary>
            Sword,
            /// <summary>
            /// 长枪
            /// </summary>
            Spear,
            /// <summary>
            /// 匕首
            /// </summary>
            Dagger,
            /// <summary>
            /// 机枪
            /// </summary>
            Machinegun,
            /// <summary>
            /// 狙击步枪
            /// </summary>
            SniperRifle,
            /// <summary>
            /// 魔杖
            /// </summary>
            Twig,
            /// <summary>
            /// 魔法书
            /// </summary>
            MagicBook,
            /// <summary>
            /// 饰品
            /// </summary>
            Decoration,
            NUM
        };



        public delegate void VoidCallback();
        public delegate void ItemDiscard();
        public delegate void ItemGain();
        /// <summary>
        /// 物品单元格拖拽功能 中 判断是否接受该物品的代理函数
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <param name="number">数量</param>
        /// <returns>
        /// TRUE：不允许
        /// FALSE：允许
        /// </returns>
        public delegate bool Charge(int itemID, int number);

        public static class Copy<TIn, TOut>
        {

            private static readonly Func<TIn, TOut> cache = GetFunc();
            private static Func<TIn, TOut> GetFunc()
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();

                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite)
                        continue;

                    MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }

                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

                return lambda.Compile();
            }

            public static TOut Trans(TIn tIn)
            {
                return cache(tIn);
            }

        }                                  //复制对象

    }



}
