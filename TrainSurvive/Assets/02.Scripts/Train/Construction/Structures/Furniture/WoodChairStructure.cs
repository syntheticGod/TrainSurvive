/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 12:35:06
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class WoodChairStructure : Structure {

    public WoodChairStructure() : base() { }
    protected WoodChairStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "木椅",
        Description = "椅子",
        WorkAll = 0.01f,
        BuildCosts = new ItemData[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 2,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

}
