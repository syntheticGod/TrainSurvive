/*
 * 描述：对World部分进行封装
 * 作者：项叶盛
 * 创建时间：2018/11/29 13:39:21
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;
using Assets._02.Scripts.zhxUIScripts;

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
            return world.money >= cost;
        }
        public bool Pay(int cost)
        {
            if (world.money < cost) return false;
            world.money -= cost;
            return true;
        }
        public void AddMoney(int money)
        {
            world.money += money;
        }
        public bool PushItemToTrain(Item item)
        {
            Debug.LogError("列车购买，待完善");
            return false;
        }
        public int Money
        {
            get
            {
                return world.money;
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
        public bool IfTeamOuting
        {
            get { return world.ifOuting; }
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
            //TODO:将身上的物品返回
            Debug.Log("探险队：我们（人数：" + world.numOut + "）回车了，带回食物：" + remain + "，列车现在有食物：" + world.getFoodIn());
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
        public void TeamSetMapPos(Vector2Int mapPos)
        {
            world.posTeamX = mapPos.x;
            world.posTeamY = mapPos.y;
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
        public int GetFoodIn()
        {
            return (int)world.getFoodIn();
        }
        public int GetFootOutMax()
        {
            return (int)world.foodOutMax;
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

