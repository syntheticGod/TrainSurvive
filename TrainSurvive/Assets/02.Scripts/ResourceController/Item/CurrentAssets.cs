/*
 * 描述：流动资产（可以交易的物品），包含：拥有的数量
 *          这部分数据需要存储在World里面。所以不继承ItemInfo，只要一个引用即可。
 * 作者：项叶盛
 * 创建时间：2019/1/3 3:51:55
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using TTT.Resource;
using System;
using TTT.Utility;

namespace TTT.Item
{
    [SerializeField]
    public class CurrentAssets1 : ISerializable, IEquatable<CurrentAssets1>, IComparable<CurrentAssets1>
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemID { get; }
        /// <summary>
        /// 物品拥有数量
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 物品的最大堆叠数
        /// </summary>
        public int MaxNumber { get { return Item.MaxPileNum; } }
        /// <summary>
        /// 物品名字
        /// </summary>
        public string Name { get { return Item.Name; } }
        /// <summary>
        /// 物品简介
        /// </summary>
        public string Description { get { return Item.Description; } }
        /// <summary>
        /// 获取Item信息
        /// </summary>
        public ItemInfo Item { get { return StaticResource.GetItemInfoByID<ItemInfo>(ItemID); } }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            CurrentAssets1 assets = obj as CurrentAssets1;
            if (assets == null) return false;
            return Equals(assets);
        }
        public override int GetHashCode()
        {
            var hashCode = 424428371;
            hashCode = hashCode * -1521134295 + ItemID.GetHashCode();
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            return hashCode;
        }
        public bool Equals(CurrentAssets1 other)
        {
            return ItemID == other.ItemID && Number == other.Number;
        }
        public int CompareTo(CurrentAssets1 other)
        {
            if (ItemID != other.ItemID)
            {
                return Number.CompareTo(other.Number);
            }
            return ItemID.CompareTo(other.ItemID);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemID", ItemID, typeof(int));
            info.AddValue("Number", Number, typeof(int));
        }
        public CurrentAssets1(SerializationInfo info, StreamingContext context)
        {
            ItemID = info.GetInt32("ItemID");
            Number = info.GetInt32("Number");
        }
        public CurrentAssets1(int itemID, int number)
        {
            ItemID = itemID;
            Number = number;
        }

        //----------
        static int[] materialIDPool = new int[] { 201, 211, 212, 213, 214, 221, 222, 223, 231, 232, 233, 234 };
        static int[] weaponIDPool = new int[] { 0 };
        static int[] specailIDPool = new int[] { 700 };
        public static CurrentAssets1 RandomMaterial()
        {
            return new CurrentAssets1(materialIDPool[MathTool.RandomInt(materialIDPool.Length)], MathTool.RandomRange(1, 5));
        }
        public static CurrentAssets1 RandomWeapon()
        {
            return new CurrentAssets1(weaponIDPool[MathTool.RandomInt(weaponIDPool.Length)], 1);
        }
        public static CurrentAssets1 RandomSpecail()
        {
            return new CurrentAssets1(specailIDPool[MathTool.RandomInt(specailIDPool.Length)], 1);
        }
    }
}
