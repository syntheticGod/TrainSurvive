/*
 * 描述：消耗品类
 * 作者：张皓翔
 * 创建时间：2018/10/31 18:54:54
 * 版本：v0.1
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
    public class Consumable : Item
    {
        //--------  重写属性内层  ----------
        private int _id;
        private string _name;
        private PublicData.ItemType _item_type;
        private PublicData.Rarity _rarity;
        private float _size;
        [NonSerialized]
        private Sprite _sprite;
        private string _description;
        private int _max_pile_num;
        private int _cur_pile_num;

        //--------  特有属性内层  ----------
        [NonSerialized]
        private PublicData.ConsumableType _con_type;
        [NonSerialized]
        private PublicData.VoidCallback itemDiscard;
        [NonSerialized]
        private PublicData.VoidCallback itemGain;
        [NonSerialized]
        private PublicData.VoidCallback onUse;

        //--------  重写属性外层  ----------
        public override PublicData.ItemType itemType
        {
            get
            {
                return _item_type;
            }
        }
        public override int id {
            get
            {
                return _id;
            }
        }
        public override string name
        {
            get
            {
                return _name;
            }
        }
        public override PublicData.Rarity rarity
        {
            get
            {
                return _rarity;
            }
        }
        public override float size
        {
            get
            {
                return _size;
            }
        }
        public override Sprite sprite {
            get
            {
                return _sprite;
            }
        }
        public override string description
        {
            get
            {
                return _description;
            }
        }
        public override int maxPileNum
        {
            get
            {
                return _max_pile_num;
            }
        }
        public override int currPileNum
        {
            get
            {
                return _cur_pile_num;
            }
            set
            {
                _cur_pile_num = value;
                if(belongGrid != null)
                    belongGrid.Refresh();
            }
        }

        //---------- 特有属性外层  ----------
        public PublicData.ConsumableType conType
        {
            get
            {
                return _con_type;
            }
        }

        //---------- 构造  ------------------
        public Consumable()
        {

        }
        public Consumable(int id)
        {
            _id = id;
            belongGrid = null;
            _name = PublicMethod.GenerateRdString(UnityEngine.Random.Range(2, 5));
            _item_type = PublicData.ItemType.consumable;
            _rarity = (PublicData.Rarity)UnityEngine.Random.Range(0, 5);
            _size = UnityEngine.Random.Range(0.1f, 0.5f);
            _sprite = Resources.Load<Sprite>("ZHXTemp/Consumable_img");
            _description = PublicMethod.GenerateRdString(UnityEngine.Random.Range(10, 20));
            _con_type = (PublicData.ConsumableType)UnityEngine.Random.Range(0, 3);
            _max_pile_num = 5;
            _cur_pile_num = 1;
            itemDiscard = PublicMethod.Useless;
            itemGain = PublicMethod.Useless;
            onUse = PublicMethod.Useless;

        }

        //---------- 重写方法  ---------------
        public override void OnDiscard() => itemDiscard();

        public override void OnGain() => itemGain();

        public override string ToString()
        {
            string result = string.Format("ID:{0}\n名字：{1}\n类型：{2}\n消耗品类型：{3}\n占用空间：{4}\n", id, name, itemType, conType, size);
            return result;
        }

        //----------  特有方法  ---------------
        public void OnUse() => onUse();

    }
}
