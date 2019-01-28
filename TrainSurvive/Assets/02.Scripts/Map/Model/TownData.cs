
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
using System.Runtime.Serialization;

namespace WorldMap.Model
{
    [Serializable]
    public class TownData : ISerializable
    {
        public TownData(Vector2Int pos, TownInfo info)
        {
            Pos = pos;
            Info = info;
        }
        public TownData(SerializationInfo info, StreamingContext context)
        {
            Info = info.GetValue("TownInfo", typeof(TownInfo)) as TownInfo;
            int posx = info.GetInt32("PosX");
            int posy = info.GetInt32("PosY");
            Pos = new Vector2Int(posx, posy);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TownInfo", Info, typeof(TownInfo));
            info.AddValue("PosX", Pos.x, typeof(int));
            info.AddValue("PosY", Pos.y, typeof(int));
        }
        public TownInfo Info { get; private set; }
        public Vector2Int Pos { get; private set; }

        public ETownType TownType { get { return Info.Type; } }
        public int ID { get { return Info.ID; } }
        public string Name { get { return Info.Name; } }

        
        /// <summary>
        /// 招募NPC
        /// </summary>
        /// <param name="theOne"></param>
        /// <returns></returns>
        public bool RecruitNPC(NpcData theOne)
        {
            //if (!NPCs.Remove(theOne))
            //{
            //    Debug.Log("城镇：酒馆中没有该NPC：" + theOne.Name);
            //    return false;
            //}
            //Debug.Log("城镇：NPC：" + theOne.Name + "走了");
            return true;
        }
        /// <summary>
        /// 在城镇中寻找指定ID的NPC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NpcData FindNPCByID(int id)
        {
            //foreach (NpcData npc in NPCs)
            //    if (npc.ID == id)
            //        return npc;
            //Debug.LogError("当前城镇" + Name + " 不存在指定NPC：" + id);
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
            //int index = Goods.IndexOf(goods);
            //if (index == -1)
            //{
            //    Debug.Log("商店：商品不存在：" + goods.Name);
            //    return false;
            //}
            //if (goods.Number < number)
            //{
            //    Debug.Log("商店：商品数量不足，我有：" + goods.Number + " 需求：" + number);
            //    return false;
            //}
            //goods.Number = -number;
            //if (goods.Number == 0)
            //    Goods.Remove(goods);
            return true;
        }
        /// <summary>
        /// 向商店售卖
        /// </summary>
        /// <param name="goods"></param>
        public void SellGoods(ItemData goods)
        {
            //Goods.Add(goods);
        }
        public override string ToString()
        {
            return "城镇名：" + Name + " ";
        }

    }
}
