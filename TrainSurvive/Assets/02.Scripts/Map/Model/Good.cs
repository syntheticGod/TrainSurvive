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
        public int Price { private set; get; }
        public Item item { private set; get; }
        public static Good Random()
        {
            Good good = new Good();
            good.item = RandomItem((PublicData.ItemType)StaticResource.RandomInt((int)PublicData.ItemType.num));
            good.Price = StaticResource.RandomInt(500) + 500;
            return good;
        }
        private static Item RandomItem(PublicData.ItemType itemType)
        {
            Item item;
            switch (itemType)
            {
                case PublicData.ItemType.weapon:
                    item = new Weapon(1);
                    break;
                case PublicData.ItemType.consumable:
                    item = new Consumable(1);
                    break;
                default:
                case PublicData.ItemType.material:
                    item = new Assets._02.Scripts.zhxUIScripts.Material(1);
                    break;
            }
            return item;
        }
        public bool TryBuyOne()
        {
            return true;
        }
    }
}