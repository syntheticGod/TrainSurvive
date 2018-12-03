/*
 * 描述：基础木工
 * 作者：刘旭涛
 * 创建时间：2018/12/3 21:58:32
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Carpentry0Tech : Tech {
    public Carpentry0Tech() : base() { }
    protected Carpentry0Tech(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public override int[] Dependencies { get; } = { 0 };
    public override string Name { get; } = "基础木工";
    public override string Description { get; } = "基础木工";
    public override float TotalWorks { get; } = 1 * 6;

    public override void OnCompleted() {

    }
}
