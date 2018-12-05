/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 12:37:09
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class WoodBoxStructure : Structure {

    public WoodBoxStructure() : base() { }
    protected WoodBoxStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "木箱",
        Description = "仓库容量上限增加",
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

    protected override void OnStart() {
        // TODO 
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        // TODO
    }
}
