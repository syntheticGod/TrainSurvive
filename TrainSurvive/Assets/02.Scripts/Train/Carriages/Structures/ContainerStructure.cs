/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/31 12:54:01
 * 版本：v0.7
 */
using System;
using System.Runtime.Serialization;
using UnityEngine;

public class ContainerStructure : CarriageStructure {
    public ContainerStructure(string name, bool initialEnabled) : base(name, initialEnabled) { }

    protected ContainerStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _addEnergyMax = (float)info.GetValue("_addEnergyMax", typeof(float));
        _addFoodMax = (float)info.GetValue("_addFoodMax", typeof(float));
        _addMemberMax = info.GetInt32("_addMemberMax");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("_addEnergyMax", _addEnergyMax);
        info.AddValue("_addMemberMax", _addMemberMax);
        info.AddValue("_addFoodMax", _addFoodMax);
    }

    /// <summary>
    /// 增加动能上限
    /// </summary>
    public float AddEnergyMax {
        get {
            return _addEnergyMax;
        }
        set {
            float change = value - _addEnergyMax;
            World.getInstance().setEnergyMax(World.getInstance().getEnergyMax() + change);
            _addEnergyMax = value;
        }
    }
    /// <summary>
    /// 增加队伍上限
    /// </summary>
    public int AddMemberMax {
        get {
            return _addMemberMax;
        }
        set {
            int change = value - _addMemberMax;
            World.getInstance().Persons.MaxMember += change;
            _addMemberMax = value;
        }
    }
    /// <summary>
    /// 增加食物上限
    /// </summary>
    public float AddFoodMax {
        get {
            return _addFoodMax;
        }
        set {
            float change = value - _addFoodMax;
            World.getInstance().setFoodInMax(World.getInstance().getFoodInMax() + change);
            _addFoodMax = value;
        }
    }
    
    [StructurePublicField(Tooltip = "增加动能上限")]
    private float _addEnergyMax;
    [StructurePublicField(Tooltip = "增加队伍上限")]
    private int _addMemberMax;
    [StructurePublicField(Tooltip = "增加食物上限")]
    private float _addFoodMax;

    public override void OnUpgraded(CarriageResearchSetting upgrade) {
        base.OnUpgraded(upgrade);
        string[] parameters = upgrade.Parameter.Split('|');
        if (parameters.Length != 3) {
            Debug.LogError("第" + upgrade.ID + "号升级所需参数为([float]AddEnergyMax|[int]AddMemberMax|[float]AddFoodMax)");
            return;
        }

        if (parameters[0].Length > 0) {
            float value = float.Parse(parameters[0]);
            AddEnergyMax = value;
        }
        if (parameters[1].Length > 0) {
            int value = int.Parse(parameters[1]);
            AddMemberMax = value;
        }
        if (parameters[2].Length > 0) {
            float value = float.Parse(parameters[2]);
            AddFoodMax = value;
        }
    }

    public override void OnStart() {
        base.OnStart();
        AddEnergyMax = _addEnergyMax;
        AddMemberMax = _addMemberMax;
        AddFoodMax = _addFoodMax;
    }
}
