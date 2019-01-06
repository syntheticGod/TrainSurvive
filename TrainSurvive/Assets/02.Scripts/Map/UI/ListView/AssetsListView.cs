/*
 * 描述：物品的ListView
 * 作者：项叶盛
 * 创建时间：2018/12/5 3:50:00
 * 版本：v0.1
 */

using WorldMap.Model;

using TTT.UI;
using TTT.Utility;
using UnityEngine.EventSystems;
using UnityEngine;

namespace WorldMap.UI
{
    public class AssetsListView : ResourceListViewBase, IDropHandler, IDropMessageReceiver
    {
        protected override void Awake()
        {
            base.Awake();
        }
        protected override ResourceItemBase GetResourceItemBase(ListViewItem item, ItemData data)
        {
            AssetsItemView view = ViewTool.ForceGetComponentInChildren<AssetsItemView>(item, "AssetsItem");
            view.DropMsgReceriver = this;
            view.SetItemData(data);
            return view;
        }
        public void OnDrop(PointerEventData eventData)
        {
            AssetsItemView item = eventData.pointerDrag.GetComponent<AssetsItemView>();
            if (item == null || item.IfEmpty())
                return;
            Debug.Log("ListView OnDrop ItemID:" + item.ItemID + " Number:" + item.Number);
            ItemData itemData = new ItemData(item.ItemID, item.Number);
            AddItem(itemData);
            World.getInstance().storage.AddItem(itemData);
            item.DropSucess();
            item.DropMsgReceriver?.DragOutCallBack(item);
        }
        public void DragOutCallBack(AssetsItemView item)
        {
            Debug.Log("ListView DragOutCallBack");
            ItemData dragOutItem = Datas[item.Index];
            if (RemoveData(dragOutItem))
            {
                World.getInstance().storage.RemoveItem(dragOutItem.ID, dragOutItem.Number);
            }
            else
            {
                Debug.Log("错误的索引 ：" + item.Index);
            }
        }
    }
}