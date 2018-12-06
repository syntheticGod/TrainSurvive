/*
 * 描述：初级列车扩充
 * 作者：刘旭涛
 * 创建时间：2018/12/3 21:31:22
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class TrainExpand0Tech : Tech {
    public TrainExpand0Tech() : base() { }
    protected TrainExpand0Tech(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public override int[] Dependencies { get; } = { 0 };
    public override string Name { get; } = "初级列车扩充";
    public override string Description { get; } = "允许玩家建造新车厢（12米*3米）。";
    public override float TotalWorks { get; } = 2 * 6;

    public override void OnCompleted() {
        ConstructionManager.CarriageUnlocks[0] = true;
    }
}
