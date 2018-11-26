/*
 * 描述：科技树
 * 作者：刘旭涛
 * 创建时间：2018/11/25 21:57:47
 * 版本：v0.1
 */
using System;
using UnityEngine;
using UnityEngine.UI;

public class TechTree : MonoBehaviour {

    [Serializable]
    public struct Line {
        public Image[] Lines;
    }

    [Tooltip("单个科技点Prefab")]
    public GameObject TechPrefab;
    [Tooltip("Vertical Group用于放置科技点Prefab")]
    public GameObject VerticalGroup;
    [Tooltip("用于连接科技点Prefab的线")]
    public GameObject LinePrefab;
    [Tooltip("高亮颜色")]
    public Color HighlightColor;
    [Tooltip("可用颜色")]
    public Color EnabledColor;
    [Tooltip("禁用颜色")]
    public Color DisabledColor;
    [Tooltip("已修颜色")]
    public Color CompletedColor;
    [Tooltip("记录科技点实例（不需要手工设置）")]
    public GameObject[] TechObjects;
    [Tooltip("记录科技点连线实例（不需要手工设置）")]
    public Line[] TechLines;
    
    /// <summary>
    /// 登记科技列表
    /// </summary>
    public Tech[] Techs { get; } = {
        new TEST_Tech(),
        new TEST_Tech(),
        new TEST_Tech(),
        new TEST_Tech(),
        new TEST_Tech(),
        new TEST_Tech(),
        new TEST_Tech()
    };

    /// <summary>
    /// 登记科技依赖关系，以下标标记
    /// </summary>
    public int[][] TechDependencies { get; } = {
        new int[]{ },
        new int[]{ 0 },
        new int[]{ 1 },
        new int[]{ 0 },
        new int[]{ 2, 3 },
        new int[]{ 0 },
        new int[]{ 1, 5 }
    };

    private int _currentSelect = -1;
    public int CurrentSelect {
        get {
            return _currentSelect;
        }
        set {
            if (_currentSelect >= 0 && TechObjects.Length > _currentSelect) {
                TechObjects[_currentSelect].GetComponent<Image>().color = EnabledColor;
                for (int i = 0; i < TechLines[_currentSelect].Lines.Length; i++) {
                    TechLines[_currentSelect].Lines[i].color = EnabledColor;
                    TechObjects[TechDependencies[_currentSelect][i]].GetComponent<Image>().color = EnabledColor;
                }
            }
            if (value >= 0 && TechObjects.Length > value) {
                TechObjects[value].GetComponent<Image>().color = HighlightColor;
                for (int i = 0; i < TechLines[value].Lines.Length; i++) {
                    TechLines[value].Lines[i].color = HighlightColor;
                    TechObjects[TechDependencies[value][i]].GetComponent<Image>().color = HighlightColor;
                }
            }
            _currentSelect = value;
        }
    }

    private void Awake() {
        for (int i = 0; i < TechObjects.Length; i++) {
            int index = i;
            TechObjects[index].GetComponent<Button>().onClick.AddListener(() => {
                CurrentSelect = index;
            });
        }
    }
}
