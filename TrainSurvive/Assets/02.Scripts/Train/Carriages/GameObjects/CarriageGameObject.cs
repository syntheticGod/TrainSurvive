/*
 * 描述：列车车厢
 * 作者：刘旭涛
 * 创建时间：2019/1/28 15:00:32
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageGameObject : MonoBehaviour {
    
    [Tooltip("Backend类名")]
    public string BackendClass;

    #region 组件
    private CarriageUI C_CarriageUI {
        get {
            if (_c_CarriageUI == null) {
                _c_CarriageUI = GameObject.Find("Canvas/CarriageUI/" + name).GetComponent<CarriageUI>();
            }
            return _c_CarriageUI;
        }
    }
    #endregion

    #region 公有属性
    public CarriageBackend CarriageBackend { get; set; }
    #endregion

    #region 私有属性
    private Dictionary<string, TrainSpriteController> StructureSprites { get; } = new Dictionary<string, TrainSpriteController>();
    #endregion

    #region 严禁调用的隐藏变量
    private CarriageUI _c_CarriageUI;
    #endregion

    #region 生命周期
    private void OnEnable() {
        UpdateSprite();
        CarriageBackend.OnUpgraded += CarriageBackend_OnUpgraded;
    }
    private void OnDisable() {
        CarriageBackend.OnUpgraded -= CarriageBackend_OnUpgraded;
    }
    #endregion

    #region 私有函数
    private void OnMouseOver() {
        if (Input.GetMouseButtonUp(1)) {
            C_CarriageUI.Carriage = CarriageBackend;
            C_CarriageUI.gameObject.SetActive(true);
        }
    }

    private void CarriageBackend_OnUpgraded(int id) {
        CarriageResearchSetting setting = CarriageBackend.ResearchSettings[id];
        foreach (CarriageResearchSetting.UpgradeSprite upgradeSprite in setting.UpgradeSprites) {
            if (!StructureSprites.ContainsKey(upgradeSprite.Name)) {
                StructureSprites.Add(upgradeSprite.Name, transform.Find(upgradeSprite.Name).GetComponent<TrainSpriteController>());
            }
            StructureSprites[upgradeSprite.Name].Level = upgradeSprite.Level;
        }
    }
    private void UpdateSprite() {
        TrainSpriteController[] controllers = GetComponentsInChildren<TrainSpriteController>();
        foreach (TrainSpriteController controller in controllers) {
            controller.Level = controller.DefaultLevel;
        }
        foreach (int upgraded in CarriageBackend.UpgradedID) {
            CarriageBackend_OnUpgraded(upgraded);
        }
    }
    #endregion

    #region 公有函数
    #endregion
}
