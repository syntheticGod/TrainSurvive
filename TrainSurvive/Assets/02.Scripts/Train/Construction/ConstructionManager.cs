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
      //  0       1      2       3      4
        "能源", "研究", "家具", "加工", "农业",
        "车厢"
    };

    /// <summary>
    /// 编译时注册的车厢
    /// </summary>
    public static TrainCarriage[] Carriages { get; } = {
        /* 0 */ new CabCarriage()
    };

    /// <summary>
    /// 编译时注册的结构
    /// </summary>
    public static Structure[] Structures { get; } = {
        /* 0 */ new EnergyCoreStructure(),
        /* 1 */ new SteamEngineStructure(),
        /* 2 */ new GasEngineStructure(),
        /* 3 */ new BatteryStructure(),
        /* 4 */ new GeneratorStructure(),
        /* 5 */ new BigGeneratorStructure(),
        /* 6 */ new MagicCoreStructure(),
        /* 7 */ new ResearchBenchStructure(),
        /* 8 */ new ResearchBench2Structure(),
        /* 9 */ new ResearchBench3Structure(),
        /* 10 */ new ResearchBench4Structure(),
        /* 11 */ new ResearchBench5Structure(),
        /* 12 */ new BedStructure(),
        /* 13 */ new Bed2Structure(),
        /* 14 */ new Bed3Structure(),
        /* 15 */ new WoodTableStructure(),
        /* 16 */ new WoodChairStructure(),
        /* 17 */ new WoodBoxStructure(),
        /* 18 */ new TorchStructure(),
        /* 19 */ new KeroseneLampStructure(),
        /* 20 */ new DropLightStructure(),
        /* 21 */ new TableLampStructure(),
        /* 22 */ new HeatingRoomStructure(),
        /* 23 */ new CoolingRoomStructure(),
        /* 24 */ new SmallWoodStructure(),
        /* 25 */ new MediumWoodStructure(),
        /* 26 */ new BigWoodStructure()
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
    /// 车厢解锁情况，与world同步。
    /// </summary>
    public static bool[] CarriageUnlocks {
        get {
            if (World.getInstance().carriageUnlock == null) {
                World.getInstance().carriageUnlock = new bool[Carriages.Length];
            }
            return World.getInstance().carriageUnlock;
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
    /// <param name="index">要放置的物体编号（见Structures）。</param>
    public void Place(int index) {
        PlaceState = State.PLACING;
        Facility facility = Instantiate(FacilityPrefab.gameObject).GetComponent<Facility>();
        facility.Structure = Structures[index].Instantiate();
        StartCoroutine(moveFacility(facility));
    }

    /// <summary>
    /// 调用该方法以开始放置一个列车车厢，左键放置，右键退出。
    /// 材料不足时将自动标红并阻止放置。
    /// </summary>
    /// <param name="facility">要放置的车厢编号（见Carriages）。</param>
    public void PlaceCarriage(int index) {
        PlaceState = State.PLACING;
        TrainCarriage carriage = Carriages[index].Instantiate();
        TrainCarriageObject carriageObject = Instantiate(carriage.Info.Object).GetComponent<TrainCarriageObject>();
        carriageObject.TrainCarriage = carriage;
        carriageObject.gameObject.SetActive(true);
        StartCoroutine(placingCarriage(carriageObject));
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
        if (World.getInstance().carriageInstArray.Count == 0) {
            TrainCarriage carriage = new CabCarriage();
            carriage.Place(new Vector3(0.8f, -0.3f, 0), true);
            World.getInstance().carriageInstArray.Add(carriage);
            carriage = new CabCarriage();
            carriage.Place(GetPlacablePointForTrainCarriage(carriage), true);
            World.getInstance().carriageInstArray.Add(carriage);
        }
        for (int i = 0; i < World.getInstance().carriageInstArray.Count; i++) {
            TrainCarriage carriage = World.getInstance().carriageInstArray[i];
            TrainCarriageObject carriageObj = Instantiate(carriage.Info.Object, carriage.Position, Quaternion.identity).GetComponent<TrainCarriageObject>();
            carriageObj.TrainCarriage = carriage;
            if (carriage.CarriageState == TrainCarriage.State.BUILDING) {
                carriage.OnStateChange += OnCarriageStateChange;
            }
            carriageObj.gameObject.SetActive(true);
        }
        for (int i = 0; i < World.getInstance().buildInstArray.Count; i++) {
            Structure structure = World.getInstance().buildInstArray[i];
            Facility facility = Instantiate(FacilityPrefab.gameObject, structure.Position, Quaternion.identity).GetComponent<Facility>();
            facility.Structure = structure;
            structure.OnStateChange += OnStructureStateChange;
            facility.gameObject.SetActive(true);
        }
    }

    private IEnumerator placingCarriage(TrainCarriageObject carriageObject) {
        Transform carriageTransform = carriageObject.GetComponent<Transform>();
        Vector3 point = GetPlacablePointForTrainCarriage(carriageObject.TrainCarriage);
        carriageTransform.SetPositionAndRotation(point, Quaternion.identity);

        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float vOrthographicSize = 0;
        float vxCameraPos = 0;
        float vyCameraPos = 0;
        float originOrthographicSize = Camera.main.orthographicSize;
        Vector3 originCameraPos = Camera.main.transform.position;
        while (PlaceState == State.PLACING) {
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, 5, ref vOrthographicSize, 0.2f);
            float x = Mathf.SmoothDamp(Camera.main.transform.position.x, point.x, ref vxCameraPos, 0.2f);
            float y = Mathf.SmoothDamp(Camera.main.transform.position.y, point.y, ref vyCameraPos, 0.2f);
            Camera.main.transform.SetPositionAndRotation(new Vector3(x, y, Camera.main.transform.position.z), Quaternion.identity);

            bool canPlace = false;
            if (carriageObject.TrainCarriage.IsCostsAvailable()) {
                for (int i = 0; i < carriageObject.SpriteRenderers.Length; i++) {
                    carriageObject.SpriteRenderers[i].color = OriginColor;
                }
                canPlace = true;
            } else {
                for (int i = 0; i < carriageObject.SpriteRenderers.Length; i++) {
                    carriageObject.SpriteRenderers[i].color = BlockColor;
                }
            }

            // 按右键退出
            if (Input.GetMouseButton(1)) {
                StopPlacing();
                StartCoroutine(moveCamera(originCameraPos, originOrthographicSize));
            }
            // 左键放置
            if (Input.GetMouseButton(0) && canPlace) {
                placeCarriage(carriageObject, carriageTransform.position);
            }
            yield return wait;
        }

        if (PlaceState == State.IDLE) {
            Destroy(carriageObject.gameObject);
        } else {
            PlaceState = State.IDLE;
        }
    }

    private IEnumerator moveCamera(Vector3 toPos, float toSize) {
        float vs = 0.06f;
        float vpx = 0.06f;
        float vpy = 0.06f;
        while (vs > 0.05f || vpx > 0.05f || vpy > 0.05f) {
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, toSize, ref vs, 0.2f);
            float x = Mathf.SmoothDamp(Camera.main.transform.position.x, toPos.x, ref vpx, 0.2f);
            float y = Mathf.SmoothDamp(Camera.main.transform.position.y, toPos.y, ref vpy, 0.2f);
            Camera.main.transform.SetPositionAndRotation(new Vector3(x, y, Camera.main.transform.position.z), Quaternion.identity);
            yield return 1;
        }
    }

    private IEnumerator moveFacility(Facility facility) {
        Transform fTransform = facility.GetComponent<Transform>();
        SpriteRenderer fSpriteRenderer = facility.GetComponent<SpriteRenderer>();
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (PlaceState == State.PLACING) {
            RaycastHit2D? hit = getPlacablePointByMousePosition(facility.Structure.Info.RequiredLayers, facility.Structure.Info.LayerOrientation);
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
    
    private void placeCarriage(TrainCarriageObject carriageObject, Vector3 position) {
        carriageObject.TrainCarriage.OnStateChange += OnCarriageStateChange;
        if (!carriageObject.TrainCarriage.Place(position, false)) {
            carriageObject.TrainCarriage.OnStateChange -= OnCarriageStateChange;
            for (int i = 0; i < carriageObject.SpriteRenderers.Length; i++) {
                carriageObject.SpriteRenderers[i].color = BlockColor;
            }
            return;
        }
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

    private void OnCarriageStateChange(TrainCarriage carriage) {
        switch (carriage.CarriageState) {
            case TrainCarriage.State.IDLE:
                carriage.OnStateChange -= OnCarriageStateChange;
                break;
            case TrainCarriage.State.CANCLE:
                World.getInstance().carriageInstArray.Remove(carriage);
                carriage.OnStateChange -= OnCarriageStateChange;
                break;
            case TrainCarriage.State.BUILDING:
                World.getInstance().carriageInstArray.Add(carriage);
                break;
        }
    }

    private static RaycastHit2D? getPlacablePointByMousePosition(LayerMask acceptableLayers, Vector2 orientation) {
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(_mousePos.x, _mousePos.y);
        float minDistance = float.MaxValue;
        RaycastHit2D? result = null;
        RaycastHit2D[] hits = new RaycastHit2D[50];
        int length = Physics2D.RaycastNonAlloc(mousePos, orientation, hits, float.MaxValue, acceptableLayers.value);
        for (int i = 0; i < length; i++) {
            RaycastHit2D hit = hits[i];
            if (hit.collider && hit.distance < minDistance && hit.distance > 0) {
                result = hit;
                minDistance = hit.distance;
            }
        }
        return result;
    }

    private Vector3 GetPlacablePointForTrainCarriage(TrainCarriage carriage) {
        TrainCarriage last = World.getInstance().carriageInstArray[World.getInstance().carriageInstArray.Count - 1];
        float x = last.Position.x - last.Info.Size.x / 2 - carriage.Info.Size.x / 2;
        float y = carriage.Info.Size.y / 2 - last.Info.Size.y / 2 + last.Position.y;
        return new Vector3(x, y, carriage.Position.z);
    }
}
