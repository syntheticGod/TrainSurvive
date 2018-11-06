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
        public enum Rarity { poor = 0, common, rare, epic, legendary };
        public enum ItemType { weapon = 0, consumable, material }
        public enum ConsumableType { onetimeRecovery = 0, buff, other}; 
        public delegate void VoidCallback();
        public delegate void ItemDiscard();
        public delegate void ItemGain();
        public delegate bool Charge(Item item);

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
