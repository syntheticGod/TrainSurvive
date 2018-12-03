/*
 * 描述：建造管理器
 * 作者：刘旭涛
 * 创建时间：2018/11/23 12:15:07
 * 版本：v0.1
 */
using System.Collections;
using UnityEngine;

public class ConstructionManager : MonoBehaviour {

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

    public static ConstructionManager Instance { get; private set; }

    [Tooltip("原颜色")]
    [SerializeField]
    private Color OriginColor = new Color(1, 1, 1, 0.6f);
    [Tooltip("阻止建造的颜色")]
    [SerializeField]
    private Color BlockColor = new Color(1, 0, 0, 0.6f);
    [Tooltip("注册的设施Prefab")]
    [SerializeField]
    private Facility FacilityPrefab;

    /// <summary>
    /// 分类名称
    /// </summary>
    public static string[] Classes { get; } = {
        "能源", "研究", "家具", "加工", "农业"
    };

    /// <summary>
    /// 编译时注册的结构
    /// </summary>
    public static Structure[] Structures { get; } = {
        new HeatCoreStructure()
    };

    /// <summary>
    /// 建筑解锁情况，与world同步。
    /// </summary>
    public static bool[] StructureUnlocks {
        get {
            if (World.getInstance().buildUnlock == null) {
                World.getInstance().buildUnlock = new bool[Structures.Length];
            }
            return World.getInstance().buildUnlock;
        }
    }

    /// <summary>
    /// 当前是否处于放置模式。
    /// </summary>
    public State PlaceState { get; private set; } = State.IDLE;
    
    private void Awake() {
        Instance = this;
        
        // 载入建筑
        Load();
    }
    
    private void OnDestroy() {
        Instance = null;
    }

    /// <summary>
    /// 调用该方法以开始放置一个物体，物体将跟随鼠标位置，左键放置，右键退出。
    /// 当设施与在ProjectSettings/Physics2D中设置的允许与Facility层碰撞的层发生碰撞时将自动标红并阻止放置。
    /// 采用NonAlloc优化GC，因此同时发生的碰撞不得超过缓冲大小（现在是50，大部分情况应该够用了）。
    /// </summary>
    /// <param name="facility">要放置的物体编号（见Structures）。</param>
    public void Place(int index) {
        PlaceState = State.PLACING;
        Facility facility = Instantiate(FacilityPrefab.gameObject).GetComponent<Facility>();
        facility.Structure = Structures[index].Instantiate();
        StartCoroutine(moveFacility(facility));
    }

    /// <summary>
    /// 停止当前的放置模式。
    /// </summary>
    public void StopPlacing() {
        PlaceState = State.IDLE;
    }
    
    /// <summary>
    /// 读档！
    /// </summary>
    private void Load() {
        for (int i = 0; i < World.getInstance().buildInstArray.Count; i++) {
            Structure structure = World.getInstance().buildInstArray[i];
            Facility facility = Instantiate(FacilityPrefab.gameObject, structure.Position, Quaternion.identity).GetComponent<Facility>();
            facility.Structure = structure;
            structure.OnStateChange += OnStructureStateChange;
            facility.gameObject.SetActive(true);
        }
    }

    private IEnumerator moveFacility(Facility facility) {
        Transform fTransform = facility.GetComponent<Transform>();
        SpriteRenderer fSpriteRenderer = facility.GetComponent<SpriteRenderer>();
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (PlaceState == State.PLACING) {
            RaycastHit2D? hit = getPlacablePointByMousePosition(facility.Structure.Info.RequiredLayers);
            bool isCollided = false;
            if (hit.HasValue) {
                fTransform.SetPositionAndRotation(hit.Value.point, Quaternion.identity);
                facility.gameObject.SetActive(true);
                if (facility.Structure.IsCostsAvailable()) {
                    isCollided = checkCollided(facility.gameObject, hit.Value);
                    if (isCollided) {
                        fSpriteRenderer.color = BlockColor;
                    } else {
                        fSpriteRenderer.color = OriginColor;
                    }
                } else {
                    fSpriteRenderer.color = BlockColor;
                }
            } else {
                facility.gameObject.SetActive(false);
            }
            // 按右键退出
            if (Input.GetMouseButton(1)) {
                StopPlacing();
            }
            // 左键放置
            if (Input.GetMouseButton(0) && facility.gameObject.activeSelf && !isCollided) {
                place(facility, fTransform.position, fSpriteRenderer);
            }
            yield return wait;
        }

        if (PlaceState == State.IDLE) {
            facility.gameObject.SetActive(false);
            Destroy(facility.gameObject);
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

    private void place(Facility facility, Vector3 position, SpriteRenderer spriteRenderer) {
        facility.Structure.OnStateChange += OnStructureStateChange;
        if (!facility.Structure.Place(position)) {
            facility.Structure.OnStateChange -= OnStructureStateChange;
            spriteRenderer.color = BlockColor;
            return;
        }
        spriteRenderer.color = facility.HighlightColor;
        PlaceState = State.PLACED;
    }

    private void OnStructureStateChange(Structure structure) {
        switch (structure.FacilityState) {
            case Structure.State.CANCLE:
            case Structure.State.REMOVING:
                World.getInstance().buildInstArray.Remove(structure);
                structure.OnStateChange -= OnStructureStateChange;
                break;
            case Structure.State.BUILDING:
                World.getInstance().buildInstArray.Add(structure);
                break;
        }
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
