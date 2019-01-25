/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/25 14:38:17
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed2ItemSUI : FacilityUI {

    [Serializable]
    public struct SeedSprite {
        public string Name;
        public Sprite Sprite;
    }

    [Tooltip("配方Prefab")]
    public FormulaUI_None_1 FormulaPrefab;
    [Tooltip("列表Content")]
    public RectTransform ScrollContent;
    [Tooltip("种子名和图片")]
    public SeedSprite[] SeedSprites;

    private new Seed2ItemStructure Structure {
        get {
            return base.Structure as Seed2ItemStructure;
        }
    }

    private void Awake() {
        foreach (Formula<Seed2ItemStructure.Conversion> formula in Structure.Conversions) {
            FormulaUI_None_1 formulaUI = Instantiate(FormulaPrefab, ScrollContent).GetComponent<FormulaUI_None_1>();
            foreach(SeedSprite seedSprite in SeedSprites) {
                if (seedSprite.Name == formula.Conversion.FromSeedName) {
                    formulaUI.Input = seedSprite.Sprite;
                    break;
                }
            }
            formulaUI.Name = formula.Conversion.FromSeedName;
            formulaUI.OutputItem = new ItemData(formula.Conversion.ToItemID, formula.Conversion.ToItemNum);
            formulaUI.Time = (int)(formula.Conversion.ProcessTime * 10 / (Structure.ProcessSpeed * Structure.ProcessSpeedRatio));
            formulaUI.OnPriorityChanged += (priority) => {
                int newIndex = formula.Priority + priority;
                if (newIndex < 0 || newIndex >= Structure.Conversions.Count) {
                    return;
                }
                Formula<Seed2ItemStructure.Conversion> origin = Structure.Conversions[newIndex];
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

        foreach (Formula<Seed2ItemStructure.Conversion> formula in Structure.Conversions) {
            formula.OnAcquireCount += Formula_OnAcquireCount;
            formula.OnCountChanged += Formula_OnCountChanged;
            formula.OnProgressChanged += Formula_OnProgressChanged;
        }
    }

    private void OnDisable() {
        foreach (Formula<Seed2ItemStructure.Conversion> formula in Structure.Conversions) {
            formula.OnAcquireCount -= Formula_OnAcquireCount;
            formula.OnCountChanged -= Formula_OnCountChanged;
            formula.OnProgressChanged -= Formula_OnProgressChanged;
            formula.Count = ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_None_1>().ProduceCount;
        }
    }

    private void Formula_OnProgressChanged(int priority, float max, float value) {
        ScrollContent.GetChild(priority).GetComponent<FormulaUI_None_1>().ChangeProgress(0, max, value);
    }

    private void Formula_OnCountChanged(int priority, int count) {
        ScrollContent.GetChild(priority).GetComponent<FormulaUI_None_1>().ProduceCount = count;
    }

    private int Formula_OnAcquireCount(int priority) {
        return ScrollContent.GetChild(priority).GetComponent<FormulaUI_None_1>().ProduceCount;
    }

    private void UpdateUI() {
        foreach (Formula<Seed2ItemStructure.Conversion> formula in Structure.Conversions) {
            ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_None_1>().ProduceCount = formula.Count;
            ScrollContent.GetChild(formula.Priority).GetComponent<FormulaUI_None_1>().ChangeProgress(0, formula.Conversion.ProcessTime, formula.Progress);
        }
    }
}
