/*
 * 描述：UI管理器
 * 作者：刘旭涛
 * 创建时间：2018/11/7 13:20:29
 * 版本：v0.1
 */
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIManager : MonoBehaviour {
    
    public static UIManager Instance { get; private set; }

    [Tooltip("InventoryPanel子物体")]
    [SerializeField]
    private RectTransform InventoryPanel;
    [Tooltip("Facility UI子物体")]
    [SerializeField]
    private RectTransform FacilityUI;
    [Tooltip("InfoPanel子物体")]
    [SerializeField]
    private InfoPanel InfoPanel;

    private GameObject _currentFacilityUI;
    private GameObject currentFacilityUI {
        get {
            return _currentFacilityUI;
        }
        set {
            if (_currentFacilityUI != null) {
                _currentFacilityUI.SetActive(false);
            }
            if (value != null) {
                value.SetActive(true);
            }
            _currentFacilityUI = value;
        }
    }

    private Dictionary<string, FacilityUI> facilityUIs { get; set; }

    private void Awake() {
        Instance = this;
        facilityUIs = new Dictionary<string, FacilityUI>();
        FacilityUI[] uis = FacilityUI.GetComponentsInChildren<FacilityUI>(true);
        for (int i = 0; i < uis.Length; i++) {
            facilityUIs.Add(uis[i].gameObject.name, uis[i]);
        }
    }

    private void OnDestroy() {
        Instance = null;
    }

    /// <summary>
    /// 显示设施查看界面
    /// </summary>
    /// <param name="ui">UI物体名称</param>
    public void ShowFaclityUI(string ui, Structure structure) {
        FacilityUI facilityUI = facilityUIs[ui];
        facilityUI.Structure = structure;
        currentFacilityUI = facilityUI.gameObject;
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

    /// <summary>
    /// 显示消息界面
    /// </summary>
    public void ShowInfoPanel(string title, string content) {
        InfoPanel.TitleText = title;
        InfoPanel.ContentText = content;
        InfoPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏消息界面
    /// </summary>
    public void HideInfoPanel() {
        InfoPanel.gameObject.SetActive(false);
    }
}
