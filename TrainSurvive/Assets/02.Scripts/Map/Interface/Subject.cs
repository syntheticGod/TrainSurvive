/*
 * 描述：观察者模式的观察目标
 *          观察目标身上存在两种监听者队列：
 *          各个状态的监听者（0~STATE.NUM-1）
 *          监听所有状态的监听者（STATE.NUM）
 * 作者：项叶盛
 * 创建时间：2018/11/22 11:32:41
 * 版本：v0.1
 */
using System.Collections.Generic;

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
        List<ObserverWithEcho>[] Observers { get; }
        int MaxState();
    }
    public static class SubjectExtension
    {
        /// <summary>
        /// 绑定监听所有状态的监听者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="obs"></param>
        /// <returns></returns>
        public static bool Attach<T>(this T subject, Observer obs, int echo = 0) where T : Subject
        {
            return Attach(subject, obs, subject.MaxState(), echo);
        }
        /// <summary>
        /// 绑定指定状态的监听者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="obs"></param>
        /// <param name="state">state==STATE.NUM时，表示监听所有状态</param>
        /// <returns></returns>
        public static bool Attach<T>(this T subject, Observer obs, int state, int echo) where T : Subject
        {
            if (state > subject.MaxState())
                return false;
            subject.Observers[state].Add(new ObserverWithEcho { Observer = obs, EchoCode = echo });
            return true;
        }
        /// <summary>
        /// 移出监听所有状态的监听这
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="obs">监听者</param>
        /// <returns></returns>
        public static bool Detach<T>(this T subject, Observer obs) where T : Subject
        {
            return Detach(subject, obs, subject.MaxState());
        }
        /// <summary>
        /// 移出监听指定状态的监听者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="obs">监听者</param>
        /// <param name="state">指定状态</param>
        /// <returns></returns>
        public static bool Detach<T>(this T subject, Observer obs, int state) where T : Subject
        {
            if (state > subject.MaxState()) return false;
            //不关心EchoCode是多少，所以EchoCode=0
            return subject.Observers[state].Remove(new ObserverWithEcho { Observer = obs, EchoCode = 0 });
        }
        /// <summary>
        /// 通知监听者状态发生变化了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <param name="state">新状态</param>
        /// <returns></returns>
        public static bool Notify<T>(this T subject, int state) where T : Subject
        {
            if (state >= subject.MaxState()) return false;
            //通知相应监听状态的监听者
            foreach (var obs in subject.Observers[state])
            {
                obs.Observer.ObserverUpdate(state, obs.EchoCode);
            }
            //通知监听所有状态的监听者
            foreach (var obs in subject.Observers[subject.MaxState()])
            {
                obs.Observer.ObserverUpdate(state, obs.EchoCode);
            }
            return true;
        }
    }
    public abstract class SubjectBase : Subject
    {
        public List<ObserverWithEcho>[] observers;
        public List<ObserverWithEcho>[] Observers
        {
            get
            {
                return observers;
            }
        }
        protected SubjectBase()
        {
            //最后一个存监听所有状态
            observers = new List<ObserverWithEcho>[MaxState()+1];
            for (int i = 0; i < observers.Length; ++i)
            {
                observers[i] = new List<ObserverWithEcho>();
            }
        }
        //状态的最大值
        public abstract int MaxState();
    }
}

