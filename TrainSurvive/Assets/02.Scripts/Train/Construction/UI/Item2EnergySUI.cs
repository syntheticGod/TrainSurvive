/*
 * 描述：物品转能源建筑UI
 * 作者：刘旭涛
 * 创建时间：2018/12/4 14:40:29
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using TTT.Utility;
using UnityEngine;
using UnityEngine.UI;
using WorldMap.UI;

public class Item2EnergySUI : FacilityUI {
    [Tooltip("配方Prefab")]
    public FormulaUI_1_Energy FormulaPrefab;
    [Tooltip("列表Content")]
    public RectTransform ScrollContent;

    private new Item2EnergyStructure Structure {
        get {
            return base.Structure as Item2EnergyStructure;
        }
    }

    private void Awake() {
        foreach (Formula<Item2EnergyStructure.Conversion> formula in Structure.Conversions) {
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
                Formula<Item2EnergyStructure.Conversion> origin = Structure.Conversions[newIndex];
                Structure.Conversions[newIndex] = formula;
                Structure.Conversions[formula.Priority] = origin;
                ScrollContent.GetChild(newIndex).transform.SetSiblingIndex(formula.Priority);
                formulaUI.transform.SetSiblingIndex(newIndex);
                origin.Priority = formula.Priority;
                formula.Priority = newIndex;
            };
        }
    }

    private void OnEnable() {
        UpdateUI();

        foreach (Formula<Item2EnergyStructure.Conversion> formula in Structure.Conversions) {
            formula.OnAcquireCount += Formula_OnAcquireCount;
            formula.OnCountChanged += Formula_OnCountChanged;
            formula.OnProgressChanged += Formula_OnProgressChanged;
        }
    }

    private void OnDisable() {
        foreach (Formula<Item2EnergyStructure.Conversion> formula in Structure.Conversions) {
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
        foreach (Formula<Item2EnergyStructure.Conversion> formula in Structure.Conversions) {
            ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_Energy>().ProduceCount = formula.Count;
            ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_Energy>().ChangeProgress(0, formula.Conversion.ProcessTime, formula.Progress);
        }
    }
}
