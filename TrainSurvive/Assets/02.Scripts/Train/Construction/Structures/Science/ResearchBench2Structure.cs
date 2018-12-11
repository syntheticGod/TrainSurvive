/*
 * 描述：二级研究台
 * 作者：刘旭涛
 * 创建时间：2018/12/4 18:25:02
 * 版本：v0.1
 */
using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class ResearchBench2Structure : Structure {

    public ResearchBench2Structure() : base() { }
    protected ResearchBench2Structure(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static FixedInfo _info = new FixedInfo {
        Name = "2级研究台",
        Description = "研究台",
        WorkAll = 3 * 6,
        BuildCosts = new ItemData[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 1,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{ Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    protected virtual float Ratio { get; } = 1.2f;
    
    protected override void OnStart() {
        SetRatios();
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        SetRatios();
    }

    private void SetRatios() {
        float max = 1;
        for (int i = 0; i < World.getInstance().buildInstArray.Count; i++) {
            Structure structure = World.getInstance().buildInstArray[i];
            if (structure.FacilityState == State.WORKING && structure is ResearchBench2Structure) {
                max = Mathf.Max((structure as ResearchBench2Structure).Ratio, max);
            }
        }
        //for (int i = 0; i < TechTreeManager.Techs.Length; i++) {
        //    TechTreeManager.Techs[i].WorkSpeedRatio = max;
        //}
    }
}
