/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/31 13:28:28
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class GeneratorStructure : CarriageStructure {
    public GeneratorStructure(string name, bool initialEnabled) : base(name, initialEnabled) { }

    protected GeneratorStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _addEnergy = info.GetSingle("_addEnergy");
        _addFood = info.GetInt32("_addFood");
        _deltaTime = info.GetSingle("_deltaTime");
        AddEnergyRatio = info.GetSingle("AddEnergyRatio");
        AddFoodRatio = info.GetSingle("AddFoodRatio");
        IsOn = info.GetBoolean("IsOn");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("_addEnergy", _addEnergy);
        info.AddValue("_addFood", _addFood);
        info.AddValue("_deltaTime", _deltaTime);
        info.AddValue("AddEnergyRatio", AddEnergyRatio);
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
    /// 产生食物比例
    /// </summary>
    public float AddFoodRatio { get; set; } = 1;
    /// <summary>
    /// 开关
    /// </summary>
    public bool IsOn { get; set; } = true;

    private Coroutine RunningCoroutine { get; set; }
    
    [StructurePublicField(Tooltip = "产生动能")]
    private float _addEnergy;
    [StructurePublicField(Tooltip = "产生食物")]
    private int _addFood;
    [StructurePublicField(Tooltip = "生产时间间隔")]
    private float _deltaTime;

    public override void OnStart() {
        base.OnStart();
        RunningCoroutine = TimeController.getInstance().StartCoroutine(Run());
    }

    public override void OnUpgraded(CarriageResearchSetting upgrade) {
        base.OnUpgraded(upgrade);
        string[] parameters = upgrade.Parameter.Split('|');
        if (parameters.Length != 2) {
            Debug.LogError("第" + upgrade.ID + "号升级所需参数为([float]AddEnergyRatio|[float]AddFoodRatio)");
            return;
        }

        if (parameters[0].Length > 0) {
            float value = float.Parse(parameters[0]);
            AddEnergyRatio = value;
        }
        if (parameters[1].Length > 0) {
            float value = float.Parse(parameters[1]);
            AddFoodRatio = value;
        }
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => {
            return IsOn &&
            World.getInstance().getEnergy() + AddEnergy * AddEnergyRatio <= World.getInstance().getEnergyMax() && World.getInstance().getEnergy() + AddEnergy * AddEnergyRatio >= 0 &&
            World.getInstance().getFoodIn() + AddFood * AddFoodRatio <= World.getInstance().getFoodInMax() && World.getInstance().getFoodIn() + AddFood * AddFoodRatio >= 0;
        });
        while (true) {
            UpdateState("Generating", false);
            yield return wait;
            UpdateState("Generating", true);
            World.getInstance().addEnergy(AddEnergy * AddEnergyRatio);
            World.getInstance().addFoodIn(AddFood * AddFoodRatio);
            if (DeltaTime == 0) {
                yield return 1;
            } else {
                yield return new WaitForSeconds(DeltaTime);
            }
        }
    }
}
