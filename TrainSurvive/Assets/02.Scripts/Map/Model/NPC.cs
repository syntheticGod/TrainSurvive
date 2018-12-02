/*
 * 描述：对外部Person类的封装，用于WorldMap内部。
 * 作者：项叶盛
 * 创建时间：2018/11/21 23:06:19
 * 版本：v0.1
 */
using System;

namespace WorldMap.Model
{
    [Serializable]
    public class NPC
    {
        public Person PersonInfo { private set; get; }
        private NPC()
        {
            PersonInfo = Person.CreatePerson();
            bool sex = StaticResource.RandomInt(2) != 0;
            PersonInfo.name = StaticResource.RandomNPCName(sex);
            PersonInfo.vitality = StaticResource.RandomInt(10) + 1;
            PersonInfo.strength = StaticResource.RandomInt(10) + 1;
            PersonInfo.agile = StaticResource.RandomInt(10) + 1;
            PersonInfo.technique = StaticResource.RandomInt(10) + 1;
            PersonInfo.intelligence = StaticResource.RandomInt(10) + 1;
        }
        public static NPC Random()
        {
            return new NPC();
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