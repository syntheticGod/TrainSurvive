/*
 * 描述：进阶木工
 * 作者：刘旭涛
 * 创建时间：2018/12/3 22:02:16
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Carpentry1Tech : Tech {
    public Carpentry1Tech() : base() { }
    protected Carpentry1Tech(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public override int[] Dependencies { get; } = { 2, 3 };
    public override string Name { get; } = "进阶木工";
    public override string Description { get; } = "进阶木工";
    public override float TotalWorks { get; } = 3 * 6;

    public override void OnCompleted() {
        ConstructionManager.StructureUnlocks[13] = true;
        ConstructionManager.StructureUnlocks[25] = true;
    }
}
