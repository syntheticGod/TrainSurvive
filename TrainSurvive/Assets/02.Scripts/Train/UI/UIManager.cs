/*
 * 描述：UI管理器
 * 作者：刘旭涛
 * 创建时间：2018/11/7 13:20:29
 * 版本：v0.1
 */
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIManager : MonoBehaviour {
    
    public static UIManager Instance { get; private set; }

    [Tooltip("InventoryPanel子物体")] [SerializeField]
    private RectTransform InventoryPanel;

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

    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }

    /// <summary>
    /// 显示设施查看界面
    /// </summary>
    /// <param name="ui">UI实体</param>
    public void ShowFaclityUI(FacilityUI ui, Structure structure) {
        ui.Structure = structure;
        currentFacilityUI = ui.gameObject;
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
