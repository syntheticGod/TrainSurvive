/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/6 14:52:06
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class CabCarriage : TrainCarriage {

    public CabCarriage() : base() { }
    protected CabCarriage(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static readonly FixedInfo _info = new FixedInfo {
        Name = "驾驶室",
        Description = "驾驶室",
        Size = new Vector2(15.48f, 4.66f),
        BuildCosts = new ItemData[] { },
        WorkAll = 0.01f,
        PrefabPath = "Prefabs/Train/Carriages/cab"
    };

    public override FixedInfo Info { get; } = _info;
    
}
