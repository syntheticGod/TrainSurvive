/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/21 19:48:43
 * 版本：v0.1
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
namespace Assets._02.Scripts.zhxUIScripts
{
    class SpecialItem : Item
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
                if (belongGrid != null)
                    belongGrid.Refresh();
            }
        }

        //  构造------------------------------
        public SpecialItem()
        {

        }
        public SpecialItem(int id)
        {
            string xmlString = Resources.Load("xml/items").ToString();
            string XPath = string.Format("./special[@id='{0:D3}']", id);
            Debug.Log(XPath);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            XmlNode root = document.SelectSingleNode("items");
            XmlNode aimNode = root.SelectSingleNode(XPath);

            _id = id;
            belongGrid = null;
            _name = aimNode.Attributes["name"].Value;
            _item_type = PublicData.ItemType.SpecialItem;
            _rarity = (PublicData.Rarity)int.Parse(aimNode.Attributes["rarity"].Value);
            _size = 0;
            _max_pile_num = int.Parse(aimNode.Attributes["maxPileNum"].Value);
            _cur_pile_num = 1;
            _description = aimNode.Attributes["description"].Value;
            _sprite = Resources.Load<Sprite>(aimNode.Attributes["spritepath"].Value);
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
