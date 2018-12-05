/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/4 16:41:28
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class BigGeneratorStructure : GeneratorStructure {

    public BigGeneratorStructure() : base() { }
    protected BigGeneratorStructure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "大发电机",
        Description = "按功率将核心中的热能转化为蓄电池的电能",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 0,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "关闭", Action = (structure) => (structure as GeneratorStructure).IsRunning = false },
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    /// <summary>
    /// 处理时间
    /// </summary>
    public override float ProcessTime { get; } = 2;

    /// <summary>
    /// 转化率
    /// </summary>
    public override float ConversionRate { get; } = 5;

}
