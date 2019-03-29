/*
 * 描述：观察者模式的观察目标
 *          观察目标身上存在两种监听者队列：
 *          各个状态的监听者（0~STATE.NUM-1）
 *          监听所有状态的监听者（STATE.NUM）
 * 作者：项叶盛
 * 创建时间：2018/11/22 11:32:41
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WorldMap
{
    public struct ObserverWithEcho
    {
        public Observer Observer;
        //回送码，当一个观察者观察多个对象时可以使用Echo来表示不同的对象。
        public int EchoCode;
        public override bool Equals(object obj)
        {
            return Observer.Equals(((ObserverWithEcho)obj).Observer);
        }
        public override int GetHashCode()
        {
            return Observer.GetHashCode();
        }
    }
    public interface Subject
    {
        /// <summary>
        /// 绑定监听所有状态的监听者
        /// </summary>
        /// <param name="obs">监听者</param>
        /// <returns></returns>
        bool Attach(Observer obs, int echo = 0);
        /// <summary>
        /// 移出监听所有状态的监听这
        /// </summary>
        /// <param name="obs">监听者</param>
        /// <returns></returns>
        bool Detach(Observer obs);
        /// <summary>
        /// 通知监听者状态发生变化了，同时夹带信息
        /// </summary>
        /// <param name="state">新状态</param>
        /// <param name="tag">信息</param>
        /// <returns></returns>
        bool Notify(int state, object tag = null);
    }
    [Serializable]
    public abstract class SubjectBase : Subject, ISerializable
    {
        [NonSerialized]
        private List<ObserverWithEcho> m_observers;
        protected SubjectBase()
        {
            m_observers = new List<ObserverWithEcho>();
        }
        public SubjectBase(SerializationInfo info, StreamingContext context) : this()
        { }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        { }
        public bool Attach(Observer obs, int echo = 0)
        {
            m_observers.Add(new ObserverWithEcho { Observer = obs, EchoCode = echo });
            return true;
        }
        public bool Detach(Observer obs)
        {
            //不关心EchoCode是多少，所以EchoCode=0
            return m_observers.Remove(new ObserverWithEcho { Observer = obs, EchoCode = 0 });
        }
        public bool Notify(int state, object tag = null)
        {
            foreach (var obs in m_observers)
                obs.Observer.ObsUpdate(state, obs.EchoCode, tag);
            return true;
        }
    }
}

