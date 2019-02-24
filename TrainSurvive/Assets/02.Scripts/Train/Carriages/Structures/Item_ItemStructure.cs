/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/2 14:10:06
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Item_ItemStructure : CarriageStructure {

    [Serializable]
    public struct Conversion : IConversion {
        public int FromItemID;
        public int FromItemNum;
        public int ToItemID;
        public int ToItemNum;
        public float ProcessTime;
        public int UnlockUpgradeID;
        public float GetProcessTime { get { return ProcessTime; } }
    }

    public Item_ItemStructure(string name, bool initialEnabled) : base(name, initialEnabled) { }

    protected Item_ItemStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
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

    public override void OnStart() {
        base.OnStart();
        RunningCoroutine = TimeController.getInstance().StartCoroutine(Run());
    }

    public override void OnUpgraded(CarriageResearchSetting upgrade) {
        base.OnUpgraded(upgrade);
        string[] parameters = upgrade.Parameter.Split('|');
        if (parameters.Length != 2) {
            Debug.LogError("第" + upgrade.ID + "号升级所需参数为([float]ProcessSpeed|[int]Concurrency)");
            return;
        }
        
        if (parameters[0].Length > 0) {
            float rate = float.Parse(parameters[0]);
            ProcessSpeedRatio = rate;
        }
        if (parameters[1].Length > 0) {
            int value = int.Parse(parameters[1]);
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
                if ((formula.Conversion.UnlockUpgradeID == -1 || CarriageBackend.UpgradedID.Contains(formula.Conversion.UnlockUpgradeID)) && (formula.Count > 0 || formula.Count == -1)) {
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
        foreach (Formula<Conversion> formula in Conversions) {
            if ((formula.Conversion.UnlockUpgradeID == -1 || CarriageBackend.UpgradedID.Contains(formula.Conversion.UnlockUpgradeID)) && (formula.Count > 0 || formula.Count == -1)) {
                if (PublicMethod.IfHaveEnoughItems(new ItemData[] { new ItemData(formula.Conversion.FromItemID, formula.Conversion.FromItemNum) })) {
                    return true;
                }
            }
        }
        return false;
    }
}
