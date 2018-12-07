/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 12:25:34
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class WoodTableStructure : Structure {

    public WoodTableStructure() : base() { }
    protected WoodTableStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "木桌",
        Description = "可摆放小物品",
        LayerName = "Table",
        WorkAll = 0.01f,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 2,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;
    
}
