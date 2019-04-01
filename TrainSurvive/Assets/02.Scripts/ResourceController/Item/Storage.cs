/*
 * 描述：物品的静态数据结构
 *          从items.xml文件中读取到的在整个程序中不会改变的信息。
 * 作者：项叶盛
 * 创建时间：2019/1/3 15:56:58
 * 版本：v0.7
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using WorldMap;

namespace TTT.Item
{
    [Serializable]
    public class Storage : SubjectBase
    {
        public enum EAction
        {
            NONE = -1,
            ADD_ITEM,
            REMOVE_ITEM
        }
        private List<ItemData> storage;
        public int Count { get { return storage.Count; } }
        /// <summary>
        /// 判断仓库中是否包含指定ID的物品
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <returns>
        /// TRUE：仓库中有该ID的物品
        /// FLASE：仓库中没有该ID的物品
        /// </returns>
        public bool ContainItem(int id)
        {
            foreach(ItemData data in storage)
            {
                if (data.ID == id)
                    return true;
            }
            return false;
        }
        public bool ContainItem(int id, int number)
        {
            return GetNumberByID(id) >= number;
        }
        /// <summary>
        /// 获取仓库中指定ID物品的数量
        /// 因为仓库中相同ID的物品 不一定存在一起，可能会分开。这是为了拆分物品而设计。
        /// </summary>
        /// <param name="id">指定物品的ID</param>
        /// <returns>
        /// 非0：物品的所有数量
        /// 0：不存在该物品
        /// </returns>
        public int GetNumberByID(int id)
        {
            int number = 0;
            foreach (ItemData data in storage)
            {
                if (data.ID == id)
                {
                    number += data.Number;
                }
            }
            return number;
        }
        /// <summary>
        /// 直接往仓库最后添加物品，不会进行整理
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <param name="number">物品数量</param>
        public void AddItem(int itemID, int number)
        {
            ItemData itemData = new ItemData(itemID, number, true);
            storage.Add(itemData);
            Notify((int)EAction.ADD_ITEM, itemData);
        }
        /// <summary>
        /// 直接往仓库最后添加物品，不会进行整理
        /// </summary>
        /// <param name="assets">物品对象</param>
        public void AddItem(ItemData item)
        {
            AddItem(item.ID, item.Number);
        }
        /// <summary>
        /// 从仓库中删除指定数量的指定ID的物品
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <param name="number">数量</param>
        /// <returns>
        /// TRUE：足够
        /// FALSE：库存不足
        /// </returns>
        public bool RemoveItem(int id, int number)
        {
            int numInStorage = GetNumberByID(id);
            if (numInStorage < number) return false;
            for(int i = 0; i < storage.Count; i++)
            {
                if(storage[i].ID == id)
                {
                    //当前框数量不足，继续往后减少。
                    if(storage[i].Number < number)
                    {
                        number -= storage[i].Number;
                        storage.RemoveAt(i);
                        i--;
                    }
                    //当前框数量刚好够减
                    else if (storage[i].Number == number)
                    {
                        storage.RemoveAt(i);
                        break;
                    }
                    //当前框数量有剩余
                    else
                    {
                        storage[i].Number -= number;
                    }
                }
            }
            Notify((int)EAction.REMOVE_ITEM, new ItemData(id, number));
            return true;
        }
        /// <summary>
        /// 从仓库中获取指定ID的物品列表。
        /// 返回的物品列表 中的 数量是无序的。
        /// </summary>
        /// <param name="id">指定ID</param>
        /// <returns>
        /// NULL：不存在该物品
        /// NOT NULL：所有物品都叠加在一起了
        /// </returns>
        public List<ItemData> GetItemsByID(int id)
        {
            List<ItemData> ret = new List<ItemData>();
            foreach (ItemData data in storage)
            {
                if (data.ID == id)
                    ret.Add(data);
            }
            return ret;
        }
        public void SortStorage()
        {
            storage.Sort();
            for (int i = 0; i < storage.Count - 1; i++)
            {
                //相同的物品融合到前一个物品中
                if (storage[i].ID == storage[i + 1].ID)
                {
                    int MaxNumber = storage[i].MaxPileNum;
                    if (storage[i].Number + storage[i + 1].Number > MaxNumber)
                    {
                        //两个物品的数量大于最大堆叠数
                        int deltaNumber = MaxNumber - storage[i].Number;
                        storage[i].Number = MaxNumber;
                        storage[i + 1].Number -= deltaNumber;
                    }
                    else
                    {
                        //两个物品的数量小于等于最大堆叠数，删去后一物品
                        storage[i].Number += storage[i + 1].Number;
                        storage.RemoveAt(i + 1);
                    }
                }
            }
        }
        public List<ItemData> CloneStorage()
        {
            return new List<ItemData>(storage);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemDatas", storage);
        }
        public Storage(SerializationInfo info, StreamingContext context)
        {
            storage = info.GetValue("ItemDatas", typeof(List<ItemData>)) as List<ItemData>;
            if (storage == null) Debug.LogError("ItemDatas读档失败");
        }
        public Storage()
        {
            storage = new List<ItemData>();
        }
    }
}