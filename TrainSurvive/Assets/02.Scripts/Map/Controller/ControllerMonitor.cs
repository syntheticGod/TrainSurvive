/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/19 11:35:16
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;

namespace WorldMap.Controller {
    public class ControllerMonitor : MonoBehaviour
    {
        void Start()
        {
            if (WorldForMap.Instance.IfTeamOuting)
            {
                ControllerManager.Instance.UnfocusController("Train", "Character");
                ControllerManager.Instance.FocusController("Team", "Character");
            }
            else
            {
                ControllerManager.Instance.FocusController("Train", "Character");
                ControllerManager.Instance.HideController("Team", "Character");
            }
        }

        void Update()
        { }
    }
}