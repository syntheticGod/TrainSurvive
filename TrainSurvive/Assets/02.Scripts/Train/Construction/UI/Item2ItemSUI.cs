/*
 * 描述：物品转物品建筑UI
 * 作者：刘旭涛
 * 创建时间：2018/12/5 16:08:56
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item2ItemSUI : FacilityUI {

    [Tooltip("配方Prefab")]
    public FormulaUI_1_1 FormulaPrefab;
    [Tooltip("列表Content")]
    public RectTransform ScrollContent;

    private new Item2ItemStructure Structure {
        get {
            return base.Structure as Item2ItemStructure;
        }
    }

    private void Awake() {
        foreach (Item2ItemStructure.Formula formula in Structure.Conversions) {
            FormulaUI_1_1 formulaUI = Instantiate(FormulaPrefab, ScrollContent).GetComponent<FormulaUI_1_1>();
            formulaUI.RawItem = new ItemData(formula.Conversion.FromItemID, formula.Conversion.FromItemNum);
            formulaUI.OutputItem = new ItemData(formula.Conversion.ToItemID, formula.Conversion.ToItemNum);
            formulaUI.OnPriorityChanged += (priority) => {
                int newIndex = formula.Priority + priority;
                if (newIndex < 0 || newIndex >= Structure.Conversions.Count) {
                    return;
                }
                Item2ItemStructure.Formula origin = Structure.Conversions[newIndex];
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

        foreach(Item2ItemStructure.Formula formula in Structure.Conversions) {
            formula.OnAcquireCount += Formula_OnAcquireCount;
            formula.OnCountChanged += Formula_OnCountChanged;
            formula.OnProgressChanged += Formula_OnProgressChanged;
        }
    }

    private void OnDisable() {
        foreach (Item2ItemStructure.Formula formula in Structure.Conversions) {
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
        foreach (Item2ItemStructure.Formula formula in Structure.Conversions) {
            ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_1>().ProduceCount = formula.Count;
            ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_1_1>().ChangeProgress(0, formula.Conversion.ProcessTime, formula.Progress);
        }
    }
}
