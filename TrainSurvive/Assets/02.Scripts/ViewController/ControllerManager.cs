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
namespace TTT.Controller
{
    public static class ControllerManager
    {
        public static bool FocusController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            if(baseController.gameObject.activeSelf == false)
                baseController.gameObject.SetActive(true);
            baseController.Focus();
            return true;
        }
        public static bool UnfocusController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            baseController.UnFocus();
            return true;
        }
        public static bool ShowController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            if (baseController.Show() && !baseController.gameObject.activeSelf)
                baseController.gameObject.SetActive(true);
            return true;
        }
        public static bool HideController(string path, string root)
        {
            BaseController baseController = FindBaseController(path, root);
            if (baseController == null)
                return false;
            if (baseController.UnFocus() && baseController.gameObject.activeSelf)
                baseController.gameObject.SetActive(false);
            return true;
        }
        private static BaseController FindBaseController(string path, string root)
        {
            Transform targetObject = GameObject.Find(root).transform.Find(path);
            if (targetObject == null)
                return null;
            return targetObject.GetComponent<BaseController>();
        }
        public static T GetWindow<T>(string windowName)
            where T : WindowsController
        {
            return ViewTool.ForceGetComponentInChildren<T>(GameObject.Find("Canvas"), windowName, false);
        }
        public static bool LoadWindow(string prefabName)
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