/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/6 15:36:53
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TTT.UI
{
    public abstract class MergableListView<M> : BaseListView<M>
        where M : Mergable
    {
        /// <summary>
        /// 当存在一样的对象时，会将不同的对象融合在一起，修改数量
        /// </summary>
        /// <param name="data"></param>
        public override void AddItem(M data)
        {
            int i;
            for(i = 0; i < Datas.Count; ++i)
            {
                if(data.Equals(Datas[i]) && (Datas[i].Number() + data.Number())<= Datas[i].MaxNumber())
                {
                    Datas[i].Merge(data);
                    break;
                }
            }
            if(i == Datas.Count)
            {
                Datas.Add(data);
            }
            Refresh();
        }
        /// <summary>
        /// 当存在一样的对象时，修改数量
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool RemoveData(M data)
        {
            int i;
            for (i = 0; i < Datas.Count; ++i)
            {
                if (data.Equals(Datas[i]))
                {
                    if ((Datas[i].Number() - data.Number()) > 0)
                        Datas[i].Demerge(data);
                    else if (Datas[i].Number() - data.Number() == 0)
                        Datas.Remove(data);
                    else
                        continue;
                    break;
                }
            }
            if (i == Datas.Count)
                return false;
            Refresh();
            return true;
        }
        /// <summary>
        /// 刷新ListView，删去数量小于等于0的。
        /// </summary>
        protected override void RefreshData()
        {
            for (int i = 0; i < Datas.Count; i++)
            {
                if (Datas[i].Number() <= 0)
                {
                    Datas.RemoveAt(i--);
                    continue;
                }
                if (onItemFilter == null || !onItemFilter(Datas[i]))
                    OnItemView(AppendItem(), Datas[i], i);
            }
        }
    }
    public interface Mergable
    {
        int Number();
        int MaxNumber();
        void Merge(Mergable other);
        void Demerge(Mergable other);
    }
}