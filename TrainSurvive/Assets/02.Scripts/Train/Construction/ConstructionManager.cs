/*
 * 描述：建造管理器
 * 作者：刘旭涛
 * 创建时间：2018/11/23 12:15:07
 * 版本：v0.1
 */
using System.Collections;
using UnityEngine;

public class ConstructionManager : MonoBehaviour {
    
    public static ConstructionManager Instance { get; private set; }

    [Tooltip("注册的设施Prefab")]
    public Facility[] FacilityPrefabs;

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

    public struct BuildInst {
        public int Prefab;
        public Vector3 Position;
        public object Others;
    }

    /// <summary>
    /// 当前是否处于放置模式。
    /// </summary>
    public State PlaceState { get; private set; } = State.IDLE;

    private Color OriginColor { get; } = new Color(1, 1, 1, 0.6f);
    private Color BlockColor { get; } = new Color(1, 0, 0, 0.6f);

    private void Awake() {
        Instance = this;
        for (int i = 0; i < FacilityPrefabs.Length; i++) {
            FacilityPrefabs[i].ID = i;
        }
    }

    /// <summary>
    /// 加载对象时，注册事件，同时载入状态。
    /// </summary>
    private void OnEnable() {
        World.getInstance().saveDelegateHandler += Save;
        Load();
    }

    /// <summary>
    /// 停止时自动保存内容到World，同时移除事件。
    /// </summary>
    private void OnDisable() {
        World.getInstance().saveDelegateHandler -= Save;
        Save();
    }

    /// <summary>
    /// 调用该方法以开始放置一个物体，物体将跟随鼠标位置，左键放置，右键退出。
    /// 当设施与在ProjectSettings/Physics2D中设置的允许与Facility层碰撞的层发生碰撞时将自动标红并阻止放置。
    /// 采用NonAlloc优化GC，因此同时发生的碰撞不得超过缓冲大小（现在是50，大部分情况应该够用了）。
    /// </summary>
    /// <param name="facility">要放置的物体编号（见FacilityPrefabs）。</param>
    public void Place(int prefab) {
        PlaceState = State.PLACING;
        GameObject facilityGO = Instantiate(FacilityPrefabs[prefab].gameObject);
        facilityGO.SetActive(false);
        StartCoroutine(moveFacility(facilityGO));
    }

    /// <summary>
    /// 停止当前的放置模式。
    /// </summary>
    public void StopPlacing() {
        PlaceState = State.IDLE;
    }
    
    /// <summary>
    /// 有存档，
    /// </summary>
    public void Save() {
        Facility[] facilities = FindObjectsOfType<Facility>();
        BuildInst[] buildInsts = new BuildInst[facilities.Length];
        for (int i = 0; i < facilities.Length; i++) {
            buildInsts[i] = new BuildInst {
                Prefab = facilities[i].ID,
                Position = facilities[i].gameObject.transform.position,
                Others = facilities[i].OnSave()
            };
        }
        World.getInstance().buildInstArray = buildInsts;
    }

    /// <summary>
    /// 就有读档！
    /// </summary>
    public void Load() {
        BuildInst[] buildInsts = World.getInstance().buildInstArray;
        if (buildInsts == null)
            return;
        for (int i = 0; i < buildInsts.Length; i++) {
            Facility facility = Instantiate(FacilityPrefabs[buildInsts[i].Prefab].gameObject, buildInsts[i].Position, Quaternion.identity).GetComponent<Facility>();
            facility.OnLoad(buildInsts[i].Others);
        }
    }

    private IEnumerator moveFacility(GameObject facilityGO) {
        Facility facility = facilityGO.GetComponent<Facility>();
        Transform fTransform = facilityGO.GetComponent<Transform>();
        SpriteRenderer fSpriteRenderer = facilityGO.GetComponent<SpriteRenderer>();
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (PlaceState == State.PLACING) {
            RaycastHit2D? hit = getPlacablePointByMousePosition(facility.RequireLayers);
            bool isCollided = false;
            if (hit.HasValue) {
                fTransform.SetPositionAndRotation(hit.Value.point, Quaternion.identity);
                facilityGO.SetActive(true);
                if (facility.IsCostsAvailable()) {
                    isCollided = checkCollided(facilityGO, hit.Value);
                    if (isCollided) {
                        fSpriteRenderer.color = BlockColor;
                    } else {
                        fSpriteRenderer.color = OriginColor;
                    }
                } else {
                    fSpriteRenderer.color = BlockColor;
                }
            } else {
                facilityGO.SetActive(false);
            }
            // 按右键退出
            if (Input.GetMouseButton(1)) {
                StopPlacing();
            }
            // 左键放置
            if (Input.GetMouseButton(0) && facilityGO.activeSelf && !isCollided) {
                place(facility, fSpriteRenderer);
            }
            yield return wait;
        }

        if (PlaceState == State.IDLE) {
            facilityGO.SetActive(false);
            Destroy(facilityGO);
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

    private void place(Facility facility, SpriteRenderer spriteRenderer) {
        if (!facility.OnPlaced()) {
            spriteRenderer.color = BlockColor;
            return;
        }
        spriteRenderer.color = facility.HighlightColor;
        PlaceState = State.PLACED;
    }

    private static RaycastHit2D? getPlacablePointByMousePosition(LayerMask acceptableLayers) {
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(_mousePos.x, _mousePos.y);
        float minDistance = float.MaxValue;
        RaycastHit2D? result = null;
        RaycastHit2D[] hits = new RaycastHit2D[50];
        int length = Physics2D.RaycastNonAlloc(mousePos, Vector2.down, hits, float.MaxValue, acceptableLayers.value);
        for (int i = 0; i < length; i++) {
            RaycastHit2D hit = hits[i];
            if (hit.collider && hit.distance < minDistance && hit.distance > 0) {
                result = hit;
                minDistance = hit.distance;
            }
        }
        return result;
    }
}
