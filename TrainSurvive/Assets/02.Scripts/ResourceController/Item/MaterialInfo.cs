/*
 * 描述：材料物品信息
 * 作者：项叶盛
 * 创建时间：2019/1/3 2:28:18
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Xml;

namespace TTT.Item
{
    public class MaterialInfo : ItemInfo
    {
        private const int BASE_PRICE = 10;
        /// <summary>
        /// 转化为食物的系数
        /// </summary>
        public float FacFood { get; private set; }
        /// <summary>
        /// 转化为能源的系数
        /// </summary>
        public float FacEnergy { get; private set; }
        /// <summary>
        /// 价格系数
        /// </summary>
        public float FacPrice { get; private set; }
        /// <summary>
        /// 是否是食材
        /// </summary>
        public bool Can2Food { get { return UnityEngine.Mathf.Approximately(FacFood, 0); } }
        /// <summary>
        /// 是否是能源材料
        /// </summary>
        public bool Can2Energy { get { return UnityEngine.Mathf.Approximately(FacEnergy, 0); } }
        public override int GetOriginPrice()
        {
            return UnityEngine.Mathf.RoundToInt(BASE_PRICE * FacPrice);
        }

        public override int GetSellPrice()
        {
            return UnityEngine.Mathf.RoundToInt(BASE_PRICE * FacPrice * SellRatio);
        }
        public MaterialInfo(XmlNode node)
            : base(node)
        {
            Type = PublicData.ItemType.Material;
            FacFood = float.Parse(node.Attributes["facFood"].Value);
            FacEnergy = float.Parse(node.Attributes["facEnergy"].Value);
            FacPrice = float.Parse(node.Attributes["facPrice"].Value);
        }

    }
}