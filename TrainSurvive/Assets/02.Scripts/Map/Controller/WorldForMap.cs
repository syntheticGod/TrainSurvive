/*
 * 描述：对World部分进行封装
 * 作者：项叶盛
 * 创建时间：2018/11/29 13:39:21
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;

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
        /// <summary>
        /// 添加英雄
        /// </summary>
        /// <param name="person"></param>
        public void AddPerson(Person person)
        {
            world.persons.Add(person);
        }
        public bool IfTeamOuting
        {
            get { return world.ifOuting; }
        }
        public void DoGather()
        {
            world.ifGather = true;
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
        public void PrepareData()
        {
            for(int i = 0; i < world.towns.Length; ++i)
            {
                Model.Town town = world.towns[i];
                posToTown[new Vector2Int(town.PosIndexX, town.PosIndexY)] = town;
            }
        }
    }
}

