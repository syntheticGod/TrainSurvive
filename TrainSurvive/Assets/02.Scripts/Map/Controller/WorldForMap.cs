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
        public bool IfMoneyEnough(int cost)
        {
            return world.IfMoneyEnough(cost);
        }
        public bool Pay(int cost)
        {
            return world.PayByMoney(cost);
        }
        public void AddMoney(int money)
        {
            world.addMoney(money);
        }
        private bool PushItem(int itemID, int number)
        {
            world.storage.AddItem(itemID, number);
            return true;
        }
        public void PushItemToTrain(int itemID, int numberBuy)
        {
            PushItem(itemID, numberBuy);
        }
        public bool PushGoodsToTeam(int itemID, int numberBuy)
        {
            PushItem(itemID, numberBuy);
            return true;
        }
        public bool CanPushGoodsToTeam(int itemID, int numberBuy)
        {
            //无限仓库
            return true;
        }
        public float GetPackWeightInTeam()
        {
            //TODO 暂时使用数量代替重量
            return world.storage.Count;
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
        /// <summary>
        /// DEBUG模式，添加金钱等
        /// </summary>
        public void InitInDebug()
        {
#if DEBUG
            WorldForMap.Instance.AddMoney(10000);
            world.addStrategy(1000);
#endif
        }
        //-----------------------------Team----------↓↓↓↓↓↓↓↓↓↓
        public bool IfTeamOuting
        {
            get { return world.ifTeamOuting; }
        }
        /// <summary>
        /// 探险队移动回调
        /// </summary>
        public void TeamMoving()
        {
            world.ifTrainMoving = true;
        }
        /// <summary>
        /// 探险队停止回调
        /// </summary>
        public void TeamStandeBy()
        {
            world.ifTeamMoving = false;
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
        public void StopGather()
        {
        }
        /// <summary>
        /// 探险队回车
        /// </summary>
        public void TeamGetIn()
        {
            world.ifTeamOuting = false;
            world.ifTrainMoving = false;
            //探险队放回食物
            int remain = (int)world.getFoodOut();
            world.setFoodOut(0);
            if (world.addFoodIn(remain) != 1)
            {
                Debug.LogWarning("探险队增加内部食物不正常");
            }
            Debug.Log("探险队：我们（人数：" + world.numOut + "）回车了。" +
                "带回食物：" + remain + "，列车现在有食物：" + world.getFoodIn());
            world.numIn = world.Persons.Count;
            world.numOut = 0;
        }
        /// <summary>
        /// 探险队外出
        /// </summary>
        public void TeamGetOut()
        {
            int food = world.Persons.Count * 200;
            if (food > world.getFoodIn())
                food = (int)world.getFoodIn();
            if (world.addFoodIn(-food) != 1)
            {
                Debug.LogWarning("列车食物减少不正常——探险队外出！");
            }
            world.numIn = 0;
            world.numOut = world.Persons.Count;
            Debug.Log("列车：探险队外出了，剩下" + world.numIn + "人，剩下" + world.getFoodIn() + "食物");
            if (!world.setFoodOut((uint)food))
            {
                Debug.LogWarning("探险队携带外出食物不正常！");
            }
            Debug.Log("探险队：我们（一共" + world.numOut + "人）外出了，带走了" + world.getFoodOut() + "点食物");
            world.FullOutVit();
            world.ifTeamOuting = true;
        }
        public int TeamGetFoodOut()
        {
            return (int)world.getFoodOut();
        }
        public int TeamGetFootOutMax()
        {
            return (int)world.getFoodOutMax();
        }
        public void TeamSetFoodOut(int food)
        {
            world.setFoodOut(food < 0 ? 0 : (uint)food);
        }
        public int TeamAddFoodIn(int food)
        {
            return world.addFoodIn(food);
        }
        //-----------------------------Team----------↑↑↑↑↑↑↑↑↑↑↑↑

        //-----------------------------Train----------↓↓↓↓↓↓↓↓↓↓
        public void TrainMoving()
        {
            world.ifTrainMoving = true;
            if (WorldForMap.Instance.IfTeamOuting)
                Debug.LogError("错误，小队外出状态下，列车被允许移动");
        }
        public void TrainStop()
        {
            world.ifTrainMoving = false;
        }
        public int TrainGetFoodIn()
        {
            return (int)world.getFoodIn();
        }
        public bool IfEnergyEmpty()
        {
            return Mathf.Approximately(world.getEnergy(), 0);
        }
        //-----------------------------Train----------↑↑↑↑↑↑↑↑↑↑↑↑
        
    }
}

