/*
 * 描述：对外部Person类的封装，用于WorldMap内部。
 * 作者：项叶盛
 * 创建时间：2018/11/21 23:06:19
 * 版本：v0.1
 */
using System;
using TTT.Utility;

namespace WorldMap.Model
{
    [Serializable]
    public class NpcData
    {

        public Person PersonInfo { private set; get; }
        public string Name { get { return PersonInfo.name; } }
        public int Strength { get { return PersonInfo.strength; } }
        public int ID { get; private set; }
        private NpcData()
        {
            PersonInfo = Person.RandomPerson();
            ID = MathTool.GenerateID();
        }
        public static NpcData Random()
        {
            return new NpcData();
        }
        public string Info
        {
            get
            {
                return "名字：" + PersonInfo.name + "\n" +
                    "力量：" + PersonInfo.strength + 
                    " 体力：" + PersonInfo.vitality + 
                    " 敏捷：" + PersonInfo.agile + 
                    "\n技巧：" + PersonInfo.technique + 
                    " 智力：" + PersonInfo.intelligence;
            }
        }
    }
}