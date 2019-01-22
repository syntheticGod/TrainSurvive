/*
 * 描述：相对AssetsItemView添加如下内容：
              1、可拖拽
 * 作者：项叶盛
 * 创建时间：2019/1/21 19:24:26
 * 版本：v0.7
 */
using TTT.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TTT.UI
{
    public class DragableAssetsItemView : AssetsItemView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 由拖拽产生的拖拽物体
        /// </summary>
        private AssetsItemView dragingItemView;
        /// <summary>
        /// 物体被拖出后，有其他物体接收后的回调函数
        /// </summary>
        public IDropMessageReceiver DropMsgReceiver { get; set; }
        public int Index { get; set; }
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            //Debug.Log("OnBeginDrag id:" + ItemID + " number:" + Number);
            if (!IfEmpty())
            {
                ShowDragingView();
            }
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (dragingItemView != null)
                dragingItemView.GetComponent<RectTransform>().position = Input.mousePosition;
            //Debug.Log("OnDrag id:" + ItemID + " number:" + Number + " " + dragingItem.name);
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!IfEmpty())
            {
                DisappearDragingView();
            }
        }
        /// <summary>
        /// 当物品被别的任何其他对象接收时，其他物体会回调该函数
        /// </summary>
        public void CallBackDropSucess()
        {
            DisappearDragingView();
            DropMsgReceiver?.CallBackDragOut(this);
        }
        /// <summary>
        /// 显示拖拽物体
        /// </summary>
        private void ShowDragingView()
        {
            Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            dragingItemView = ViewTool.ForceGetComponentInChildren<AssetsItemView>(canvas, "TempDragObject");
            dragingItemView.SetItemData(ItemID, Number);
            CompTool.ForceGetComponent<CanvasGroup>(dragingItemView).blocksRaycasts = false;
            RectTransform rect = dragingItemView.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.offsetMin = new Vector2(-50f, -50f);
            rect.offsetMax = new Vector2(50f, 50f);
            rect.position = Input.mousePosition;
            dragingItemView.gameObject.SetActive(true);
        }
        /// <summary>
        /// 使拖拽物体消失
        /// </summary>
        private void DisappearDragingView()
        {
            if (dragingItemView.gameObject.activeSelf)
            {
                dragingItemView.gameObject.SetActive(false);
            }
        }
    }
}