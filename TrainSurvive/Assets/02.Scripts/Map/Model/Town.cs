/*
 * 描述：城镇具体信息类，WorldMap命名空间的Town值有坐标信息
 * 作者：项叶盛
 * 创建时间：2018/11/22 0:41:29
 * 版本：v0.1
 */

using System.Collections.Generic;
using System;
using UnityEngine;

namespace WorldMap.Model
{
    [Serializable]
    public class Town
    {
        [NonSerialized]
        const int MaxNPC = 3;
        private Town()
        {
            
        }
        //public Town(Town town)
        //{
        //    PosIndexX = town.PosIndexX;
        //    PosIndexY = town.PosIndexY;
        //    Name = town.Name;
        //    LevelShop = town.LevelShop;
        //    LevelTavern = town.LevelTavern;
        //    LevelSchool = town.LevelSchool;
        //    Goods = town.Goods;
        //    NPCs = town.NPCs;
        //}
        public static Town Random()
        {
            Town ret = new Town();
            ret.NPCs = new List<NPC>(MaxNPC);
            for (int i = 0; i < MaxNPC; ++i)
            {
                ret.NPCs.Add(NPC.Random());
            }
            ret.Name = StaticResource.RandomTownName();
            return ret;
        }
        //地图坐标
        public int PosIndexX { get; set; }
        public int PosIndexY { get; set; }
        //城镇名字
        public string Name { get; private set; }
        //商店等级
        public int LevelShop { get; private set; }
        //酒馆等级
        public int LevelTavern { get; private set; }
        //学校等级
        public int LevelSchool { get; private set; }
        //商品数组
        public List<Good> Goods { get; private set; }
        //酒馆角色
        public List<NPC> NPCs { get; private set; }
        //商品数量
        public int GoodsCnt { get { return Goods.Count; } }
        //酒馆角色数量
        public int NPCCnt { get { return NPCs.Count; } }
        public bool RecruitNPC(NPC theOne)
        {
            if (!NPCs.Contains(theOne))
                return false;
            WorldForMap.Instance.AddPerson(theOne.PersonInfo);
            NPCs.Remove(theOne);
            return true;
        }
        //城镇简介
        public string Info
        {
            get
            {
                return "城镇：" + Name + "\n" +
                    "酒馆人数：" + NPCCnt;
            }
        }

    }
}
