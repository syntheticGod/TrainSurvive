/*
 * 描述：自动关闭上下文菜单
 * 作者：刘旭涛
 * 创建时间：2018/10/30 19:20:17
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuClose : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private bool isOuside = false;
    
    void Update() {
        if (Input.GetMouseButton(1)) {
            Destroy(gameObject);
        }
        if (isOuside && Input.GetMouseButton(0)) {
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isOuside = false;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isOuside = true;
    }
}
