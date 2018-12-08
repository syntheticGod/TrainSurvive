/*
 * 描述：高等木工
 * 作者：刘旭涛
 * 创建时间：2018/12/3 22:03:48
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Carpentry2Tech : Tech {
    public Carpentry2Tech() : base() { }
    protected Carpentry2Tech(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public override int[] Dependencies { get; } = { 4 };
    public override string Name { get; } = "高等木工";
    public override string Description { get; } = "高等木工";
    public override float TotalWorks { get; } = 5 * 6;

    public override void OnCompleted() {
        ConstructionManager.StructureUnlocks[14] = true;
        ConstructionManager.StructureUnlocks[26] = true;
    }
}
