/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 23:14:58
 * 版本：v0.1
 */
using UnityEngine;

using TTT.Utility;

namespace WorldMap.Controller
{
    public class ControllerManager
    {
        public static bool FocusController(string gameObjectPath, string root)
        {
            Transform targetObject = GameObject.Find(root).transform.Find(gameObjectPath);
            if (targetObject == null) return false;
            BaseController baseController = targetObject.GetComponent<BaseController>();
            if (baseController == null) return false;
            baseController.Focus();
            return true;
        }
        public static void ShowWindow<T>(string windowName)
            where T : WindowsController
        {
            GetWindow<T>(windowName).ShowWindow();
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
            RectTransform window = ViewTool.ForceGetComponent<RectTransform>(load);
            Transform canvas = GameObject.Find("Canvas").transform;
            window.SetParent(canvas);
            window.anchorMax = window.anchorMin = new Vector2(0.5F, 0.5F);
            window.localPosition = new Vector3(0.0F, 0.0F, 0.0F);
            ViewTool.ForceGetComponent<WindowsController, NullWindowsController>(window);
            return true;
        }
        public static bool HideWindow(WindowsController windows)
        {
            if (windows == null) return false;
            windows.HideWindow();
            return true;
        }
    }
}