/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:40
 * 版本：v0.1
 */
/*
 * 描述：
 * 作者：����
 * 创建时间：2018/10/30 22:25:28
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace 测试代码
{
    class Program
    {
       
        static void Main(string[] args)
        {
            XmlDocument xmldoc;
            XmlElement xmlelement;
            xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmldoc.AppendChild(xmldecl);
            xmlelement = xmldoc.CreateElement("", "Items", "");
            xmldoc.AppendChild(xmlelement);

            XmlElement weaponNode = xmldoc.CreateElement("weapon");
            XmlElement materialNode = xmldoc.CreateElement("material");
            XmlElement otherNode = xmldoc.CreateElement("other");

            weaponNode.SetAttribute("id", "000");
            weaponNode.SetAttribute("name", "隔壁老王的示范剑");
            weaponNode.SetAttribute("type", "1");
            weaponNode.SetAttribute("rarity", "4");
            weaponNode.SetAttribute("size", "13");
            weaponNode.SetAttribute("range", "1");
            weaponNode.SetAttribute("atk", "56");
            weaponNode.SetAttribute("ats", "1.2");
            weaponNode.SetAttribute("spd", "1");
            weaponNode.SetAttribute("crc", "0.15");
            weaponNode.SetAttribute("crd", "1.5");
            weaponNode.SetAttribute("hit", "1");
            weaponNode.SetAttribute("arec", "1");
            weaponNode.SetAttribute("sdmg", "1");
            weaponNode.SetAttribute("description", "传说中的隔壁老王在光临贵府后留下的宝剑");
            weaponNode.SetAttribute("spritepath", "ItemSprite/Weapon_img");

            materialNode.SetAttribute("id", "200");
            materialNode.SetAttribute("name", "隔壁老王遗落的裤头们");
            materialNode.SetAttribute("rarity", "3");
            materialNode.SetAttribute("size", "0.7");
            materialNode.SetAttribute("pilenum", "5");
            materialNode.SetAttribute("description", "传说中的隔壁老王匆忙离去后忘记带走的絮状织物");
            materialNode.SetAttribute("spritepath", "ItemSprite/Material_img");

            otherNode.SetAttribute("id", "700");
            otherNode.SetAttribute("name", "隔壁老王之魂");
            otherNode.SetAttribute("rarity", "2");
            otherNode.SetAttribute("pilenum", "1");
            otherNode.SetAttribute("description", "老王的怨念所化成的结晶体");
            otherNode.SetAttribute("spritepath", "ItemSprite/Other_img");

            xmlelement.AppendChild(weaponNode);
            xmlelement.AppendChild(materialNode);
            xmlelement.AppendChild(otherNode);
            xmldoc.Save("Items.xml");
            //xmldoc.Load("items.xml");
            //XmlNode node = xmldoc.SelectSingleNode("Items");

            //XmlNodeList nodeList = node.ChildNodes;

            //XmlNode temp = node.SelectSingleNode("./item[@id='1']");
            //Console.WriteLine(temp.Attributes["description"].Value);
            //Console.ReadKey();
        }
    }
}
