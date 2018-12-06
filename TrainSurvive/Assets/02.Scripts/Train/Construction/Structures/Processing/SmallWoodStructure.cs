/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 15:11:00
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections.Generic;

[Serializable]
public class SmallWoodStructure : Structure {

    public struct Conversion {
        public float ProcessTime { get; set; }
        public int Id { get; set; }
        public int Produce { get; set; }

        public Conversion(int id, int produce, float processTime) {
            Id = id;
            Produce = produce;
            ProcessTime = processTime;
        }
    }

    public SmallWoodStructure() : base() { }
    protected SmallWoodStructure(SerializationInfo info, StreamingContext context) : base(info, context) {
        Progress = (float)info.GetValue("Progress", typeof(float));
        Raw = (Item)info.GetValue("Raw", typeof(Item));
        Output = (Item)info.GetValue("Output", typeof(Item));
        ConversionRatio = (float)info.GetValue("ConversionRatio", typeof(float));
        ProcessSpeedRatio = (float)info.GetValue("ProcessSpeedRatio", typeof(float));
    }

    private static FixedInfo _info = new FixedInfo {
        Name = "小型制木机",
        Description = "将原木处理成木材，1原木=5木材",
        WorkAll = 0.01f,
        BuildCosts = new Cost[] { },
        SpritePath = "Sprite/map/building-inn",
        Class = 3,
        IsOnceFunction = false,
        Actions = new ButtonAction[]{
            new ButtonAction{Name = "查看", Action = (structure) => UIManager.Instance?.ShowFaclityUI("SmallWoodFactory", structure) },
            new ButtonAction{Name = "拆除", Action = (structure) => structure.FacilityState = State.REMOVING }
        }
    };
    public override FixedInfo Info { get; } = _info;

    /// <summary>
    /// 可接受可燃物及默认转化率
    /// </summary>
    public virtual Dictionary<int, Conversion> AcceptableRaw { get; } = new Dictionary<int, Conversion> {
        {231, new Conversion(231, 5, 3) }
    };
    
    /// <summary>
    /// 转化率
    /// </summary>
    public virtual float ConversionRatio { get; set; } = 1;

    /// <summary>
    /// 处理时间比例
    /// </summary>
    public virtual float ProcessSpeedRatio { get; set; } = 1;

    public float Progress {
        get {
            return _progress;
        }
        private set {
            _progress = value;
            if (Raw == null) {
                CallOnProgressChange(0, 0, value);
            } else {
                CallOnProgressChange(0, AcceptableRaw[Raw.id].ProcessTime, value);
            }
        }
    }

    public Item Raw {
        get {
            _raw = OnAcquireRaw == null ? _raw : OnAcquireRaw.Invoke();
            return _raw;
        }
        set {
            _raw = value;
        }
    }
    public Item Output {
        get {
            _output = OnAcquireOutput == null ? _output : OnAcquireOutput.Invoke();
            return _output;
        }
        set {
            _output = value;
        }
    }

    public Action<Item> OnOutputUpdate;
    public Func<Item> OnAcquireOutput;

    public Action OnRawUpdate;
    public Func<Item> OnAcquireRaw;
    
    private float _progress;
    private Item _output;
    private Item _raw;

    protected override void OnStart() {
        TimeController.getInstance().StartCoroutine(Run());
    }
    
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Progress", Progress);
        info.AddValue("Raw", Raw);
        info.AddValue("Output", Output);
        info.AddValue("ConversionRatio", ConversionRatio);
        info.AddValue("ProcessSpeedRatio", ProcessSpeedRatio);
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => Raw != null && Raw.currPileNum >= 1 && (Output == null || (Output.id == AcceptableRaw[Raw.id].Id && Output.maxPileNum - Output.currPileNum >= (int)(AcceptableRaw[Raw.id].Produce * ConversionRatio))));
        while (FacilityState == State.WORKING) {
            if (!(Raw != null && Raw.currPileNum >= 1 && (Output == null || (Output.id == AcceptableRaw[Raw.id].Id && Output.maxPileNum - Output.currPileNum >= (int)(AcceptableRaw[Raw.id].Produce * ConversionRatio))))) {
                Progress = 0;
                yield return wait;
            }
            if (Progress < AcceptableRaw[Raw.id].ProcessTime) {
                Progress += Time.deltaTime * ProcessSpeedRatio;
            } else {
                Progress = 0;
                if (Output == null) {
                    Item output = PublicMethod.GenerateItem(AcceptableRaw[Raw.id].Id, (int)(AcceptableRaw[Raw.id].Produce * ConversionRatio))[0];
                    Output = output;
                    OnOutputUpdate?.Invoke(output);
                } else {
                    Output.currPileNum += (int)(AcceptableRaw[Raw.id].Produce * ConversionRatio);
                }
                if (--Raw.currPileNum == 0) {
                    Raw = null;
                    OnRawUpdate?.Invoke();
                }
            }
            yield return 1;
        }
    }
}