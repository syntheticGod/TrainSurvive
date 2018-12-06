/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 3:50:00
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using WorldMap.Model;

namespace WorldMap.UI
{
    public class TeamPackListView : MergableListView<Good>
    {
        protected override void OnItemView(ListViewItem item, Good data)
        {
            TeamPackItem view = Utility.ForceGetComponentInChildren<TeamPackItem>(item, "PackItem");
            view.SetNumber(data.Number);
            view.SetTarget(data.item);
            view.SetMarkLevel((int)data.item.rarity);
        }
    }
}