/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
/*
 * 描述：物品祖先抽象类
 * 作者：张皓翔
 * 创建时间：2018/10/31 18:42:11
 * 版本：v0.1
 * 注释：所有派生类在构造函数中用ID通过XML查找对应属性来初始化
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace Assets._02.Scripts.zhxUIScripts
{
    [Serializable]
    public abstract class Item
    {
        [NonSerialized]
        public ItemController belongGrid;
        
        public abstract PublicData.ItemType itemType
        {
            get;
        }
        public abstract int id {            //物品唯一ID
            get;
        }
        public abstract string name         //物品名字
        {
            get;
        }
        public abstract PublicData.Rarity rarity          //物品稀有度 0.泛滥  1.普通  2.不常见  3.稀有  4.极其稀有
        {
            get;
        }
        public abstract float size          //物品占用空间体积
        {
            get;
        }
        public abstract Sprite sprite        //物品的贴图
        {
            get;
        }
        public abstract string description  //物品描述
        {
            get;
        }
        public abstract int maxPileNum      //最大堆叠数
        {
            get;
        }
        public abstract int currPileNum     //当前堆叠数
        {
            get;
            set;
        }
        public abstract void OnGain();           //当获得物品时触发

        public abstract void OnDiscard();  //当丢弃物品时触发
        public Item Clone()
        {
            return this.MemberwiseClone() as Item;
        }



    }
}
