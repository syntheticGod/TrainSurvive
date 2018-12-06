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

    private int _activeGroup;

    private void Awake() {
        VerticalGroups = new GameObject[ConstructionManager.Classes.Length];
        for (int i = 0; i < ConstructionManager.Classes.Length; i++) {
            int index = i;
            GameObject buttonGO = Instantiate(ButtonPrefab, ClassContent);
            buttonGO.GetComponentInChildren<Text>().text = ConstructionManager.Classes[index];
            VerticalGroups[index] = Instantiate(VerticalGroupPrefab, StructureContent);
            VerticalGroups[index].SetActive(false);
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                if (index == ConstructionManager.Classes.Length - 1) {
                    bool flag = World.getInstance().carriageInstArray[World.getInstance().carriageInstArray.Count - 1].CarriageState == TrainCarriage.State.IDLE;
                    for (int j = 0; j < ConstructionManager.CarriageUnlocks.Length; j++) {
                        Structures[ConstructionManager.StructureUnlocks.Length + j].SetActive(ConstructionManager.CarriageUnlocks[j]);
                        Structures[ConstructionManager.StructureUnlocks.Length + j].GetComponent<Button>().interactable = flag;
                    }
                } else {
                    for (int j = 0; j < ConstructionManager.StructureUnlocks.Length; j++) {
                        Structures[j].SetActive(ConstructionManager.StructureUnlocks[j]);
                    }
                }
                ActiveGroup = index;
                StructuresScrollRect.SetActive(true);
            });
        }
        Structures = new GameObject[ConstructionManager.Structures.Length + ConstructionManager.Carriages.Length];
        for (int i = 0; i < ConstructionManager.Structures.Length; i++) {
            int index = i;
            GameObject buttonGO = Instantiate(ButtonPrefab, VerticalGroups[ConstructionManager.Structures[index].Info.Class].transform);
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                ConstructionManager.Instance.Place(index);
                StructuresScrollRect.SetActive(false);
                ConstructionObject.SetActive(false);
            });
            buttonGO.GetComponentInChildren<Text>().text = ConstructionManager.Structures[index].Info.Name;
            Structures[index] = buttonGO;
        }
        for (int i = 0; i < ConstructionManager.Carriages.Length; i++) {
            int index = i;
            GameObject buttonGO = Instantiate(ButtonPrefab, VerticalGroups[VerticalGroups.Length - 1].transform);
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                ConstructionManager.Instance.PlaceCarriage(index);
                StructuresScrollRect.SetActive(false);
                ConstructionObject.SetActive(false);
            });
            buttonGO.GetComponentInChildren<Text>().text = ConstructionManager.Carriages[index].Info.Name;
            Structures[index + ConstructionManager.Structures.Length] = buttonGO;
        }
    }
}
