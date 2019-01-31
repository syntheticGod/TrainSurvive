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
        CarriageBackend.OnStructureStateChanged += CarriageBackend_OnStructureStateChanged;
    }

    private void OnDisable() {
        CarriageBackend.OnUpgraded -= CarriageBackend_OnUpgraded;
        CarriageBackend.OnStructureStateChanged -= CarriageBackend_OnStructureStateChanged;
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
        if (setting.StructureName.Length > 0 && setting.SpriteLevel >= 0) {
            if (!StructureSprites.ContainsKey(setting.StructureName)) {
                Transform obj = transform.Find(setting.StructureName);
                if (obj == null)
                    return;
                StructureSprites.Add(setting.StructureName, obj.GetComponent<TrainSpriteController>());
            }
            StructureSprites[setting.StructureName].Level = setting.SpriteLevel;
        }
    }
    private void CarriageBackend_OnStructureStateChanged(string name, string state, object value) {
        if (!StructureSprites.ContainsKey(name)) {
            Transform obj = transform.Find(name);
            if (obj == null)
                return;
            StructureSprites.Add(name, obj.GetComponent<TrainSpriteController>());
        }
        StructureSprites[name].SetBool(state, (bool) value);
    }

    private void UpdateSprite() {
        TrainSpriteController[] controllers = GetComponentsInChildren<TrainSpriteController>();
        foreach (TrainSpriteController controller in controllers) {
            controller.Level = controller.DefaultLevel;
        }
        foreach (int upgraded in CarriageBackend.UpgradedID) {
            CarriageBackend_OnUpgraded(upgraded);
        }

        foreach (CarriageStructure structure in CarriageBackend.Structures.Values) {
            foreach (KeyValuePair<string, object> item in structure.States) {
                CarriageBackend_OnStructureStateChanged(structure.Name, item.Key, item.Value);
            }
        }
    }
    #endregion
    
}
