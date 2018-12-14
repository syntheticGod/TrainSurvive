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

    public Item2ItemStructure(int id) : base(id) { }

    protected Item2ItemStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _conversions = (Conversion[])info.GetValue("_conversions", typeof(Conversion[]));
        _processSpeed = (float)info.GetValue("_processSpeed", typeof(float));
        Progress = (float)info.GetValue("Progress", typeof(float));
        Raw = (ItemData)info.GetValue("Raw", typeof(ItemData));
        Output = (ItemData)info.GetValue("Output", typeof(ItemData));
        ConversionRateRatio = (float)info.GetValue("ConversionRateRatio", typeof(float));
        ProcessSpeedRatio = (float)info.GetValue("ProcessSpeedRatio", typeof(float));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Progress", Progress);
        info.AddValue("Raw", Raw);
        info.AddValue("Output", Output);
        info.AddValue("ConversionRateRatio", ConversionRateRatio);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("_conversions", _conversions);
        info.AddValue("_processSpeed", _processSpeed);
    }

    /// <summary>
    /// 转化列表
    /// </summary>
    public Dictionary<int, Conversion> Conversions {
        get {
            if (_conversionsDic == null) {
                _conversionsDic = new Dictionary<int, Conversion>();
                foreach (Conversion conversion in _conversions) {
                    _conversionsDic.Add(conversion.FromItemID, conversion);
                }
            }
            return _conversionsDic;
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
    /// 处理进度
    /// </summary>
    public float Progress {
        get {
            return _progress;
        }
        private set {
            _progress = value;
            if (Raw == null) {
                CallOnProgressChange(0, 0, value);
            } else {
                CallOnProgressChange(0, Conversions[Raw.id].ProcessTime, value);
            }
        }
    }
    /// <summary>
    /// 材料
    /// </summary>
    public ItemData Raw {
        get {
            OnAcquireRaw?.Invoke(ref _raw);
            return _raw;
        }
        set {
            _raw = value;
        }
    }
    /// <summary>
    /// 输出
    /// </summary>
    public ItemData Output {
        get {
            OnAcquireOutput?.Invoke(ref _output);
            return _output;
        }
        set {
            _output = value;
        }
    }

    public Action<ItemData> OnRawUpdate;
    public Action<ItemData> OnOutputUpdate;
    public RefAction<ItemData> OnAcquireRaw;
    public RefAction<ItemData> OnAcquireOutput;

    private Coroutine RunningCoroutine { get; set; }

    [StructurePublicField(Tooltip = "转化列表")]
    private Conversion[] _conversions;
    [StructurePublicField(Tooltip = "处理速度")]
    private float _processSpeed;

    private Dictionary<int, Conversion> _conversionsDic;
    private float _progress;
    private ItemData _raw;
    private ItemData _output;

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
        actions.AddFirst(new ButtonAction("操作", (structure) => UIManager.Instance?.ShowFaclityUI("sui_" + structure.ID, structure)));
        return actions;
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => Raw != null && Raw.num >= Conversions[Raw.id].FromItemNum && (Output == null || (Output.id == Conversions[Raw.id].ToItemID && Output.item.maxPileNum - Output.num >= (int)(Conversions[Raw.id].ToItemNum * ConversionRateRatio))));
        WaitWhile waitCosts = new WaitWhile(() => IsRunningOut);
        while (FacilityState == State.WORKING) {
            if (!(Raw != null && Raw.num >= 1 && (Output == null || (Output.id == Conversions[Raw.id].ToItemID && Output.item.maxPileNum - Output.num >= (int)(Conversions[Raw.id].ToItemNum * ConversionRateRatio))))) {
                Progress = 0;
                IsCosting = false;
                yield return wait;
                IsCosting = true;
            }
            yield return waitCosts;
            if (Progress < Conversions[Raw.id].ProcessTime) {
                Progress += Time.deltaTime * ProcessSpeed * ProcessSpeedRatio;
            } else {
                Progress = 0;
                if (Output == null) {
                    Output = new ItemData(Conversions[Raw.id].ToItemID, (int)(Conversions[Raw.id].ToItemNum * ConversionRateRatio));
                } else {
                    Output.num += (int)(Conversions[Raw.id].ToItemNum * ConversionRateRatio);
                }
                int newNum = Raw.num - Conversions[Raw.id].FromItemNum;
                if (newNum <= 0) {
                    Raw = null;
                } else {
                    Raw.num = newNum;
                }
                OnOutputUpdate?.Invoke(_output);
                OnRawUpdate?.Invoke(_raw);
            }
            yield return 1;
        }
    }
}
