/*
 * 描述：可产生、转换能量的建筑类型
 * 作者：刘旭涛
 * 创建时间：2018/12/14 17:35:40
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class EnergyGeneratableStructure : Structure {
    public EnergyGeneratableStructure(int id) : base(id) { }

    protected EnergyGeneratableStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _addElect = info.GetSingle("_addElect");
        _addEnergy = info.GetSingle("_addEnergy");
        _addFood = info.GetInt32("_addFood");
        _deltaTime = info.GetSingle("_deltaTime");
        AddEnergyRatio = info.GetSingle("AddEnergyRatio");
        AddElectRatio = info.GetSingle("AddElectRatio");
        AddFoodRatio = info.GetSingle("AddFoodRatio");
        IsOn = info.GetBoolean("IsOn");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("_addElect", _addElect);
        info.AddValue("_addEnergy", _addEnergy);
        info.AddValue("_addFood", _addFood);
        info.AddValue("_deltaTime", _deltaTime);
        info.AddValue("AddEnergyRatio", AddEnergyRatio);
        info.AddValue("AddElectRatio", AddElectRatio);
        info.AddValue("AddFoodRatio", AddFoodRatio);
        info.AddValue("IsOn", IsOn);
    }

    /// <summary>
    /// 产生动能
    /// </summary>
    public float AddEnergy {
        get {
            return _addEnergy;
        }
    }
    /// <summary>
    /// 产生电能
    /// </summary>
    public float AddElect {
        get {
            return _addElect;
        }
    }
    /// <summary>
    /// 产生食物
    /// </summary>
    public int AddFood {
        get {
            return _addFood;
        }
    }
    /// <summary>
    /// 生产时间间隔
    /// </summary>
    public float DeltaTime {
        get {
            return _deltaTime;
        }
    }
    /// <summary>
    /// 产生动能比例
    /// </summary>
    public float AddEnergyRatio { get; set; } = 1;
    /// <summary>
    /// 产生电能比例
    /// </summary>
    public float AddElectRatio { get; set; } = 1;
    /// <summary>
    /// 产生食物比例
    /// </summary>
    public float AddFoodRatio { get; set; } = 1;
    /// <summary>
    /// 开关
    /// </summary>
    public bool IsOn { get; set; } = true;

    private Coroutine RunningCoroutine { get; set; }

    [StructurePublicField(Tooltip = "产生电能")]
    private float _addElect;
    [StructurePublicField(Tooltip = "产生动能")]
    private float _addEnergy;
    [StructurePublicField(Tooltip = "产生食物")]
    private int _addFood;
    [StructurePublicField(Tooltip = "生产时间间隔")]
    private float _deltaTime;

    protected override void OnStart() {
        base.OnStart();
        RunningCoroutine = TimeController.getInstance().StartCoroutine(Run());
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        if (RunningCoroutine != null)
            TimeController.getInstance().StopCoroutine(RunningCoroutine);
    }

    public override LinkedList<ButtonAction> GetButtonActions() {
        LinkedList<ButtonAction> actions = base.GetButtonActions();
        if (IsOn) {
            actions.AddFirst(new ButtonAction("关闭", (facility) => IsOn = false));
        } else {
            actions.AddFirst(new ButtonAction("打开", (facility) => IsOn = true));
        }
        return actions;
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => {
            return IsOn &&
            World.getInstance().getEnergy() + AddEnergy * AddEnergyRatio <= World.getInstance().getEnergyMax() && World.getInstance().getEnergy() + AddEnergy * AddEnergyRatio >= 0 &&
            World.getInstance().getElectricity() + AddElect * AddElectRatio <= World.getInstance().getElectricityMax() && World.getInstance().getElectricity() + AddElect * AddElectRatio >= 0 &&
            World.getInstance().getFoodIn() + AddFood * AddFoodRatio <= World.getInstance().getFoodInMax() && World.getInstance().getFoodIn() + AddFood * AddFoodRatio >= 0;
        });
        while (FacilityState == State.WORKING) {
            yield return wait;
            World.getInstance().addEnergy(AddEnergy * AddEnergyRatio);
            World.getInstance().addElectricity(AddElect * AddElectRatio);
            World.getInstance().addFoodIn((int)(AddFood * AddFoodRatio));
            if (DeltaTime == 0) {
                yield return 1;
            } else {
                yield return new WaitForSeconds(DeltaTime);
            }
        }
    }
}
