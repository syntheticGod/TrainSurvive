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
        const int MaxNPCCnt = 3;
        [NonSerialized]
        const int MaxGoodMaterialCnt = 3;
        [NonSerialized]
        const int MaxGoodWeaponCnt = 2;
        [NonSerialized]
        const int MaxGoodSpecailCnt = 1;
        private Town()
        { }
        public static Town Random()
        {
            Town ret = new Town();
            ret.NPCs = new List<NPC>(MaxNPCCnt);
            ret.Goods = new List<Good>(MaxGoodWeaponCnt+MaxGoodMaterialCnt);
            for (int i = 0; i < MaxNPCCnt; ++i)
            {
                ret.NPCs.Add(NPC.Random());
            }
            for(int i = 0; i < MaxGoodWeaponCnt; ++i)
            {
                ret.Goods.Add(Good.RandomWeapon());
            }
            for(int i = 0; i < MaxGoodMaterialCnt; ++i)
            {
                ret.Goods.Add(Good.RandomMaterial());
            }
            for(int i = 0; i < MaxGoodSpecailCnt; ++i)
            {
                ret.Goods.Add(Good.RandomSpecail());
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
        /// <summary>
        /// 招募NPC
        /// </summary>
        /// <param name="theOne"></param>
        /// <returns></returns>
        public bool RecruitNPC(NPC theOne)
        {
            if (!NPCs.Contains(theOne))
                return false;
            WorldForMap.Instance.AddPerson(theOne.PersonInfo);
            NPCs.Remove(theOne);
            return true;
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
        public bool BuyGoods(Good goods, int number)
        {
            int index = Goods.IndexOf(goods);
            if (index == -1)
            {
                Debug.Log("商店：商品不存在：" + goods.item.name);
                return false;
            }
            if (!goods.DecreaseNumber(number))
            {
                Debug.Log("商店：商品数量不足，我有：" + goods.Number+" 需求："+number);
                return false;
            }
            if (goods.Number == 0)
                Goods.Remove(goods);
            return true;
        }
        /// <summary>
        /// 向商店售卖
        /// </summary>
        /// <param name="goods"></param>
        public void SellGoods(Good goods)
        {
            Goods.Add(goods);
        }
        //城镇简介
        public string Info
        {
            get
            {
                return "城镇名：" + Name + " " +
                    "酒馆人数：" + NPCCnt + " "+
                    "商品数：" + GoodsCnt + " ";
            }
        }

    }
}
