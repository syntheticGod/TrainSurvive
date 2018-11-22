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
    public interface Subject
    {
        List<Observer>[] Observers { get; }
        int MaxState { get; }
    }
    namespace Model
    {
        public static class SubjectExtension
        {
            /// <summary>
            /// 绑定监听所有状态的监听者
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="subject"></param>
            /// <param name="obs"></param>
            /// <returns></returns>
            public static bool Attach<T>(this T subject, Observer obs) where T : Subject
            {
                return Attach(subject, obs, subject.MaxState);
            }
            /// <summary>
            /// 绑定指定状态的监听者
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="subject"></param>
            /// <param name="obs"></param>
            /// <param name="state">state==STATE.NUM时，表示监听所有状态</param>
            /// <returns></returns>
            public static bool Attach<T>(this T subject, Observer obs, int state) where T : Subject
            {
                if (state > subject.MaxState)
                    return false;
                subject.Observers[state].Add(obs);
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
                return Detach(subject, obs, subject.MaxState);
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
                if (state > subject.MaxState) return false;
                return subject.Observers[state].Remove(obs);
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
                if (state >= subject.MaxState) return false;
                //通知相应监听状态的监听者
                foreach (var obs in subject.Observers[state])
                {
                    obs.ObserverUpdate(state);
                }
                //通知监听所有状态的监听者
                foreach (var obs in subject.Observers[subject.MaxState])
                {
                    obs.ObserverUpdate(state);
                }
                return true;
            }
        }
    }

}

