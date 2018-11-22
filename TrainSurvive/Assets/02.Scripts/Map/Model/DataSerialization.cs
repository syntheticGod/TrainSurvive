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
        private List<List<Serializable>> serializables;
        [NonSerialized]
        private static DataSerialization instance;
        private DataSerialization()
        {
            serializables = new List<List<Serializable>>();
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
            //int townNumOfX = towns.GetLength(0);
            //int townNumOfZ = towns.GetLength(1);
            //townModels = new Town[townNumOfX * townNumOfZ];
            //int index = 0;
            //for (int x = 0; x < townNumOfX; ++x)
            //    for (int z = 0; z < townNumOfZ; ++z)
            //        townModels[index++] = new Town(towns[x, z]);
        }
        [NonSerialized]
        public Town[] townModels;
        [NonSerialized]
        public Person[] personModels;
        /// <summary>
        /// 序列化所需要的数据
        /// </summary>
        public void BeforeSerialize()
        {
            //Debug.Assert(null != townModels, "DataSerialization未初始化，先Init");
            //List<Serializable> towns = new List<Serializable>();
            //for (int x = 0; x < townModels.Length; ++x)
            //        towns.Add(townModels[x].Serialize());
            //serializables.Add(towns);

            //Debug.Assert(null != personModels, "DataSerialization未初始化，先Init");
            //List<Serializable> persons = new List<Serializable>();
            //for (int x = 0; x < personModels.Length; ++x)
            //    persons.Add(personModels[x].Serialize());
            //serializables.Add(persons);
        }
        /// <summary>
        /// 将序列化数据加载到到相应对象
        /// </summary>
        public void AfterDeserialize(object serialization)
        {
            //instance = serialization as DataSerialization;
            //List<Serializable> towns = serializables[0];
            //townModels = new Town[towns.Count];
            //for (int x = 0; x < towns.Count; ++x)
            //    townModels[x].Deserialize(towns[x]);

            //List<Serializable> persons = serializables[1];
            //personModels = new Person[persons.Count];
            //for (int x = 0; x < persons.Count; ++x)
            //    personModels[x].Deserialize(persons[x]);
        }
    }
}