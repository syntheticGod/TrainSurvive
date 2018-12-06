/*
 * 描述：内燃机
 * 作者：刘旭涛
 * 创建时间：2018/12/4 15:31:27
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class GasEngineStructure : SteamEngineStructure {

    public GasEngineStructure() : base() { }
    protected GasEngineStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "内燃机",
        Description = "内燃机",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 0,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "查看", Action = (structure) => UIManager.Instance?.ShowFaclityUI("GasEngine", structure) },
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;
    
    public override Dictionary<int, Conversion> AcceptableGas { get; } = new Dictionary<int, Conversion> {
        {231, new Conversion(3, 20) }
    };
    
}
