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
}
