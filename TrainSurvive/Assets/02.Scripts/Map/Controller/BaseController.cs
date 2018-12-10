/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 23:37:32
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace WorldMap.Controller
{
    public abstract class BaseController : MonoBehaviour
    {
        private static BaseController focusingController = null;
        protected WorldForMap world;
        protected RectTransform rectTransform { get { return gameObject.GetComponent<RectTransform>(); } }
        private Vector2 leftMouseDownPosition;
        private Vector2 rightMouseDownPosition;

        protected enum MouseState
        {
            None = 0,//空，没有鼠标事件
            Down,//点下，未移动
            Draging,//正在拖动
            Up,//释放
            Click//点击了，（拖动距离过小）
        }
        private MouseState leftMouseState = MouseState.None;
        protected MouseState LeftMouseState
        {
            get { return leftMouseState; }
            set { leftMouseState = value; /*LogMouseEvent(value, "左键");*/ }
        }
        private MouseState rightMouseState = MouseState.None;
        protected MouseState RightMouseState
        {
            get { return rightMouseState; }
            set { rightMouseState = value; /*LogMouseEvent(value, "右键");*/ }
        }
        protected virtual void Awake()
        {
            world = WorldForMap.Instance;
            CreateModel();
        }
        protected virtual void Start()
        { }
        protected virtual void OnEnable()
        { }
        protected virtual void OnDisable()
        { }
        protected virtual void Update()
        { }
        public bool Focus()
        {
            if(focusingController != null && focusingController != this)
                focusingController.UnfocusBehaviour();
            if (!FocusBehaviour())
                return false;
            focusingController = this;
            return true;
        }
        /// <summary>
        /// 焦距行为函数
        /// 用于加载界面等行为
        /// </summary>
        /// <returns>
        /// FALSE：阻止焦距行为。
        /// TRUE：允许焦距
        /// </returns>
        protected abstract bool FocusBehaviour();
        protected abstract void UnfocusBehaviour();
        protected abstract void CreateModel();
        /// <summary>
        /// 设置鼠标的状态
        /// </summary>
        protected void MouseEventDetecter()
        {
            bool leftMouse = false, rightMouse = false;
            if ((leftMouse = Input.GetKeyDown(KeyCode.Mouse0)) | (rightMouse = Input.GetKeyDown(KeyCode.Mouse1)))
            {
                if (leftMouse)
                {
                    leftMouseDownPosition = Input.mousePosition;
                    LeftMouseState = MouseState.Down;
                }
                if (rightMouse)
                {
                    rightMouseDownPosition = Input.mousePosition;
                    RightMouseState = MouseState.Down;
                }
            }
            else if ((leftMouse = Input.GetKey(KeyCode.Mouse0)) | (rightMouse = Input.GetKey(KeyCode.Mouse1)))
            {
                if (leftMouse && !Utility.Approximately(leftMouseDownPosition, Input.mousePosition))
                    LeftMouseState = MouseState.Draging;
                if (rightMouse && !Utility.Approximately(rightMouseDownPosition, Input.mousePosition))
                    RightMouseState = MouseState.Draging;
            }
            else if ((leftMouse = Input.GetKeyUp(KeyCode.Mouse0)) | (rightMouse = Input.GetKeyUp(KeyCode.Mouse1)))
            {
                if (leftMouse)
                    LeftMouseState = Utility.Approximately(leftMouseDownPosition, Input.mousePosition) ? MouseState.Click : MouseState.Up;
                if (rightMouse)
                    RightMouseState = Utility.Approximately(rightMouseDownPosition, Input.mousePosition) ? MouseState.Click : MouseState.Up;
            }
            else
            {
                if (!leftMouse)
                    LeftMouseState = MouseState.None;
                if (!rightMouse)
                    RightMouseState = MouseState.None;
            }
        }
        protected void LogMouseEvent(MouseState mouseState, string prefix)
        {
            if (mouseState == MouseState.None)
                return;
            switch (mouseState)
            {
                case MouseState.Down:
                    Debug.Log(prefix + "鼠标点下");
                    break;
                case MouseState.Draging:
                    Debug.Log(prefix + "鼠标拖拽中");
                    break;
                case MouseState.Up:
                    Debug.Log(prefix + "鼠标释放");
                    break;
                case MouseState.Click:
                    Debug.Log(prefix + "鼠标点击");
                    break;
                case MouseState.None:
                    Debug.Log(prefix + "无鼠标事件");
                    break;
            }
        }
    }
}