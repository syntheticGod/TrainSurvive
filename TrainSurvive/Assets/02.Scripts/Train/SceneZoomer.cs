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

    private void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.mouseScrollDelta != Vector2.zero) {
                float value = Camera.main.orthographicSize - Input.mouseScrollDelta.y * ZoomSpeed * Time.unscaledDeltaTime;
                value = Mathf.Clamp(value, MinZoom, MaxZoom);
                if (transform.position.y + value > MaxZoom) {
                    transform.Translate(0, Camera.main.orthographicSize - value, 0, Space.World);
                }else if(transform.position.y - value < -MaxZoom) {
                    transform.Translate(0, value - Camera.main.orthographicSize, 0, Space.World);
                }
                Camera.main.orthographicSize = value;
            }
            if (Input.GetMouseButton(0)) {
                float h = Input.GetAxis("Mouse X") * MoveSpeed * Time.unscaledDeltaTime;
                float v = Input.GetAxis("Mouse Y") * MoveSpeed * Time.unscaledDeltaTime;
                Vector3 newPos = transform.position - new Vector3(h, v, 0);
                if (newPos.y + Camera.main.orthographicSize <= MaxZoom && newPos.y - Camera.main.orthographicSize >= -MaxZoom) {
                    transform.Translate(0, -v, 0, Space.World);
                }
                transform.Translate(-h, 0, 0, Space.World);
            }
        }
    }
}
