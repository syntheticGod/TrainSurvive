/*
 * 描述：列车上的设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:08:00
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Facility : MonoBehaviour {

    public enum State {
        /// <summary>
        /// 待建造
        /// </summary>
        NONE,
        /// <summary>
        /// 取消建造
        /// </summary>
        CANCLE,
        /// <summary>
        /// 建造中
        /// </summary>
        BUILDING,
        /// <summary>
        /// 工作中
        /// </summary>
        WORKING,
        /// <summary>
        /// 已停止
        /// </summary>
        STOPPED,
        /// <summary>
        /// 拆除中
        /// </summary>
        REMOVING
    }

    [Serializable]
    public class Cost {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        /// 耗材量，实际耗材量为耗材量 * 耗材量系数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 耗材量系数
        /// </summary>
        public int Ratio { get; set; } = 1;
    }

    [Tooltip("可以支持该设施的建筑平台类型。")]
    public LayerMask RequireLayers;
    [Tooltip("状态指示器。")] [SerializeField]
    private Indicator IndicatorPrefab;
    [Tooltip("设施UI Prefab。")] [SerializeField]
    private GameObject FacilityUIPrefab;

    /// <summary>
    /// 在ConstructionManager里注册的ID，自动生成。
    /// </summary>
    public int ID { get; set; }
    /// <summary>
    /// 状态指示器
    /// </summary>
    protected Indicator Indicator { get; private set; }
    /// <summary>
    /// 设施名字
    /// </summary>
    public abstract string Name { get; }
    /// <summary>
    /// 总建造工作量，实际工作量为总工作量 * 工作量系数
    /// </summary>
    public abstract float WorkAll { get; }
    /// <summary>
    /// 建造耗材
    /// </summary>
    public abstract Cost[] BuildCosts { get; }
    /// <summary>
    /// 鼠标移动上去时显示的高亮颜色
    /// </summary>
    public virtual Color HighlightColor { get; } = new Color(1, 1, 1, 0.8f);
    
    /// <summary>
    /// 总建造工作量系数
    /// </summary>
    public float WorkRatio { get; set; } = 1;
    /// <summary>
    /// 当前建造工作量，set可以自动修改进度条数值保持同步。
    /// </summary>
    public float WorkNow {
        get {
            return Indicator.Progress;
        }
        protected set {
            Indicator.Progress = value;
        }
    }
    /// <summary>
    /// 设施状态，切换状态时将自动关闭上下文菜单，并根据修改的状态自动调用该状态的回调。
    /// 因此修改状态后不需要重复调用OnXXX回调。
    /// </summary>
    public State FacilityState {
        get {
            return _FacilityState;
        }
        protected set {
            if (contextMenu != null) {
                contextMenu.Close();
                contextMenu = null;
            }
            if (value == _FacilityState) return;
            State previous = _FacilityState;
            _FacilityState = value;
            switch (value) {
                case State.BUILDING: {
                    StartCoroutine(building());
                }break;
                case State.WORKING: {
                    Indicator.ToggleStopIndicator(false);
                    OnStart();
                }break;
                case State.STOPPED: {
                    Indicator.ToggleStopIndicator(true);
                    OnStop();
                }break;
                case State.REMOVING: {
                    Indicator.ToggleStopIndicator(false);
                    if (previous == State.WORKING) {
                        OnStop();
                    }
                    OnRemove();
                    StopAllCoroutines();
                    Destroy(gameObject);
                }break;
                case State.CANCLE: {
                    Indicator.HideProgress();
                    StopAllCoroutines();
                    Destroy(gameObject);
                    returnCosts(1);
                }break;
            }
        }
    }
    private State _FacilityState = State.NONE;
    
    private ContextMenu contextMenu;

    private static UIManager _uiManager;
    protected static UIManager uiManager {
        get {
            if (_uiManager == null) {
                _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
            }
            return _uiManager;
        }
    }

    private GameObject _facilityUIObj;
    protected GameObject facilityUIObj {
        get {
            if(_facilityUIObj == null)
                _facilityUIObj = uiManager.CreateFacilityUI(FacilityUIPrefab);
            return _facilityUIObj;
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

    /// <summary>
    /// override的话务必base.Awake()一下。
    /// </summary>
    protected virtual void Awake() {
        if (gameObject.layer != LayerMask.NameToLayer("Facility")) {
            Debug.LogError("该Facility对象的layer必须设置为Facility。", this);
        }

        if (IndicatorPrefab == null) {
            Debug.LogError("该Facility对象必须填写Indicator。", this);
        } else {
            Indicator = Instantiate(IndicatorPrefab, transform);
        }

        if (FacilityUIPrefab == null) {
            Debug.LogError("该Facility对象必须填写FacilityUIPrefab。", this);
        }
    }
    
    /// <summary>
    /// override的话务必base.OnMouseExit()一下。
    /// </summary>
    protected virtual void OnMouseExit() {
        if (FacilityState == State.NONE) {
            return;
        }
        spriteRenderer.color = Color.white;
    }

    /// <summary>
    /// override的话务必base.OnMouseOver()一下。
    /// </summary>
    protected virtual void OnMouseOver() {
        if (FacilityState != State.NONE) {
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
                    makeContextMenu(contextMenu);
                    contextMenu.Render(mousePos);
                }
            }
        }
    }

    /// <summary>
    /// 放置物体后的回调，如果override的话务必base.OnPlaced()一下。
    /// </summary>
    /// <returns>true放置成功，false放置失败。</returns>
    public virtual bool OnPlaced() {
        if (IsCostsAvailable()) {
            cost();
            FacilityState = State.BUILDING;
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// 判断当前建材是否充足。
    /// </summary>
    /// <returns></returns>
    public bool IsCostsAvailable() {
        // TODO
        return true;
    }

    /// <summary>
    /// 请求移除物体时的回调。
    /// </summary>
    protected virtual void OnRemove() {
        returnCosts(0.8f);
    }

    /// <summary>
    /// 当设施进入启动状态的回调。
    /// </summary>
    protected abstract void OnStart();
    /// <summary>
    /// 当打开设施界面时。
    /// </summary>
    protected abstract void OnInitFacilityUI();
    /// <summary>
    /// 当设施停止工作时的回调。
    /// </summary>
    protected virtual void OnStop() { }
    /// <summary>
    /// 当载入时的回调。
    /// </summary>
    public virtual void OnLoad(dynamic obj) {
        WorkRatio = obj.WorkRatio;
        WorkNow = obj.WorkNow;
        FacilityState = obj.FacilityState;
    }
    /// <summary>
    /// 当存档时的回调。
    /// </summary>
    public virtual object OnSave() {
        return new {
            WorkRatio,
            WorkNow,
            FacilityState
        };
    }

    /// <summary>
    /// 创建右键上下文菜单，主要根据FacilityState在菜单中添加按钮以及事件。
    /// 重写后可以添加新的上下文菜单项，也可以覆盖已有某一项的事件。
    /// </summary>
    protected virtual void makeContextMenu(ContextMenu contextMenu) {
        switch (FacilityState) {
            case State.BUILDING:
                contextMenu.PutButton("停止", 0, () => FacilityState = State.CANCLE);
                break;
            case State.WORKING:
            case State.STOPPED:
                contextMenu.PutButton("查看", 0, () => {
                    uiManager.ShowFaclityUI(facilityUIObj);
                    OnInitFacilityUI();
                });
                contextMenu.PutButton("拆除", 1, () => FacilityState = State.REMOVING);
                break;
            default:
                break;
        }
    }

    private IEnumerator building() {
        Indicator.ShowProgress(0, WorkAll * WorkRatio, WorkNow);
        while (WorkNow < WorkAll * WorkRatio) {
            WorkNow += Time.deltaTime;
            yield return 1;
        }
        Indicator.HideProgress();
        FacilityState = State.WORKING;
    }

    private void cost() {
        // TODO
    }

    private void returnCosts(float ratio) {
        // TODO
    }
}
