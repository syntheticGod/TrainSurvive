/*
 * 描述：武器信息
 * 作者：项叶盛
 * 创建时间：2019/1/3 2:28:18
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Xml;
using TTT.Resource;

namespace TTT.Item
{
    public class WeaponInfo : ItemInfo
    {

        /// <summary>
        /// 初始价格
        /// </summary>
        public int Price { get; }
        /// <summary>
        /// 武器类型
        /// </summary>
        public PublicData.WeaponType WeaponType { get; }
        
        public WeaponInfo(XmlNode node)
            : base(node)
        {
            Type = PublicData.ItemType.Weapon;
            Price = int.Parse(node.Attributes["price"].Value);
            WeaponType = (PublicData.WeaponType)int.Parse(node.Attributes["type"].Value);
        }

        public override int GetOriginPrice()
        {
            return Price;
        }

        public override int GetSellPrice()
        {
            return UnityEngine.Mathf.RoundToInt(Price * SellRatio);
        }
    }
}