/*
 * 描述：基础科学
 * 作者：刘旭涛
 * 创建时间：2018/12/3 21:43:23
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Science0Tech : Tech {
    public Science0Tech() : base() { }
    protected Science0Tech(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public override int[] Dependencies { get; } = { 0 };
    public override string Name { get; } = "基础科学";
    public override string Description { get; } = "允许玩家建造2级研究台，许多科技的研究以基础科学为前提。";
    public override float TotalWorks { get; } = 1 * 6;

    public override void OnCompleted() {
        ConstructionManager.StructureUnlocks[8] = true;
    }
}
