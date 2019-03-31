/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/28 0:19:26
 * 版本：v0.7
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TTT.UI;
using TTT.Utility;
using TTT.Xml;
using UnityEngine;

namespace WorldMap.Model
{
    [Serializable]
    public class TownDataSet : SubjectBase
    {
        public enum EAction
        {
            NONE = -1,
            /// <summary>
            /// 删除NPC
            /// </summary>
            REMOVE_NPC,
        }
        private List<TownData> towns = new List<TownData>();
        private int ID_Increasement = 1000;
        /// <summary>
        /// 城镇ID 到 城镇的映射
        /// </summary>
        private SerializableDictionary<int, TownData> idToTown = new SerializableDictionary<int, TownData>();
        /// <summary>
        /// 地图坐标 到 城镇的映射
        /// </summary>
        private SerializableDictionary<SerializableVector2Int, TownData> posToTown = new SerializableDictionary<SerializableVector2Int, TownData>();
        /// <summary>
        /// 区域坐标 到 城镇的映射
        /// </summary>
        private SerializableDictionary<SerializableVector2Int, TownData> arePosToTown = new SerializableDictionary<SerializableVector2Int, TownData>();
        public TownDataSet()
        {
        }
        public TownDataSet(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            towns = (List<TownData>)info.GetValue("Towns", typeof(List<TownData>));
            idToTown = (SerializableDictionary<int, TownData>)info.GetValue("idToTowns", typeof(SerializableDictionary<int, TownData>));
            posToTown = (SerializableDictionary<SerializableVector2Int, TownData>)info.GetValue("posToTowns", typeof(SerializableDictionary<SerializableVector2Int, TownData>));
            arePosToTown = (SerializableDictionary<SerializableVector2Int, TownData>)info.GetValue("arePosToTowns", typeof(SerializableDictionary<SerializableVector2Int, TownData>));
            ID_Increasement = info.GetInt32("ID_Inc");
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Towns", towns);
            info.AddValue("idToTowns", idToTown);
            info.AddValue("posToTowns", posToTown);
            info.AddValue("arePosToTowns", arePosToTown);
            info.AddValue("ID_Inc", ID_Increasement);
        }
        private void StoreTownData(TownData data)
        {
            towns.Add(data);
            //建立索引
            idToTown.Add(data.ID, data);
            posToTown.Add(new SerializableVector2Int(data.Pos), data);
            arePosToTown.Add(new SerializableVector2Int(data.Info.PosInArea), data);
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
                StoreTownData(new TownData(townsFromMap[posx, posy].position, info));
            }
            //随机普通城镇
            for (int x = 0; x < townNumOfX; ++x)
                for (int y = 0; y < townNumOfY; ++y)
                {
                    if (isSpecailTown[x, y]) continue;
                    TownInfo info = TownInfo.Random(TownInfoLoader.Instance.RandomTownName(), x, y);
                    StoreTownData(new TownData(townsFromMap[x, y].position, info));
                }
        }
        /// <summary>
        /// 获取一个新的城镇ID
        /// </summary>
        /// <returns>新的城镇ID</returns>
        public int NewID()
        {
            return ID_Increasement++;
        }
        /// <summary>
        /// 随机获取一个普通城镇的ID
        /// </summary>
        /// <returns></returns>
        public int RandomCommenTownID()
        {
            return MathTool.RandomRange(1000, ID_Increasement);
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
            return posToTown.TryGetValue(new SerializableVector2Int(pos), out town);
        }
        public TownData Find(Vector2Int pos)
        {
            TownData town;
            posToTown.TryGetValue(new SerializableVector2Int(pos), out town);
            return town;
        }
        /// <summary>
        /// 在所有城镇中寻找NPC，再删除
        /// </summary>
        /// <param name="npcID"></param>
        public bool RemoveNpc(int npcID, out TownData town)
        {
            for (int i = 0; i < towns.Count; ++i)
            {
                if (towns[i].RemoveNpc(npcID))
                {
                    town = towns[i];
                    Notify((int)EAction.REMOVE_NPC, npcID);
                    return true;
                }
            }
            town = null;
            return false;
        }
        public bool RemoveNpc(int npcID)
        {
            for (int i = 0; i < towns.Count; ++i)
            {
                if (towns[i].RemoveNpc(npcID))
                {
                    Notify((int)EAction.REMOVE_NPC, npcID);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据城镇ID查找查找
        /// </summary>
        /// <param name="id"></param>
        /// <param name="town"></param>
        /// <returns>
        /// TRUE：存在城镇
        /// FALSE：不存在城镇
        /// </returns>
        public bool Find(int id, out TownData town)
        {
            return idToTown.TryGetValue(id, out town);
        }
        public TownData Find(int id)
        {
            TownData town;
            idToTown.TryGetValue(id, out town);
            return town;
        }
        /// <summary>
        /// 根据区域坐标查找城镇
        /// </summary>
        /// <param name="xInAre"></param>
        /// <param name="yInAre"></param>
        /// <param name="town"></param>
        /// <returns>
        /// TRUE：存在城镇
        /// FALSE：不存在城镇
        /// </returns>
        public bool Find(int xInAre, int yInAre, out TownData town)
        {
            return arePosToTown.TryGetValue(new SerializableVector2Int(xInAre, yInAre), out town);
        }
        public TownData Find(int xInAre, int yInAre)
        {
            TownData town;
            arePosToTown.TryGetValue(new SerializableVector2Int(xInAre, yInAre), out town);
            return town;
        }
    }
}