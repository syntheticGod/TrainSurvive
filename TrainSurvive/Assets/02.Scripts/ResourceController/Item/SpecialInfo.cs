/*
 * 描述：特殊物品信息
 * 作者：项叶盛
 * 创建时间：2019/1/3 2:28:18
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Xml;

namespace TTT.Item
{
    public class SpecialInfo : ItemInfo
    {
        /// <summary>
        /// 初始价格
        /// </summary>
        public int Price { get; }
        public SpecialInfo(XmlNode node)
            : base(node)
        {
            Price = int.Parse(node.Attributes["price"].Value);
            Type = PublicData.ItemType.SpecialItem;
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