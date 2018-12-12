/*
 * 描述：建筑列表
 * 作者：刘旭涛
 * 创建时间：2018/12/2 10:45:13
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

public class StructMenu : MonoBehaviour {

    [Tooltip("建筑菜单物体")]
    [SerializeField]
    private GameObject ConstructionObject;

    [Tooltip("建筑ScrollRect")]
    [SerializeField]
    private GameObject StructuresScrollRect;

    [Tooltip("建筑Content")]
    [SerializeField]
    private RectTransform StructureContent;

    [Tooltip("分类Content")]
    [SerializeField]
    private RectTransform ClassContent;

    [Tooltip("按钮的Prefab")]
    [SerializeField]
    private GameObject ButtonPrefab;

    [Tooltip("Vertical Group Prefab")]
    [SerializeField]
    private GameObject VerticalGroupPrefab;

    /// <summary>
    /// 每个分类的Group
    /// </summary>
    private GameObject[] VerticalGroups { get; set; }

    private int ActiveGroup {
        get {
            return _activeGroup;
        }
        set {
            VerticalGroups[_activeGroup].SetActive(false);
            VerticalGroups[value].SetActive(true);
            _activeGroup = value;
        }
    }

    private GameObject[] Structures { get; set; }
    private GameObject[] Carriages { get; set; }

    private int _activeGroup;

    private void Awake() {
        VerticalGroups = new GameObject[ConstructionManager.Classes.Length + 1];
        for (int i = 0; i < VerticalGroups.Length; i++) {
            int index = i;
            VerticalGroups[index] = Instantiate(VerticalGroupPrefab, StructureContent);
            VerticalGroups[index].SetActive(false);
            GameObject buttonGO = Instantiate(ButtonPrefab, ClassContent);
            if(i < ConstructionManager.Classes.Length) {
                buttonGO.GetComponentInChildren<Text>().text = ConstructionManager.Classes[index];
                buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                    for (int j = 0; j < ConstructionManager.StructureSettings.Length; j++) {
                        if (ConstructionManager.StructureSettings[j] == null) {
                            continue;
                        }
                        Structures[j].SetActive(ConstructionManager.StructureSettings[j].HasUnlocked());
                    }
                    ActiveGroup = index;
                    StructuresScrollRect.SetActive(true);
                });
            } else {
                buttonGO.GetComponentInChildren<Text>().text = "车厢";
                buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                    bool flag = ConstructionManager.Instance.Carriages.Last.Value.CarriageState == TrainCarriage.State.IDLE;
                    for (int j = 0; j < ConstructionManager.CarriageSettings.Length; j++) {
                        if (ConstructionManager.CarriageSettings[j] == null) {
                            continue;
                        }
                        Carriages[j].SetActive(ConstructionManager.CarriageSettings[j].HasUnlocked());
                        Carriages[j].GetComponent<Button>().interactable = flag;
                    }
                    ActiveGroup = index;
                    StructuresScrollRect.SetActive(true);
                });
            }
        }
        
        Structures = new GameObject[ConstructionManager.StructureSettings.Length];
        for (int i = 0; i < ConstructionManager.StructureSettings.Length; i++) {
            if (ConstructionManager.StructureSettings[i] == null) {
                continue;
            }
            int index = i;
            GameObject buttonGO = Instantiate(ButtonPrefab, VerticalGroups[ConstructionManager.StructureSettings[i].Class].transform);
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                ConstructionManager.Instance.Place(index);
                StructuresScrollRect.SetActive(false);
                ConstructionObject.SetActive(false);
            });
            buttonGO.GetComponentInChildren<Text>().text = ConstructionManager.StructureSettings[index].Name;
            Structures[index] = buttonGO;
        }

        Carriages = new GameObject[ConstructionManager.CarriageSettings.Length];
        for (int i = 0; i < ConstructionManager.CarriageSettings.Length; i++) {
            if (ConstructionManager.CarriageSettings[i] == null) {
                continue;
            }
            int index = i;
            GameObject buttonGO = Instantiate(ButtonPrefab, VerticalGroups[VerticalGroups.Length - 1].transform);
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                ConstructionManager.Instance.PlaceCarriage(index);
                StructuresScrollRect.SetActive(false);
                ConstructionObject.SetActive(false);
            });
            buttonGO.GetComponentInChildren<Text>().text = ConstructionManager.CarriageSettings[index].Name;
            Carriages[index] = buttonGO;
        }
    }
}
