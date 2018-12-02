/*
 * 描述：科技树
 * 作者：刘旭涛
 * 创建时间：2018/11/25 21:57:47
 * 版本：v0.1
 */
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TechTree : MonoBehaviour {

    public static TechTree Instance { get; private set; }

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
    [Tooltip("解锁颜色")]
    public Color UnlockedColor;
    [Tooltip("锁定颜色")]
    public Color LockedColor;
    [Tooltip("已修颜色")]
    public Color CompletedColor;

    /// <summary>
    /// 记录科技点实例
    /// </summary>
    [HideInInspector]
    public ProgressButton[] TechObjects;

    /// <summary>
    /// 记录科技点连线实例
    /// </summary>
    [HideInInspector]
    public Line[] TechLines;

    /// <summary>
    /// 登记科技列表
    /// </summary>
    public static Tech[] Techs { get; private set; } = {
        new TEST_Tech0(),
        new TEST_Tech1(),
        new TEST_Tech2(),
        new TEST_Tech3(),
        new TEST_Tech4(),
        new TEST_Tech5(),
        new TEST_Tech6(),
        new TEST_Tech7()
    };
    
    /// <summary>
    /// 当前选中项
    /// </summary>
    public int CurrentSelect {
        get {
            return _currentSelect;
        }
        set {
            if (_currentSelect >= 0 && TechObjects.Length > _currentSelect) {
                TechObjects[_currentSelect].Outline.enabled = false;
            }
            if (value >= 0 && TechObjects.Length > value) {
                TechObjects[value].Outline.enabled = true;
                UpdateInfoPanel(value);
            }
            _currentSelect = value;
        }
    }
    private int _currentSelect = -1;

    /// <summary>
    /// 当前研究项
    /// </summary>
    public int CurrentWorking {
        get {
            return _currentWorking;
        }
        set {
            if (_currentWorking >= 0 && Techs.Length > _currentWorking) {
                Techs[_currentWorking].StopWorking();
            }
            if (value >= 0 && Techs.Length > value) {
                Techs[value].StartWorking();
            }
            _currentWorking = value;
        }
    }
    private int _currentWorking = -1;

    private Text _title;
    private Text Title {
        get {
            if (_title == null) {
                _title = transform.Find("InfoPanel/OperatePanel/Text").GetComponent<Text>();
            }
            return _title;
        }
    }

    private Text _description;
    private Text Description {
        get {
            if (_description == null) {
                _description = transform.Find("InfoPanel/Info/Viewport/Content").GetComponent<Text>();
            }
            return _description;
        }
    }

    private ProgressButton _researchButton;
    private ProgressButton ResearchButton {
        get {
            if (_researchButton == null) {
                _researchButton = transform.Find("InfoPanel/OperatePanel/ProgressButton").GetComponent<ProgressButton>();
            }
            return _researchButton;
        }
    }

    private void Awake() {
        Instance = this;
        for (int i = 0; i < TechObjects.Length; i++) {
            int index = i;
            TechObjects[index].Action = () => {
                CurrentSelect = index;
            };
            TechObjects[index].MaxValue = Techs[index].TotalWorks * Techs[index].WorkRatio;
        }
        ResearchButton.Action = ClickResearch;
    }
    
    private void OnEnable() {
        // 载入存档
        Techs = World.getInstance().techArray;

        for (int i = 0; i < Techs.Length; i++) {
            if (Techs[i].TechState == Tech.State.WORKING) {
                CurrentWorking = i;
            }
            TechObjects[i].Value = Techs[i].CurrentWorks;
            UpdateColorState(i);
        }
        CurrentSelect = 0;
        StartCoroutine(ResearchStateChange());
        StartCoroutine(TreeStateChange());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private void OnDestroy() {
        Instance = null;
    }

    /// <summary>
    /// 更新信息面板数据
    /// </summary>
    /// <param name="tech">ID</param>
    public void UpdateInfoPanel(int tech) {
        if (tech < 0 || TechObjects.Length <= tech) {
            return;
        }
        Title.text = Techs[tech].Name;
        Description.text = Techs[tech].Description;
    }

    /// <summary>
    /// 更新科技树信息
    /// </summary>
    /// <param name="tech">ID</param>
    public void UpdateColorState(int tech) {
        if (tech < 0 || TechObjects.Length <= tech) {
            return;
        }
        Color lineColor = LockedColor;
        switch (Techs[tech].TechState) {
            case Tech.State.COMPLETED:
                TechObjects[tech].Value = TechObjects[tech].MaxValue;
                TechObjects[tech].ButtonColor = UnlockedColor;
                lineColor = TechObjects[tech].MaskColor;
                break;
            case Tech.State.LOCKED:
                TechObjects[tech].Value = 0;
                TechObjects[tech].ButtonColor = LockedColor;
                lineColor = LockedColor;
                break;
            case Tech.State.UNLOCKED:
            case Tech.State.WORKING:
                TechObjects[tech].ButtonColor = UnlockedColor;
                lineColor = UnlockedColor;
                break;
        }
        
        for (int i = 0; i < Techs.Length; i++) { // 依赖的连接线
            for (int j = 0; j < Techs[i].Dependencies.Length; j++) {
                if (Techs[i].Dependencies[j] == tech) {
                    TechLines[i].Lines[j].color = lineColor;
                }
                if(Techs[i].TechState == Tech.State.UNLOCKED) {
                    TechObjects[i].ButtonColor = UnlockedColor;
                    for (int k = 0; k < Techs.Length; k++) { // 间接依赖的连接线
                        for (int l = 0; l < Techs[k].Dependencies.Length; l++) {
                            if (Techs[k].Dependencies[l] == i) {
                                TechLines[k].Lines[l].color = UnlockedColor;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ClickResearch() {
        if (Techs[CurrentSelect].TechState == Tech.State.WORKING) {
            CurrentWorking = -1;
        } else if (Techs[CurrentSelect].TechState == Tech.State.UNLOCKED) {
            CurrentWorking = CurrentSelect;
        }
    }
    
    private IEnumerator ResearchStateChange() {
        while (true) {
            int prevSelect = CurrentSelect;
            Tech.State state = Techs[prevSelect].TechState;
            ResearchButton.MaxValue = Techs[prevSelect].TotalWorks * Techs[prevSelect].WorkRatio;
            switch (state) {
                case Tech.State.COMPLETED:
                    ResearchButton.Value = ResearchButton.MaxValue;
                    ResearchButton.Text.text = "已研究";
                    ResearchButton.IsEnabled = false;
                    yield return new WaitWhile(() => prevSelect == CurrentSelect && state == Techs[CurrentSelect].TechState);
                    break;
                case Tech.State.LOCKED:
                    ResearchButton.Value = 0;
                    ResearchButton.Text.text = "未解锁";
                    ResearchButton.IsEnabled = false;
                    yield return new WaitWhile(() => prevSelect == CurrentSelect && state == Techs[CurrentSelect].TechState);
                    break;
                case Tech.State.UNLOCKED:
                    ResearchButton.Value = Techs[prevSelect].CurrentWorks;
                    ResearchButton.Text.text = "研究";
                    ResearchButton.IsEnabled = true;
                    yield return new WaitWhile(() => prevSelect == CurrentSelect && state == Techs[CurrentSelect].TechState);
                    break;
                case Tech.State.WORKING:
                    ResearchButton.Value = Techs[prevSelect].CurrentWorks;
                    ResearchButton.Text.text = "研究中";
                    if (!ResearchButton.IsEnabled) ResearchButton.IsEnabled = true;
                    yield return 1;
                    break;
            }
        }
    }

    private IEnumerator TreeStateChange() {
        while (true) {
            while (CurrentWorking >= 0 && Techs.Length > CurrentWorking && Techs[CurrentWorking].TechState == Tech.State.WORKING) {
                TechObjects[CurrentWorking].Value = Techs[CurrentWorking].CurrentWorks;
                yield return 1;
            }
            UpdateColorState(CurrentWorking);
            CurrentWorking = -1;
            yield return new WaitWhile(() => CurrentWorking < 0 || Techs.Length <= CurrentWorking);
        }
    }
}
