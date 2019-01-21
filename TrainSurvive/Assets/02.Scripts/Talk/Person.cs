/*
 * 描述：
 * 作者：李巡
 * 创建时间：2019/1/21 15:34:52
 * 版本：v0.7
 */
using System;
using Story.MyTools;

namespace Story.Faker{
    
    public static class FakeValue{
        /// <summary>
        /// 虚构的魅力值，魅力值越高，可询问的属性越多
        /// </summary>
        public static int charm = 2;

    }
    
    //随机事件池
    public class RandomEventpool{
        public static RandomEventpool instance = new RandomEventpool();

        private int eventNum = 3;

        public bool existEvent(){
            return eventNum > 0;
        }
        
    }
}