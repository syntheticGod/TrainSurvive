/*
 * 描述：热能核心
 * 作者：刘旭涛
 * 创建时间：2018/12/3 23:01:39
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class HeatCoreStructure : Structure {
    
    public HeatCoreStructure() : base() { }
    protected HeatCoreStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "热能核心",
        Description = "存储热能的能源核心，获得初始热能上限2000",
        WorkAll = 0,
        BuildCosts = new Cost[]{ },
        SpritePath = "Sprite/map/building-inn",
        Class = 0,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    protected override void OnStart() {
        World.getInstance().energyMax += 2000;
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        World.getInstance().energyMax -= 2000;
    }
}
