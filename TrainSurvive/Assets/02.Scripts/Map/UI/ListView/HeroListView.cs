/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 3:26:42
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class HeroListView : BaseListView<Person>
    {
        protected override void OnItemView(ListViewItem item, Person data)
        {
            Utility.ForceGetComponentInChildren<PersonBaseItem>(item, "Hero").GetComponentInChildren<Text>().text = data.name;
            item.Tag = data;
        }
    }
}