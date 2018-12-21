/*
 * 描述：建造管理器
 * 作者：刘旭涛
 * 创建时间：2018/11/23 12:15:07
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
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
    /// 结构实体
    /// </summary>
    public LinkedList<Structure> Structures {
        get {
            return World.getInstance().buildInstArray;
        }
    }

    /// <summary>
    /// 结构实体
    /// </summary>
    public LinkedList<TrainCarriage> Carriages {
        get {
            return World.getInstance().carriageInstArray;
        }
    }

    /// <summary>
    /// 建筑分类
    /// </summary>
    public static string[] Classes {
        get {
            return ResourceLoader.GetResource<StructureClassSetting>("Structures/Classes").Classes;
        }
    }

    /// <summary>
    /// 编译时注册的结构
    /// </summary>
    public static StructureSetting[] StructureSettings {
        get {
            if (_structureSettings == null) {
                StructureSetting[] structureSettings = ResourceLoader.GetResources<StructureSetting>("Structures", false);
                int max = -1;
                for (int i = 0; i < structureSettings.Length; i++) {
                    max = Mathf.Max(structureSettings[i].ID, max);
                }
                _structureSettings = new StructureSetting[max + 1];
                for (int i = 0; i < structureSettings.Length; i++) {
                    _structureSettings[structureSettings[i].ID] = structureSettings[i];
                }
            }
            return _structureSettings;
        }
    }

    /// <summary>
    /// 编译时注册的车厢
    /// </summary>
    public static CarriageSetting[] CarriageSettings {
        get {
            if (_carriageSetting == null) {
                CarriageSetting[] structureSettings = ResourceLoader.GetResources<CarriageSetting>("Carriages", false);
                int max = -1;
                for (int i = 0; i < structureSettings.Length; i++) {
                    max = Mathf.Max(structureSettings[i].ID, max);
                }
                _carriageSetting = new CarriageSetting[max + 1];
                for (int i = 0; i < structureSettings.Length; i++) {
                    _carriageSetting[structureSettings[i].ID] = structureSettings[i];
                }
            }
            return _carriageSetting;
        }
    }

    /// <summary>
    /// 当前是否处于放置模式。
    /// </summary>
    public State PlaceState { get; private set; } = State.IDLE;

    private static StructureSetting[] _structureSettings;
    private static CarriageSetting[] _carriageSetting;

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
    /// <param name="id">要放置的物体编号（见StructureSettings）。</param>
    public void Place(int id) {
        PlaceState = State.PLACING;
        Facility facility = Instantiate(FacilityPrefab.gameObject).GetComponent<Facility>();
        facility.Structure = StructureSettings[id].Instantiate();
        StartCoroutine(moveFacility(facility));
    }

    public void Move(Facility facility) {
        PlaceState = State.PLACING;
        StartCoroutine(MoveFacilityPosition(facility));
    }

    /// <summary>
    /// 调用该方法以开始放置一个列车车厢，左键放置，右键退出。
    /// 材料不足时将自动标红并阻止放置。
    /// </summary>
    /// <param name="id">要放置的车厢编号（见CarriageSettings）。</param>
    public void PlaceCarriage(int id) {
        PlaceState = State.PLACING;
        TrainCarriage carriage = new TrainCarriage(id);
        TrainCarriageObject carriageObject = Instantiate(CarriageSettings[id].Prefab).GetComponent<TrainCarriageObject>();
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
        if (Carriages.Count == 0) {
            TrainCarriage carriage = new TrainCarriage(0);
            carriage.Place(new Vector3(0.8f, -0.3f, 0), true);
            Carriages.AddLast(carriage);
            carriage = new TrainCarriage(0);
            carriage.Place(GetPlacablePointForTrainCarriage(carriage), true);
            Carriages.AddLast(carriage);
        }
        foreach (TrainCarriage carriage in Carriages) {
            TrainCarriageObject carriageObj = Instantiate(CarriageSettings[carriage.ID].Prefab, carriage.Position, Quaternion.identity).GetComponent<TrainCarriageObject>();
            carriageObj.TrainCarriage = carriage;
            if (carriage.CarriageState == TrainCarriage.State.BUILDING) {
                carriage.OnStateChange += OnCarriageStateChange;
            }
            carriageObj.gameObject.SetActive(true);
        }
        foreach (Structure structure in Structures) {
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

        UIManager.Instance?.ShowInfoPanel(CarriageSettings[carriageObject.TrainCarriage.ID].Name, CarriageSettings[carriageObject.TrainCarriage.ID].Description);

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
            } else if (Input.GetMouseButton(0)) {
                UIManager.Instance?.ShowInfoPanel(CarriageSettings[carriageObject.TrainCarriage.ID].Name, CarriageSettings[carriageObject.TrainCarriage.ID].Description);
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
        while (!Input.GetMouseButton(0) && (vs > 0.05f || vpx > 0.05f || vpy > 0.05f)) {
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, toSize, ref vs, 0.2f);
            float x = Mathf.SmoothDamp(Camera.main.transform.position.x, toPos.x, ref vpx, 0.2f);
            float y = Mathf.SmoothDamp(Camera.main.transform.position.y, toPos.y, ref vpy, 0.2f);
            Camera.main.transform.SetPositionAndRotation(new Vector3(x, y, Camera.main.transform.position.z), Quaternion.identity);
            yield return 1;
        }
    }

    private IEnumerator MoveFacilityPosition(Facility facility) {
        Transform fTransform = facility.GetComponent<Transform>();
        SpriteRenderer fSpriteRenderer = facility.GetComponent<SpriteRenderer>();
        Color originColor = fSpriteRenderer.color;
        
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (PlaceState == State.PLACING) {
            RaycastHit2D? hit = getPlacablePointByMousePosition(StructureSettings[facility.Structure.ID].RequiredLayers, StructureSettings[facility.Structure.ID].LayerOrientation);
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
                facility.Structure.Position = fTransform.position;
                fSpriteRenderer.color = facility.HighlightColor;
                PlaceState = State.PLACED;
            }
            yield return wait;
        }

        if (PlaceState == State.IDLE) {
            fTransform.SetPositionAndRotation(facility.Structure.Position, Quaternion.identity);
            fSpriteRenderer.color = originColor;
        } else {
            PlaceState = State.IDLE;
        }
    }

    private IEnumerator moveFacility(Facility facility) {
        Transform fTransform = facility.GetComponent<Transform>();
        SpriteRenderer fSpriteRenderer = facility.GetComponent<SpriteRenderer>();

        UIManager.Instance?.ShowInfoPanel(StructureSettings[facility.Structure.ID].Name, StructureSettings[facility.Structure.ID].Description);

        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (PlaceState == State.PLACING) {
            RaycastHit2D? hit = getPlacablePointByMousePosition(StructureSettings[facility.Structure.ID].RequiredLayers, StructureSettings[facility.Structure.ID].LayerOrientation);
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
            } else if (Input.GetMouseButton(0)) {
                UIManager.Instance?.ShowInfoPanel(StructureSettings[facility.Structure.ID].Name, StructureSettings[facility.Structure.ID].Description);
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
            UIManager.Instance?.ShowInfoPanel(StructureSettings[facility.Structure.ID].Name, StructureSettings[facility.Structure.ID].Description);
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
                Structures.Remove(structure);
                structure.OnStateChange -= OnStructureStateChange;
                break;
            case Structure.State.BUILDING:
                Structures.AddLast(structure);
                break;
        }
    }

    private void OnCarriageStateChange(TrainCarriage carriage) {
        switch (carriage.CarriageState) {
            case TrainCarriage.State.IDLE:
                carriage.OnStateChange -= OnCarriageStateChange;
                break;
            case TrainCarriage.State.CANCLE:
                Carriages.Remove(carriage);
                carriage.OnStateChange -= OnCarriageStateChange;
                break;
            case TrainCarriage.State.BUILDING:
                Carriages.AddLast(carriage);
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
        TrainCarriage last = Carriages.Last.Value;
        float x = last.Position.x - CarriageSettings[last.ID].Size.x / 2 - CarriageSettings[last.ID].Size.x / 2;
        float y = CarriageSettings[last.ID].Size.y / 2 - CarriageSettings[last.ID].Size.y / 2 + last.Position.y;
        return new Vector3(x, y, carriage.Position.z);
    }
}
