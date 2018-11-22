/*
 * 描述：城镇具体信息类，WorldMap命名空间的Town值有坐标信息
 * 作者：项叶盛
 * 创建时间：2018/11/22 0:41:29
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;
using System;

namespace WorldMap.Model
{
    public class Town
    {
        public Town()
        {
            Name = "测试名字";
        }

        //城镇名字
        public string Name { get; private set; }
        //商店等级
        public int LevelShop { get; private set; }
        //酒馆等级
        public int LevelTavern { get; private set; }
        //学校等级
        public int LevelSchool { get; private set; }
        //商品数量
        public int GoodsCnt { get { return Goods.Count; } }
        //商品数组
        public List<Good> Goods { get; private set; }
        //酒馆角色
        public List<Person> PersonsInTavern { get; private set; }
        //酒馆角色数量
        public int PersonCnt { get { return PersonsInTavern.Count; } }
        //城镇简介
        public string Info { get { return "城镇：" + Name + "\n信息"; } }

        //public void Random()
        //{

        //}

        //public Serializable Serialize()
        //{
        //    TownSerializable serializable = new TownSerializable();
        //    serializable.ID = 0;
        //    serializable.position = position;
        //    return serializable;
        //}

        //public void Deserialize(Serializable serializable)
        //{
        //    //必须是TownSerializable实例
        //    Debug.Assert(serializable.GetType().IsAssignableFrom(typeof(TownSerializable)));
        //    TownSerializable townSerializable = serializable as TownSerializable;
        //    position = townSerializable.position;
        //}
        //TODO: 测试序列化代码
    }
    //[Serializable]
    //public class TownSerializable : Serializable
    //{
    //    public uint ID;
    //    public Vector2Int position;
    //}
}
