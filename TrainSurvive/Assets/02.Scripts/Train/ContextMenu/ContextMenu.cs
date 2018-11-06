/*
 * 描述：列车上设施右键上下文菜单
 * 作者：刘旭涛
 * 创建时间：2018/10/30 15:53:07
 * 版本：v0.1
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContextMenu {
    
    private class Pair {
        public string Text { get; set; }
        public UnityAction OnClick { get; set; }
    }

    private static GameObject _canvasPrefab;
    private static GameObject canvasPrefab {
        get {
            if (_canvasPrefab == null) {
                _canvasPrefab = Resources.Load<GameObject>("Prefabs/Train/ContextMenu/Canvas");
            }
            return _canvasPrefab;
        }
    }

    private static GameObject _buttonPrefab;
    private static GameObject buttonPrefab {
        get {
            if (_buttonPrefab == null) {
                _buttonPrefab = Resources.Load<GameObject>("Prefabs/Train/ContextMenu/Button");
            }
            return _buttonPrefab;
        }
    }

    private List<Pair> buttons = new List<Pair>();
    private GameObject canvasGO;

    /// <summary>
    /// 在菜单中添加/更新/删除一个按钮。
    /// </summary>
    /// <param name="text">按钮文本</param>
    /// <param name="index">显示位置，从0开始，如果为负数则保持原优先级（更新时）或放在队列末尾（添加时）。</param>
    /// <param name="onClick">按钮事件，如果为null则从列表中删除该项。</param>
    public void PutButton(string text, int index, UnityAction onClick) {
        for (int i = 0; i < buttons.Count; i++) {
            if (buttons[i].Text == text) {
                if (onClick == null) {
                    // 删除
                    buttons.RemoveAt(i);
                } else {
                    if (index < 0) {
                        // 更新
                        buttons[i].OnClick = onClick;
                    } else {
                        // 重排
                        buttons.RemoveAt(i);
                        break;
                    }
                }
                return;
            }
        }
        if (onClick != null) {
            Pair pair = new Pair() { Text = text, OnClick = onClick };
            if (index < 0 || index >= buttons.Count) {
                buttons.Add(pair);
            } else {
                buttons.Insert(index, pair);
            }
        }
    }
    
    /// <summary>
    /// 绘制一个菜单
    /// </summary>
    /// <param name="point">菜单的位置</param>
    public void Render(Vector2 point) {
        canvasGO = Object.Instantiate(canvasPrefab);
        RectTransform rectTransform = canvasGO.GetComponent<RectTransform>();
        rectTransform.SetPositionAndRotation(point, Quaternion.identity);

        RectTransform panel = rectTransform.GetChild(0) as RectTransform;
        VerticalLayoutGroup verticalLayoutGroup = panel.GetComponent<VerticalLayoutGroup>();
        Rect buttonRect = buttonPrefab.GetComponent<RectTransform>().rect;
        float canvasHeight = buttons.Count * buttonRect.height + verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom + (buttons.Count - 1) * verticalLayoutGroup.spacing;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasHeight);
        float canvasWidth = buttonRect.width + verticalLayoutGroup.padding.left + verticalLayoutGroup.padding.right;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasWidth);

        for (int i = 0; i < buttons.Count; i++) {
            GameObject buttonGO = Object.Instantiate(buttonPrefab, panel);
            Button button = buttonGO.GetComponent<Button>();
            Text text = buttonGO.GetComponentInChildren<Text>();
            text.text = buttons[i].Text;
            button.onClick.AddListener(Close);
            button.onClick.AddListener(buttons[i].OnClick);
        }
    }

    /// <summary>
    /// 关闭菜单
    /// </summary>
    public void Close() {
        if (canvasGO != null) {
            Object.Destroy(canvasGO);
        }
    }
}
