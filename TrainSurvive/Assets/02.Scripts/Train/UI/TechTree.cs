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
    
    [Serializable]
    public class Line {
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
    /// 当前选中项
    /// </summary>
    public int CurrentSelect {
        get {
            return _currentSelect;
        }
        set {
            if (_currentSelect >= 0 && TechObjects[_currentSelect] != null) {
                TechObjects[_currentSelect].Outline.enabled = false;
            }
            if (value >= 0 && TechObjects[value] != null) {
                TechObjects[value].Outline.enabled = true;
                UpdateInfoPanel(value);
            }
            _currentSelect = value;
        }
    }
    private int _currentSelect = -1;
    
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
        // 初始化各种ProgressButton
        for (int i = 0; i < TechObjects.Length; i++) {
            if (TechObjects[i] == null)
                continue;
            int index = i;
            TechObjects[index].Action = () => {
                CurrentSelect = index;
            };
            TechObjects[index].MaxValue = TechTreeManager.TechSettings[index].TotalWorks;
        }
        ResearchButton.Action = ClickResearch;
    }
    
    private void OnEnable() {
        for (int i = 0; i < TechObjects.Length; i++) {
            if (TechObjects[i] == null)
                continue;
            TechObjects[i].Value = TechTreeManager.Instance.Techs[i].CurrentWorks;
            UpdateColorState(i);
        }
        CurrentSelect = TechTreeManager.Instance.CurrentWorking == -1 ? 0 : TechTreeManager.Instance.CurrentWorking;
        StartCoroutine(ResearchStateChange()); // 打开面板时监听选中科技的状态，及时更新Info Panel里的ProgressButton。
        StartCoroutine(TreeStateChange()); // 打开面板时监听正在工作的科技的状态，即使更新树的ProgressButton。
        if (World.getInstance().techUnlock <= 0) {
            StartCoroutine(CheckTechUnlock());
        }
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
    
    /// <summary>
    /// 更新信息面板数据
    /// </summary>
    /// <param name="tech">ID</param>
    public void UpdateInfoPanel(int tech) {
        if (tech < 0 || TechObjects[tech] == null) {
            return;
        }
        Title.text = TechTreeManager.TechSettings[tech].Name;
        Description.text = TechTreeManager.TechSettings[tech].Description;
    }

    /// <summary>
    /// 更新科技树信息
    /// </summary>
    /// <param name="tech">ID</param>
    public void UpdateColorState(int tech) {
        if (tech < 0 || TechObjects[tech] == null) {
            return;
        }
        Color lineColor = LockedColor;
        switch (TechTreeManager.Instance.Techs[tech].TechState) {
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
        
        for (int i = 0; i < TechTreeManager.Instance.Techs.Length; i++) { // 依赖的连接线
            if (TechTreeManager.TechSettings[i] == null)
                continue;
            for (int j = 0; j < TechTreeManager.TechSettings[i].Dependencies.Length; j++) {
                if (TechTreeManager.TechSettings[i].Dependencies[j] == tech) {
                    TechLines[i].Lines[j].color = lineColor;
                }
                if(TechTreeManager.Instance.Techs[i].TechState == Tech.State.UNLOCKED) {
                    TechObjects[i].ButtonColor = UnlockedColor;
                    for (int k = 0; k < TechTreeManager.Instance.Techs.Length; k++) { // 间接依赖的连接线
                        if (TechTreeManager.TechSettings[k] == null)
                            continue;
                        for (int l = 0; l < TechTreeManager.TechSettings[k].Dependencies.Length; l++) {
                            if (TechTreeManager.TechSettings[k].Dependencies[l] == i) {
                                TechLines[k].Lines[l].color = UnlockedColor;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ClickResearch() {
        if (TechTreeManager.Instance.Techs[CurrentSelect].TechState == Tech.State.WORKING) {
            TechTreeManager.Instance.CurrentWorking = -1;
        } else if (TechTreeManager.Instance.Techs[CurrentSelect].TechState == Tech.State.UNLOCKED) {
            TechTreeManager.Instance.CurrentWorking = CurrentSelect;
        }
    }
    
    private IEnumerator ResearchStateChange() {
        while (true) {
            int prevSelect = CurrentSelect;
            Tech.State state = TechTreeManager.Instance.Techs[prevSelect].TechState;
            ResearchButton.MaxValue = TechTreeManager.TechSettings[prevSelect].TotalWorks;
            switch (state) {
                case Tech.State.COMPLETED:
                    ResearchButton.Value = ResearchButton.MaxValue;
                    ResearchButton.Text.text = "已研究";
                    ResearchButton.IsEnabled = false;
                    yield return new WaitWhile(() => prevSelect == CurrentSelect && state == TechTreeManager.Instance.Techs[CurrentSelect].TechState);
                    break;
                case Tech.State.LOCKED:
                    ResearchButton.Value = 0;
                    ResearchButton.Text.text = "未解锁";
                    ResearchButton.IsEnabled = false;
                    yield return new WaitWhile(() => prevSelect == CurrentSelect && state == TechTreeManager.Instance.Techs[CurrentSelect].TechState);
                    break;
                case Tech.State.UNLOCKED:
                    ResearchButton.Value = TechTreeManager.Instance.Techs[prevSelect].CurrentWorks;
                    ResearchButton.Text.text = "研究";
                    ResearchButton.IsEnabled = true;
                    yield return new WaitWhile(() => prevSelect == CurrentSelect && state == TechTreeManager.Instance.Techs[CurrentSelect].TechState);
                    break;
                case Tech.State.WORKING:
                    ResearchButton.Value = TechTreeManager.Instance.Techs[prevSelect].CurrentWorks;
                    ResearchButton.Text.text = "研究中";
                    if (!ResearchButton.IsEnabled) ResearchButton.IsEnabled = true;
                    yield return 1;
                    break;
            }
        }
    }

    private IEnumerator TreeStateChange() {
        while (true) {
            while (TechTreeManager.Instance.CurrentWorking >= 0 && TechTreeManager.Instance.Techs[TechTreeManager.Instance.CurrentWorking].TechState == Tech.State.WORKING) {
                TechObjects[TechTreeManager.Instance.CurrentWorking].Value = TechTreeManager.Instance.Techs[TechTreeManager.Instance.CurrentWorking].CurrentWorks;
                yield return 1;
            }
            UpdateColorState(TechTreeManager.Instance.CurrentWorking);
            TechTreeManager.Instance.CurrentWorking = -1;
            yield return new WaitWhile(() => TechTreeManager.Instance.CurrentWorking < 0);
        }
    }

    private IEnumerator CheckTechUnlock() {
        yield return new WaitUntil(() => World.getInstance().techUnlock > 0);
        for (int i = 0; i < TechObjects.Length; i++) {
            if (TechObjects[i] == null)
                continue;
            TechObjects[i].Value = TechTreeManager.Instance.Techs[i].CurrentWorks;
            UpdateColorState(i);
        }
    }
}
