/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/2 14:26:46
 * 版本：v0.7
 */
using System;
using UnityEngine;

public class Item_ItemSUI : InitableUI {
    
    public GameObject FormulaPrefab {
        get {
            if (_FormulaPrefab == null) {
                _FormulaPrefab = ResourceLoader.GetResource<GameObject>("Prefabs/Train/Formula_1_1");
            }
            return _FormulaPrefab;
        }
    }
    public Transform ScrollContent {
        get {
            if (_ScrollContent == null) {
                _ScrollContent = transform.Find("Viewport/Content");
            }
            return _ScrollContent;
        }
    }

    private Item_ItemStructure Structure { get; set; }

    private Transform _ScrollContent;
    private GameObject _FormulaPrefab;
    
    private void OnEnable() {
        UpdateUI();

        foreach (Formula<Item_ItemStructure.Conversion> formula in Structure.Conversions) {
            formula.OnAcquireCount += Formula_OnAcquireCount;
            formula.OnCountChanged += Formula_OnCountChanged;
            formula.OnProgressChanged += Formula_OnProgressChanged;
        }
    }

    private void OnDisable() {
        foreach (Formula<Item_ItemStructure.Conversion> formula in Structure.Conversions) {
            formula.OnAcquireCount -= Formula_OnAcquireCount;
            formula.OnCountChanged -= Formula_OnCountChanged;
            formula.OnProgressChanged -= Formula_OnProgressChanged;
            formula.Count = ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_1>().ProduceCount;
        }
    }

    private void Formula_OnProgressChanged(int priority, float max, float value) {
        ScrollContent.GetChild(priority).GetComponent<FormulaUI_1_1>().ChangeProgress(0, max, value);
    }

    private void Formula_OnCountChanged(int priority, int count) {
        ScrollContent.GetChild(priority).GetComponent<FormulaUI_1_1>().ProduceCount = count;
    }

    private int Formula_OnAcquireCount(int priority) {
        return ScrollContent.GetChild(priority).GetComponent<FormulaUI_1_1>().ProduceCount;
    }

    private void UpdateUI() {
        foreach (Formula<Item_ItemStructure.Conversion> formula in Structure.Conversions) {
            FormulaUI_1_1 formulaUI = ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_1>();
            if (formula.Conversion.UnlockUpgradeID == -1 || Structure.CarriageBackend.UpgradedID.Contains(formula.Conversion.UnlockUpgradeID)) {
                formulaUI.gameObject.SetActive(true);
                formulaUI.ProduceCount = formula.Count;
                formulaUI.ChangeProgress(0, formula.Conversion.ProcessTime, formula.Progress);
                formulaUI.Time = (int)(formula.Conversion.ProcessTime * 10 / (Structure.ProcessSpeed * Structure.ProcessSpeedRatio));
            } else {
                formulaUI.gameObject.SetActive(false);
            }
        }
    }

    public override void Init(CarriageBackend carriage) {
        Structure = carriage.Structures[gameObject.name] as Item_ItemStructure;
        foreach (Formula<Item_ItemStructure.Conversion> formula in Structure.Conversions) {
            FormulaUI_1_1 formulaUI = Instantiate(FormulaPrefab, ScrollContent).GetComponent<FormulaUI_1_1>();
            formulaUI.RawItem = new ItemData(formula.Conversion.FromItemID, formula.Conversion.FromItemNum);
            formulaUI.OutputItem = new ItemData(formula.Conversion.ToItemID, formula.Conversion.ToItemNum);
            formulaUI.Time = (int)(formula.Conversion.ProcessTime * 10 / (Structure.ProcessSpeed * Structure.ProcessSpeedRatio));
            formulaUI.OnPriorityChanged += (priority) => {
                int newIndex = formula.Priority + priority;
                if (newIndex < 0 || newIndex >= Structure.Conversions.Count) {
                    return;
                }
                Formula<Item_ItemStructure.Conversion> origin = Structure.Conversions[newIndex];
                Structure.Conversions[newIndex] = formula;
                Structure.Conversions[formula.Priority] = origin;
                ScrollContent.GetChild(newIndex).transform.SetSiblingIndex(formula.Priority);
                formulaUI.transform.SetSiblingIndex(newIndex);
                origin.Priority = formula.Priority;
                formula.Priority = newIndex;
            };
        }
    }
}
