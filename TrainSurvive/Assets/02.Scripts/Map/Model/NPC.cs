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
        private Person personInfo;
        private NPC()
        {
            personInfo = Person.CreatePerson();
            bool sex = StaticResource.RandomInt(2) != 0;
            personInfo.name = StaticResource.RandomNPCName(sex);
            personInfo.vitality = StaticResource.RandomInt(10) + 1;
            personInfo.strength = StaticResource.RandomInt(10) + 1;
            personInfo.agile = StaticResource.RandomInt(10) + 1;
            personInfo.technique = StaticResource.RandomInt(10) + 1;
            personInfo.intellgence = StaticResource.RandomInt(10) + 1;
        }
        public static NPC Random()
        {
            return new NPC();
        }
        public string Info
        {
            get
            {
                return "名字：" + personInfo.name + "\n" +
                    "力量：" + personInfo.strength + 
                    " 体力：" + personInfo.vitality + 
                    " 敏捷：" + personInfo.agile + 
                    "\n技巧：" + personInfo.technique + 
                    " 智力：" + personInfo.intellgence;
            }
        }
    }
}