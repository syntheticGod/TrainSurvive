/*
 * 描述：物品的ListView
 * 作者：项叶盛
 * 创建时间：2018/12/5 3:50:00
 * 版本：v0.1
 */

using WorldMap.Model;

using TTT.UI;
using TTT.Utility;

namespace WorldMap.UI
{
    public class AssetsListView : ResourceListViewBase
    {
        protected override ResourceItemBase GetResourceItemBase(ListViewItem item, ItemData data)
        {
            AssetsItemView view = ViewTool.ForceGetComponentInChildren<AssetsItemView>(item, "AssetsItem");
            view.SetItemData(data);
            return view;
        }
    }
}