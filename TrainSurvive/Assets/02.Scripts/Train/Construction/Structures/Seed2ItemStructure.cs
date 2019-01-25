/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/25 14:39:20
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Seed2ItemStructure : EnergyCostableStructure {

    [Serializable]
    public struct Conversion : IConversion {
        public string FromSeedName;
        public int ToItemID;
        public int ToItemNum;
        public float ProcessTime;
        public float GetProcessTime { get { return ProcessTime; } }
    }

    public Seed2ItemStructure(int id) : base(id) { }

    protected Seed2ItemStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        _conversions = (Conversion[])info.GetValue("_conversions", typeof(Conversion[]));
        _processSpeed = (float)info.GetValue("_processSpeed", typeof(float));
        _conversionsList = (List<Formula<Conversion>>)info.GetValue("_conversionsList", typeof(List<Formula<Conversion>>));
        ProcessSpeedRatio = (float)info.GetValue("ProcessSpeedRatio", typeof(float));
        Concurrency = info.GetInt32("Concurrency");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
        info.AddValue("Concurrency", Concurrency);
        info.AddValue("_conversions", _conversions);
        info.AddValue("_processSpeed", _processSpeed);
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
    /// 处理速度
    /// </summary>
    public float ProcessSpeed {
        get {
            return _processSpeed;
        }
    }
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

    private List<Formula<Conversion>> _conversionsList;

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
        WaitUntil wait = new WaitUntil(WaitForAvailable);
        WaitWhile waitCosts = new WaitWhile(() => IsRunningOut);
        while (FacilityState == State.WORKING) {
            if (!WaitForAvailable()) {
                foreach (Formula<Conversion> formula in Conversions) {
                    formula.Progress = 0;
                }
                IsCosting = false;
                yield return wait;
                IsCosting = true;
            }
            yield return waitCosts;
            int currentConcurrency = 0;
            foreach (Formula<Conversion> formula in Conversions) {
                if (formula.Count > 0 || formula.Count == -1) {
                    currentConcurrency++;
                    if (currentConcurrency <= Concurrency) {
                        if (formula.Progress < formula.Conversion.ProcessTime) {
                            formula.Progress += Time.deltaTime * ProcessSpeed * ProcessSpeedRatio;
                        } else {
                            formula.Progress = 0;
                            PublicMethod.AppendItemsInBackEnd(new ItemData[] { new ItemData(formula.Conversion.ToItemID, formula.Conversion.ToItemNum) });
                            formula.Count--;
                        }
                    } else {
                        break;
                    }
                }
            }
            yield return 1;
        }
    }

    private bool WaitForAvailable() {
        foreach (Formula<Conversion> formula in Conversions) {
            if (formula.Count > 0 || formula.Count == -1) {
                return true;
            }
        }
        return false;
    }
}
