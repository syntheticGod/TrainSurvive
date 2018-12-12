/*
 * 描述：列车上的设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:08:00
 * 版本：v0.1
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    private Indicator _indicator;
    private Indicator Indicator {
        get {
            if (_indicator == null) {
                _indicator = GetComponentInChildren<Indicator>();
                _indicator.GetComponent<RectTransform>().localPosition = new Vector3(0, ConstructionManager.StructureSettings[Structure.ID].Sprite.bounds.size.y - ConstructionManager.StructureSettings[Structure.ID].Sprite.pivot.y / ConstructionManager.StructureSettings[Structure.ID].Sprite.pixelsPerUnit, 0);
            }
            return _indicator;
        }
    }

    private ContextMenu contextMenu;
    
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

    private void Awake() {
        gameObject.layer = ConstructionManager.StructureSettings[Structure.ID].Layer;
    }

    private void Start() {
        spriteRenderer.sprite = ConstructionManager.StructureSettings[Structure.ID].Sprite;
        BoxCollider.offset = ConstructionManager.StructureSettings[Structure.ID].Sprite.bounds.center;
        BoxCollider.size = ConstructionManager.StructureSettings[Structure.ID].Sprite.bounds.size;
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
        contextMenu?.Close();
        switch (Structure.FacilityState) {
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
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    contextMenu = new ContextMenu();
                    MakeContextMenu(contextMenu);
                    contextMenu.Render(mousePos);
                }
                if (Input.GetMouseButtonUp(0)) {
                    UIManager.Instance?.ShowInfoPanel(ConstructionManager.StructureSettings[Structure.ID].Name, ConstructionManager.StructureSettings[Structure.ID].Description);
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
                Structure.ButtonAction[] actions = Structure.GetButtonActions();
                for (int i = 0; i < actions.Length; i++) {
                    int index = i;
                    contextMenu.PutButton(actions[index].Title, index, () => actions[index].Action(Structure));
                }
                break;
            default:
                break;
        }
    }
}
