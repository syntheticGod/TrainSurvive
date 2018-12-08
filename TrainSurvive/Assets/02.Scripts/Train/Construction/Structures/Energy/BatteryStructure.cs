/*
 * 描述：蓄电池
 * 作者：刘旭涛
 * 创建时间：2018/12/4 15:46:11
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class BatteryStructure : Structure {

    public BatteryStructure() : base() { }
    protected BatteryStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "蓄电池",
        Description = "存储电能的能源核心，获得电能上限1000",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 0,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{ Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    protected override void OnStart() {
        // TODO
    }
}
