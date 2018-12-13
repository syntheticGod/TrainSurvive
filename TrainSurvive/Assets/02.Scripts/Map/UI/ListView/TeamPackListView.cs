/*
 * 描述：小队背包中使用到的物品的容器（ListView）
 * 作者：项叶盛
 * 创建时间：2018/12/5 3:50:00
 * 版本：v0.1
 */
using WorldMap.Model;
using TTT.Utility;

namespace WorldMap.UI
{
    public class TeamPackListView : ResourceListViewBase
    {
        protected override void OnItemView(ListViewItem item, Good data)
        {
            TeamPackItem view = ViewTool.ForceGetComponentInChildren<TeamPackItem>(item, "PackItem");
            view.SetNumber(data.Number);
            view.SetTarget(data.item);
            view.SetMarkLevel((int)data.item.rarity);
        }
    }
}