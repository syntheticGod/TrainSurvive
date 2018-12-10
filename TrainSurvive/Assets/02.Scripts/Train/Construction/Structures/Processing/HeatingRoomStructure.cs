/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 13:17:08
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class HeatingRoomStructure : Structure {

    public HeatingRoomStructure() : base() { }
    protected HeatingRoomStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "加热室",
        Description = "加热室",
        WorkAll = 0,
        BuildCosts = new ItemData[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 3,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

}
