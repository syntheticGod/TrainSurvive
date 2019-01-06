
/*
 * 描述：城镇具体信息类，WorldMap命名空间的Town值有坐标信息
 * 作者：项叶盛
 * 创建时间：2018/11/22 0:41:29
 * 版本：v0.1
 */
using UnityEngine;
using System;
using System.Collections.Generic;

using TTT.Resource;

namespace WorldMap.Model
{
    [Serializable]
    public class Town
    {
        [NonSerialized]
        const int MaxNPCCnt = 3;
        [NonSerialized]
        const int MaxGoodMaterialCnt = 3;
        [NonSerialized]
        const int MaxGoodWeaponCnt = 0;
        [NonSerialized]
        const int MaxGoodSpecailCnt = 1;
        private Town()
        { }
        public static Town Random()
        {
            Town ret = new Town();
            ret.NPCs = new List<NPC>(MaxNPCCnt);
            ret.Goods = new List<ItemData>(MaxGoodWeaponCnt + MaxGoodMaterialCnt);
            for (int i = 0; i < MaxNPCCnt; ++i)
            {
                ret.NPCs.Add(NPC.Random());
            }
            for (int i = 0; i < MaxGoodWeaponCnt; ++i)
            {
                ret.Goods.Add(ItemData.RandomWeapon());
            }
            for (int i = 0; i < MaxGoodMaterialCnt; ++i)
            {
                ret.Goods.Add(ItemData.RandomMaterial());
            }
            for (int i = 0; i < MaxGoodSpecailCnt; ++i)
            {
                ret.Goods.Add(ItemData.RandomSpecail());
            }
            ret.Name = StaticResource.RandomTownName();
            return ret;
        }
        public ETownType TownType;
        public string SpecialBuilding;
        //地图坐标
        public int PosIndexX { get; set; }
        public int PosIndexY { get; set; }
        //城镇名字
        public string Name { get; set; }
        //商店等级
        public int LevelShop { get; private set; }
        //酒馆等级
        public int LevelTavern { get; private set; }
        //学校等级
        public int LevelSchool { get; private set; }
        //商品数组
        public List<ItemData> Goods { get; private set; }
        //酒馆角色
        public List<NPC> NPCs { get; private set; }
        //商品数量
        public int GoodsCnt { get { return Goods.Count; } }
        //酒馆角色数量
        public int NPCCnt { get { return NPCs.Count; } }
        /// <summary>
        /// 招募NPC
        /// </summary>
        /// <param name="theOne"></param>
        /// <returns></returns>
        public bool RecruitNPC(NPC theOne)
        {
            if (!NPCs.Remove(theOne))
            {
                Debug.Log("城镇：酒馆中没有该NPC：" + theOne.Name);
                return false;
            }
            Debug.Log("城镇：NPC：" + theOne.Name + "走了");
            return true;
        }
        /// <summary>
        /// 在城镇中寻找指定ID的NPC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NPC FindNPCByID(int id)
        {
            foreach(NPC npc in NPCs)
                if (npc.ID == id)
                    return npc;
            return null;
        }
        /// <summary>
        /// 购买商品
        /// </summary>
        /// <param name="goods"></param>
        /// <param name="number"></param>
        /// <returns>
        /// TRUE：购买成功
        /// FALSE：商品不存在，或数量不足
        /// </returns>
        public bool BuyGoods(ItemData goods, int number)
        {
            int index = Goods.IndexOf(goods);
            if (index == -1)
            {
                Debug.Log("商店：商品不存在：" + goods.Name);
                return false;
            }
            if(goods.Number < number)
            {
                Debug.Log("商店：商品数量不足，我有：" + goods.Number + " 需求：" + number);
                return false;
            }
            goods.Number = -number;
            if (goods.Number == 0)
                Goods.Remove(goods);
            return true;
        }
        /// <summary>
        /// 向商店售卖
        /// </summary>
        /// <param name="goods"></param>
        public void SellGoods(ItemData goods)
        {
            Goods.Add(goods);
        }
        //城镇简介
        public string Info
        {
            get
            {
                return "城镇名：" + Name + " " +
                    "酒馆人数：" + NPCCnt + " " +
                    "商品数：" + GoodsCnt + " ";
            }
        }

    }
}
