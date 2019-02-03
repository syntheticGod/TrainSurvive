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
using TTT.UI;
using TTT.Resource;
using TTT.Utility;

public class UnitInventoryCtrl : DragableAndDropableAssetsItemView
{
    //public GameObject grid;        //包含的物品格实例
    //private ItemGridCtrl gridCtrl;
    //public bool isEquipmentGrid = false;
    public void GeneratorItem(int id, int num = 1)
    {
        if(!IfEmpty())
            SetItemData(id, num);
    }

    public override void OnDrop(PointerEventData eventData) {
        if (enabled) {
            base.OnDrop(eventData);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        if (enabled) {
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData) {
        if (enabled) {
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData) {
        if (enabled) {
            base.OnEndDrag(eventData);
        }
    }
}
