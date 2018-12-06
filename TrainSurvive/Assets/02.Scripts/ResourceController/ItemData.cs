/*
 * 描述：一个简单的封装了物品ID和数量的类，可以装到自己的List<ItemData>里面对容器内的物品进行序列化保存
 * 作者：张皓翔
 * 创建时间：2018/12/4 17:26:10
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public int num;
    public ItemData(int _id, int _num)
    {
        id = _id;
        num = _num;
    }
    /// <summary>
     /// 第一次获取时，动态加载。
     /// </summary>
    [NonSerialized]
    private Item m_item;
    public Item item
    {
        get
        {
            if (m_item == null)
            {
                Item[] items = PublicMethod.GenerateItem(id, num);
                m_item = items[0];
                if (items.Length > 1)
                    num = m_item.currPileNum;
            }
            return m_item;
        }
    }
}

