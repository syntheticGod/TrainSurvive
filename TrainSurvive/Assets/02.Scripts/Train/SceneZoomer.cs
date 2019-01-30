/*
 * 描述：缩放移动相机
 * 作者：刘旭涛
 * 创建时间：2018/12/2 17:10:57
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneZoomer : MonoBehaviour {

    [Tooltip("最小值")]
    [SerializeField]
    private float MinZoom;

    [Tooltip("最大值")]
    [SerializeField]
    private float MaxZoom;

    [Tooltip("缩放速度")]
    [SerializeField]
    private float ZoomSpeed;

    [Tooltip("拖动速度")]
    [SerializeField]
    private float MoveSpeed;

    [Tooltip("背景包围盒")]
    [SerializeField]
    private BoxCollider2D C_Box;

    private Vector3 LeftBottom {
        get {
            return C_Box.bounds.min;
        }
    }
    private Vector3 RightTop {
        get {
            return C_Box.bounds.max;
        }
    }

    private bool CanDrag { get; set; }
    
    private void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.mouseScrollDelta != Vector2.zero) {
                float orthographicSize = Camera.main.orthographicSize - Input.mouseScrollDelta.y * ZoomSpeed * Time.unscaledDeltaTime;
                orthographicSize = Mathf.Clamp(orthographicSize, MinZoom, MaxZoom);
                Camera.main.orthographicSize = orthographicSize;
                float cameraHalfWidth = orthographicSize * ((float)Screen.width / Screen.height);
                float x = Mathf.Clamp(transform.position.x, LeftBottom.x + cameraHalfWidth, RightTop.x - cameraHalfWidth);
                float y = Mathf.Clamp(transform.position.y, LeftBottom.y + orthographicSize, RightTop.y - orthographicSize);
                transform.position = new Vector3(x, y, transform.position.z);
            }
            if (Input.GetMouseButtonDown(0)) {
                CanDrag = true;
            }
            if (Input.GetMouseButton(0) && CanDrag) {
                float h = Input.GetAxis("Mouse X") * MoveSpeed * Time.unscaledDeltaTime;
                float v = Input.GetAxis("Mouse Y") * MoveSpeed * Time.unscaledDeltaTime;
                float orthographicSize = Camera.main.orthographicSize;
                float cameraHalfWidth = orthographicSize * ((float)Screen.width / Screen.height);
                float x = Mathf.Clamp(transform.position.x - h, LeftBottom.x + cameraHalfWidth, RightTop.x - cameraHalfWidth);
                float y = Mathf.Clamp(transform.position.y - v, LeftBottom.y + orthographicSize, RightTop.y - orthographicSize);
                transform.position = new Vector3(x, y, transform.position.z);
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            CanDrag = false;
        }
    }
}
