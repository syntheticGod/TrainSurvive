/*
 * 描述：对外部Good类的封装，用于WorldMap内部。
 * 作者：项叶盛
 * 创建时间：2018/11/21 23:10:55
 * 版本：v0.1
 */
using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

using TTT.UI;
using TTT.Utility;

using Assets._02.Scripts.zhxUIScripts;

namespace WorldMap.Model
{
    [Serializable]
    public class Good : ISerializable, Mergable
    {
        static int[] materialIDPool = new int[] { 201, 211, 212, 213, 214, 221, 222, 223, 231, 232, 233, 234 };
        static int[] weaponIDPool = new int[] { 0 };
        static int[] specailIDPool = new int[] { 700 };
        private ItemData itemData;
        public int Price { private set; get; }
        public int ItemID { private set { itemData.id = value; } get { return itemData.id; } }
        public int Number { private set { itemData.num = value; } get { return itemData.num; } }
        public int MaxNumber { get { return item.maxPileNum; } }
        public string Name { get { return item.name; } }
        public PublicData.ItemType ItemType { get { return item.itemType; } }
        public Item item { private set; get; }
        public Good(ItemData itemData)
        {
            this.itemData = itemData;
            item = itemData.item;
            //TODO：读取价格
            Price = 1000;
        }
        public Good(SerializationInfo info, StreamingContext context)
        {
            itemData = info.GetValue("ItemData", typeof(ItemData)) as ItemData;
            item = itemData.item;
            Price = info.GetInt32("Price");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemData", itemData);
            info.AddValue("Price", Price);
        }
        public Good Clone()
        {
            Good good = new Good(new ItemData(ItemID, Number));
            good.Price = Price;
            good.item = item.Clone();
            return good;
        }
        public override bool Equals(object goods)
        {
            if (goods is Good == false) return false;
            return (goods as Good).ItemID == ItemID;
        }
        public override int GetHashCode()
        {
            return -1218291565 + ItemID.GetHashCode();
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
            if(number < 0)
            {
                number = 0;
                Debug.LogError("商品：数量小于0，" + item.name);
            }
            if (number > item.maxPileNum)
            {
                number = item.maxPileNum;
                Debug.LogError("商品：数量大于最大数量，" + item.name);
            }
            Number = item.currPileNum = number;
        }
        /// <summary>
        /// 如果总数量大于最大上限，则会设成最大值。
        /// </summary>
        /// <param name="number"></param>
        public void AddNumber(int number)
        {
            SetNumber(Number + number);
        }
        public void MinusNumber(int number)
        {
            SetNumber(Number - number);
        }
        int Mergable.Number()
        {
            return Number;
        }

        int Mergable.MaxNumber()
        {
            return MaxNumber;
        }

        public void Merge(Mergable other)
        {
            Debug.Assert(other is Good);
            AddNumber((other as Good).Number);
        }

        public void Demerge(Mergable other)
        {
            Debug.Assert(other is Good);
            MinusNumber((other as Good).Number);
        }












        //----------
        public static Good RandomMaterial()
        {
            //这里随机数量，在最后生成Item的时候，会根据最大数量裁剪
            ItemData itemData = new ItemData(materialIDPool[MathTool.RandomInt(materialIDPool.Length)], MathTool.RandomRange(1, 5));
            Good good = new Good(itemData);
            good.Price = MathTool.RandomInt(500) + 500;
            return good;
        }
        public static Good RandomWeapon()
        {
            Good good = new Good(new ItemData(weaponIDPool[MathTool.RandomInt(weaponIDPool.Length)], 1));
            good.Price = MathTool.RandomRange(1000, 2001);
            return good;
        }
        public static Good RandomSpecail()
        {
            Good good = new Good(new ItemData(specailIDPool[MathTool.RandomInt(weaponIDPool.Length)], 1));
            good.Price = MathTool.RandomRange(10000, 20001);
            return good;
        }
        public static List<Good> AddGood(List<Good> storage, Good goods)
        {
            int i;
            for(i = 0; i < storage.Count;++i)
            {
                Good iGoods = storage[i];
                if(goods.ItemID == iGoods.ItemID && (iGoods.Number + goods.Number) <= iGoods.MaxNumber)
                {
                    iGoods.AddNumber(goods.Number);
                }
            }
            if(i == storage.Count)
            {
                storage.Add(goods);
            }
            return storage;
        }

    }
}