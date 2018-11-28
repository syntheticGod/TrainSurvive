/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 19:22:30
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    private Camera mainCamera;
    void Awake()
    {
        mainCamera = Camera.main;
        Debug.Assert(null != mainCamera, "需要将主摄像机的Tag改为MainCamera");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

        }
    }
}
