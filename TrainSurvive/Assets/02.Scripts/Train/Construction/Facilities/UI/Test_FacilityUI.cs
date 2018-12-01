/*
 * 描述：测试用UI
 * 作者：刘旭涛
 * 创建时间：2018/11/7 14:36:06
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using UnityEngine;

public class Test_FacilityUI : FacilityUI {
    
    public UnitInventoryCtrl Raw;
    public UnitInventoryCtrl Gas;
    public UnitInventoryCtrl Food;

    private UIManager _uiManager;
    private UIManager UIManager {
        get {
            if (_uiManager == null) {
                _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
            }
            return _uiManager;
        }
    }

    private new TEST_Facility Structure {
        get {
            return base.Structure as TEST_Facility;
        }
    }

    private void Awake() {
        Raw.ChargeIn = (item) => {
            Debug.Log("Raw: " + item.currPileNum);
            return true;
        };
        Gas.ChargeIn = (item) => {
            Debug.Log("Gas: " + item.currPileNum);
            return true;
        };
        Food.ChargeIn = (item) => { return false; };
    }

    private void OnEnable() {
        OnFoodUpdate(Structure.Food);
        OnRawUpdate(Structure.Raw);
        OnGasUpdate(Structure.Gas);

        UIManager.ToggleInventoryPanel(true);

        Structure.OnAcquireFood = OnAcquireFood;
        Structure.OnFoodUpdate = OnFoodUpdate;
        Structure.OnAcquireGas = OnAcquireGas;
        Structure.OnGasUpdate = OnGasUpdate;
        Structure.OnAcquireRaw = OnAcquireRaw;
        Structure.OnRawUpdate = OnRawUpdate;
    }

    private void OnDisable() {
        Structure.OnAcquireFood = null;
        Structure.OnFoodUpdate = null;
        Structure.OnAcquireGas = null;
        Structure.OnGasUpdate = null;
        Structure.OnAcquireRaw = null;
        Structure.OnRawUpdate = null;
    }
    
    private Item OnAcquireFood() {
        return Food.GetItem();
    }
    private void OnFoodUpdate(Item item) {
        if (item == null || item.currPileNum <= 0) {
            Food.Clear();
        } else {
            Food.GeneratorItem(item.id, item.currPileNum);
        }
    }
    private Item OnAcquireRaw() {
        return Raw.GetItem();
    }
    private void OnRawUpdate(Item item) {
        if (item == null || item.currPileNum <= 0) {
            Raw.Clear();
        } else {
            Raw.GeneratorItem(item.id, item.currPileNum);
        }
    }
    private Item OnAcquireGas() {
        return Gas.GetItem();
    }
    private void OnGasUpdate(Item item) {
        if (item == null || item.currPileNum <= 0) {
            Gas.Clear();
        } else {
            Gas.GeneratorItem(item.id, item.currPileNum);
        }
    }
}
