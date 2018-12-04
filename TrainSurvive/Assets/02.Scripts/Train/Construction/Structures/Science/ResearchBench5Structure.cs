/*
 * 描述：五级研究台
 * 作者：刘旭涛
 * 创建时间：2018/12/4 18:25:02
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class ResearchBench5Structure : ResearchBench2Structure {

    public ResearchBench5Structure() : base() { }
    protected ResearchBench5Structure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "5级研究台",
        Description = "研究台",
        WorkAll = 9 * 6,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 1,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{ Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    protected override float Ratio { get; } = 2.0f;
}
