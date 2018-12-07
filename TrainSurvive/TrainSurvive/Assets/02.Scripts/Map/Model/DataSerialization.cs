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
    internal struct SerializableVector2Int
    {
        int x, y;
        internal SerializableVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        internal SerializableVector2Int(Vector2Int v)
        {
            this.x = v.x;
            this.y = v.y;
        }
    }
    [Serializable]
    public class DataSerialization
    {

        private Town[] towns;
        private Dictionary<SerializableVector2Int, Town> posToTown;
        public static DataSerialization Instance { get; private set; } = new DataSerialization();
        private DataSerialization()
        {
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
            this.posToTown = new Dictionary<SerializableVector2Int, Town>();
            int index = 0;
            for (int x = 0; x < townNumOfX; ++x)
                for (int z = 0; z < townNumOfZ; ++z)
                {
                    Town town = Town.Random();
                    town.PosIndexX = towns[x, z].position.x;
                    town.PosIndexY = towns[x, z].position.y;
                    SerializableVector2Int posKey = new SerializableVector2Int(town.PosIndexX, town.PosIndexY);
                    if (posToTown.ContainsKey(posKey))
                        posToTown[posKey] = town;
                    else
                        posToTown.Add(posKey, town);
                    this.towns[index++] = town;
                }
        }
        public void Init(DataSerialization ds)
        {
            //不直接替代的原因是：防止加载数据之前的引用失效。
            Instance.towns = ds.towns;
            Instance.posToTown = ds.posToTown;
        }
        public bool Find(Vector2Int posIndex, out Town town)
        {
            return posToTown.TryGetValue(new SerializableVector2Int(posIndex), out town);
        }
        public void Clean()
        {
            towns = null;
            posToTown = null;
        }
    }
}