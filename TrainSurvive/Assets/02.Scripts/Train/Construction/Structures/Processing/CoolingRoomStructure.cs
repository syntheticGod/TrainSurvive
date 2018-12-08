/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 15:07:31
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class CoolingRoomStructure : Structure {

    public CoolingRoomStructure() : base() { }
    protected CoolingRoomStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "冷藏室",
        Description = "冷藏室",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 3,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

}
