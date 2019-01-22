/*
 * 描述：物品列表
            相对ResourceListView添加如下功能：
            1、允许别的物体拖入
 * 作者：项叶盛
 * 创建时间：2018/12/5 3:50:00
 * 版本：v0.1
 */

using WorldMap.Model;

using TTT.UI;
using TTT.UI.ListView;
using TTT.Utility;
using UnityEngine.EventSystems;
using UnityEngine;

namespace WorldMap.UI
{
    public class DragableResourceLV : ResourceListView, IDropHandler, IDropMessageReceiver
    {
        protected override void Awake()
        {
            base.Awake();
        }
        protected override ResourceItemBase GetView(ListViewItem item, ItemData data, int index)
        {
            DragableAssetsItemView view = ViewTool.ForceGetComponentInChildren<DragableAssetsItemView>(item, "AssetsItem");
            view.DropMsgReceiver = this;
            view.Index = index;
            view.SetItemData(data);
            return view;
        }
        public void OnDrop(PointerEventData eventData)
        {
            DragableAssetsItemView item = eventData.pointerDrag.GetComponent<DragableAssetsItemView>();
            if (item == null || item.IfEmpty())
                return;
            Debug.Log("ListView OnDrop ItemID:" + item.ItemID + " Number:" + item.Number);
            ItemData itemData = new ItemData(item.ItemID, item.Number);
            AddItem(itemData);
            World.getInstance().storage.AddItem(itemData);
            item.CallBackDropSucess();
        }
        public void CallBackDragOut(DragableAssetsItemView item)
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