using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrainCarriageObject : MonoBehaviour {

    /// <summary>
    /// 建筑实体
    /// </summary>
    public TrainCarriage TrainCarriage { get; set; }
    
    private Indicator _indicator;
    private Indicator Indicator {
        get {
            if (_indicator == null) {
                _indicator = GetComponentInChildren<Indicator>();
            }
            return _indicator;
        }
    }

    private ContextMenu contextMenu;

    private SpriteRenderer[] _spriteRenderers;
    public SpriteRenderer[] SpriteRenderers {
        get {
            if (_spriteRenderers == null)
                _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            return _spriteRenderers;
        }
    }

    private Collider2D[] _colliders;
    private Collider2D[] Colliders {
        get {
            if (_colliders == null)
                _colliders = GetComponentsInChildren<Collider2D>();
            return _colliders;
        }
    }
    
    private void OnEnable() {
        if (TrainCarriage.CarriageState == TrainCarriage.State.IDLE) {
            for (int i = 0; i < Colliders.Length; i++) {
                Colliders[i].enabled = true;
            }
        } else {
            TrainCarriage.OnStateChange += OnStateChange;
        }
        TrainCarriage.OnProgressChange += OnProgressChange;
    }

    private void OnDisable() {
        TrainCarriage.OnProgressChange -= OnProgressChange;
        TrainCarriage.OnStateChange -= OnStateChange;
    }

    private void OnProgressChange(float start, float end, float value) {
        if (value == 0) {
            Indicator.HideProgress();
        } else {
            Indicator.ShowProgress(start, end, value);
        }
    }

    private void OnStateChange(TrainCarriage carriage) {
        contextMenu?.Close();
        switch (TrainCarriage.CarriageState) {
            case TrainCarriage.State.CANCLE:
                Destroy(gameObject);
                break;
            case TrainCarriage.State.IDLE:
                for (int i = 0; i < SpriteRenderers.Length; i++) {
                    SpriteRenderers[i].color = Color.white;
                }
                for (int i = 0; i < Colliders.Length; i++) {
                    Colliders[i].enabled = true;
                }
                break;
        }
    }
    
    protected void OnMouseOver() {
        if (TrainCarriage.CarriageState == TrainCarriage.State.BUILDING) {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                // 右键菜单
                if (Input.GetMouseButtonUp(1)) {
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
        contextMenu.PutButton("停止", 0, () => TrainCarriage.CarriageState = TrainCarriage.State.CANCLE);
    }
}
