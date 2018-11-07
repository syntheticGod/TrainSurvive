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

    private float rawProgress;
    private float gasProgress;
    private Item raw;
    private Item gas;

    protected override void Awake() {
        base.Awake();
        Array.Sort(AcceptableRawFoods);
        Array.Sort(AcceptableGas);
    }

    protected override void OnStart() {
        StartCoroutine(run());
    }
    
    protected override void OnInitFacilityUI(GameObject facilityUI) {
        uiManager.ToggleInventoryPanel(true);
        Test_FacilityUI ui = facilityUI.GetComponent<Test_FacilityUI>();
        ui.RawFood.ChargeIn = (item) => {
            Debug.Log("Raw: " + item.currPileNum);
            raw = item;
            rawProgress = item.currPileNum;
            // Array.BinarySearch(AcceptableRawFoods, item.id);
            return true;
        };
        ui.Gas.ChargeIn = (item) => {
            Debug.Log("Gas: " + item.currPileNum);
            gas = item;
            gasProgress = item.currPileNum;
            // Array.BinarySearch(AcceptableGas, item.id);
            return true;
        };
        ui.Food.ChargeIn = (item) => { return false; };
    }

    private IEnumerator run() {
        while (true) {
            if (rawProgress > 0 && gasProgress > 0) {
                rawProgress -= Time.deltaTime;
                gasProgress -= Time.deltaTime;
            }
            yield return 1;
        }
    }
}
