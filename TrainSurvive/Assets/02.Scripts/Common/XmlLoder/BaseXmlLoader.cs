/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/27 23:13:32
 * 版本：v0.7
 */
using System.Xml;
using UnityEngine;

namespace TTT.Xml
{
    public abstract class BaseXmlLoader
    {
        protected BaseXmlLoader(string fliename)
        {
            string xmlString = Resources.Load("xml/"+ fliename).ToString();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            LoadFromXml(document);
        }
        protected abstract void LoadFromXml(XmlDocument document);
    }
}