/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/29 13:49:01
 * 版本：v0.7
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TTT.Xml;

namespace WorldMap.Model
{
    [Serializable]
    public class NpcDataSet
    {
        [SerializeField]
        private List<NpcData> npcs = new List<NpcData>();
        /// <summary>
        /// 建档时的初始化函数，必须先初始化World.Towns的建档初始化函数
        /// </summary>
        public void Init()
        {
            List<int> npcIds = NpcInfoLoader.Instance.AllNpcID();
            foreach(int id in npcIds)
            {
                NpcData data = new NpcData(id);
                npcs.Add(data);
                //-1的为不分配至城镇的NPC
                if(data.Info.Birthplace != -1)
                {
                    TownData town;
                    if(World.getInstance().Towns.Find(data.Info.Birthplace, out town))
                    {
                        town.Npcs.Add(data.ID);
                    }
                    else
                    {
                        Debug.LogError("NPC数据初始化失败，找不到当前NPC{"+data+"}的出生地城镇。");
                    }
                }
            }
        }
    }
}