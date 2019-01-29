/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/25 12:00:12
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using TTT.UI;
using UnityEngine;
using UnityEngine.UI;

public class FormulaUI_1_Energy : MonoBehaviour {
    
    [SerializeField]
    private Image OutputImage;
    [SerializeField]
    private Text OutputNum;
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
    [SerializeField]
    private Text TimeText;
    [SerializeField]
    private Sprite[] EnergyImage;

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
    public Item_EnergyStructure.EnergyType OutputType {
        get {
            return _outputType;
        }
        set {
            _outputType = value;
            OutputImage.sprite = EnergyImage[(int)value];
        }
    }
    public int Time {
        get {
            return _time;
        }
        set {
            _time = value;
            TimeText.text = "" + value;
        }
    }
    public int OutputCount {
        get {
            return _outputCount;
        }
        set {
            _outputCount = value;
            OutputNum.text = "" + value;
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

    private ItemData _rawItem;
    private Item_EnergyStructure.EnergyType _outputType;
    private int _outputCount, _time;

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
