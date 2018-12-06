/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/6 3:23:18
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using Assets._02.Scripts.zhxUIScripts;

namespace WorldMap.Model
{
    public class InventoryForTeam
    {
        public Inventory Inventory { private set; get; }
        public WorldForMap world;
        public InventoryForTeam(float MaxValue)
        {
            Inventory = new Inventory(MaxValue);
            world = WorldForMap.Instance;
        }
        /// <summary>
        /// 采集获得的物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int PushItem(Item item)
        {
            Good good = Good.RandomByItem(item);
            world.PushGoodsToTeam(good, good.Number);
            return Inventory.PushItem(item);
        }
        public bool CanPushItemToPack(Good good, int number)
        {
            return true;
        }
        public void PushItemFromShop(Good good, int number)
        {
            world.PushGoodsToTeam(good, number);
            Item item = good.item.Clone();
            item.currPileNum = number;
            //FOR TEST 判断背包是否放的下
            Inventory.PushItem(item);
        }
        public float GetWeight()
        {
            return Inventory.currSize;
        }
    }
}
