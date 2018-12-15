/*
 * 描述：物品转换能源建筑类
 * 作者：刘旭涛
 * 创建时间：2018/12/13 17:10:33
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Item2EnergyStructure : Structure {

    [Serializable]
    public struct Conversion {
        public int ItemID;
        public float EnergyRate;
        public float ProcessTimeRatio;
        public float ProduceEnergyRatio;

        public float ProcessTime {
            get {
                return ProcessTimeRatio * EnergyRate;
            }
        }
        public float ProduceEnergy {
            get {
                return ProduceEnergyRatio * EnergyRate;
            }
        }
    }

    public Item2EnergyStructure(int id) : base(id) { }

    protected Item2EnergyStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _conversions = (Conversion[])info.GetValue("_conversions", typeof(Conversion[]));
        _conversionRate = (float)info.GetValue("_conversionRate", typeof(float));
        _processSpeed = (float)info.GetValue("_processSpeed", typeof(float));
        _isElect = info.GetBoolean("_isElect");
        Progress = (float)info.GetValue("Progress", typeof(float));
        Gas = (ItemData)info.GetValue("Gas", typeof(ItemData));
        ConversionRateRatio = (float)info.GetValue("ConversionRateRatio", typeof(float));
        ProcessSpeedRatio = (float)info.GetValue("ProcessSpeedRatio", typeof(float));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Progress", Progress);
        info.AddValue("Gas", Gas);
        info.AddValue("ConversionRateRatio", ConversionRateRatio);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("_conversions", _conversions);
        info.AddValue("_conversionRate", _conversionRate);
        info.AddValue("_processSpeed", _processSpeed);
        info.AddValue("_isElect", _isElect);
    }

    /// <summary>
    /// 转化列表
    /// </summary>
    public Dictionary<int, Conversion> Conversions {
        get {
            if (_conversionsDic == null) {
                _conversionsDic = new Dictionary<int, Conversion>();
                foreach(Conversion conversion in _conversions) {
                    _conversionsDic.Add(conversion.ItemID, conversion);
                }
            }
            return _conversionsDic;
        }
    }
    /// <summary>
    /// 转化率
    /// </summary>
    public float ConversionRate {
        get {
            return _conversionRate;
        }
    }
    /// <summary>
    /// 处理速度
    /// </summary>
    public float ProcessSpeed {
        get {
            return _processSpeed;
        }
    }
    /// <summary>
    /// 能源类型，是电能吗？
    /// </summary>
    public bool IsElect {
        get {
            return _isElect;
        }
    }
    /// <summary>
    /// 转化率比例
    /// </summary>
    public float ConversionRateRatio { get; set; } = 1;
    /// <summary>
    /// 处理速度比例
    /// </summary>
    public float ProcessSpeedRatio { get; set; } = 1;
    /// <summary>
    /// 处理进度
    /// </summary>
    public float Progress {
        get {
            return _progress;
        }
        private set {
            _progress = value;
            if (Gas == null) {
                CallOnProgressChange(0, 0, value);
            } else {
                CallOnProgressChange(0, Conversions[Gas.id].ProcessTime, value);
            }
        }
    }
    /// <summary>
    /// 材料
    /// </summary>
    public ItemData Gas {
        get {
            OnAcquireGas?.Invoke(ref _gas);
            return _gas;
        }
        set {
            _gas = value;
        }
    }

    public Action<ItemData> OnGasUpdate;
    public RefAction<ItemData> OnAcquireGas;

    private Coroutine RunningCoroutine { get; set; }

    [StructurePublicField(Tooltip = "转化列表")]
    private Conversion[] _conversions;
    [StructurePublicField(Tooltip = "转化率")]
    private float _conversionRate;
    [StructurePublicField(Tooltip = "处理速度")]
    private float _processSpeed;
    [StructurePublicField(Tooltip = "能源类型，是电能吗？")]
    private bool _isElect;

    private Dictionary<int, Conversion> _conversionsDic;
    private float _progress;
    private ItemData _gas;

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
        actions.AddFirst(new ButtonAction("操作", (facility) => UIManager.Instance?.ShowFaclityUI("sui_" + facility.Structure.ID, facility.Structure)));
        return actions;
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => Gas != null && Gas.num >= 1 && World.getInstance().getEnergy() < World.getInstance().getEnergyMax());
        while (FacilityState == State.WORKING) {
            if (!(Gas != null && Gas.num >= 1 && World.getInstance().getEnergy() < World.getInstance().getEnergyMax())) {
                Progress = 0;
                yield return wait;
            }
            if (Progress < Conversions[Gas.id].ProcessTime) {
                Progress += Time.deltaTime * ProcessSpeed * ProcessSpeedRatio;
            } else {
                Progress = 0;
                World.getInstance().addEnergy(Conversions[Gas.id].ProduceEnergy * ConversionRate * ConversionRateRatio);
                if (--Gas.num == 0) {
                    Gas = null;
                }
                OnGasUpdate?.Invoke(_gas);
            }
            yield return 1;
        }
    }
}
