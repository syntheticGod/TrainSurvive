/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/28 0:19:26
 * 版本：v0.7
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TTT.Xml;
using UnityEngine;

namespace WorldMap.Model
{
    [Serializable]
    public class TownDataSet
    {
        [SerializeField]
        private List<TownData> towns = new List<TownData>();
        [NonSerialized]
        private bool init = false;
        [NonSerialized]
        private Dictionary<int, TownData> idToTown = new Dictionary<int, TownData>();
        /// <summary>
        /// 城镇ID 到 城镇的映射
        /// </summary>
        private Dictionary<int, TownData> IdToTown { get { if (!init) Init(); return idToTown; } }
        [NonSerialized]
        private Dictionary<Vector2Int, TownData> posToTown = new Dictionary<Vector2Int, TownData>();
        /// <summary>
        /// 地图坐标 到 城镇的映射
        /// </summary>
        private Dictionary<Vector2Int, TownData> PosToTown { get { if (!init) Init(); return posToTown; } }

        public void Init()
        {
            init = true;
            posToTown = new Dictionary<Vector2Int, TownData>();
            idToTown = new Dictionary<int, TownData>();
            //设置坐标到城镇的索引
            foreach (TownData townData in towns)
            {
                posToTown.Add(townData.Pos, townData);
                idToTown.Add(townData.ID, townData);
            }
        }
        /// <summary>
        /// 建立存档时 初始化调用
        /// </summary>
        /// <param name="townsFromMap"></param>
        public void Init(Town[,] townsFromMap)
        {
            int townNumOfX = townsFromMap.GetLength(0);
            int townNumOfY = townsFromMap.GetLength(1);
            bool[,] isSpecailTown = new bool[townNumOfX, townNumOfY];
            for (int i = 1; i <= TownInfoLoader.Instance.SpecailTownsCount; i++)
            {
                //特殊城镇的ID为 [1,特殊城镇数量]
                TownInfo info = TownInfoLoader.Instance.FindSTownInfoByID(i);
                int posx = info.PosInArea.x;
                int posy = info.PosInArea.y;
                isSpecailTown[posx, posy] = true;
                TownData data = new TownData(townsFromMap[posx, posy].position, info);
                towns.Add(data);
                idToTown.Add(data.ID, data);
                posToTown.Add(data.Pos, data);
            }
            //随机普通城镇
            for (int x = 0; x < townNumOfX; ++x)
                for (int y = 0; y < townNumOfY; ++y)
                {
                    if (isSpecailTown[x, y]) continue;
                    TownInfo info = TownInfo.Random(TownInfoLoader.Instance.RandomTownName(), x, y);
                    TownData data = new TownData(townsFromMap[x, y].position, info);
                    towns.Add(data);
                    idToTown.Add(data.ID, data);
                    posToTown.Add(data.Pos, data);
                }
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
        public bool Find(Vector2Int pos, out TownData town)
        {
            return PosToTown.TryGetValue(pos, out town);
        }
        public bool Find(int id, out TownData town)
        {
            return IdToTown.TryGetValue(id, out town);
        }
        public bool Find(int xInAre, int yInAre, out TownData town)
        {
            foreach(TownData data in towns)
            {
                if(data.Info.PosInArea.x == xInAre && data.Info.PosInArea.y == yInAre)
                {
                    town = data;
                    return true;
                }
            }
            town = null;
            return false;
        }
    }
}