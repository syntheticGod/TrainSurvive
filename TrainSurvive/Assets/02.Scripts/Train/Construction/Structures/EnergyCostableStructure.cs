/*
 * 描述：消耗能源的建筑类
 * 作者：刘旭涛
 * 创建时间：2018/12/13 10:25:24
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class EnergyCostableStructure : Structure {
    public EnergyCostableStructure(int id) : base(id) { }

    protected EnergyCostableStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _costEnergy = (float)info.GetValue("_costEnergy", typeof(float));
        _costElect = (float)info.GetValue("_costElect", typeof(float));
        CostEnergyRatio = (float)info.GetValue("CostEnergyRatio", typeof(float));
        CostElectRatio = (float)info.GetValue("CostElectRatio", typeof(float));
        IsCosting = info.GetBoolean("IsCosting");
        IsRunningOut = info.GetBoolean("IsRunningOut");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("_costEnergy", _costEnergy);
        info.AddValue("_costElect", _costElect);
        info.AddValue("CostEnergyRatio", CostEnergyRatio);
        info.AddValue("CostElectRatio", CostElectRatio);
        info.AddValue("IsCosting", IsCosting);
        info.AddValue("IsRunningOut", IsRunningOut);
    }

    /// <summary>
    /// 消耗动能
    /// </summary>
    public float CostEnergy {
        get {
            return _costEnergy;
        }
    }
    /// <summary>
    /// 消耗电能
    /// </summary>
    public float CostElect {
        get {
            return _costElect;
        }
    }
    /// <summary>
    /// 消耗动能比例
    /// </summary>
    public float CostEnergyRatio { get; set; } = 1;
    /// <summary>
    /// 消耗电能比例
    /// </summary>
    public float CostElectRatio { get; set; } = 1;
    /// <summary>
    /// 是否正在消耗
    /// </summary>
    public bool IsCosting { get; protected set; }
    /// <summary>
    /// 资源耗尽？
    /// </summary>
    public bool IsRunningOut { get; private set; }
    
    private Coroutine RunningCoroutine { get; set; }

    [StructurePublicField(Tooltip = "消耗电能")]
    private float _costElect;
    [StructurePublicField(Tooltip = "消耗动能")]
    private float _costEnergy;

    protected override void OnStart() {
        base.OnStart();
        RunningCoroutine = TimeController.getInstance().StartCoroutine(RunCost());
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        if (RunningCoroutine != null)
            TimeController.getInstance().StopCoroutine(RunningCoroutine);
    }

    private IEnumerator RunCost() {
        WaitUntil wait = new WaitUntil(() => IsCosting);
        while(FacilityState == State.WORKING) {
            yield return wait;
            bool energyRunningOut = World.getInstance().addEnergy(-CostEnergy * CostEnergyRatio * Time.deltaTime) != 1;
            bool electRunningOut = World.getInstance().addElectricity(-CostElect * CostElectRatio * Time.deltaTime) != 1;
            IsRunningOut = energyRunningOut || electRunningOut;
            yield return 1;
        }
    }
}
