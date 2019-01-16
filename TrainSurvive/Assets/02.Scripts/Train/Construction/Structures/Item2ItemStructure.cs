/*
 * 描述：物品转物品建筑类型
 * 作者：刘旭涛
 * 创建时间：2018/12/14 11:23:18
 * 版本：v0.1
 */
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

        foreach (Formula formula in _conversionsList) {
            formula.OnProgressChanged += Formula_OnProgressChanged;
        }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("ConversionRateRatio", ConversionRateRatio);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
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
    
    private Coroutine RunningCoroutine { get; set; }

    [StructurePublicField(Tooltip = "转化列表")]
    private Conversion[] _conversions;
    [StructurePublicField(Tooltip = "处理速度")]
    private float _processSpeed;

    private List<Formula> _conversionsList;

    protected override void OnStart() {
        base.OnStart();// TODO
        //RunningCoroutine = TimeController.getInstance().StartCoroutine(Run());
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

    //private IEnumerator Run() {
    //    WaitUntil wait = new WaitUntil(() => Raw != null && Raw.Number >= Conversions[Raw.ID].FromItemNum && (Output == null || (Output.ID == Conversions[Raw.ID].ToItemID && Output.MaxPileNum - Output.Number >= (int)(Conversions[Raw.ID].ToItemNum * ConversionRateRatio))));
    //    WaitWhile waitCosts = new WaitWhile(() => IsRunningOut);
    //    while (FacilityState == State.WORKING) {
    //        if (!(Raw != null && Raw.Number >= 1 && (Output == null || (Output.ID == Conversions[Raw.ID].ToItemID && Output.MaxPileNum - Output.Number >= (int)(Conversions[Raw.ID].ToItemNum * ConversionRateRatio))))) {
    //            Progress = 0;
    //            IsCosting = false;
    //            yield return wait;
    //            IsCosting = true;
    //        }
    //        yield return waitCosts;
    //        if (Progress < Conversions[Raw.ID].ProcessTime) {
    //            Progress += Time.deltaTime * ProcessSpeed * ProcessSpeedRatio;
    //        } else {
    //            Progress = 0;
    //            if (Output == null) {
    //                Output = new ItemData(Conversions[Raw.ID].ToItemID, (int)(Conversions[Raw.ID].ToItemNum * ConversionRateRatio));
    //            } else {
    //                Output.Number += (int)(Conversions[Raw.ID].ToItemNum * ConversionRateRatio);
    //            }
    //            int newNum = Raw.Number - Conversions[Raw.ID].FromItemNum;
    //            if (newNum <= 0) {
    //                Raw = null;
    //            } else {
    //                Raw.Number = newNum;
    //            }
    //            OnOutputUpdate?.Invoke(_output);
    //            OnRawUpdate?.Invoke(_raw);
    //        }
    //        yield return 1;
    //    }
    //}
}
