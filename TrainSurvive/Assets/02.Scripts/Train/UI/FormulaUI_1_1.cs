/*
 * 描述：1个物品加工1个物品的合成配方条目
 * 作者：刘旭涛
 * 创建时间：2019/1/16 15:07:48
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WorldMap.UI;

public class FormulaUI_1_1 : MonoBehaviour {

    [SerializeField]
    private AssetsItemView Output;
    [SerializeField]
    private AssetsItemView Raw;
    [SerializeField]
    private Slider Slider;
    [SerializeField]
    private Button Up;
    [SerializeField]
    private Button Down;
    [SerializeField]
    private AutomataUI AutomataUI;

    public ItemData RawItem {
        get {
            return _rawItem;
        }
        set {
            _rawItem = value;
            Raw.Clear();
            Raw.SetItemData(value.ID, value.Number);
        }
    }
    public ItemData OutputItem {
        get {
            return _outputItem;
        }
        set {
            _outputItem = value;
            Output.Clear();
            Output.SetItemData(value.ID, value.Number);
        }
    }
    public int ProduceCount {
        get {
            return AutomataUI.Value;
        }
        set {
            AutomataUI.Value = value;
        }
    }
    
    public event Action<int> OnPriorityChanged;

    private ItemData _rawItem, _outputItem;

    private void Awake() {
        Up.onClick.AddListener(() => OnPriorityChanged?.Invoke(-1));
        Down.onClick.AddListener(() => OnPriorityChanged?.Invoke(1));
    }
    
    public void ChangeProgress(float min, float max, float value) {
        Slider.minValue = min;
        Slider.maxValue = max;
        Slider.value = value;
    }
}
