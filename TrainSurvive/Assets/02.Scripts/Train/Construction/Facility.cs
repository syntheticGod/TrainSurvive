/*
 * 描述：列车上的设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:08:00
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Facility : MonoBehaviour {
    
    [Tooltip("鼠标移动上去时显示的高亮颜色")]
    public Color HighlightColor = new Color(1, 1, 1, 0.8f);

    /// <summary>
    /// 建筑实体
    /// </summary>
    public Structure Structure { get; set; }
    /// <summary>
    /// UI Prefab
    /// </summary>
    public GameObject UIPrefab { get; set; }
    /// <summary>
    /// UI
    /// </summary>
    private FacilityUI UI { get; set; }

    private Indicator _indicator;
    private Indicator Indicator {
        get {
            if (_indicator == null) {
                _indicator = GetComponentInChildren<Indicator>();
                _indicator.GetComponent<RectTransform>().localPosition = new Vector3(0, Structure.Info.Sprite.bounds.size.y, 0);
            }
            return _indicator;
        }
    }

    private ContextMenu contextMenu;

    private UIManager _uiManager;
    private UIManager uiManager {
        get {
            if (_uiManager == null) {
                _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
            }
            return _uiManager;
        }
    }
    
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer spriteRenderer {
        get {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

    private BoxCollider2D _boxCollider;
    private BoxCollider2D BoxCollider {
        get {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider2D>();
            return _boxCollider;
        }
    }

    private void Start() {
        spriteRenderer.sprite = Structure.Info.Sprite;
        BoxCollider.offset = Structure.Info.Sprite.bounds.center;
        BoxCollider.size = Structure.Info.Sprite.bounds.size;
    }

    private void OnEnable() {
        Structure.OnProgressChange += OnProgressChange;
        Structure.OnStateChange += OnStateChange;
    }

    private void OnDisable() {
        Structure.OnProgressChange -= OnProgressChange;
        Structure.OnStateChange -= OnStateChange;
    }

    private void OnProgressChange(float start, float end, float value) {
        if (value == 0) {
            Indicator.HideProgress();
        } else {
            Indicator.ShowProgress(start, end, value);
        }
    }

    private void OnStateChange(Structure structure) {
        switch (Structure.FacilityState) {
            case Structure.State.WORKING:
                UI = uiManager.CreateFacilityUI<FacilityUI>(UIPrefab);
                UI.Structure = Structure;
                break;
            case Structure.State.REMOVING:
            case Structure.State.CANCLE:
                Destroy(gameObject);
                break;
        }
    }

    protected void OnMouseExit() {
        if (Structure.FacilityState == Structure.State.NONE) {
            return;
        }
        spriteRenderer.color = Color.white;
    }
    
    protected void OnMouseOver() {
        if (Structure.FacilityState != Structure.State.NONE) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                spriteRenderer.color = Color.white;
            } else {
                spriteRenderer.color = HighlightColor;
                // 右键菜单
                if (Input.GetMouseButtonUp(1)) {
                    // 关闭操作菜单
                    uiManager.HideFacilityUI();
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    contextMenu = new ContextMenu();
                    MakeContextMenu(contextMenu);
                    contextMenu.Render(mousePos);
                }
            }
        }
    }
    
    /// <summary>
    /// 创建右键上下文菜单，主要根据FacilityState在菜单中添加按钮以及事件。
    /// </summary>
    private void MakeContextMenu(ContextMenu contextMenu) {
        switch (Structure.FacilityState) {
            case Structure.State.BUILDING:
                contextMenu.PutButton("停止", 0, () => Structure.FacilityState = Structure.State.CANCLE);
                break;
            case Structure.State.WORKING:
                contextMenu.PutButton("查看", 0, () => uiManager.ShowFaclityUI(UI.gameObject));
                contextMenu.PutButton("拆除", 1, () => Structure.FacilityState = Structure.State.REMOVING);
                break;
            default:
                break;
        }
    }
}
