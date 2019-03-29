/*
 * 描述：对World部分进行封装
 * 作者：项叶盛
 * 创建时间：2018/11/29 13:39:21
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;

using WorldMap.Model;
using TTT.Utility;
using TTT.Resource;
using WorldMap.UI;
using System;
using TTT.UI;

namespace WorldMap
{
    public class WorldForMap
    {
        private World world;
        public static WorldForMap Instance { get; } = new WorldForMap();
        private WorldForMap()
        {
            world = World.getInstance();
        }
        /// <summary>
        /// 售卖仓库中的物品
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <param name="number">物品数量</param>
        private void SellGoods(int itemID, int numberSell)
        {
            if (!world.storage.RemoveItem(itemID, numberSell))
            {
                Debug.LogError("售卖错误，数量不足，或商品不存在。ID：" + itemID + "数量：" + numberSell);
            }
        }
        /// <summary>
        /// 探险队售卖物品，售卖仓库中的物品
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="numberSell"></param>
        public void SellGoodsFromTeam(int itemID, int numberSell)
        {
            SellGoods(itemID, numberSell);
        }
        /// <summary>
        /// 列车售卖物品，售卖仓库中的物品
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="numberSell"></param>
        public void SellGoodsFromTrain(int itemID, int numberSell)
        {
            SellGoods(itemID, numberSell);
        }
        public int Money { get { return (int)world.getMoney(); } }
        //-----------------------------Team----------↓↓↓↓↓↓↓↓↓↓
        public bool IfTeamOuting
        {
            get { return world.ifTeamOuting; }
        }
        /// <summary>
        /// 探险队采集
        /// </summary>
        /// <returns>
        /// TRUE：采集完成
        /// FALSE：体力不足
        /// </returns>
        public bool DoGather()
        {
            if (world.outVit < 20)
                return false;
            world.addOutVit(-20);
            int number = 0;
            //采集次数
            int cntGather = 1;
            for (int i = 0; i < cntGather; i++)
                number += Gather.gather();
            if (number == 0)
                FlowInfo.ShowInfo("采集信息", "很遗憾一个东西都没采集到");
            return true;
        }
        public int TeamGetFootOutMax()
        {
            return (int)world.getFoodOutMax();
        }
        //-----------------------------Team----------↑↑↑↑↑↑↑↑↑↑↑↑
        public int TrainGetFoodIn()
        {
            return (int)world.getFoodIn();
        }
        public bool IfEnergyEmpty()
        {
            return Mathf.Approximately(world.getEnergy(), 0);
        }
    }
}

