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
            world.numIn = 3;
            //在列车内部随机生成人物
            for (int i = 0; i < world.numIn; i++)
            {
                AddPerson(Person.RandomPerson());
            }
            //world.money = 9999;//9,999
        }
        
        public bool IfMoneyEnough(int cost)
        {
            return world.getMoney() >= cost;
        }
        
        public bool Pay(int cost)
        {
            return world.addMoney(-cost);
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
                if(item.id == itemID)
                {
                    item.num -= numberSell;
                    if (item.num <= 0)
                    {
                        Debug.Assert(world.itemDataInTeam.Remove(item));
                        if(item.num < 0)
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
                        if(item.num < 0)
                            Debug.LogError("系统：探险队售卖数量减扣错误");
                    }
                    break;
                }
            }
        }
        public List<Good> GetGoodsInTeam()
        {
            List<Good> goods = new List<Good>();
            foreach(ItemData item in world.itemDataInTeam)
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
        public int Money
        {
            get
            {
                return (int)world.getMoney();
            }
        }
        /// <summary>
        /// 添加英雄
        /// </summary>
        /// <param name="person"></param>
        public void AddPerson(Person person)
        {
            world.persons.Add(person);
        }
        public List<Person> GetHeros()
        {
            return world.persons;
        }
        public int PersonCount()
        {
            return world.persons.Count;
        }
        public int MaxPersonCount()
        {
            return world.personNumMax;
        }
        public bool IfTeamOuting
        {
            get { return world.ifOuting; }
        }
        public bool IfTeamGathering
        {
            get { return world.ifGather; }
        }
        /// <summary>
        /// 探险队移动回调
        /// </summary>
        public void TeamMoving()
        {
            world.ifMoving = true;
            world.ifGather = false;
        }
        /// <summary>
        /// 探险队采集回调
        /// </summary>
        public void DoGather()
        {
            world.ifGather = true;
            world.ifMoving = false;
        }
        public void StopGather()
        {
            world.ifGather = false;
            world.ifMoving = false;
        }
        /// <summary>
        /// 探险队停止回调
        /// </summary>
        public void TeamStandeBy()
        {
            world.ifMoving = false;
            world.ifGather = false;
        }
        /// <summary>
        /// 探险队回车
        /// </summary>
        public void TeamGetIn()
        {
            world.ifOuting = false;
            world.ifMoving = false;
            world.ifGather = false;
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
            world.numIn += world.numOut;
            world.numOut = 0;
            foreach(Person person in world.persons)
            {
                person.ifOuting = false;
            }
        }
        /// <summary>
        /// 探险队外出
        /// </summary>
        /// <param name="food"></param>
        /// <param name="selectedPersons"></param>
        public void TeamGetOut(int food, List<Person> selectedPersons)
        {
            if (world.addFoodIn(-food) != 1)
            {
                Debug.LogWarning("列车食物减少不正常——探险队外出！");
            }
            world.numIn = world.persons.Count - selectedPersons.Count;
            Debug.Log("列车：探险队外出了，剩下" + world.numIn + "人，剩下" + world.getFoodIn() + "食物");
            if (!world.setFoodOut((uint)food))
            {
                Debug.LogWarning("探险队携带外出食物不正常！");
            }
            world.numOut = selectedPersons.Count;
            Debug.Log("探险队：我们（一共" + world.numOut + "人）外出了，带走了" + world.getFoodOut() + "点食物");
            foreach(Person person in selectedPersons)
            {
                person.ifOuting = true;
            }
            world.ifOuting = true;
        }
        public void TeamRecruit(Person person)
        {
            person.ifOuting = true;
            world.persons.Add(person);
        }
        public void TrainRecruit(Person person)
        {
            person.ifOuting = false;
            world.persons.Add(person);
        }
        public void TeamSetMapPos(Vector2Int mapPos)
        {
            world.posTeamX = mapPos.x;
            world.posTeamY = mapPos.y;
        }
        public int TeamNumber()
        {
            return world.numOut;
        }
        public Vector2Int TeamMapPos()
        {
            return new Vector2Int(world.posTeamX, world.posTeamY);
        }
        public int TeamGetFoodOut()
        {
            return (int)world.getFoodOut();
        }
        public void TeamSetFoodOut(int food)
        {
            world.setFoodOut(food < 0 ? 0 : (uint)food);
        }
        public int TeamAddFoodIn(int food)
        {
            return world.addFoodIn(food);
        }
        public void TrainMoving()
        {
            world.ifMoving = true;
            world.ifGather = false;
            world.ifOuting = false;
        }
        public void TrainStop()
        {
            world.ifMoving = false;
            world.ifGather = false;
            world.ifOuting = false;
        }
        public int GetFoodIn()
        {
            return (int)world.getFoodIn();
        }
        public int GetFootOutMax()
        {
            return (int)world.getFoodOutMax();
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
                    town.PosIndexX = towns[x, z].position.x;
                    town.PosIndexY = towns[x, z].position.y;
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

