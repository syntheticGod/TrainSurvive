/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 3:50:00
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using Assets._02.Scripts.zhxUIScripts;

namespace WorldMap.UI
{
    public class ItemListView : ListViewController<Item>
    {
        protected override void OnItemView(ListViewItem item, Item data)
        {
            TeamPackItem view = Utility.ForceGetComponentInChildren<TeamPackItem>(item, "PackItem");
            view.SetNumber(data.currPileNum);
            view.SetTarget(data);
            view.SetMarkLevel((int)data.rarity);
        }
    }
}