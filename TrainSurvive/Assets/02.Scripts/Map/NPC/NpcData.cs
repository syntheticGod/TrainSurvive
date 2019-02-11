/*
 * 描述：对外部Person类的封装，用于WorldMap内部。
 * 作者：项叶盛
 * 创建时间：2018/11/21 23:06:19
 * 版本：v0.1
 */
using System;
using TTT.Utility;
using TTT.Xml;

namespace WorldMap.Model
{
    [Serializable]
    public class NpcData
    {
        public int ID { get; private set; }
        public NpcInfo Info { get { return NpcInfoLoader.Instance.Find(ID); } }
        public NpcData(int id)
        {
            ID = id;
        }
        public override string ToString()
        {
            return "ID：" + ID + "；名字：" + Info.Name;
        }
    }
}