/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 23:14:58
 * 版本：v0.1
 */
using UnityEngine;

using System.Collections.Generic;

using TTT.Utility;
//TODO 整理各个窗口 控制类之间的关系
namespace WorldMap.Controller
{
    public class ControllerManager
    {
        private class ControllerWithName
        {
            public string Name;
            public BaseController Controller;
            public ControllerWithName(string name, BaseController controller)
            {
                Name = name;
                Controller = controller;
            }
            public override bool Equals(object obj)
            {
                if (obj is ControllerWithName)
                    return (obj as ControllerWithName).Name.Equals(Name);
                else if (obj is string)
                    return Name.Equals(obj);
                return false;
            }
            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }
        private Stack<ControllerWithName> controllerStack;
        public static ControllerManager Instance { get; } = new ControllerManager();
        private ControllerManager() {
            controllerStack = new Stack<ControllerWithName>();
        }
        public void RegisterController(string name, BaseController controller)
        {
            foreach (ControllerWithName itr in controllerStack)
            {
                if (itr.Equals(name))
                {
                    itr.Controller = controller;
                    return;
                }
            }
            controllerStack.Push(new ControllerWithName(name, controller));
        }
        public BaseController PeekController()
        {
            return controllerStack.Count > 0 ? controllerStack.Peek().Controller : null;
        }
        public void PopController()
        {
            controllerStack.Pop();
            if (controllerStack.Count > 0)
                controllerStack.Peek().Controller.Focus();
        }
        public bool FocusController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            if(baseController.gameObject.activeSelf == false)
                baseController.gameObject.SetActive(true);
            baseController.Focus();
            return true;
        }
        public bool UnfocusController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            baseController.UnFocus();
            return true;
        }
        public bool ShowController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            if (baseController.Show() && !baseController.gameObject.activeSelf)
                baseController.gameObject.SetActive(true);
            return true;
        }
        public bool HideController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            if (baseController.UnFocus() && baseController.gameObject.activeSelf)
                baseController.gameObject.SetActive(false);
            return true;
        }
        private BaseController FindBaseController(string path, string root)
        {
            Transform targetObject = GameObject.Find(root).transform.Find(path);
            if (targetObject == null)
                return null;
            return targetObject.GetComponent<BaseController>();
        }
        public T GetWindow<T>(string windowName)
            where T : WindowsController
        {
            return ViewTool.ForceGetComponentInChildren<T>(GameObject.Find("Canvas"), windowName, false);
        }
        public bool LoadWindow(string prefabName)
        {
            GameObject load = Resources.Load<GameObject>(prefabName);
            if (load = null) return false;
            load.name = prefabName;
            RectTransform window = CompTool.ForceGetComponent<RectTransform>(load);
            Transform canvas = GameObject.Find("Canvas").transform;
            window.SetParent(canvas);
            window.anchorMax = window.anchorMin = new Vector2(0.5F, 0.5F);
            window.localPosition = new Vector3(0.0F, 0.0F, 0.0F);
            CompTool.ForceGetComponent<WindowsController, NullWindowsController>(window);
            return true;
        }
    }
}