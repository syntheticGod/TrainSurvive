/*
 * 描述：用于测试的建筑设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:11:02
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TEST_Facility : Facility {
    public override string Name { get; } = "Bili";
    public override float WorkAll { get; } = 5;
    public override Cost[] BuildCosts { get; } = {
        new Cost(){ItemID = 0, Count = 0}
    };
    
    [SerializeField]
    private int[] AcceptableRawFoods;
    [SerializeField]
    private int[] AcceptableGas;

    public float ProcessTime { get; } = 3;
    public float Progress {
        get {
            return Indicator.Progress;
        }
        protected set {
            Indicator.Progress = value;
        }
    }

    private Test_FacilityUI _facilityUI;
    private Test_FacilityUI facilityUI {
        get {
            if (_facilityUI == null) {
                _facilityUI = facilityUIObj.GetComponent<Test_FacilityUI>();
                _facilityUI.RawFood.ChargeIn = (item) => {
                    Debug.Log("Raw: " + item.currPileNum);
                    // Array.BinarySearch(AcceptableRawFoods, item.id);
                    return true;
                };
                _facilityUI.Gas.ChargeIn = (item) => {
                    Debug.Log("Gas: " + item.currPileNum);
                    // Array.BinarySearch(AcceptableGas, item.id);
                    return true;
                };
                _facilityUI.Food.ChargeIn = (item) => { return false; };
            }
            return _facilityUI;
        }
    }

    protected override void Awake() {
        base.Awake();
        Array.Sort(AcceptableRawFoods);
        Array.Sort(AcceptableGas);
    }

    protected override void OnStart() {
        StartCoroutine(run());
    }
    
    protected override void OnInitFacilityUI() {
        uiManager.ToggleInventoryPanel(true);
    }

    private IEnumerator run() {
        Indicator.ShowProgress(0, ProcessTime, Progress);
        WaitUntil wait = new WaitUntil(() => facilityUI.Gas.CanConsume(1) && facilityUI.RawFood.CanConsume(1) && facilityUI.Food.CanGeneratorItem(200, 1));
        while (FacilityState == State.WORKING) {
            if (!facilityUI.Gas.CanConsume(1) || !facilityUI.RawFood.CanConsume(1) || !facilityUI.Food.CanGeneratorItem(200, 1)) {
                Indicator.HideProgress();
                Progress = 0;
                yield return wait;
                Indicator.ShowProgress(0, ProcessTime, Progress);
            }
            if (Progress < ProcessTime) {
                Progress += Time.deltaTime;
            } else {
                Progress = 0;
                facilityUI.Gas.Consume(1);
                facilityUI.RawFood.Consume(1);
                facilityUI.Food.GeneratorItem(200, 1);
            }
            yield return 1;
        }
        Indicator.HideProgress();
    }
}
