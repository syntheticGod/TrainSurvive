/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 19:28:13
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Item_EnergyStructure : CarriageStructure {
    public enum EnergyType {
        ENERGY, FOOD
    }

    [Serializable]
    public struct Conversion : IConversion {
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

        public float GetProcessTime {
            get {
                return ProcessTime;
            }
        }
    }

    public Item_EnergyStructure(string name, bool initialEnabled) : base(name, initialEnabled) { }

    protected Item_EnergyStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _conversions = (Conversion[])info.GetValue("_conversions", typeof(Conversion[]));
        _conversionRate = (float)info.GetValue("_conversionRate", typeof(float));
        _processSpeed = (float)info.GetValue("_processSpeed", typeof(float));
        _energyType = (EnergyType)info.GetValue("_energyType", typeof(EnergyType));
        _conversionsList = (List<Formula<Conversion>>)info.GetValue("_conversionsList", typeof(List<Formula<Conversion>>));
        ConversionRateRatio = (float)info.GetValue("ConversionRateRatio", typeof(float));
        ProcessSpeedRatio = (float)info.GetValue("ProcessSpeedRatio", typeof(float));
        Concurrency = info.GetInt32("Concurrency");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("ConversionRateRatio", ConversionRateRatio);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("Concurrency", Concurrency);
        info.AddValue("_conversions", _conversions);
        info.AddValue("_conversionRate", _conversionRate);
        info.AddValue("_processSpeed", _processSpeed);
        info.AddValue("_energyType", _energyType);
        info.AddValue("_conversionsList", Conversions);
    }

    /// <summary>
    /// 配方列表
    /// </summary>
    public List<Formula<Conversion>> Conversions {
        get {
            if (_conversionsList == null) {
                _conversionsList = new List<Formula<Conversion>>();
                foreach (Conversion conversion in _conversions) {
                    _conversionsList.Add(new Formula<Conversion>(conversion, _conversionsList.Count));
                }
            }
            return _conversionsList;
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
    /// 并发数
    /// </summary>
    public int Concurrency { get; set; } = 1;

    private Coroutine RunningCoroutine { get; set; }
    
    [StructurePublicField(Tooltip = "转化列表")]
    private Conversion[] _conversions;
    [StructurePublicField(Tooltip = "转化率")]
    private float _conversionRate;
    [StructurePublicField(Tooltip = "处理速度")]
    private float _processSpeed;
    [StructurePublicField(Tooltip = "能源类型")]
    private EnergyType _energyType;

    private List<Formula<Conversion>> _conversionsList;

    public override void OnStart() {
        base.OnStart();
        RunningCoroutine = TimeController.getInstance().StartCoroutine(Run());
    }

    public override void OnUpgraded(CarriageResearchSetting upgrade) {
        base.OnUpgraded(upgrade);
        string[] parameters = upgrade.Parameter.Split('|');
        if (parameters.Length != 3) {
            Debug.LogError("第" + upgrade.ID + "号升级所需参数为([float]ConversionRate|[float]ProcessSpeed|[int]Concurrency)");
            return;
        }

        if (parameters[0].Length > 0) {
            float rate = float.Parse(parameters[0]);
            ConversionRateRatio = rate / ConversionRate;
        }
        if (parameters[1].Length > 0) {
            float rate = float.Parse(parameters[1]);
            ProcessSpeedRatio = rate / ProcessSpeed;
        }
        if (parameters[2].Length > 0) {
            int value = int.Parse(parameters[2]);
            Concurrency = value;
        }
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(WaitForAvailable);
        while (true) {
            if (!WaitForAvailable()) {
                foreach (Formula<Conversion> formula in Conversions) {
                    formula.Progress = 0;
                }
                UpdateState("Running", false);
                yield return wait;
                UpdateState("Running", true);
            }
            int currentConcurrency = 0;
            foreach (Formula<Conversion> formula in Conversions) {
                if (formula.Count > 0 || formula.Count == -1) {
                    if (PublicMethod.IfHaveEnoughItems(new ItemData[] { new ItemData(formula.Conversion.ItemID, 1) })) {
                        currentConcurrency++;
                        if (currentConcurrency <= Concurrency) {
                            if (formula.Progress < formula.Conversion.ProcessTime) {
                                formula.Progress += Time.deltaTime * ProcessSpeed * ProcessSpeedRatio;
                            } else {
                                formula.Progress = 0;
                                PublicMethod.ConsumeItems(new ItemData[] { new ItemData(formula.Conversion.ItemID, 1) });
                                float generatedAmount = formula.Conversion.ProduceEnergy * ConversionRate * ConversionRateRatio;
                                switch (GeneratedEnergyType) {
                                    case EnergyType.ENERGY:
                                        World.getInstance().addEnergy(generatedAmount);
                                        break;
                                    case EnergyType.FOOD:
                                        World.getInstance().addFoodIn(generatedAmount);
                                        break;
                                }
                                formula.Count--;
                            }
                        } else {
                            break;
                        }
                    }
                }
            }
            yield return 1;
        }
    }

    private bool WaitForAvailable() {
        bool energyFull = false;
        switch (GeneratedEnergyType) {
            case EnergyType.ENERGY:
                energyFull = World.getInstance().getEnergy() >= World.getInstance().getEnergyMax();
                break;
            case EnergyType.FOOD:
                energyFull = World.getInstance().getFoodIn() >= World.getInstance().getFoodInMax();
                break;
        }
        if (energyFull) {
            return false;
        }
        foreach (Formula<Conversion> formula in Conversions) {
            if (formula.Count > 0 || formula.Count == -1) {
                if (PublicMethod.IfHaveEnoughItems(new ItemData[] { new ItemData(formula.Conversion.ItemID, 1) })) {
                    return true;
                }
            }
        }
        return false;
    }
}
