/*
 * 描述：物品转换能源建筑类
 * 作者：刘旭涛
 * 创建时间：2018/12/13 17:10:33
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Item2EnergyStructure : Structure {

    public enum EnergyType {
        ENERGY, ELECT, FOOD
    }

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
        _energyType = (EnergyType)info.GetValue("_energyType", typeof(EnergyType));
        Progress = (float)info.GetValue("Progress", typeof(float));
        Gas = (ItemData)info.GetValue("Gas", typeof(ItemData));
        ConversionRateRatio = (float)info.GetValue("ConversionRateRatio", typeof(float));
        ProcessSpeedRatio = (float)info.GetValue("ProcessSpeedRatio", typeof(float));
        AutomataEnabled = info.GetBoolean("AutomataEnabled");
        AutomataCount = info.GetInt32("AutomataCount");
        AutomataItem = info.GetInt32("AutomataItem");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Progress", Progress);
        info.AddValue("Gas", Gas);
        info.AddValue("ConversionRateRatio", ConversionRateRatio);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("AutomataEnabled", AutomataEnabled);
        info.AddValue("AutomataCount", AutomataCount);
        info.AddValue("AutomataItem", AutomataItem);
        info.AddValue("_conversions", _conversions);
        info.AddValue("_conversionRate", _conversionRate);
        info.AddValue("_processSpeed", _processSpeed);
        info.AddValue("_energyType", _energyType);
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
    /// 能源类型
    /// </summary>
    public EnergyType GeneratedEnergyType {
        get {
            return _energyType;
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
                CallOnProgressChange(0, Conversions[Gas.ID].ProcessTime, value);
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
    /// <summary>
    /// 自动化是否启动
    /// </summary>
    public bool AutomataEnabled { get; set; }
    /// <summary>
    /// 自动化次数
    /// </summary>
    public int AutomataCount { get; set; }
    /// <summary>
    /// 自动化配方
    /// </summary>
    public int AutomataItem { get; set; }


    public Action<ItemData> OnGasUpdate;
    public RefAction<ItemData> OnAcquireGas;
    public Action<int> OnAutomataCountChange;

    private Coroutine RunningCoroutine { get; set; }
    private Coroutine AutoFillCoroutine { get; set; }

    [StructurePublicField(Tooltip = "转化列表")]
    private Conversion[] _conversions;
    [StructurePublicField(Tooltip = "转化率")]
    private float _conversionRate;
    [StructurePublicField(Tooltip = "处理速度")]
    private float _processSpeed;
    [StructurePublicField(Tooltip = "能源类型")]
    private EnergyType _energyType;

    private Dictionary<int, Conversion> _conversionsDic;
    private float _progress;
    private ItemData _gas;

    protected override void OnStart() {
        base.OnStart();
        RunningCoroutine = TimeController.getInstance().StartCoroutine(Run());
        AutoFillCoroutine = TimeController.getInstance().StartCoroutine(AutoFill());
    }

    protected override void OnRemoving() {
        base.OnRemoving();
        if (RunningCoroutine != null)
            TimeController.getInstance().StopCoroutine(RunningCoroutine);
        if (AutoFillCoroutine != null)
            TimeController.getInstance().StopCoroutine(AutoFillCoroutine);
    }

    public override LinkedList<ButtonAction> GetButtonActions() {
        LinkedList<ButtonAction> actions = base.GetButtonActions();
        actions.AddFirst(new ButtonAction("操作", (facility) => UIManager.Instance?.ShowFaclityUI("sui_" + facility.Structure.ID, facility.Structure)));
        return actions;
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => Gas != null && Gas.Number >= 1 && World.getInstance().getEnergy() < World.getInstance().getEnergyMax());
        while (FacilityState == State.WORKING) {
            if (!(Gas != null && Gas.Number >= 1 && World.getInstance().getEnergy() < World.getInstance().getEnergyMax())) {
                Progress = 0;
                yield return wait;
            }
            if (Progress < Conversions[Gas.ID].ProcessTime) {
                Progress += Time.deltaTime * ProcessSpeed * ProcessSpeedRatio;
            } else {
                Progress = 0;
                AutomataItem = Gas.ID;
                if (AutomataCount > 0 && AutomataEnabled) {
                    AutomataCount--;
                    OnAutomataCountChange?.Invoke(AutomataCount);
                }
                float generatedAmount = Conversions[Gas.ID].ProduceEnergy * ConversionRate * ConversionRateRatio;
                switch (GeneratedEnergyType) {
                    case EnergyType.ENERGY:
                        World.getInstance().addEnergy(generatedAmount);
                        break;
                    case EnergyType.ELECT:
                        World.getInstance().addElectricity(generatedAmount);
                        break;
                    case EnergyType.FOOD:
                        World.getInstance().addFoodIn(generatedAmount);
                        break;
                }
                if (--Gas.Number == 0) {
                    Gas = null;
                }
                OnGasUpdate?.Invoke(_gas);
            }
            yield return 1;
        }
    }

    private IEnumerator AutoFill() {
        WaitUntil wait = new WaitUntil(() => AutomataEnabled && AutomataCount != 0 && Gas == null && PublicMethod.IfHaveEnoughItems(new ItemData[] { new ItemData(AutomataItem, 1) }));
        while (FacilityState == State.WORKING) {
            yield return wait;
            _gas = new ItemData(AutomataItem, 1);
            PublicMethod.ConsumeItems(new ItemData[] { _gas });
            OnGasUpdate?.Invoke(_gas);
        }
    }
}
