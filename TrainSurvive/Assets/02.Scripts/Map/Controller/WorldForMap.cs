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

namespace WorldMap
{
    public class WorldForMap
    {
        private World world;
        private Dictionary<Vector2Int, Model.Town> posToTown;
        public static WorldForMap Instance { get; } = new WorldForMap();
        private WorldForMap()
        {
            world = World.getInstance();
            posToTown = new Dictionary<Vector2Int, Model.Town>();
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
        public void PushItemToTrain(int itemID, int numberBuy)
        {
            world.itemDataInTrain.Add(new ItemData(itemID, numberBuy));
        }
        public bool CanPushGoodsToTeam(int itemID, int numberBuy)
        {
            //TODO 目前背包容量无限
            return true;
        }
        public bool PushGoodsToTeam(int itemID, int numberBuy)
        {
            world.itemDataInTeam.Add(new ItemData(itemID, numberBuy));
            return true;
        }
        public float GetPackWeightInTeam()
        {
            //TODO 暂时使用数量代替重量
            return world.itemDataInTeam.Count;
        }
        public void SellGoodsFromTeam(int itemID, int numberSell)
        {
            foreach (ItemData item in world.itemDataInTeam)
            {
                if (item.id == itemID)
                {
                    item.num -= numberSell;
                    if (item.num <= 0)
                    {
                        Debug.Assert(world.itemDataInTeam.Remove(item));
                        if (item.num < 0)
                            Debug.LogError("系统：探险队售卖数量减扣错误");
                    }
                    break;
                }
            }
        }
        public void SellGoodsFromTrain(int itemID, int numberSell)
        {
            foreach (ItemData item in world.itemDataInTrain)
            {
                if (item.id == itemID)
                {
                    item.num -= numberSell;
                    if (item.num <= 0)
                    {
                        Debug.Assert(world.itemDataInTrain.Remove(item));
                        if (item.num < 0)
                            Debug.LogError("系统：探险队售卖数量减扣错误");
                    }
                    break;
                }
            }
        }
        public List<Good> GetGoodsInTeam()
        {
            List<Good> goods = new List<Good>();
            foreach (ItemData item in world.itemDataInTeam)
            {
                goods.Add(new Good(item));
            }
            return goods;
        }
        public List<Good> GetGoodsInTrain()
        {
            List<Good> goods = new List<Good>();
            foreach (ItemData item in world.itemDataInTrain)
            {
                goods.Add(new Good(item));
            }
            return goods;
        }
        public int Money { get { return (int)world.getMoney(); } }
        /// <summary>
        /// 在列车内部随机生成人物
        /// </summary>
        /// <param name="count"></param>
        public void RandomPersonInTrain(int count)
        {
            world.numIn = count;
            for (int i = 0; i < count; i++)
            {
                Person person = Person.RandomPerson();
                //默认全部出战，直到上限
                if (i < MAX_NUMBER_FIGHER)
                {
                    person.ifReadyForFighting = true;
                }
                AddPerson(person);
            }
        }
        /// <summary>
        /// DEBUG模式，添加金钱等
        /// </summary>
        public void InitInDebug()
        {
#if DEBUG
            world.addMoney(10000);
            world.addStrategy(1000);
#endif
        }
        /// <summary>
        /// 添加英雄
        /// </summary>
        /// <param name="person"></param>
        public void AddPerson(Person person)
        {
            world.AddPerson(person);
        }
        public List<Person> GetAllPersons()
        {
            return new List<Person>(world.persons); ;
        }
        public int PersonCount()
        {
            return world.persons.Count;
        }
        public int MaxPersonCount()
        {
            return world.personNumMax;
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
            //马上采集五次
            for (int i = 0; i < 5; i++)
                Gather.gather();
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
                "带回食物：" + remain + "，列车现在有食物：" + world.getFoodIn() +
                "带回东西：" + world.itemDataInTeam.Count + "个");
            foreach (ItemData item in world.itemDataInTeam)
            {
                world.itemDataInTrain.Add(item);
            }
            world.itemDataInTeam.Clear();
            world.numIn = world.persons.Count;
            world.numOut = 0;
        }
        /// <summary>
        /// 探险队外出
        /// </summary>
        public void TeamGetOut()
        {
            int food = world.persons.Count * 200;
            if (food > world.getFoodIn())
                food = (int)world.getFoodIn();
            if (world.addFoodIn(-food) != 1)
            {
                Debug.LogWarning("列车食物减少不正常——探险队外出！");
            }
            world.numIn = 0;
            world.numOut = world.persons.Count;
            Debug.Log("列车：探险队外出了，剩下" + world.numIn + "人，剩下" + world.getFoodIn() + "食物");
            if (!world.setFoodOut((uint)food))
            {
                Debug.LogWarning("探险队携带外出食物不正常！");
            }
            Debug.Log("探险队：我们（一共" + world.numOut + "人）外出了，带走了" + world.getFoodOut() + "点食物");
            world.FullOutVit();
            world.ifTeamOuting = true;
        }
        public void TeamRecruit(Person person)
        {
            world.AddPerson(person);
        }
        public void TeamSetMapPos(Vector2Int mapPos)
        {
            world.posTeamX = mapPos.x;
            world.posTeamY = mapPos.y;
        }
        public int TeamNumber()
        {
            return world.persons.Count;
        }
        public Vector2Int TeamMapPos()
        {
            return new Vector2Int(world.posTeamX, world.posTeamY);
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
        public const int MAX_NUMBER_FIGHER = 5;
        /// <summary>
        /// 设置指定人物的出战设置
        /// </summary>
        /// <param name="person">人物</param>
        /// <param name="ifReadyForFight">是否出战</param>
        /// <returns>
        /// TRUE：设置成功
        /// FALSE：
        /// 当ifReadyForFight为TRUE时，出战人数不能超过上限
        /// 当ifReadyForFight为FALSE时，出战人数不能少于一人
        /// </returns>
        public bool TeamConfigFight(Person person, bool ifReadyForFight)
        {
            int num = 0;
            foreach (Person itr in world.persons)
            {
                if (itr.ifReadyForFighting) num++;
            }

            if (ifReadyForFight)
            {
                if (num >= MAX_NUMBER_FIGHER)
                    return false;
            }
            else
            {
                if (num <= 1)
                    return false;
            }
            person.ifReadyForFighting = ifReadyForFight;
            return true;
        }
        //-----------------------------Team----------↑↑↑↑↑↑↑↑↑↑↑↑

        //-----------------------------Train----------↓↓↓↓↓↓↓↓↓↓
        public void TrainRecruit(Person person)
        {
            world.AddPerson(person);
        }
        public void TrainMoving()
        {
            world.ifTrainMoving = true;
            if (world.ifTeamOuting)
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
        public void TrainSetMapPos(Vector2Int mapPos)
        {
            world.posTrainX = mapPos.x;
            world.posTrainY = mapPos.y;
        }
        public Vector2Int TrainMapPos()
        {
            return new Vector2Int(world.posTrainX, world.posTrainY);
        }
        public bool IfEnergyEmpty()
        {
            return Mathf.Approximately(world.getEnergy(), 0);
        }
        //-----------------------------Train----------↑↑↑↑↑↑↑↑↑↑↑↑

        /// <summary>
        /// 在World中随机生成城镇信息
        /// </summary>
        /// <param name="towns">用于获取地图生成中城镇的地图坐标</param>
        public void RandomTownsInfo(Town[,] towns)
        {
            int townNumOfX = towns.GetLength(0);
            int townNumOfZ = towns.GetLength(1);
            Model.Town[] townInfos = new Model.Town[townNumOfX * townNumOfZ];
            int index = 0;
            for (int x = 0; x < townNumOfX; ++x)
                for (int z = 0; z < townNumOfZ; ++z)
                {
                    Model.Town town = Model.Town.Random();
                    town.TownType = towns[x, z].typeId;
                    town.PosIndexX = towns[x, z].position.x;
                    town.PosIndexY = towns[x, z].position.y;
                    if (town.TownType != ETownType.COMMON)
                    {
                        town.SpecialBuilding = towns[x, z].description;
                        town.Name = towns[x, z].name;
                    }
                    townInfos[index++] = town;
                }
            world.towns = townInfos;
        }
        /// <summary>
        /// 通过地图坐标寻找城镇
        /// </summary>
        /// <param name="mapPos">地图坐标</param>
        /// <param name="town">城镇对象</param>
        /// <returns>
        /// TRUE：存在城镇
        /// FALSE：不存在城镇
        /// </returns>
        public bool FindTown(Vector2Int mapPos, out Model.Town town)
        {
            return posToTown.TryGetValue(mapPos, out town);
        }
        public void SaveGame()
        {
            world.save();
        }
        /// <summary>
        /// 读档时的初始化
        /// 1、生成城镇坐标映射
        /// </summary>
        public void PrepareData()
        {
            for (int i = 0; i < world.towns.Length; ++i)
            {
                Model.Town town = world.towns[i];
                posToTown[new Vector2Int(town.PosIndexX, town.PosIndexY)] = town;
            }
        }
    }
}

