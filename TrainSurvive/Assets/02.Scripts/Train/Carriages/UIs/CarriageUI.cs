/*
 * 描述：车厢界面
 * 作者：刘旭涛
 * 创建时间：2019/1/28 15:38:58
 * 版本：v0.7
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarriageUI : MonoBehaviour {

    [Tooltip("初始显示的UI")]
    [SerializeField]
    private string[] InitialUIs;

    #region 资源
    private GameObject P_pageButton {
        get {
            if (_p_pageButton == null) {
                _p_pageButton = ResourceLoader.GetResource<GameObject>("Prefabs/Train/ProgressButton");
            }
            return _p_pageButton;
        }
    }
    #endregion

    #region 组件
    private ProgressButton C_research {
        get {
            if (_c_research == null) {
                _c_research = transform.Find("Research").GetComponent<ProgressButton>();
            }
            return _c_research;
        }
    }
    private Transform C_pageButtonGroup {
        get {
            if (_c_pageButtonGroup == null) {
                _c_pageButtonGroup = transform.Find("PageButtonGroup/Viewport/Content");
            }
            return _c_pageButtonGroup;
        }
    }
    private Transform C_pages {
        get {
            if (_c_pages == null) {
                _c_pages = transform.Find("Pages");
            }
            return _c_pages;
        }
    }
    private InitableUI C_researchPage {
        get {
            if (_c_researchPage == null) {
                _c_researchPage = transform.Find("ResearchPage").GetComponent<InitableUI>();
            }
            return _c_researchPage;
        }
    }
    #endregion

    #region 私有属性
    private Dictionary<string, InitableUI> Pages { get; } = new Dictionary<string, InitableUI>();
    private Dictionary<string, ProgressButton> PageButtons { get; } = new Dictionary<string, ProgressButton>();
    #endregion

    #region 公有属性
    public string CurrentPage {
        get {
            return _currentPage;
        }
        set {
            if (_currentPage != null) {
                Pages[_currentPage].gameObject.SetActive(false);
                PageButtons[_currentPage].Outline.enabled = false;
            }
            Pages[value].gameObject.SetActive(true);
            PageButtons[value].Outline.enabled = true;
            _currentPage = value;
        }
    }
    public CarriageBackend Carriage { get; set; }
    #endregion

    #region 严禁调用的隐藏变量
    private ProgressButton _c_research;
    private Transform _c_pageButtonGroup;
    private Transform _c_pages;
    private GameObject _p_pageButton;
    private string _currentPage;
    private InitableUI _c_researchPage;
    #endregion

    #region 生命周期
    private void Awake() {
        // 初始化各个页面的按钮
        for (int i = 0; i < C_pages.childCount; i++) {
            GameObject page = C_pages.GetChild(i).gameObject;
            string name = page.name;
            GameObject buttonInst = Instantiate(P_pageButton, C_pageButtonGroup);
            ProgressButton progressButton = buttonInst.GetComponent<ProgressButton>();
            buttonInst.name = name;
            buttonInst.GetComponentInChildren<Text>().text = page.name;
            progressButton.Action = OnClickPageButton;
            InitableUI initableUI = page.GetComponent<InitableUI>();
            initableUI.Init(Carriage);
            Pages.Add(name, initableUI);
            PageButtons.Add(name, progressButton);
            page.SetActive(false);
            progressButton.gameObject.SetActive(false);
        }
        // 将研究界面加入
        C_researchPage.Init(Carriage);
        Pages.Add("Research", C_researchPage);
        PageButtons.Add("Research", C_research);
        C_research.Action = OnClickPageButton;
        // 初始显示研究页
        CurrentPage = "Research";

        // 控制UI显示
        foreach(string ui in InitialUIs) {
            ToggleUI(ui, true);
        }
        foreach (int id in Carriage.UpgradedID) {
            Carriage_OnUpgraded(id);
        }
        Carriage.OnUpgraded += Carriage_OnUpgraded;
    }

    #endregion

    #region 私有函数
    private void OnClickPageButton(ProgressButton button) {
        CurrentPage = button.gameObject.name;
    }
    private void Carriage_OnUpgraded(int id) {
        CarriageResearchSetting setting = Carriage.ResearchSettings[id];
        if (setting.StructureName.Length > 0 && setting.UnlockUI) {
            ToggleUI(setting.StructureName, true);
        }
    }
    #endregion

    #region 公有函数
    public void ToggleUI(string name, bool enable) {
        PageButtons[name].gameObject.SetActive(enable);
    }
    #endregion
}
