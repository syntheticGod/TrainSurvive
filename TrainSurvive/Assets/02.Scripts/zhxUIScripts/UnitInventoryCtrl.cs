/*
 * 描述：专门为只能嵌入一个物品的容器设计的附加类
 * 作者：张皓翔
 * 创建时间：2018/11/6 19:53:41
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets._02.Scripts.zhxUIScripts;
using System;
using WorldMap.UI;
using TTT.Resource;
using TTT.Utility;

public class UnitInventoryCtrl : AssetsItemView, IDropHandler, IDropMessageReceiver
{
    //public GameObject grid;        //包含的物品格实例
    //private ItemGridCtrl gridCtrl;
    public bool isEquipmentGrid = false;
    /// <summary>
    /// 准入判断函数
    /// </summary>
    public PublicData.Charge ChargeIn;
    /// <summary>
    /// 该物品允许被拖入的回调处理函数
    /// </summary>
    public Action<int, int> OnItemIn { get; set; }
    public GameObject Prefab;
    public bool IfBeDragedOut { get; set; }
    protected override void CreateModel()
    {
        base.CreateModel();
        DropMsgReceriver = this;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!IfEmpty())
        {
            base.OnBeginDrag(eventData);
            IfBeDragedOut = true;
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!IfEmpty())
        {
            base.OnEndDrag(eventData);
            IfBeDragedOut = false;
        }
    }
    public void OnDrop(PointerEventData eventData)                                    //仅在本空间为空的情况下触发
    {
        AssetsItemView item = eventData.pointerDrag.GetComponent<AssetsItemView>();
        //如果ChargeIn为空，则表示不需要判断是否允许拖入
        if (item == null 
            || (ChargeIn?.Invoke(item.ItemID, item.Number) ?? false))
            return;
        IfBeDragedOut = false;
        SetItemData(item.ItemID, item.Number);
        item.DropSucess();
        item.DropMsgReceriver?.DragOutCallBack(item);
        Debug.Log("ItemID:" + item.ItemID + " Number:" + item.Number);
        OnItemIn?.Invoke(item.ItemID, item.Number);
    }
    public void GeneratorItem(int id, int num = 1)
    {
        if(!IfEmpty())
            SetItemData(id, num);
    }
    public void DragOutCallBack(AssetsItemView item)
    {
        Debug.Log("Unit DragOutCallBack");
        Clear();
    }
}
