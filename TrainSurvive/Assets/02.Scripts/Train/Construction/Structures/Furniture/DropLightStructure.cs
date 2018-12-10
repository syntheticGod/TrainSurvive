/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 12:55:05
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class DropLightStructure : Structure {

    public DropLightStructure() : base() { }
    protected DropLightStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "吊灯",
        Description = "依附天花板，电力设备。",
        LayerOrientation = Vector2.up,
        RequiredLayerNames = new string[]{"Ceil"},
        WorkAll = 0,
        BuildCosts = new ItemData[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 2,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

}
