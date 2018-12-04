/*
 * 描述：发电机
 * 作者：刘旭涛
 * 创建时间：2018/12/4 15:54:52
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GeneratorStructure : Structure {

    public GeneratorStructure() : base() { }
    protected GeneratorStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        Progress = (float)info.GetValue("Progress", typeof(float));
        ConversionRatio = (float)info.GetValue("ConversionRatio", typeof(float));
        ProcessRatio = (float)info.GetValue("ProcessRatio", typeof(float));
        IsRunning = info.GetBoolean("IsRunning");
    }

    private static FixedInfo _info = new FixedInfo {
        Name = "发电机",
        Description = "按功率将核心中的热能转化为蓄电池的电能",
        WorkAll = 0,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 0,
        IsOnceFunction = true,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "关闭", Action = (structure) => (structure as GeneratorStructure).IsRunning = false },
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;
    
    /// <summary>
    /// 处理时间
    /// </summary>
    public virtual float ProcessTime { get; } = 3;

    /// <summary>
    /// 转化率
    /// </summary>
    public virtual float ConversionRate { get; } = 2;

    /// <summary>
    /// 转化率比例
    /// </summary>
    public float ConversionRatio { get; set; } = 1;

    /// <summary>
    /// 处理时间比例
    /// </summary>
    public float ProcessRatio { get; set; } = 1;

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning {
        get {
            return _isRunning;
        }
        set {
            if (_isRunning == value)
                return;
            if (value) {
                RunningCoroutine = TimeController.getInstance().StartCoroutine(Run());
            } else {
                TimeController.getInstance().StopCoroutine(RunningCoroutine);
            }
            _isRunning = value;
        }
    }

    public float Progress {
        get {
            return _progress;
        }
        private set {
            _progress = value;
            CallOnProgressChange(0, ProcessTime, value);
        }
    }

    private Coroutine RunningCoroutine { get; set; }

    private static ButtonAction[] Actions { get; } = {
        new ButtonAction{Name = "开启", Action = (structure) => (structure as GeneratorStructure).IsRunning = true },
        new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
    };

    private float _progress;
    private bool _isRunning;

    protected override void OnStart() {
        IsRunning = true;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Progress", Progress);
        info.AddValue("ConversionRatio", ConversionRatio);
        info.AddValue("ProcessRatio", ProcessRatio);
        info.AddValue("IsRunning", IsRunning);
    }

    public override ButtonAction[] GetActions() {
        if (IsRunning) {
            return base.GetActions();
        } else {
            return Actions;
        }
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => World.getInstance().energy < World.getInstance().energyMax);
        while (FacilityState == State.WORKING) {
            // TODO
            //if (!(Gas != null && Gas.currPileNum >= 1 && World.getInstance().energy < World.getInstance().energyMax)) {
            //    Progress = 0;
            //    yield return wait;
            //}
            //if (Progress < ProcessTime) {
            //    Progress += Time.deltaTime;
            //} else {
            //    Progress = 0;
            //    World.getInstance().addEnergy((int)(AcceptableGas[Gas.id] * ConversionRatio));
            //    Debug.Log("Max: " + World.getInstance().energyMax);
            //    Debug.Log("Energy: " + World.getInstance().energy);
            //    if (--Gas.currPileNum == 0) {
            //        Gas = null;
            //        OnGasUpdate?.Invoke();
            //    }
            //}
            yield return 1;
        }
    }
}
