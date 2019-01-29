/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 13:17:56
 * 版本：v0.7
 */
using System.Collections.Generic;
using UnityEngine;

public class CarriageUpgradeUI : InitableUI {

    #region 资源
    private GameObject P_upgradeItem {
        get {
            if (_p_upgradeItem == null) {
                _p_upgradeItem = ResourceLoader.GetResource<GameObject>("Prefabs/Train/UpgradeItem");
            }
            return _p_upgradeItem;
        }
    }
    #endregion

    #region 组件
    private Transform C_ContentTransform {
        get {
            if (_c_ContentTransform == null) {
                _c_ContentTransform = transform.Find("Viewport/Content");
            }
            return _c_ContentTransform;
        }
    }
    #endregion

    #region 公有属性
    #endregion

    #region 私有属性
    #endregion

    #region 严禁调用的隐藏变量
    private GameObject _p_upgradeItem;
    private Transform _c_ContentTransform;
    #endregion

    #region 生命周期
    #endregion

    #region 公有函数
    public override void Init(CarriageBackend carriage) {
        foreach (CarriageResearchSetting setting in carriage.ResearchSettings.Values) {
            GameObject upgradeItem = Instantiate(P_upgradeItem, C_ContentTransform);
            CarriageUpgradeItemUI upgradeItemUI = upgradeItem.GetComponent<CarriageUpgradeItemUI>();
            upgradeItemUI.SetItem(carriage, setting, carriage.UpgradedID.Contains(setting.ID), setting.Dependency == -1 ? null : carriage.ResearchSettings[setting.Dependency].Name, carriage.UpgradedID.Contains(setting.Dependency));
        }
    }
    #endregion

    #region 私有函数
    #endregion
}
