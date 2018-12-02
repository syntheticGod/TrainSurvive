/*
 * 描述：自动关闭菜单
 * 作者：刘旭涛
 * 创建时间：2018/10/30 19:20:17
 * 版本：v0.1
 */
using UnityEngine;

public class AutoClose : MonoBehaviour {

    [Tooltip("不自动关闭的物体")]
    [SerializeField]
    private RectTransform[] IgnoreObjects;

    [Tooltip("自动关闭的物体")]
    [SerializeField]
    private GameObject[] CloseObjects;
    
    private Rect[] IgnoreRects { get; set; }

    private void Awake() {
        IgnoreRects = new Rect[IgnoreObjects.Length];
        for (int i = 0; i < IgnoreRects.Length; i++) {
            Vector3[] corners = new Vector3[4];
            IgnoreObjects[i].GetWorldCorners(corners);
            IgnoreRects[i] = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) {
            bool flag = true;
            for (int i = 0; i < IgnoreRects.Length; i++) {
                if (IgnoreObjects[i].gameObject.activeSelf && IgnoreRects[i].Contains(Input.mousePosition)) {
                    flag = false;
                    break;
                }
            }
            if (flag) {
                for (int i = 0; i < CloseObjects.Length; i++) {
                    CloseObjects[i].SetActive(false);
                }
            }
        }
    }
}
