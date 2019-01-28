/*
 * 描述：
 * 作者：NONE
 * 创建时间：2019/1/28 16:30:26
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTT.Resource;
using TTT.Item;

[System.Serializable]
public class HasItemRequirement : TaskRequirement
{
    private HasItemRequirement() { }

    public HasItemRequirement(int itemId, int needNums)
    {
        _itemId = itemId;
        _needNums = needNums;
        string itemName = StaticResource.GetItemInfoByID<ItemInfo>(_itemId).Name;
        condition = "需要物品：" + itemName + "*" + needNums; ;
        description = "你有" + needNums + "个"  +itemName+ "吗？";//以后也可以只在npc那只用这里的数据，其他话在npc数据里补充
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
        isfinish = bag.GetNumberByID(_itemId)>= _needNums;
        if (isfinish)
        {
            condition = "已持有：" + itemName + "*" + _needNums;
        }
        return isfinish;
        throw new NotImplementedException();
    }

    public override void conditionChange(int numOrId)
    {
        throw new NotImplementedException();
    }
}
