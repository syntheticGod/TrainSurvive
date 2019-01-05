/*
 * 描述：材料物品信息
 * 作者：项叶盛
 * 创建时间：2019/1/3 2:28:18
 * 版本：v0.7
 */
using System.Xml;

namespace TTT.Item
{
    public class MaterialInfo : ItemInfo
    {
        public MaterialInfo(XmlNode node)
            : base(node)
        {
        }
    }
}