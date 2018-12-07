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
            world.PushGoodsToTeam(item.id, item.currPileNum);
            return Inventory.PushItem(item);
        }
        public bool CanPushItemToPack(Good good)
        {
            Inventory.CanPushAllItem(good.item);
            return true;
        }
        public void PushItemFromShop(Good good)
        {
            world.PushGoodsToTeam(good.item.id, good.Number);
            Inventory.PushItem(good.item);
        }
        public float GetWeight()
        {
            return Inventory.currSize;
        }
    }
}
