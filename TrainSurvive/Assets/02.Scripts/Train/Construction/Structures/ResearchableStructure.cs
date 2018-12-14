/*
 * 描述：可研究建筑类
 * 作者：刘旭涛
 * 创建时间：2018/12/14 10:34:30
 * 版本：v0.1
 */
using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class ResearchableStructure : EnergyCostableStructure {
    public ResearchableStructure(int id) : base(id) { }

    protected ResearchableStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _speed = info.GetSingle("_speed");
        _speedRatio = info.GetSingle("_speedRatio");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("_speed", _speed);
        info.AddValue("_speedRatio", _speedRatio);
    }

    /// <summary>
    /// 科研速率
    /// </summary>
    public float Speed {
        get {
            return _speed;
        }
    }

    /// <summary>
    /// 速率比例
    /// </summary>
    public float SpeedRatio {
        get {
            return _speedRatio;
        }
        set {
            _speedRatio = value;
            SetRatios();
        }
    }

    [StructurePublicField(Tooltip = "科研速率")]
    private float _speed;

    private float _speedRatio;
    
    protected override void OnStart() {
        base.OnStart();
        IsCosting = true;
        SpeedRatio = 1;
        World.getInstance().techUnlock += 1;
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        SpeedRatio = 0;
        World.getInstance().techUnlock -= 1;
    }

    private void SetRatios() {
        float max = -1;
        foreach(Structure structure in ConstructionManager.Instance.Structures) {
            if (structure.FacilityState == State.WORKING && structure is ResearchableStructure) {
                max = Mathf.Max((structure as ResearchableStructure).Speed * (structure as ResearchableStructure).SpeedRatio, max);
            }
        }
        for (int i = 0; i < TechTreeManager.Instance.Techs.Length; i++) {
            if (TechTreeManager.Instance.Techs[i] == null) continue;
            TechTreeManager.Instance.Techs[i].WorkSpeedRatio = max;
        }
    }

}
