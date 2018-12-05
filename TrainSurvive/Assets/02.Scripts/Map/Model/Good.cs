/*
 * 描述：对外部Good类的封装，用于WorldMap内部。
 * 作者：项叶盛
 * 创建时间：2018/11/21 23:10:55
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using System;
using Assets._02.Scripts.zhxUIScripts;

namespace WorldMap.Model
{
    [Serializable]
    public class Good
    {
        static int[] materialIDPool = new int[] { 201, 211, 212, 213, 214, 221, 222, 223, 231, 232, 233, 234 };
        static int[] weaponIDPool = new int[] { 0 };
        static int[] specailIDPool = new int[] { 700 };
        public int Price { private set; get; }
        public int ItemID { private set; get; }
        public int Number { private set; get; }
        /// <summary>
        /// 第一次获取时，动态加载。
        /// </summary>
        [NonSerialized]
        private Item m_item;
        public Item item
        {
            get
            {
                if (m_item == null)
                {
                    Item[] items = PublicMethod.GenerateItem(ItemID, Number);
                    m_item = items[0];
                    if (items.Length > 1)
                        Number = m_item.currPileNum;
                }
                return m_item;
            }
        }
        public bool DecreaseNumber(int number)
        {
            if (Number < number) return false;
            item.currPileNum -= number;
            Number = item.currPileNum;
            return true;
        }
        /// <summary>
        /// 如果number大于最大上限，则会设成最大值。
        /// </summary>
        /// <param name="number"></param>
        public void SetNumber(int number)
        {
            if (number > item.maxPileNum)
                number = item.maxPileNum;
            Number = item.currPileNum = number;
        }
        public Good Clone()
        {
            Good good = new Good();
            good.Price = Price;
            good.ItemID = ItemID;
            good.Number = Number;
            good.m_item = m_item;
            return good;
        }
        public static Good RandomByItem(Item item)
        {
            Good good = new Good();
            good.m_item = item;
            good.ItemID = item.id;
            good.Number = item.currPileNum;
            good.Price = StaticResource.RandomInt(500) + 500;
            return good;
        }
        public static Good RandomMaterial()
        {
            Good good = new Good();
            good.ItemID = materialIDPool[StaticResource.RandomInt(materialIDPool.Length)];
            //这里随机数量，在最后生成Item的时候，会工具最大数量裁剪
            good.Number = StaticResource.RandomRange(1, 5);
            good.Price = StaticResource.RandomInt(500) + 500;
            return good;
        }
        public static Good RandomWeapon()
        {
            Good good = new Good();
            good.ItemID = weaponIDPool[StaticResource.RandomInt(weaponIDPool.Length)];
            good.Number = 1;
            good.Price = StaticResource.RandomRange(1000, 2001);
            return good;
        }
        public static Good RandomSpecail()
        {
            Good good = new Good();
            good.ItemID = specailIDPool[StaticResource.RandomInt(specailIDPool.Length)];
            good.Number = 1;
            good.Price = StaticResource.RandomRange(10000, 20001);
            return good;
        }
    }
}