/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 12:51:45
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class KeroseneLampStructure : Structure {

    public KeroseneLampStructure() : base() { }
    protected KeroseneLampStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "煤油灯",
        Description = "依附桌子，自动点亮",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 2,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

}
