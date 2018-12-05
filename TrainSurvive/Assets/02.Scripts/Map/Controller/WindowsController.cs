/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 23:45:14
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WorldMap.Controller
{
    public abstract class WindowsController : BaseController, IDragHandler
    {
        //默认大小为 1920*1080 的 1/4
        protected Vector2 ScreenSize = new Vector2(1920F, 1080F);
        public enum WindowSizeType
        {
            SMALL,     //1/9屏幕  等于   0.33*0.33
            MIDDLE,   //1/4屏幕  等于   0.50*0.50
            BIG,          //1/2屏幕  约等于0.70*0.70
            FULL        //将近满屏 约等于0.95*0.95
        }
        public WindowSizeType m_windowSizeType = WindowSizeType.MIDDLE;
        public WindowSizeType WinSizeType
        {
            set
            {
                m_windowSizeType = value;
                float scale;
                switch (value)
                {
                    case WindowSizeType.SMALL:
                        scale = 0.33F;
                        IsWinMovable = true;
                        break;
                    default:
                    case WindowSizeType.MIDDLE:
                        scale = 0.5F;
                        IsWinMovable = true;
                        break;
                    case WindowSizeType.BIG:
                        scale = 0.7F;
                        IsWinMovable = false;
                        break;
                    case WindowSizeType.FULL:
                        scale = 0.95F;
                        IsWinMovable = false;
                        break;
                }
                WinSize = ScreenSize * scale;
                float halfOfWidth = WinSize.x / 2;
                float halfOfHeight = WinSize.y / 2;
                RectTransform rect = rectTransform;
                rect.offsetMin = new Vector2(-halfOfWidth, -halfOfHeight);
                rect.offsetMax = new Vector2(halfOfWidth, halfOfHeight);
            }
            get { return m_windowSizeType; }
        }
        public bool IsWinMovable { set; get; } = true;
        public Vector2 WinSize { set; get; }
        public Vector2 Padding { set; get; }
        protected static Vector2 defaultMinAnchor = new Vector2(0.5F, 0.5F);
        protected static Vector2 defaultMaxAnchor = new Vector2(0.5F, 0.5F);
        protected Image backgroudImage;
        protected static Color containerColor = new Color(0.8F, 0.8F, 0.8F);
        protected sealed override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void OnEnable()
        {
            AfterShowWindow();
        }
        protected override void Update()
        {
            base.Update();
        }
        protected override void CreateModel()
        {
            RectTransform rect = rectTransform;
            backgroudImage = Utility.ForceGetComponent<Image>(gameObject);
            rect.anchorMin = defaultMinAnchor;
            rect.anchorMax = defaultMaxAnchor;
            WinSizeType = m_windowSizeType;
        }
        public bool ShowWindow()
        {
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("窗口已显示");
                return false;
            }
            if (!Focus())
            {
                Debug.Log("窗口拒绝被焦距，导致拒绝显示窗口");
                return false;
            }
            if (!PrepareDataBeforeShowWindow())
            {
                Debug.Log("窗口数据未准备好，拒绝显示");
                return false;
            }
            gameObject.SetActive(true);
            return true;
        }
        protected abstract bool PrepareDataBeforeShowWindow();
        protected abstract void AfterShowWindow();
        public void HideWindow()
        {
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!IsWinMovable) return;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                RectTransform rect = rectTransform;
                rect.position = eventData.position;
            }
        }
    }
    public class BaseWindowsController : WindowsController
    {
        protected override bool PrepareDataBeforeShowWindow()
        {
            return true;
        }
        protected override void AfterShowWindow()
        { }
        protected override bool FocusBehaviour()
        {
            return true;
        }

        protected override void UnfocusBehaviour()
        { }
    }
}