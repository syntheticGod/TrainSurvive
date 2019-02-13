/*
 * 描述：可修改最大值的设施类
 * 作者：刘旭涛
 * 创建时间：2018/12/13 13:54:04
 * 版本：v0.1
 */
using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class MaxValueModifiableStructure : Structure {
    public MaxValueModifiableStructure(int id) : base(id) { }

    protected MaxValueModifiableStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _addElectMax = (float)info.GetValue("_addElectMax", typeof(float));
        _addEnergyMax = (float)info.GetValue("_addEnergyMax", typeof(float));
        _addFoodMax = (float)info.GetValue("_addFoodMax", typeof(float));
        _addMemberMax = info.GetInt32("_addMemberMax");
        _addElectMaxRatio = (float)info.GetValue("_addElectMaxRatio", typeof(float));
        _addEnergyMaxRatio = (float)info.GetValue("_addEnergyMaxRatio", typeof(float));
        _addFoodMaxRatio = (float)info.GetValue("_addFoodMaxRatio", typeof(float));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("_addElectMax", _addElectMax);
        info.AddValue("_addEnergyMax", _addEnergyMax);
        info.AddValue("_addMemberMax", _addMemberMax);
        info.AddValue("_addFoodMax", _addFoodMax);
        info.AddValue("_addElectMaxRatio", _addElectMaxRatio);
        info.AddValue("_addEnergyMaxRatio", _addEnergyMaxRatio);
        info.AddValue("_addFoodMaxRatio", _addFoodMaxRatio);
    }

    /// <summary>
    /// 增加动能上限
    /// </summary>
    public float AddEnergyMax {
        get {
            return _addEnergyMax;
        }
    }
    /// <summary>
    /// 增加电能上限
    /// </summary>
    public float AddElectMax {
        get {
            return _addElectMax;
        }
    }
    /// <summary>
    /// 增加队伍上限
    /// </summary>
    public int AddMemberMax {
        get {
            return _addMemberMax;
        }
    }
    /// <summary>
    /// 增加食物上限
    /// </summary>
    public float AddFoodMax {
        get {
            return _addFoodMax;
        }
    }

    /// <summary>
    /// 增加动能上限比例
    /// </summary>
    public float AddEnergyMaxRatio {
        get {
            return _addEnergyMaxRatio;
        }
        set {
            if (value == _addEnergyMaxRatio) return;
            World.getInstance().setEnergyMax(World.getInstance().getEnergyMax() + AddEnergyMax * (value - _addEnergyMaxRatio));
            _addEnergyMaxRatio = value;
        }
    }
    /// <summary>
    /// 增加电能上限比例
    /// </summary>
    public float AddElectMaxRatio {
        get {
            return _addElectMaxRatio;
        }
        set {
            if (value == _addElectMaxRatio) return;
            World.getInstance().setElectricityMax(World.getInstance().getElectricityMax() + AddElectMax * (value - _addElectMaxRatio));
            _addElectMaxRatio = value;
        }
    }
    /// <summary>
    /// 增加食物上限比例
    /// </summary>
    public float AddFoodMaxRatio {
        get {
            return _addFoodMaxRatio;
        }
        set {
            if (value == _addFoodMaxRatio) return;
            World.getInstance().setFoodInMax(World.getInstance().getFoodInMax() + AddFoodMax * (value - _addFoodMaxRatio));
            _addFoodMaxRatio = value;
        }
    }

    [StructurePublicField(Tooltip = "增加电能上限")]
    private float _addElectMax;
    [StructurePublicField(Tooltip = "增加动能上限")]
    private float _addEnergyMax;
    [StructurePublicField(Tooltip = "增加队伍上限")]
    private int _addMemberMax;
    [StructurePublicField(Tooltip = "增加食物上限")]
    private float _addFoodMax;

    private float _addElectMaxRatio;
    private float _addEnergyMaxRatio;
    private float _addFoodMaxRatio;

    protected override void OnStart() {
        base.OnStart();
        if (!HasTriggered) {
            AddElectMaxRatio = 1;
            AddEnergyMaxRatio = 1;
            AddFoodMaxRatio = 1;
            World.getInstance().Persons.MaxMember += AddMemberMax;
        }
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        AddElectMaxRatio = 0;
        AddEnergyMaxRatio = 0;
        AddFoodMaxRatio = 0;
        World.getInstance().Persons.MaxMember -= AddMemberMax;
    }
}
