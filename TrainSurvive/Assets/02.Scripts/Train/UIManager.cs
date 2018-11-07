/*
 * 描述：UI管理器
 * 作者：刘旭涛
 * 创建时间：2018/11/7 13:20:29
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIManager : MonoBehaviour {

    [Tooltip("FacilityUI子物体")] [SerializeField]
    private RectTransform FacilityUI;
    [Tooltip("InventoryPanel子物体")] [SerializeField]
    private RectTransform InventoryPanel;

    private RectTransform _currentFacilityUI;
    private RectTransform currentFacilityUI {
        get {
            return _currentFacilityUI;
        }
        set {
            if (_currentFacilityUI != null) {
                _currentFacilityUI.gameObject.SetActive(false);
            }
            if (value != null) {
                value.gameObject.SetActive(true);
            }
            _currentFacilityUI = value;
        }
    }

    private Dictionary<string, RectTransform> cacheFacilityUI = new Dictionary<string, RectTransform>();
    private RectTransform getFacilityUI(string name) {
        if (cacheFacilityUI.ContainsKey(name)) {
            return cacheFacilityUI[name];
        }
        RectTransform child = FacilityUI.Find(name) as RectTransform;
        if (child == null) {
            Debug.LogError("在FacilityUI下没有找到: " + name, FacilityUI);
            return null;
        }
        cacheFacilityUI[name] = child;
        return child;
    }

    /// <summary>
    /// 显示设施查看界面
    /// </summary>
    /// <param name="name">UI在Inspector里的名称</param>
    public GameObject ShowFacilityUI(string name) {
        RectTransform child = getFacilityUI(name);
        currentFacilityUI = child;
        return currentFacilityUI.gameObject;
    }

    /// <summary>
    /// 隐藏设施界面
    /// </summary>
    public void HideFacilityUI() {
        currentFacilityUI = null;
    }

    /// <summary>
    /// 显示或消除Inventory界面
    /// </summary>
    /// <param name="show">true显示，false隐藏</param>
    public void ToggleInventoryPanel(bool show) {
        InventoryPanel.gameObject.SetActive(show);
    }
}
