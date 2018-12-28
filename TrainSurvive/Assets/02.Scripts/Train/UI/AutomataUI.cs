/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/27 14:52:21
 * 版本：v0.1
 */
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
    [SerializeField]
    private Toggle Toggle;

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
    public bool Enabled {
        get {
            return Toggle.isOn;
        }
        set {
            Toggle.isOn = value;
        }
    }

    public System.Action<bool, int> OnChangeState;

    private void Awake() {
        Up.onClick.AddListener(() => Value++);
        Down.onClick.AddListener(() => { if (Value - 1 >= 0) Value--; });
        Unlimited.onClick.AddListener(() => Value = -1);
        Zero.onClick.AddListener(() => Value = 0);
        Toggle.onValueChanged.AddListener((state) => OnChangeState?.Invoke(state, Value));
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
        Input.onValueChanged.AddListener((s) => OnChangeState?.Invoke(Enabled, Value));
    }

}
