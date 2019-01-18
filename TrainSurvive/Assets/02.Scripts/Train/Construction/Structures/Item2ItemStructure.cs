/*
 * 描述：物品转物品建筑类型
 * 作者：刘旭涛
 * 创建时间：2018/12/14 11:23:18
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Item2ItemStructure : EnergyCostableStructure {

    [Serializable]
    public struct Conversion {
        public int FromItemID;
        public int FromItemNum;
        public int ToItemID;
        public int ToItemNum;
        public float ProcessTime;
    }
    
    [Serializable]
    public class Formula : ISerializable {
        public Conversion Conversion { get; set; }
        public float Progress {
            get {
                return _progress;
            }
            set {
                _progress = value;
                OnProgressChanged?.Invoke(Priority, Conversion.ProcessTime, value);
            }
        }
        public int Count {
            get {
                _count = OnAcquireCount?.Invoke(Priority) ?? _count;
                return _count;
            }
            set {
                _count = value;
                OnCountChanged?.Invoke(Priority, value);
            }
        }
        public int Priority { get; set; }

        public event Action<int, float, float> OnProgressChanged;
        public event Action<int, int> OnCountChanged;
        public event Func<int, int> OnAcquireCount;

        private float _progress;
        private int _count;
        
        public Formula(Conversion conversion, int priority) {
            Conversion = conversion;
            Priority = priority;
        }

        private Formula(SerializationInfo info, StreamingContext context) {
            Conversion = (Conversion)info.GetValue("Conversion", typeof(Conversion));
            Progress = info.GetSingle("Progress");
            Count = info.GetInt32("Count");
            Priority = info.GetInt32("Priority");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Conversion", Conversion);
            info.AddValue("Progress", Progress);
            info.AddValue("Count", Count);
            info.AddValue("Priority", Priority);
        }
    }

    public Item2ItemStructure(int id) : base(id) { }

    protected Item2ItemStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _conversions = (Conversion[])info.GetValue("_conversions", typeof(Conversion[]));
        _processSpeed = (float)info.GetValue("_processSpeed", typeof(float));
        _conversionsList = (List<Formula>)info.GetValue("_conversionsList", typeof(List<Formula>));
        ConversionRateRatio = (float)info.GetValue("ConversionRateRatio", typeof(float));
        ProcessSpeedRatio = (float)info.GetValue("ProcessSpeedRatio", typeof(float));
        Concurrency = info.GetInt32("Concurrency");

        foreach (Formula formula in _conversionsList) {
            formula.OnProgressChanged += Formula_OnProgressChanged;
        }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("ConversionRateRatio", ConversionRateRatio);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("Concurrency", Concurrency);
        info.AddValue("_conversions", _conversions);
        info.AddValue("_processSpeed", _processSpeed);
        info.AddValue("_conversionsList", Conversions);
    }

    /// <summary>
    /// 配方列表
    /// </summary>
    public List<Formula> Conversions {
        get {
            if (_conversionsList == null) {
                _conversionsList = new List<Formula>();
                foreach (Conversion conversion in _conversions) {
                    _conversionsList.Add(new Formula(conversion, _conversionsList.Count));
                }
            }
            return _conversionsList;
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
    [StructurePublicField(Tooltip = "处理速度")]
    private float _processSpeed;

    private List<Formula> _conversionsList;

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

    private void Formula_OnProgressChanged(int priority, float max, float progress) {
        if (progress == 0) {
            CallOnProgressChange(0, 0, 0);
        } else {
            CallOnProgressChange(0, max, progress);
        }
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(WaitForAvailable);
        WaitWhile waitCosts = new WaitWhile(() => IsRunningOut);
        while (FacilityState == State.WORKING) {
            if (!WaitForAvailable()) {
                IsCosting = false;
                yield return wait;
                IsCosting = true;
            }
            yield return waitCosts;
            int currentConcurrency = 0;
            foreach (Formula formula in Conversions) {
                if (formula.Count > 0 || formula.Count == -1) {
                    if (PublicMethod.IfHaveEnoughItems(new ItemData[] { new ItemData(formula.Conversion.FromItemID, formula.Conversion.FromItemNum) })) {
                        currentConcurrency++;
                        if (currentConcurrency <= Concurrency) {
                            if (formula.Progress < formula.Conversion.ProcessTime) {
                                formula.Progress += Time.deltaTime * ProcessSpeed * ProcessSpeedRatio;
                            } else {
                                formula.Progress = 0;
                                PublicMethod.ConsumeItems(new ItemData[] { new ItemData(formula.Conversion.FromItemID, formula.Conversion.FromItemNum) });
                                PublicMethod.AppendItemsInBackEnd(new ItemData[] { new ItemData(formula.Conversion.ToItemID, formula.Conversion.ToItemNum) });
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
        foreach (Formula formula in Conversions) {
            if (formula.Count > 0 || formula.Count == -1) {
                if (PublicMethod.IfHaveEnoughItems(new ItemData[] { new ItemData(formula.Conversion.FromItemID, formula.Conversion.FromItemNum) })) {
                    return true;
                }
            }
        }
        return false;
    }
}
