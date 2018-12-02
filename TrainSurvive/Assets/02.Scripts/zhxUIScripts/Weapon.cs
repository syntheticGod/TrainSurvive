/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
/*
 * 描述：武器类
 * 作者：张皓翔
 * 创建时间：2018/10/31 18:43:11
 * 版本：v0.1
 * 
 * 
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace Assets._02.Scripts.zhxUIScripts
{
    [Serializable]
    public class Weapon : Item
    {
        [NonSerialized]
        private PublicData.VoidCallback onEquip;                   //获得物品时执行的回调函数
        [NonSerialized]
        private PublicData.VoidCallback onUnfix;                //丢弃物品时执行的回调函数
        [NonSerialized]
        private PublicData.VoidCallback weaponEffect;               //武器被动技能的回调函数（测试时先不使用）
        //  重写属性内层--------------------
        private int _id;
        private string _name;
        private PublicData.Rarity _rarity;
        private PublicData.ItemType _item_type;
        private float _size;
        [NonSerialized]
        private Sprite _sprite;
        private string _description;
        private int _max_pile_num;
        private int _cur_pile_num;

        //  特有属性内层------------------
        private PublicData.WeaponType _weapon_type;
        private float _range;
        private float _fac_atk;                                     //最终攻击力：测试时先用固定攻击力代替
        private float _fac_ats;
        private float _fac_spd;
        private float _fac_hrec;
        private float _fac_arec;
        private float _fac_sdmg;
        private float _mod_crc;
        private float _mod_crd;
        private float _mod_hit;
        //private float _base_atk;                                    //测试时先用随机数代替基础攻击决策

        //  特有属性外层  ------------------
        public float range
        {
            get
            {
                return _range;
            }
        }
        public float facAtk
        {
            get
            {
                return _fac_atk;
            }
        }
        public float facAts
        {
            get
            {
                return _fac_ats;
            }
        }
        public float facSpd
        {
            get
            {
                return _fac_spd;
            }
        }
        public float modCrc
        {
            get
            {
                return _mod_crc;
            }
        }
        public float modCrd
        {
            get
            {
                return _mod_crd;

            }
        }
        public float modHit
        {
            get
            {
                return _mod_hit;
            }
        }
        public float facArec
        {
            get
            {
                return _fac_arec;
            }
        }
        public float facHrec
        {
            get
            {
                return _fac_hrec;
            }
        }
        public float facSdmg
        {
            get
            {
                return _fac_sdmg;
            }
        }
        public PublicData.WeaponType weaponType
        {
            get
            {
                return _weapon_type;
            }
        }
        //  属性重写（外层）----------------
        public override int id {
            get
            {
                return _id;
            }
        }
        public override string name {
            get
            {
                return _name;
            }
        }
        public override PublicData.Rarity rarity {
            get
            {
                return _rarity; 
            }
        }
        public override PublicData.ItemType itemType
        {
            get
            {
                return _item_type;
            }
        }
        public override float size {
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
                {
                    belongGrid.Refresh();
                }
            }
        }

        //  构造  -------------------------
        public Weapon()
        {
        }

        public Weapon(int id)                                       //利用ID，在XML里查找相应的参数，然后初始化这个武器对象，连带的，                                                           
        {                                                           //武器被动技能可随机选择回调函数。
            string xmlString = Resources.Load("xml/items").ToString();
            string XPath = string.Format("./weapon[@id='{0:D3}']", id);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNode root = document.SelectSingleNode("items");
            XmlNode aimNode = root.SelectSingleNode(XPath);

            _id = id;
            belongGrid = null;
            _name = aimNode.Attributes["name"].Value;
            _item_type = PublicData.ItemType.Weapon;
            _weapon_type = (PublicData.WeaponType)int.Parse(aimNode.Attributes["type"].Value);
            _rarity = (PublicData.Rarity)int.Parse(aimNode.Attributes["rarity"].Value);
            _size = float.Parse(aimNode.Attributes["size"].Value);
            _max_pile_num = 1;
            _cur_pile_num = 1;
            _range = float.Parse(aimNode.Attributes["range"].Value);
            _fac_atk = float.Parse(aimNode.Attributes["atk"].Value);
            _fac_ats = float.Parse(aimNode.Attributes["ats"].Value);
            _fac_spd = float.Parse(aimNode.Attributes["spd"].Value);
            _mod_crc = float.Parse(aimNode.Attributes["crc"].Value);
            _mod_crd = float.Parse(aimNode.Attributes["crd"].Value);
            _mod_hit = float.Parse(aimNode.Attributes["hit"].Value);
            _fac_hrec = float.Parse(aimNode.Attributes["hrec"].Value);
            _fac_arec = float.Parse(aimNode.Attributes["arec"].Value);
            _fac_sdmg = float.Parse(aimNode.Attributes["sdmg"].Value);
            _description = aimNode.Attributes["description"].Value;
            _sprite = Resources.Load<Sprite>(aimNode.Attributes["spritepath"].Value);
            //_id = id;
            //belongGrid = null;
            //_name = PublicMethod.GenerateRdString(Random.Range(3, 6));
            //_rarity = (PublicData.Rarity)Random.Range(0, 5);
            //_weapon_type = (PublicData.WeaponType)Random.Range(0, 6);
            //_size = Random.Range(1.0f, 10.0f);
            //_sprite = Resources.Load<Sprite>("ItemSprite/Weapon_img");
            //_description = PublicMethod.GenerateRdString(Random.Range(10, 30));
            //_range = Random.Range(0.2f, 10.0f);
            //_fac_ats = Random.Range(0.2f, 2.5f);
            //_mod_crc = Random.Range(0.0f, 0.8f);
            //_mod_crd = Random.Range(0.0f, 2.0f);
            //_mod_hit = Random.Range(-1.0f, 1.0f);
            //_base_atk = Random.Range(5f, 200f);
            //_fac_atk = _base_atk + _base_atk * Random.Range(-0.2f, 0.2f);
            //_fac_spd = 1.0f;
            //_item_type = PublicData.ItemType.Weapon;
            //_max_pile_num = 1;
            //_cur_pile_num = 1;

            onEquip = PublicMethod.Useless;
            onUnfix = PublicMethod.Useless;
            weaponEffect = PublicMethod.Useless;
        }

        //重写方法  ------------------------------
        public override string ToString()
        {
            string result = string.Format("ID:{0}\n名称：{1}\n武器类型：{8}\n攻击力：{2}\n攻击范围：{3}\n攻击速度：{4}\n暴击率:{5}\n暴击伤害：{6}\n减伤：{7}\n", id, name, facAtk, range, facAts, modCrc, modCrd, modHit, weaponType.ToString());
            return result;
        }

        public override void OnGain()
        {
            
        }

        public override void OnDiscard()
        {

        }

        //特有方法  --------------------------------
        public void SpecialEffect() => weaponEffect();

        public void OnEquip() => onEquip();                         //当本武器被装备

        public void OnUnfix() => onUnfix();                         //当本武器被卸载

        public bool Update()                                        //本武器升级
        {
            //占位
            return true;
        }

        public void Decompose()                                     //本武器被分解
        {
            //占位
           
        }
    }
}
