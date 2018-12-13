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
        _addInventoryMax = (float)info.GetValue("_addInventoryMax", typeof(float));
        _addMemberMax = info.GetInt32("_addMemberMax");
        _addElectMaxRatio = (float)info.GetValue("_addElectMaxRatio", typeof(float));
        _addEnergyMaxRatio = (float)info.GetValue("_addEnergyMaxRatio", typeof(float));
        _addInventoryMaxRatio = (float)info.GetValue("_addInventoryMaxRatio", typeof(float));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("_addElectMax", _addElectMax);
        info.AddValue("_addEnergyMax", _addEnergyMax);
        info.AddValue("_addInventoryMax", _addInventoryMax);
        info.AddValue("_addMemberMax", _addMemberMax);
        info.AddValue("_addElectMaxRatio", _addElectMaxRatio);
        info.AddValue("_addEnergyMaxRatio", _addEnergyMaxRatio);
        info.AddValue("_addInventoryMaxRatio", _addInventoryMaxRatio);
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
    /// 增加仓库上限
    /// </summary>
    public float AddInventoryMax {
        get {
            return _addInventoryMax;
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
    /// 增加仓库上限比例
    /// </summary>
    public float AddInventoryMaxRatio {
        get {
            return _addInventoryMaxRatio;
        }
        set {
            if (value == _addInventoryMaxRatio) return;
            // TODO World.getInstance().max(World.getInstance().getElectricityMax() + AddElectMax * (value - _addElectMaxRatio));
            _addInventoryMaxRatio = value;
        }
    }
    
    [StructurePublicField(Tooltip = "增加电能上限")]
    private float _addElectMax;
    [StructurePublicField(Tooltip = "增加动能上限")]
    private float _addEnergyMax;
    [StructurePublicField(Tooltip = "增加仓库上限")]
    private float _addInventoryMax;
    [StructurePublicField(Tooltip = "增加队伍上限")]
    private int _addMemberMax;

    private float _addElectMaxRatio;
    private float _addEnergyMaxRatio;
    private float _addInventoryMaxRatio;

    protected override void OnStart() {
        base.OnStart();
        if (!HasTriggered) {
            AddElectMaxRatio = 1;
            AddEnergyMaxRatio = 1;
            AddInventoryMaxRatio = 1;
            World.getInstance().personNumMax += AddMemberMax;
        }
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        AddElectMaxRatio = 0;
        AddEnergyMaxRatio = 0;
        AddInventoryMaxRatio = 0;
        World.getInstance().personNumMax -= AddMemberMax;
    }
}
