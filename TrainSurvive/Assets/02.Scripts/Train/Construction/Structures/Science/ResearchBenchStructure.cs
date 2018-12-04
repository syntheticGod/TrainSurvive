/*
 * 描述：研究台
 * 作者：刘旭涛
 * 创建时间：2018/12/4 16:57:21
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class ResearchBenchStructure : Structure {

    public ResearchBenchStructure() : base() { }
    protected ResearchBenchStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "研究台",
        Description = "研究台",
        WorkAll = 2 * 6,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 1,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{ Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    protected override void OnStart() {
        World.getInstance().techUnlock += 1;
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        World.getInstance().techUnlock -= 1;
    }
}
