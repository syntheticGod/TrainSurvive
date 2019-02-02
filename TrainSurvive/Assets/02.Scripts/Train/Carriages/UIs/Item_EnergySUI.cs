/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 20:23:02
 * 版本：v0.7
 */
using System.Collections.Generic;
using UnityEngine;

public class Item_EnergySUI : InitableUI {
    
    public GameObject FormulaPrefab {
        get {
            if (_FormulaPrefab == null) {
                _FormulaPrefab = ResourceLoader.GetResource<GameObject>("Prefabs/Train/Formula_1_Energy");
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

    private Item_EnergyStructure Structure { get; set; }


    private Transform _ScrollContent;
    private GameObject _FormulaPrefab;
    
    private void OnEnable() {
        UpdateUI();

        foreach (Formula<Item_EnergyStructure.Conversion> formula in Structure.Conversions) {
            formula.OnAcquireCount += Formula_OnAcquireCount;
            formula.OnCountChanged += Formula_OnCountChanged;
            formula.OnProgressChanged += Formula_OnProgressChanged;
        }
    }

    private void OnDisable() {
        foreach (Formula<Item_EnergyStructure.Conversion> formula in Structure.Conversions) {
            formula.OnAcquireCount -= Formula_OnAcquireCount;
            formula.OnCountChanged -= Formula_OnCountChanged;
            formula.OnProgressChanged -= Formula_OnProgressChanged;
            formula.Count = ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_Energy>().ProduceCount;
        }
    }

    private void Formula_OnProgressChanged(int priority, float max, float value) {
        ScrollContent.GetChild(priority).GetComponent<FormulaUI_1_Energy>().ChangeProgress(0, max, value);
    }

    private void Formula_OnCountChanged(int priority, int count) {
        ScrollContent.GetChild(priority).GetComponent<FormulaUI_1_Energy>().ProduceCount = count;
    }

    private int Formula_OnAcquireCount(int priority) {
        return ScrollContent.GetChild(priority).GetComponent<FormulaUI_1_Energy>().ProduceCount;
    }

    private void UpdateUI() {
        foreach (Formula<Item_EnergyStructure.Conversion> formula in Structure.Conversions) {
            FormulaUI_1_Energy formulaUI = ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_Energy>();
            if (formula.Conversion.UnlockUpgradeID == -1 || Structure.CarriageBackend.UpgradedID.Contains(formula.Conversion.UnlockUpgradeID)) {
                formulaUI.gameObject.SetActive(true);
                formulaUI.ProduceCount = formula.Count;
                formulaUI.ChangeProgress(0, formula.Conversion.ProcessTime, formula.Progress);
                formulaUI.OutputCount = (int)(formula.Conversion.ProduceEnergy * Structure.ConversionRate * Structure.ConversionRateRatio);
                formulaUI.Time = (int)(formula.Conversion.ProcessTime * 10 / (Structure.ProcessSpeed * Structure.ProcessSpeedRatio));
            } else {
                formulaUI.gameObject.SetActive(false);
            }
        }
    }

    public override void Init(CarriageBackend carriage) {
        Structure = carriage.Structures[gameObject.name] as Item_EnergyStructure;
        foreach (Formula<Item_EnergyStructure.Conversion> formula in Structure.Conversions) {
            FormulaUI_1_Energy formulaUI = Instantiate(FormulaPrefab, ScrollContent).GetComponent<FormulaUI_1_Energy>();
            formulaUI.RawItem = new ItemData(formula.Conversion.ItemID, 1);
            formulaUI.OutputType = Structure.GeneratedEnergyType;
            formulaUI.OutputCount = (int)(formula.Conversion.ProduceEnergy * Structure.ConversionRate * Structure.ConversionRateRatio);
            formulaUI.Time = (int)(formula.Conversion.ProcessTime * 10 / (Structure.ProcessSpeed * Structure.ProcessSpeedRatio));
            formulaUI.OnPriorityChanged += (priority) => {
                int newIndex = formula.Priority + priority;
                if (newIndex < 0 || newIndex >= Structure.Conversions.Count) {
                    return;
                }
                Formula<Item_EnergyStructure.Conversion> origin = Structure.Conversions[newIndex];
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
