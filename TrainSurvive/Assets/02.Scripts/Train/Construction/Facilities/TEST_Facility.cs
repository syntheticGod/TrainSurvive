/*
 * 描述：用于测试的建筑设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:11:02
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class TEST_Facility : Structure {

    private static FixedInfo _info = new FixedInfo {
        Name = "TEST_Facility",
        Description = "TEST_TEST_TEST",
        WorkAll = 2,
        BuildCosts = new Cost[]{
            new Cost{ ItemID = 0, Value = 0 }
        },
        SpritePath = "Sprite/map/building-inn"
    };

    public override FixedInfo Info { get; } = _info;
    
    private static int[] AcceptableRawFoods { get; } = { 0 };
    private static int[] AcceptableGas { get; } = { 0 };

    public float ProcessTime { get; } = 3;
    public float Progress {
        get {
            return _progress;
        }
        private set {
            _progress = value;
            CallOnProgressChange(0, ProcessTime, value);
        }
    }
    public Item Raw {
        get {
            _raw = OnAcquireRaw == null ? _raw : OnAcquireRaw.Invoke();
            return _raw;
        }
        private set {
            _raw = value;
            OnRawUpdate?.Invoke(_raw);
        }
    }
    public Item Gas {
        get {
            _gas = OnAcquireGas == null ? _gas : OnAcquireGas.Invoke();
            return _gas;
        }
        private set {
            _gas = value;
            OnGasUpdate?.Invoke(_gas);
        }
    }
    public Item Food {
        get {
            _food = OnAcquireFood == null ? _food : OnAcquireFood.Invoke();
            return _food;
        }
        private set {
            _food = value;
            OnFoodUpdate?.Invoke(_food);
        }
    }
    
    public Action<Item> OnFoodUpdate;
    public Func<Item> OnAcquireFood;

    public Action<Item> OnRawUpdate;
    public Func<Item> OnAcquireRaw;

    public Action<Item> OnGasUpdate;
    public Func<Item> OnAcquireGas;

    private float _progress;
    private Item _food;
    private Item _raw;
    private Item _gas;

    protected override void OnStart() {
        TimeController.getInstance().StartCoroutine(Run());
    }

    public TEST_Facility() : base() { }

    public TEST_Facility(SerializationInfo info, StreamingContext context) : base(info, context) {
        Progress = (float)info.GetValue("Progress", typeof(float));
        Raw = (Item)info.GetValue("Raw", typeof(Item));
        Gas = (Item)info.GetValue("Gas", typeof(Item));
        Food = (Item)info.GetValue("Food", typeof(Item));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("Progress", Progress);
        info.AddValue("Raw", Raw);
        info.AddValue("Gas", Gas);
        info.AddValue("Food", Food);
    }

    private IEnumerator Run() {
        WaitUntil wait = new WaitUntil(() => Gas != null && Raw != null && Gas.currPileNum >= 1 && Raw.currPileNum >= 1 && (Food == null || Food.currPileNum < Food.maxPileNum));
        while (FacilityState == State.WORKING) {
            if (!(Gas != null && Raw != null && Gas.currPileNum >= 1 && Raw.currPileNum >= 1 && (Food == null || Food.currPileNum < Food.maxPileNum))) {
                Progress = 0;
                yield return wait;
            }
            if (Progress < ProcessTime) {
                Progress += Time.deltaTime;
            } else {
                Progress = 0;
                if(--Gas.currPileNum == 0) {
                    Gas = null;
                }
                if (--Raw.currPileNum == 0) {
                    Raw = null;
                }
                if (Food == null) {
                    Food = new Assets._02.Scripts.zhxUIScripts.Material(0);
                } else {
                    Food.currPileNum++;
                }
            }
            yield return 1;
        }
    }
}
