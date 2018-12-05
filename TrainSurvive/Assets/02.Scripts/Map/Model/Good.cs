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
        const int materialIDBase = 200;
        const int weaponIDBase = 0;
        public int Price { private set; get; }
        public Item item { private set; get; }
        public int Number
        {
            get { return item.currPileNum; }
        }
        public bool DecreaseNumber(int number)
        {
            if (item.currPileNum < number) return false;
            item.currPileNum -= number;
            return true;
        }
        public static Good RandomMaterial()
        {
            Good good = new Good();
            int randomID = StaticResource.RandomRange(231, 235);
            int randomNum = StaticResource.RandomRange(1, 5);
            good.item = PublicMethod.GenerateItem(randomID, randomNum)[0];
            good.Price = StaticResource.RandomInt(500) + 500;
            return good;
        }
        public static Good RandomWeapon()
        {
            Good good = new Good();
            int randomID = weaponIDBase + StaticResource.RandomInt(1);
            good.item = PublicMethod.GenerateItem(randomID, 1)[0];
            good.Price = StaticResource.RandomRange(1000, 2001);
            return good;
        }
        private static Item RandomItem(PublicData.ItemType itemType)
        {
            Item item = new Weapon(1);
            //switch (itemType)
            //{
            //    case PublicData.ItemType.weapon:
            //        item = new Weapon(1);
            //        break;
            //    case PublicData.ItemType.consumable:
            //        item = new Consumable(1);
            //        break;
            //    default:
            //    case PublicData.ItemType.material:
            //        item = new Assets._02.Scripts.zhxUIScripts.Material(1);
            //        break;
            //}
            return item;
        }
    }
}