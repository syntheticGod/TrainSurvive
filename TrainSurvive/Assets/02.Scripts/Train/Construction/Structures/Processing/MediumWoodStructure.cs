/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 15:50:08
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class MediumWoodStructure : SmallWoodStructure {

    public MediumWoodStructure() : base() { }
    protected MediumWoodStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "中型制木机",
        Description = "将木材加工为木板，1木材=4木板",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 3,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "查看", Action = (structure) => UIManager.Instance?.ShowFaclityUI("MediumWoodFactory", structure) },
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    /// <summary>
    /// 可接受可燃物及默认转化率
    /// </summary>
    public override Dictionary<int, Conversion> AcceptableRaw { get; } = new Dictionary<int, Conversion> {
        {231, new Conversion(231, 4, 3) }
    };
    

}
