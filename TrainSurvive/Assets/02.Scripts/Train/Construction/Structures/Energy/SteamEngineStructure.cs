/*
 * 描述：蒸汽机
 * 作者：刘旭涛
 * 创建时间：2018/12/4 14:06:13
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class SteamEngineStructure : Structure {

    public SteamEngineStructure() : base() { }
    protected SteamEngineStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        Progress = (float)info.GetValue("Progress", typeof(float));
        Gas = (Item)info.GetValue("Gas", typeof(Item));
        ConversionRatio = (float)info.GetValue("ConversionRatio", typeof(float));
        ProcessRatio = (float)info.GetValue("ProcessRatio", typeof(float));
    }

    private static FixedInfo _info = new FixedInfo {
        Name = "蒸汽机",
        Description = "燃烧各种可燃物（木炭/煤炭、各种有机物等），转化为热能",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 0,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "查看", Action = (structure) => UIManager.Instance?.ShowFaclityUI("SteamEngine", structure) },
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    /// <summary>
    /// 可接受可燃物及默认转化率
    /// </summary>
    public virtual Dictionary<int, int> AcceptableGas { get; } = new Dictionary<int, int> {
        {231, 10 }
    };

    /// <summary>
    /// 处理时间
    /// </summary>
    public virtual float ProcessTime { get; } = 3;

    /// <summary>
    /// 转化率
    /// </summary>
    public float ConversionRatio { get; set; } = 1;

    /// <summary>
    /// 处理时间比例
    /// </summary>
    public float ProcessRatio { get; set; } = 1;

    public float Progress {
        get {
            return _progress;
        }
        private set {
            _progress = value;
            CallOnProgressChange(0, ProcessTime, value);
        }
    }
   
    public Item Gas {
        get {
            _gas = OnAcquireGas == null ? _gas : OnAcquireGas.Invoke();
            return _gas;
        }
        set {
            _gas = value;
        }
    }
    
    public Action OnGasUpdate;
    public Func<Item> OnAcquireGas;

    private float _progress;
    private Item _gas;

    protected override void OnStart() {
        TimeController.getInstance().StartCoroutine(Run());
    }
    
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Progress", Progress);
        info.AddValue("Gas", Gas);
        info.AddValue("ConversionRatio", ConversionRatio);
        info.AddValue("ProcessRatio", ProcessRatio);
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => Gas != null && Gas.currPileNum >= 1 && World.getInstance().energy < World.getInstance().energyMax);
        while (FacilityState == State.WORKING) {
            if (!(Gas != null && Gas.currPileNum >= 1 && World.getInstance().energy < World.getInstance().energyMax)) {
                Progress = 0;
                yield return wait;
            }
            if (Progress < ProcessTime) {
                Progress += Time.deltaTime;
            } else {
                Progress = 0;
                World.getInstance().addEnergy((int)(AcceptableGas[Gas.id] * ConversionRatio));
                Debug.Log("Max: " + World.getInstance().energyMax);
                Debug.Log("Energy: " + World.getInstance().energy);
                if (--Gas.currPileNum == 0) {
                    Gas = null;
                    OnGasUpdate?.Invoke();
                }
            }
            yield return 1;
        }
    }
}
