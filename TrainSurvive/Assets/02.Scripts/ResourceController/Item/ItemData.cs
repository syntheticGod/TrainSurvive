/*
 * 描述：一个简单的封装了物品ID和数量的类，可以装到自己的List<ItemData>里面对容器内的物品进行序列化保存
 * 作者：张皓翔
 * 创建时间：2018/12/4 17:26:10
 * 版本：v0.1
 */
using UnityEngine;
using System;
using System.Runtime.Serialization;

using TTT.Item;
using TTT.Resource;
using TTT.Utility;

using Assets._02.Scripts.zhxUIScripts;

[Serializable]
public class ItemData
{
    /// <summary>
    /// 物品ID
    /// </summary>
    public int ID { get; }
    /// <summary>
    /// 物品拥有数量
    /// </summary>
    public int Number { get; set; }
    /// <summary>
    /// 物品名字
    /// </summary>
    public string Name { get { return Item.Name; } }
    /// <summary>
    /// 物品简介
    /// </summary>
    public string Description { get { return Item.Description; } }
    /// <summary>
    /// 物品类别
    /// </summary>
    public PublicData.ItemType Type { get { return Item.Type; } }
    /// <summary>
    /// 物品稀有度
    /// </summary>
    public PublicData.Rarity Rarity { get { return Item.Rarity; } }
    /// <summary>
    /// 初始价格
    /// </summary>
    public int OriginPrice { get { return Item.Price; } }
    /// <summary>
    /// 售卖价格
    /// </summary>
    public int SellPrice { get { return Mathf.RoundToInt(Item.Price * Item.SellRatio); } }
    /// <summary>
    /// 物品占用空间体积
    /// </summary>
    public float Size { get { return Item.Size; } }
    /// <summary>
    /// 最大堆叠数
    /// </summary>
    public int MaxPileNum { get { return Item.MaxPileNum; } }
    /// <summary>
    /// 120 x 120 像素
    /// </summary>
    public Sprite BigSprite { get { return Item.BigSprite; } }
    /// <summary>
    /// 60 x 60 像素
    /// </summary>
    public Sprite SmallSprite { get { return Item.SmallSprite; } }
    /// <summary>
    /// 获取Item信息
    /// </summary>
    public ItemInfo Item { get { return StaticResource.GetItemInfoByID<ItemInfo>(ID); } }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        ItemData assets = obj as ItemData;
        if (assets == null) return false;
        return Equals(assets);
    }
    public override int GetHashCode()
    {
        var hashCode = 424428371;
        hashCode = hashCode * -1521134295 + ID.GetHashCode();
        hashCode = hashCode * -1521134295 + Number.GetHashCode();
        return hashCode;
    }
    public bool Equals(ItemData other)
    {
        return ID == other.ID && Number == other.Number;
    }
    public int CompareTo(ItemData other)
    {
        if (ID != other.ID)
        {
            return Number.CompareTo(other.Number);
        }
        return ID.CompareTo(other.ID);
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("ItemID", ID, typeof(int));
        info.AddValue("Number", Number, typeof(int));
    }
    public ItemData(SerializationInfo info, StreamingContext context)
    {
        ID = info.GetInt32("ItemID");
        Number = info.GetInt32("Number");
    }
    public ItemData(int itemID, int number)
    {
        ID = itemID;
        Number = number;
    }
    public ItemData Clone()
    {
        return new ItemData(ID, Number);
    }

    //----------
    static int[] materialIDPool = new int[] { 201, 211, 212, 213, 214, 221, 222, 223, 231, 232, 233, 234 };
    static int[] weaponIDPool = new int[] { 0 };
    static int[] specailIDPool = new int[] { 700 };
    public static ItemData RandomMaterial()
    {
        return new ItemData(materialIDPool[MathTool.RandomInt(materialIDPool.Length)], MathTool.RandomRange(1, 5));
    }
    public static ItemData RandomWeapon()
    {
        return new ItemData(weaponIDPool[MathTool.RandomInt(weaponIDPool.Length)], 1);
    }
    public static ItemData RandomSpecail()
    {
        return new ItemData(specailIDPool[MathTool.RandomInt(specailIDPool.Length)], 1);
    }
}

