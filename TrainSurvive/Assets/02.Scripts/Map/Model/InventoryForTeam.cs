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
        public WorldForMap world;
        public InventoryForTeam(float MaxValue)
        {
            world = WorldForMap.Instance;
        }
        /// <summary>
        /// 采集获得的物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool PushItem(Item item)
        {
            return world.PushGoodsToTeam(item.id, item.currPileNum);
        }
        public bool CanPushItemToPack(int id, int number)
        {
            return world.CanPushGoodsToTeam(id, number);
        }
        public void PushItemFromShop(int id, int number)
        {
            world.PushGoodsToTeam(id, number);
        }
        public float GetWeight()
        {
            return world.GetPackWeightInTeam();
        }
    }
}
