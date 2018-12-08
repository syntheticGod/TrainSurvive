/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 15:56:21
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class BigWoodStructure : SmallWoodStructure {

    public BigWoodStructure() : base() { }
    protected BigWoodStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "大型制木机",
        Description = "将木板精制为强化木板，1=1",
        WorkAll = 0.01f,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 3,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "查看", Action = (structure) => UIManager.Instance?.ShowFaclityUI("BigWoodFactory", structure) },
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    /// <summary>
    /// 可接受可燃物及默认转化率
    /// </summary>
    public override Dictionary<int, Conversion> AcceptableRaw { get; } = new Dictionary<int, Conversion> {
        {231, new Conversion(231, 1, 3) }
    };
    
}
