/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/25 14:30:41
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using TTT.UI;
using UnityEngine;
using UnityEngine.UI;

public class FormulaUI_None_1 : MonoBehaviour {

    [SerializeField]
    private Image InputImage;
    [SerializeField]
    private Text InputName;
    [SerializeField]
    private AssetsItemView Output;
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
    public int Time {
        get {
            return _time;
        }
        set {
            _time = value;
            TimeText.text = "" + value;
        }
    }
    public Sprite Input {
        get {
            return InputImage.sprite;
        }
        set {
            InputImage.sprite = value;
        }
    }
    public string Name {
        get {
            return InputName.text;
        }
        set {
            InputName.text = value;
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

    private ItemData _outputItem;
    private int  _time;

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
