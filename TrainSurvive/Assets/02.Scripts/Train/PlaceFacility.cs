/*
 * 描述：放置设施，通过调用Place(MonoBehaviour, Facility)方法唤起放置操作
 * 作者：刘旭涛
 * 创建时间：2018/10/29 22:20:03
 * 版本：v0.1
 */
using System.Collections;
using UnityEngine;

public class PlaceFacility {

    public enum State {
        /// <summary>
        /// 不在放置模式
        /// </summary>
        IDLE,
        /// <summary>
        /// 放置模式中
        /// </summary>
        PLACING,
        /// <summary>
        /// 已放置（该状态只有一瞬）
        /// </summary>
        PLACED
    }

    /// <summary>
    /// 当前是否处于放置模式。
    /// </summary>
    public static State PlaceState { get; private set; } = State.IDLE;

    /// <summary>
    /// 调用该方法以开始放置一个物体，物体将跟随鼠标位置，左键放置，右键退出。
    /// 当设施与在ProjectSettings/Physics2D中设置的允许与Facility层碰撞的层发生碰撞时将自动标红并阻止放置。
    /// 采用OverlapBoxNonAlloc优化GC，因此同时发生的碰撞不得超过缓冲大小（现在是50，大部分情况应该够用了）。
    /// </summary>
    /// <param name="context">提供一个执行协程的上下文，必须是Active的。</param>
    /// <param name="facility">要放置的物体。</param>
    public static void Place(MonoBehaviour context, Facility prefab) {
        PlaceState = State.PLACING;
        GameObject facilityGO = Object.Instantiate(prefab.gameObject);
        facilityGO.SetActive(false);
        context.StartCoroutine(moveFacility(facilityGO));
    }

    /// <summary>
    /// 停止当前的放置模式。
    /// </summary>
    public static void StopPlacing() {
        PlaceState = State.IDLE;
    }

    private static IEnumerator moveFacility(GameObject facilityGO) {
        Facility facility = facilityGO.GetComponent<Facility>();
        Transform fTransform = facilityGO.GetComponent<Transform>();
        SpriteRenderer fSpriteRenderer = facilityGO.GetComponent<SpriteRenderer>();
        Color fSpriteColor = fSpriteRenderer.color;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (PlaceState == State.PLACING) {
            RaycastHit2D? hit = getPlacablePointByMousePosition(facility.RequireLayers);
            bool isCollided = false;
            if (hit.HasValue) {
                fTransform.SetPositionAndRotation(hit.Value.point, Quaternion.identity);
                facilityGO.SetActive(true);
                isCollided = checkCollided(facilityGO, hit.Value);
                if (isCollided) {
                    fSpriteRenderer.color = new Color(1, 0, 0, 0.6f);
                } else {
                    fSpriteRenderer.color = fSpriteColor;
                }
            } else {
                facilityGO.SetActive(false);
            }
            if (Input.GetMouseButton(1)) {
                StopPlacing();
            }
            if (Input.GetMouseButton(0) && facilityGO.activeSelf && !isCollided) {
                place(facility);
            }
            yield return wait;
        }
        if (PlaceState == State.IDLE) {
            facilityGO.SetActive(false);
            Object.Destroy(facilityGO);
        } else {
            PlaceState = State.IDLE;
        }
    }

    private static bool checkCollided(GameObject facilityGO, RaycastHit2D hit) {
        Collider2D[] colliders = new Collider2D[50];
        int length = Physics2D.OverlapBoxNonAlloc(facilityGO.GetComponent<Collider2D>().bounds.center, facilityGO.GetComponent<Collider2D>().bounds.size, 0, colliders);
        for (int i = 0; i < length; i++) {
            if (facilityGO != colliders[i].gameObject && hit.collider.gameObject != colliders[i].gameObject) {
                if (!Physics2D.GetIgnoreLayerCollision(facilityGO.layer, hit.collider.gameObject.layer)) {
                    return true;
                }
            }
        }
        return false;
    }

    private static void place(Facility facility) {
        facility.OnPlaced();
        PlaceState = State.PLACED;
    }

    private static RaycastHit2D? getPlacablePointByMousePosition(LayerMask[] acceptableLayers) {
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(_mousePos.x, _mousePos.y);
        float minDistance = float.MaxValue;
        RaycastHit2D? result = null;
        for (int i = 0; i < acceptableLayers.Length; i++) {
            LayerMask layer = acceptableLayers[i];
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.down, 999, layer.value);
            if (hit.collider && hit.distance < minDistance && hit.distance > 0) {
                result = hit;
            }
        }
        return result;
    }
}
