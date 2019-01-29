/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 13:31:28
 * 版本：v0.7
 */
using System;
using System.Collections.Generic;
using System.Text;
using TTT.UI;
using UnityEngine;
using UnityEngine.UI;
using WorldMap.UI;

public class CarriageUpgradeItemUI : MonoBehaviour {

    private static readonly Color UPGRADED_COLOR = new Color(0.3f, 1.0f, 0.22f);

    #region 资源
    private GameObject P_CostItem {
        get {
            if (_p_CostItem == null) {
                _p_CostItem = ResourceLoader.GetResource<GameObject>("Prefabs/Train/AssetsItemView");
            }
            return _p_CostItem;
        }
    }
    #endregion

    #region 组件
    private Text C_Name {
        get {
            if (_c_Name == null) {
                _c_Name = transform.Find("Name").GetComponent<Text>();
            }
            return _c_Name;
        }
    }
    private Text C_Description {
        get {
            if (_c_Description == null) {
                _c_Description = transform.Find("Description").GetComponent<Text>();
            }
            return _c_Description;
        }
    }
    private Transform C_CostsContent {
        get {
            if (_c_CostsContent == null) {
                _c_CostsContent = transform.Find("Costs/Viewport/Content");
            }
            return _c_CostsContent;
        }
    }
    private Text C_UpgradeButtonText {
        get {
            if (_c_UpgradeButtonText == null) {
                _c_UpgradeButtonText = transform.Find("Upgrade/Text").GetComponent<Text>();
            }
            return _c_UpgradeButtonText;
        }
    }
    private Image C_UpgradeButtonImage {
        get {
            if (_c_UpgradeButtonImage == null) {
                _c_UpgradeButtonImage = transform.Find("Upgrade").GetComponent<Image>();
            }
            return _c_UpgradeButtonImage;
        }
    }
    private Button C_UpgradeButton {
        get {
            if (_c_UpgradeButton == null) {
                _c_UpgradeButton = transform.Find("Upgrade").GetComponent<Button>();
            }
            return _c_UpgradeButton;
        }
    }
    private Text C_UpgradeHint {
        get {
            if (_c_UpgradeHint == null) {
                _c_UpgradeHint = transform.Find("Hint").GetComponent<Text>();
            }
            return _c_UpgradeHint;
        }
    }
    #endregion

    #region 公有属性
    #endregion

    #region 私有属性
    private CarriageBackend Carriage { get; set; }
    private CarriageResearchSetting ResearchSetting { get; set; }
    private Action<int> OnUpgradeEvent { get; set; }
    #endregion

    #region 严禁调用的隐藏变量
    private Text _c_Name;
    private Text _c_Description;
    private Transform _c_CostsContent;
    private Text _c_UpgradeButtonText;
    private Image _c_UpgradeButtonImage;
    private Button _c_UpgradeButton;
    private GameObject _p_CostItem;
    private Text _c_UpgradeHint;
    #endregion

    #region 生命周期
    private void OnDestroy() {
        Carriage.OnUpgraded -= Carriage_OnUpgraded;
    }
    #endregion

    #region 公有函数
    public void SetItem(CarriageBackend carriage, CarriageResearchSetting setting, bool upgraded, string dependencyName, bool dependencyUpgraded) {
        Carriage = carriage;
        C_UpgradeButton.onClick.RemoveAllListeners();
        C_UpgradeButton.onClick.AddListener(Upgrade);
        ResearchSetting = setting;
        C_Name.text = setting.Name;
        C_Description.text = setting.Description;
        foreach (ItemData cost in setting.Costs) {
            GameObject costItem = Instantiate(P_CostItem, C_CostsContent);
            costItem.GetComponent<AssetsItemView>().SetItemData(cost.ID, cost.Number);
            RectTransform rect = costItem.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((RectTransform)C_CostsContent).rect.height);
        }
        if (upgraded) {
            C_UpgradeButtonImage.color = UPGRADED_COLOR;
            C_UpgradeButton.interactable = false;
            C_UpgradeButtonText.text = "已升级";
            C_UpgradeHint.text = "已升级";
        } else {
            C_UpgradeButtonImage.color = Color.white;
            if (dependencyUpgraded) {
                C_UpgradeButton.interactable = true;
                C_UpgradeButtonText.text = "升级";
                C_UpgradeHint.text = "消耗材料升级车厢";
            } else {
                C_UpgradeButton.interactable = false;
                C_UpgradeButtonText.text = "锁定";
                C_UpgradeHint.text = "请先升级“" + dependencyName + "”";
            }
        }
        Carriage.OnUpgraded += Carriage_OnUpgraded;
    }
    #endregion

    #region 私有函数
    private void Upgrade() {
        List<ItemData> insufficient = Carriage.IsResearchCostsAvailable(ResearchSetting.ID);
        if (insufficient.Count == 0) {
            Carriage.Research(ResearchSetting.ID);
        } else {
            StringBuilder stringBuilder = new StringBuilder("以下材料不足：\n");
            foreach (ItemData item in insufficient) {
                stringBuilder.AppendLine(item.Name + " 缺少 " + item.Number + "个");
            }
            InfoDialog.Show(stringBuilder.ToString());
        }
    }
    private void Carriage_OnUpgraded(int id) {
        if (id == ResearchSetting.ID) {
            C_UpgradeButtonImage.color = UPGRADED_COLOR;
            C_UpgradeButton.interactable = false;
            C_UpgradeButtonText.text = "已升级";
            C_UpgradeHint.text = "已升级";
        } else if (id == ResearchSetting.Dependency) {
            C_UpgradeButton.interactable = true;
            C_UpgradeButtonText.text = "升级";
            C_UpgradeHint.text = "消耗材料升级车厢";
        }
    }
    #endregion
}
