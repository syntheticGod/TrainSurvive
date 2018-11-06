/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
/*
 * 描述：材料类
 * 作者：张皓翔
 * 创建时间：2018/10/31 18:59:54
 * 版本：v0.1
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace Assets._02.Scripts.zhxUIScripts
{
    public class Material : Item
    {
        //  重写属性内层----------------------
        private int _id;
        private string _name;
        private PublicData.ItemType _item_type;
        private PublicData.Rarity _rarity;
        private float _size;
        private string _description;
        private Sprite _sprite;
        private int _max_pile_num;
        private int _cur_pile_num;

        //  重写属性外层----------------------
        public override PublicData.ItemType itemType
        {
            get
            {
                return _item_type;
            }
        }
        public override int id
        {
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
        public override Sprite sprite
        {
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

        //  构造------------------------------
        public Material()
        {

        }
        public Material(int id)
        {
            _id = id;
            belongGrid = null;
            _name = PublicMethod.GenerateRdString(Random.Range(2, 6));
            _item_type = PublicData.ItemType.material;
            _rarity = (PublicData.Rarity)Random.Range(0, 5);
            _size = Random.Range(2.0f, 5.0f);
            _description = PublicMethod.GenerateRdString(Random.Range(20, 30));
            _sprite = Resources.Load<Sprite>("ZHXTemp/Material_img");
            _max_pile_num = 5;
            _cur_pile_num = 1;
        }

        //  重写方法  --------------------------
        public override void OnDiscard()
        {

        }

        public override void OnGain()
        {

        }

        public override string ToString()
        {
            string result = string.Format("ID:{0}\nname:{1}\n种类:{2}\n", id, name, itemType);
            return result;
        }
    }
}
