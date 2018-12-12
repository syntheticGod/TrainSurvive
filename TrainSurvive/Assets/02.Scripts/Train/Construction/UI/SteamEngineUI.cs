/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/4 14:40:29
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteamEngineUI : FacilityUI {
    
    //public UnitInventoryCtrl Gas;
    //public Slider Slider;

    //private new SteamEngineStructure Structure {
    //    get {
    //        return base.Structure as SteamEngineStructure;
    //    }
    //}

    //private void Awake() {
    //    Gas.ChargeIn = (item) => Structure.AcceptableGas.ContainsKey(item.id);
    //}

    //private void OnEnable() {
    //    UpdateUI();
    //    Structure.OnAcquireGas = Gas.GetItem;
    //    Structure.OnGasUpdate = Gas.Clear;
    //    Structure.OnProgressChange += OnProgressChange;

    //    UIManager.Instance?.ToggleInventoryPanel(true);
    //}

    //private void OnDisable() {
    //    UpdateStructure();
    //    Structure.OnAcquireGas = null;
    //    Structure.OnGasUpdate = null;
    //    Structure.OnProgressChange -= OnProgressChange;
    //}

    //private void UpdateUI() {
    //    Gas.Clear();
    //    if (Structure.Gas != null)
    //        Gas.GeneratorItem(Structure.Gas.id, Structure.Gas.currPileNum);
    //    Slider.value = Structure.Progress;
    //}

    //private void UpdateStructure() {
    //    Structure.Gas = Gas.GetItem();
    //}

    //private void OnProgressChange(float min, float max, float value) {
    //    Slider.minValue = min;
    //    Slider.maxValue = max;
    //    Slider.value = value;
    //}
}
