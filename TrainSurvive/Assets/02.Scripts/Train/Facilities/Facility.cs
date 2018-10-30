/*
 * 描述：列车上的设施
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:08:00
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Facility : MonoBehaviour {

    public enum State {
        /// <summary>
        /// 待建造
        /// </summary>
        NONE,
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

    [Header("Base Settings")]
    [Tooltip("建筑设施名称。")]
    public string Name;
    [Tooltip("总建造工作量。")]
    public uint WorkAll;
    [Tooltip("可以支持该设施的建筑平台类型。")]
    public LayerMask RequireLayers;
    [Tooltip("鼠标移动上去时显示的高亮颜色。")]
    public Color HighlightColor = new Color(1, 1, 1, 0.8f);

    /// <summary>
    /// 当前工作量
    /// </summary>
    public uint WorkNow {
        get {
            return (uint) slider.value;
        }
        protected set {
            slider.value = value;
        }
    }
    /// <summary>
    /// 设施状态
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
            _FacilityState = value;
        }
    }
    private State _FacilityState = State.NONE;

    private Slider slider;
    private SpriteRenderer spriteRenderer;
    private ContextMenu contextMenu;

    protected virtual void Awake() {
        if (gameObject.layer != LayerMask.NameToLayer("Facility")) {
            Debug.LogError("该Facility对象的layer必须设置为Facility。", this);
        }

        slider = GetComponentInChildren<Slider>(true);
        if (slider == null) {
            Debug.LogError("该Facility对象必须含有一个Slider用于显示进度。", this);
        } else {
            slider.minValue = 0;
            slider.maxValue = WorkAll;
            slider.value = 0;
            slider.gameObject.SetActive(false);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnMouseEnter() {
        if (FacilityState == State.NONE) {
            return;
        }
        spriteRenderer.color = HighlightColor;
    }

    protected virtual void OnMouseExit() {
        if (FacilityState == State.NONE) {
            return;
        }
        spriteRenderer.color = Color.white;
    }
    
    protected virtual void OnMouseOver() {
        // 右键菜单
        if (Input.GetMouseButtonUp(1)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            contextMenu = new ContextMenu();
            makeContextMenu(contextMenu);
            contextMenu.Render(mousePos);
        }
    }

    public virtual void OnPlaced() {
        FacilityState = State.BUILDING;
        slider.gameObject.SetActive(true);
        StartCoroutine(TEST_building());
    }

    public abstract void OnBuildCompleted();
    
    protected virtual void makeContextMenu(ContextMenu contextMenu) {
        switch (FacilityState) {
            case State.BUILDING:
                contextMenu.PutButton("停止", null);
                break;
            case State.WORKING:
                contextMenu.PutButton("查看", null);
                contextMenu.PutButton("关闭", null);
                contextMenu.PutButton("拆除", null);
                break;
            case State.STOPPED:
                contextMenu.PutButton("查看", null);
                contextMenu.PutButton("开启", null);
                contextMenu.PutButton("拆除", null);
                break;
            default:
                break;
        }
    }

    private IEnumerator TEST_building() {
        for(int i = 0; i < WorkAll; i++) {
            yield return new WaitForSeconds(1);
            WorkNow++;
        }
        slider.gameObject.SetActive(false);
        FacilityState = State.WORKING;
        OnBuildCompleted();
    }
}
