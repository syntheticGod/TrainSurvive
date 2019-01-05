/*
 * 描述：物品转物品建筑UI
 * 作者：刘旭涛
 * 创建时间：2018/12/5 16:08:56
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item2ItemSUI : FacilityUI {

    public UnitInventoryCtrl Output;
    public UnitInventoryCtrl Raw;
    public Slider Slider;

    private new Item2ItemStructure Structure {
        get {
            return base.Structure as Item2ItemStructure;
        }
    }

    private void Awake() {
        Raw.ChargeIn = (item) => Structure.Conversions.ContainsKey(item.id);
        Output.ChargeIn = (item) => false;
    }

    private void OnEnable() {
        UpdateUI();
        Structure.OnAcquireRaw = OnAcquireRaw;
        Structure.OnRawUpdate = OnRawUpdate;
        Structure.OnAcquireOutput = OnAcquireOutput;
        Structure.OnOutputUpdate = OnOutputUpdate;
        Structure.OnProgressChange += OnProgressChange;

        UIManager.Instance?.ToggleInventoryPanel(true);
    }

    private void OnDisable() {
        Structure.OnAcquireRaw = null;
        Structure.OnRawUpdate = null;
        Structure.OnAcquireOutput = null;
        Structure.OnOutputUpdate = null;
        Structure.OnProgressChange -= OnProgressChange;
    }

    private void UpdateUI() {
        Raw.Clear();
        if (Structure.Raw != null)
            Raw.GeneratorItem(Structure.Raw.ID, Structure.Raw.Number);
        Output.Clear();
        if (Structure.Output != null)
            Output.GeneratorItem(Structure.Output.ID, Structure.Output.Number);
        Slider.value = Structure.Progress;
    }

    private void OnAcquireRaw(ref ItemData old) {
        Item item = Raw.GetItem();
        if (item == null) { // item空
            old = null;
        } else {
            if (old == null) { // item不空，old空
                old = new ItemData(item.id, item.currPileNum);
            } else {
                if (old.ID == item.id) {  // item不空，old不空，item与old的id相同
                    old.Number = item.currPileNum;
                } else {  // item不空，old不空，item与old的id不同
                    old = new ItemData(item.id, item.currPileNum);
                }
            }
        }
    }

    private void OnRawUpdate(ItemData newItem) {
        if (newItem == null) {
            Raw.Clear();
        } else {
            Raw.GetItem().currPileNum = newItem.Number;
        }
    }

    private void OnAcquireOutput(ref ItemData old) {
        Item item = Output.GetItem();
        if (item == null) { // item空
            old = null;
        }
    }

    private void OnOutputUpdate(ItemData newItem) {
        if (Output.GetItem() == null) { // item不空，old空
            Output.GeneratorItem(newItem.ID, newItem.Number);
        } else {
            if (Output.GetItem().id == newItem.ID) {  // item不空，old不空，item与old的id相同
                Output.GetItem().currPileNum = newItem.Number;
            } else {  // item不空，old不空，item与old的id不同
                Output.Clear();
                Output.GeneratorItem(newItem.ID, newItem.Number);
            }
        }
    }

    private void OnProgressChange(float min, float max, float value) {
        Slider.minValue = min;
        Slider.maxValue = max;
        Slider.value = value;
    }
}
