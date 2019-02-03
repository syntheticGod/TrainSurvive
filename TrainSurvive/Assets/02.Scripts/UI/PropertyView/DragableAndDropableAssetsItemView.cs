/*
 * 描述：相对DragableAssetsItemView添加如下内容：
                1、接受别的物品拖到该物品上的事件
 * 作者：项叶盛
 * 创建时间：2019/1/21 19:24:26
 * 版本：v0.7
 */
using UnityEngine;
using UnityEngine.EventSystems;

using System;

using Assets._02.Scripts.zhxUIScripts;

namespace TTT.UI
{
    public class DragableAndDropableAssetsItemView : DragableAssetsItemView, IDropHandler
    {
        /// <summary>
        /// 准入判断函数
        /// </summary>
        public PublicData.Charge OnChargeIn { get; set; }
        /// <summary>
        /// 该物品允许被拖入的回调处理函数
        /// </summary>
        public Action<int, int> OnItemDropIn { get; set; }
        public virtual void OnDrop(PointerEventData eventData)
        {
            DragableAssetsItemView item = eventData.pointerDrag.GetComponent<DragableAssetsItemView>();
            //如果ChargeIn为空，则表示不需要判断是否允许拖入
            if (item == null
                || (OnChargeIn?.Invoke(this, item.ItemID, item.Number) ?? false))
                return;
            SetItemData(item.ItemID, item.Number);
            item.CallBackDropSucess();
            Debug.Log("ItemID:" + item.ItemID + " Number:" + item.Number);
            OnItemDropIn?.Invoke(item.ItemID, item.Number);
        }
    }
}