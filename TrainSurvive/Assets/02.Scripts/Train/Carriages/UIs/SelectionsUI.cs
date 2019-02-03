/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/3 13:31:32
 * 版本：v0.7
 */
using System;
using UnityEngine;

public class SelectionsUI : MonoBehaviour {
    private GameObject _p_SelectionItem;
    private Transform _c_Content;

    private GameObject P_SelectionItem {
        get {
            if (_p_SelectionItem == null) {
                _p_SelectionItem = ResourceLoader.GetResource<GameObject>("Prefabs/Train/SelectionItem");
            }
            return _p_SelectionItem;
        }
    }

    private Transform C_Content {
        get {
            if (_c_Content == null) {
                _c_Content = transform.Find("Viewport/Content");
            }
            return _c_Content;
        }
    }

    public void SetData(int[] itemIDs, Action<int> action) {
        for (int i = 0; i < C_Content.childCount; i++) {
            Destroy(C_Content.GetChild(0));
        }
        foreach (int item in itemIDs) {
            SelectionItemUI selectionItem = Instantiate(P_SelectionItem, C_Content).GetComponent<SelectionItemUI>();
            selectionItem.SetData(item, (itemData) => { action?.Invoke(itemData); gameObject.SetActive(false); });
        }
    }
}
