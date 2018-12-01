/*
 * 描述：酒馆界面对象
 * 作者：项叶盛
 * 创建时间：2018/11/28 21:36:49
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;

public class TavernUI
{
    GameObject CreateUI(Transform parent)
    {
        GameObject gameObject = new GameObject("TavernViewer", typeof(RectTransform));
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5F, 0.5F);
        gameObject.transform.parent = parent;

        return gameObject;
    }
}
