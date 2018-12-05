/*
 * 描述：床
 * 作者：刘旭涛
 * 创建时间：2018/12/5 12:18:04
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class BedStructure : Structure {

    public BedStructure() : base() { }
    protected BedStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "床",
        Description = "队伍人数上限+1",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 2,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    protected virtual int Value { get; } = 1;

    protected override void OnStart() {
        //World.getInstance(). += 2000;
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        //World.getInstance().energyMax -= 2000;
    }
}
