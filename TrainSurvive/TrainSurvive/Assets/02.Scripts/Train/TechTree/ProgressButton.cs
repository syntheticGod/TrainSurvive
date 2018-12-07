/*
 * 描述：带进度条的按钮
 * 作者：刘旭涛
 * 创建时间：2018/11/27 22:54:43
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressButton : MonoBehaviour {

    private RectTransform Transform {
        get {
            return transform as RectTransform;
        }
    }

    private RectTransform _maskRect;
    private RectTransform MaskRect {
        get {
            if (_maskRect == null) {
                _maskRect = Transform.Find("Mask") as RectTransform;
            }
            return _maskRect;
        }
    }

    private Button _maskButton;
    private Button MaskButton {
        get {
            if(_maskButton == null) {
                _maskButton = MaskRect.GetComponent<Button>();
            }
            return _maskButton;
        }
    }

    private Text _text;
    public Text Text {
        get {
            if (_text == null) {
                _text = Transform.Find("Text").GetComponent<Text>();
            }
            return _text;
        }
    }
    
    private Button _button;
    private Button Button {
        get {
            if (_button == null) {
                _button = GetComponent<Button>();
            }
            return _button;
        }
    }

    private Image _buttonImage;
    private Image ButtonImage {
        get {
            if (_buttonImage == null) {
                _buttonImage = GetComponent<Image>();
            }
            return _buttonImage;
        }
    }

    private Image _maskImage;
    private Image MaskImage {
        get {
            if (_maskImage == null) {
                _maskImage = MaskRect.GetComponent<Image>();
            }
            return _maskImage;
        }
    }

    public UnityAction Action { get; set; }

    public float MaxValue { get; set; }

    private float _value;
    public float Value {
        get {
            return _value;
        }
        set {
            _value = value;
            MaskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(value / MaxValue) * Transform.rect.width);
        }
    }

    public bool IsEnabled {
        get {
            return Button.interactable;
        }
        set {
            Button.interactable = value;
            if (value) {
                ColorBlock colorBlock = MaskButton.colors;
                colorBlock.disabledColor = MaskTransactionColor;
                MaskButton.colors = colorBlock;
            } else {
                ColorBlock colorBlock = MaskButton.colors;
                colorBlock.disabledColor = Button.colors.disabledColor;
                MaskButton.colors = colorBlock;
            }
        }
    }

    public Color ButtonColor {
        get {
            return ButtonImage.color;
        }
        set {
            ButtonImage.color = value;
        }
    }

    public Color MaskColor {
        get {
            return MaskImage.color;
        }
        set {
            MaskImage.color = value;
        }
    }

    private Outline _outline;
    public Outline Outline {
        get {
            if (_outline == null)
                _outline = GetComponent<Outline>();
            return _outline;
        }
    }

    private Color? _maskTransactionColor;
    private Color MaskTransactionColor {
        get {
            if (_maskTransactionColor == null) {
                _maskTransactionColor = Button.colors.normalColor;
            }
            return _maskTransactionColor.Value;
        }
        set {
            _maskTransactionColor = value;
        }
    }
    
    public void OnClick() {
        Action?.Invoke();
    }

    public void OnPointerEnterOrUp() {
        MaskTransactionColor = Button.colors.highlightedColor;
        if (!IsEnabled)
            return;
        ColorBlock colorBlock = MaskButton.colors;
        colorBlock.disabledColor = MaskTransactionColor;
        MaskButton.colors = colorBlock;
    }
    public void OnPointerExit() {
        MaskTransactionColor = Button.colors.normalColor;
        if (!IsEnabled)
            return;
        ColorBlock colorBlock = MaskButton.colors;
        colorBlock.disabledColor = MaskTransactionColor;
        MaskButton.colors = colorBlock;
    }
    public void OnPointerDown() {
        MaskTransactionColor = Button.colors.pressedColor;
        if (!IsEnabled)
            return;
        ColorBlock colorBlock = MaskButton.colors;
        colorBlock.disabledColor = MaskTransactionColor;
        MaskButton.colors = colorBlock;
    }

}
