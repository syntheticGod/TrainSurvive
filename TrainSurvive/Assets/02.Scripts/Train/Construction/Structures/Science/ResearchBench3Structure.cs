/*
 * 描述：三级研究台
 * 作者：刘旭涛
 * 创建时间：2018/12/4 18:25:02
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class ResearchBench3Structure : ResearchBench2Structure {

    public ResearchBench3Structure() : base() { }
    protected ResearchBench3Structure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "3级研究台",
        Description = "研究台",
        WorkAll = 5 * 6,
        BuildCosts = new ItemData[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 1,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{ Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    protected override float Ratio { get; } = 1.4f;
}
