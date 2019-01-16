/*
 * 描述：自动化控制器
 * 作者：刘旭涛
 * 创建时间：2018/12/27 14:52:21
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AutomataUI : MonoBehaviour {

    [SerializeField]
    private InputField Input;
    [SerializeField]
    private Button Up;
    [SerializeField]
    private Button Down;
    [SerializeField]
    private Button Unlimited;
    [SerializeField]
    private Button Zero;

    /// <summary>
    /// 当前数值，负数表示无限
    /// </summary>
    public int Value {
        get {
            int value;
            if(int.TryParse(Input.text, out value)) {
                return value;
            } else {
                return -1;
            }
        }
        set {
            if (value < 0) {
                Input.text = "∞";
            } else {
                Input.text = value + "";
            }
        }
    }

    public event Action<int> OnValueChanged;

    private void Awake() {
        Up.onClick.AddListener(() => { Value++; OnValueChanged?.Invoke(Value); });
        Down.onClick.AddListener(() => { if (Value - 1 >= 0) { Value--; OnValueChanged?.Invoke(Value); } });
        Unlimited.onClick.AddListener(() => { if (Value != -1) { Value = -1; OnValueChanged?.Invoke(Value); } });
        Zero.onClick.AddListener(() => { if (Value != 0) { Value = 0; OnValueChanged?.Invoke(Value); } });
        Input.onValidateInput = (text, index, addedChar) => {
            if (text.Length == 0 && addedChar == '∞') {
                return addedChar;
            }
            if (text.Length > 0 && index == 0 && addedChar == '0') {
                return '\0';
            }
            if (char.IsNumber(addedChar)) {
                return addedChar;
            }
            return '\0';
        };
        Input.onValueChanged.AddListener((text) => {
            OnValueChanged?.Invoke(Value);
        });
    }

}
