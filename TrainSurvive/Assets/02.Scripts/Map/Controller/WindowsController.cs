/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 23:45:14
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TTT.Utility;

namespace WorldMap.Controller
{
    public abstract class WindowsController : BaseController, IDragHandler
    {
        //默认大小为 1920*1080 的 1/4
        protected Vector2 ScreenSize = new Vector2(1920F, 1080F);
        private const string ImageFloder = "WindowImage/";
        public enum EWindowSizeType
        {
            SMALL,             // 6:5     => 720*600 
            MIDDLE14x12,     //     => 840*720
            BIG26x14,          //     => 1560*840 
            FULL32x18         //    => 1920*1080 
        }
        public EWindowSizeType WinSizeType
        {
            set
            {
                m_windowSizeType = value;
                const float pixelEach = 60F;
                float width, height;
                switch (value)
                {
                    case EWindowSizeType.SMALL:
                        width = 6F * pixelEach;
                        height = 5F * pixelEach;
                        IsWinMovable = true;
                        break;
                    default:
                    case EWindowSizeType.MIDDLE14x12:
                        width = 14F * pixelEach;
                        height = 12F * pixelEach;
                        IsWinMovable = false;
                        break;
                    case EWindowSizeType.BIG26x14:
                        width = 26F * pixelEach;
                        height = 14F * pixelEach;
                        IsWinMovable = false;
                        break;
                    case EWindowSizeType.FULL32x18:
                        width = 32F * pixelEach;
                        height = 18F * pixelEach;
                        IsWinMovable = false;
                        break;
                }
                WinSize = new Vector2(width, height);
                float halfOfWidth = width / 2;
                float halfOfHeight = height / 2;
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
        protected static Color containerColor = new Color(0.8F, 0.8F, 0.8F);
        //背景
        protected Image backgroundImage;
        protected string backgroudFN;
        //左上角标题
        protected bool enableTitileBar = true;
        protected Text titleText;
        //右上角关闭
        protected Button closeBtn;
        protected EWindowSizeType m_windowSizeType = EWindowSizeType.MIDDLE14x12;
        protected string m_titleString;
        protected sealed override void Awake()
        {
            base.Awake();
            AfterShowWindow();
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void OnEnable()
        {
        }
        protected override void Update()
        {
            base.Update();
        }
        protected override void CreateModel()
        {
            RectTransform rect = rectTransform;
            backgroundImage = ViewTool.ForceGetComponent<Image>(gameObject);
            rect.anchorMin = defaultMinAnchor;
            rect.anchorMax = defaultMaxAnchor;
            WinSizeType = m_windowSizeType;
            if (enableTitileBar)
            {
                //TitleBar
                RectTransform titleBar = new GameObject("TitleBar").AddComponent<RectTransform>();
                ViewTool.SetParent(titleBar, this);
                ViewTool.TopFull(titleBar, 60F);
                //Title
                Image titleBg = ViewTool.CreateImage("TitleBG");
                ViewTool.SetParent(titleBg, titleBar);
                ViewTool.LeftTop(titleBg, new Vector2(0F, 1F), new Vector2(120F, 60F), Vector2.zero);
                titleBg.color = new Color(0.9F, 0.9F, 0.9F);
                titleText = ViewTool.CreateText("Title", m_titleString);
                ViewTool.SetParent(titleText, titleBg);
                ViewTool.LeftTop(titleText, new Vector2(0F, 1F), new Vector2(120F, 60F), Vector2.zero);
                //Close Button
                closeBtn = ViewTool.CreateBtn("Close", "X");
                closeBtn.onClick.AddListener(delegate () { HideWindow(); });
                ViewTool.SetParent(closeBtn, titleBar);
                ViewTool.RightTop(closeBtn, new Vector2(1F, 1F), new Vector2(60, 60), Vector2.zero);
                ColorBlock redColorBlock = closeBtn.colors;
                redColorBlock.normalColor = new Color(1, 0, 0);
                redColorBlock.highlightedColor = new Color(0.9F, 0, 0);
                redColorBlock.pressedColor = new Color(0.8F, 0, 0);
                redColorBlock.disabledColor = new Color(0.8F, 0.8F, 0.8F);
                closeBtn.colors = redColorBlock;
            }
        }
        protected void SetTitle(string title)
        {
            titleText.text = title;
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
            //ShowWindow可能会在Awake之前调用
            world = WorldForMap.Instance;
            if (!PrepareDataBeforeShowWindow())
            {
                Debug.Log("窗口数据未准备好，拒绝显示");
                return false;
            }
            gameObject.SetActive(true);
            if (gameObject.transform.parent != null)
                AfterShowWindow();
            return true;
        }
        protected abstract bool PrepareDataBeforeShowWindow();
        protected abstract void AfterShowWindow();
        public void HideWindow()
        {
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);
        }
        public void SetBackground(string filename)
        {
            backgroudFN = filename;
            backgroundImage.sprite = Resources.Load<Sprite>(ImageFloder + filename);
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
    public class NullWindowsController : WindowsController
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

    }
}