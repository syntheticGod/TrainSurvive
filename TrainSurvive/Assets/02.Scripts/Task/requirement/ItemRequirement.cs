/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 21:45:20
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._02.Scripts.zhxUIScripts;
using TTT.Resource;
using TTT.Item;

[System.Serializable]
public class ItemRequirement : TaskRequirement
{
    private ItemRequirement() { }

    public ItemRequirement(int itemId,int needNums)
    {
        _itemId = itemId;
        _needNums = needNums;
        string itemName = StaticResource.GetItemInfoByID<ItemInfo>(_itemId).Name;
        condition = "需要物品：" + itemName + "*"+needNums; ;
        description = "我需要" + itemName + needNums+"个，你带来了吗？";//以后也可以只在npc那只用这里的数据，其他话在npc数据里补充
        isfinish = false;
    }

    public int _itemId;
    /// <summary>
    /// 需要的数量(只读），物品也需要一次交完，否则需要增加已交付数量的字段
    /// </summary>
    public int _needNums;

    public override bool achieveGoal()
    {
        if (isfinish)
            return true;

        string itemName = StaticResource.GetItemInfoByID<ItemInfo>(_itemId).Name;
        Storage bag = World.getInstance().storage;
        isfinish=bag.RemoveItem(_itemId, _needNums);
        if (isfinish)
        {
            condition = "已交付：" + itemName + "*" + _needNums;
        }
        return isfinish;
        throw new NotImplementedException();
    }

    public override void conditionChange(int numOrId)
    {
        throw new NotImplementedException();
    }
}
