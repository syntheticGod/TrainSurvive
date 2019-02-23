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
            foreach (int id in npcIds)
            {
                NpcInfo info = NpcInfoLoader.Instance.Find(id);
                int hometownID = CompileBirthplace(info.Birthplace);
                //-1的为不分配至城镇的NPC
                if (hometownID != -1)
                {
                    TownData town;
                    if (World.getInstance().Towns.Find(hometownID, out town))
                    {
                        town.Npcs.Add(id);
                    }
                    else
                    {
                        Debug.LogError("NPC数据初始化失败，找不到NPC{" + info.Name + "}的出生地城镇。");
                    }
                }
                npcs.Add(new NpcData(id, hometownID));
            }
        }
        /// <summary>
        /// 将Birthplace的字符串转化为具体的城镇
        /// </summary>
        /// <param name="birthplace">
        /// 格式见NPC.xml文件中的Birthplace字段说明
        /// </param>
        /// <returns></returns>
        private int CompileBirthplace(string birthplace)
        {
            if (birthplace.Equals("-")) return -1;
            if (birthplace[0] == '(')
            {
                birthplace = birthplace.Remove(birthplace.Length - 1, 1);
                birthplace = birthplace.Remove(0, 1);
                string[] coord = birthplace.Split(',');
                int x = int.Parse(coord[0]);
                int y = int.Parse(coord[1]);
                TownData town;
                if (!World.getInstance().Towns.Find(x, y, out town))
                {
                    Debug.LogError("找不到区块坐标为 (" + x + "," + y + ")" + "中的城镇，默认成不分配。");
                    return -1;
                }
                return town.ID;
            }
            int birthtown = int.Parse(birthplace);
            //0表示在普通城镇中随机一个
            if (birthtown == 0)
                return World.getInstance().Towns.RandomCommenTownID();
            return birthtown;
        }
        public override string ToString()
        {
            string ret = "";
            foreach (NpcData npc in npcs)
            {
                ret += npc.Info.Name + "的出生地在" + World.getInstance().Towns.Find(npc.HometownID)?.Pos + "\n";
            }
            return ret;
        }
    }
}
