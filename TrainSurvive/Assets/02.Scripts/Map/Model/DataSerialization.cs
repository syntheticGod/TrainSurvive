/*
 * 描述：WorldMap.Modle中需要持久化数据类
 * 作者：项叶盛
 * 创建时间：2018/11/22 0:53:56
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace WorldMap.Model
{
    [Serializable]
    public class DataSerialization
    {
        public Town[] towns;
        [NonSerialized]
        public Dictionary<Vector2Int, Town> posToTown;
        [NonSerialized]
        private static DataSerialization instance;
        private DataSerialization()
        {
            posToTown = new Dictionary<Vector2Int, Town>();
        }
        public static DataSerialization Instance
        {
            get
            {
                if (instance == null) instance = new DataSerialization();
                return instance;
            }
        }
        /// <summary>
        /// 随机生成初始化入口
        /// </summary>
        /// <param name="towns"></param>
        public void Init(WorldMap.Town[,] towns)
        {
            
            int townNumOfX = towns.GetLength(0);
            int townNumOfZ = towns.GetLength(1);
            this.towns = new Town[townNumOfX * townNumOfZ];
            int index = 0;
            for (int x = 0; x < townNumOfX; ++x)
                for (int z = 0; z < townNumOfZ; ++z)
                {
                    Town town = Town.Random();
                    town.PosIndexX = towns[x, z].position.x;
                    town.PosIndexY = towns[x, z].position.y;
                    if (posToTown.ContainsKey(towns[x, z].position))
                        posToTown[towns[x, z].position] = town;
                    else
                        posToTown.Add(towns[x, z].position, town);
                    this.towns[index++] = town;
                }
        }
        public void Init(DataSerialization ds)
        {
            instance = ds;
            foreach(Town town in towns)
            {
                Vector2Int pos = new Vector2Int(town.PosIndexX, town.PosIndexY);
                if (posToTown.ContainsKey(pos))
                    posToTown[pos] = town;
                else
                    posToTown.Add(pos, town);
            }
        }
        public bool Find(Vector2Int posIndex, out Town town)
        {
            return posToTown.TryGetValue(posIndex, out town);
        }
    }
}