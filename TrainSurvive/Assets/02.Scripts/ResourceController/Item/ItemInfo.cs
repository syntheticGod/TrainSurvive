/*
 * 描述：物品的静态数据结构
 *          从items.xml文件中读取到的在整个程序中不会改变的信息。
 * 作者：项叶盛
 * 创建时间：2019/1/3 1:47:58
 * 版本：v0.7
 */

using Assets._02.Scripts.zhxUIScripts;
using System.Xml;
using TTT.Resource;
using UnityEngine;

namespace TTT.Item
{
    public class ItemInfo
    {
        /// <summary>
        /// 物品唯一ID
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// 物品名字
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// //物品描述
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// 物品类别
        /// </summary>
        public PublicData.ItemType Type { get; protected set; }
        /// <summary>
        /// 物品稀有度
        /// </summary>
        public PublicData.Rarity Rarity { get; }
        /// <summary>
        /// 初始价格
        /// </summary>
        public int Price { get; }
        /// <summary>
        /// 售卖价格比例（price*sellRatio）取值范围是[0,1]之间的浮点
        /// </summary>
        public float SellRatio { get; }
        /// <summary>
        /// 物品占用空间体积
        /// </summary>
        public float Size { get; }
        /// <summary>
        /// 最大堆叠数
        /// </summary>
        public int MaxPileNum { get; }
        /// <summary>
        /// 120 x 120 像素
        /// </summary>
        public Sprite BigSprite { get; }
        /// <summary>
        /// 60 x 60 像素
        /// </summary>
        public Sprite SmallSprite { get; }
        /// <summary>
        /// 物品信息只能从XML文件中读取，一旦读取就不能被修改。
        /// </summary>
        /// <param name="node">Xml单个物体的节点</param>
        public ItemInfo(XmlNode node)
        {
            Type = PublicData.ItemType.NONE;
            ID = int.Parse(node.Attributes["id"].Value);
            Name = node.Attributes["name"].Value;
            Description = node.Attributes["description"].Value;
            Rarity = (PublicData.Rarity)int.Parse(node.Attributes["rarity"].Value);
            Price = int.Parse(node.Attributes["price"].Value);
            SellRatio = float.Parse(node.Attributes["sellRatio"].Value);
            Size = float.Parse(node.Attributes["size"].Value);
            MaxPileNum = int.Parse(node.Attributes["maxPileNum"].Value);
            BigSprite = StaticResource.GetSprite(ESprite.DEVELOPING_BIG);
            SmallSprite = StaticResource.GetSprite(ESprite.DEVELOPING_SMALL);
        }
    }

}