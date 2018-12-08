/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/5 16:08:56
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WoodFactoryUI : FacilityUI {

    public UnitInventoryCtrl Raw;
    public UnitInventoryCtrl Output;
    public Slider Slider;

    private new SmallWoodStructure Structure {
        get {
            return base.Structure as SmallWoodStructure;
        }
    }

    private void Awake() {
        Raw.ChargeIn = (item) => Structure.AcceptableRaw.ContainsKey(item.id);
        Output.ChargeIn = (item) => false;
    }

    private void OnEnable() {
        UpdateUI();
        Structure.OnAcquireOutput = Output.GetItem;
        Structure.OnOutputUpdate = OnOutputUpdate;
        Structure.OnAcquireRaw = Raw.GetItem;
        Structure.OnRawUpdate = Raw.Clear;
        Structure.OnProgressChange += OnProgressChange;

        UIManager.Instance?.ToggleInventoryPanel(true);
    }

    private void OnDisable() {
        UpdateStructure();
        Structure.OnAcquireOutput = null;
        Structure.OnOutputUpdate = null;
        Structure.OnAcquireRaw = null;
        Structure.OnRawUpdate = null;
        Structure.OnProgressChange -= OnProgressChange;
    }

    private void UpdateUI() {
        Output.Clear();
        Raw.Clear();
        if (Structure.Output != null)
            Output.GeneratorItem(Structure.Output.id, Structure.Output.currPileNum);
        if (Structure.Raw != null)
            Raw.GeneratorItem(Structure.Raw.id, Structure.Raw.currPileNum);
        Slider.value = Structure.Progress;
    }

    private void UpdateStructure() {
        Structure.Raw = Raw.GetItem();
        Structure.Output = Output.GetItem();
    }

    private void OnOutputUpdate(Item output) {
        Output.GeneratorItem(output.id, output.currPileNum);
    }

    private void OnProgressChange(float min, float max, float value) {
        Slider.minValue = min;
        Slider.maxValue = max;
        Slider.value = value;
    }
}
