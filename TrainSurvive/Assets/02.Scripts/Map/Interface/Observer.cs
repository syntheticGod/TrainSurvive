/*
 * 描述：观察者模式的观察者
 * 作者：项叶盛
 * 创建时间：2018/11/22 11:32:41
 * 版本：v0.1
 */

namespace WorldMap
{
    public interface Observer
    {
        void ObsUpdate(int state, int echo, object tag = null);
    }
}

