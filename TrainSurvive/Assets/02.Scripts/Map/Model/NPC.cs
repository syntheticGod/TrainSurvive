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
        }
        public  static NPC Random()
        {
            return new NPC();
        }
    }
}