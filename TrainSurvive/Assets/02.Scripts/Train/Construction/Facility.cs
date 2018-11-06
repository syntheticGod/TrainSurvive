/*
 * 描述：列车上的设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:08:00
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

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
    [Tooltip("操作界面。")] [SerializeField]
    private GameObject FacilityUIPrefab;

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
    public abstract Cost[] Costs { get; }
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
                    OnRemove();
                    StopAllCoroutines();
                    Destroy(gameObject);
                    returnCosts(0.8f);
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
    
    private SpriteRenderer spriteRenderer;
    private ContextMenu contextMenu;
    // 一时间只能显示一个操作菜单。
    private static GameObject _facilityUI;
    private static GameObject facilityUI {
        get {
            return _facilityUI;
        }
        set {
            if (_facilityUI != null) {
                Destroy(_facilityUI);
            }
            _facilityUI = value;
        }
    }

    private RectTransform _canvasUI;
    private RectTransform canvasUI {
        get {
            if (_canvasUI == null) {
                _canvasUI = GameObject.Find("Canvas").GetComponent<RectTransform>();
            }
            return _canvasUI;
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

        spriteRenderer = GetComponent<SpriteRenderer>();
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
                    facilityUI = null;
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
        if (cost()) {
            FacilityState = State.BUILDING;
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// 请求移除物体时的回调。
    /// </summary>
    protected abstract void OnRemove();
    /// <summary>
    /// 当设施进入启动状态的回调。
    /// </summary>
    protected abstract void OnStart();
    /// <summary>
    /// 当设施停止工作时的回调。
    /// </summary>
    protected abstract void OnStop();

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
                contextMenu.PutButton("操作", 0, () => facilityUI = Instantiate(FacilityUIPrefab, canvasUI));
                contextMenu.PutButton("关闭", 1, () => FacilityState = State.STOPPED);
                contextMenu.PutButton("拆除", 2, () => FacilityState = State.REMOVING);
                break;
            case State.STOPPED:
                contextMenu.PutButton("操作", 0, () => facilityUI = Instantiate(FacilityUIPrefab, canvasUI));
                contextMenu.PutButton("开启", 1, () => FacilityState = State.WORKING);
                contextMenu.PutButton("拆除", 2, () => FacilityState = State.REMOVING);
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

    private bool cost() {
        // TODO
        return true;
    }

    private void returnCosts(float ratio) {
        // TODO
    }
}
