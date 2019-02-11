/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/11 13:47:56
 * 版本：v0.7
 */
using Assets._02.Scripts.zhxUIScripts;
using System;
using TTT.Controller;
using TTT.Item;
using TTT.Resource;
using TTT.UI;
using UnityEngine;
using UnityEngine.UI;

public class ComposeSUI : InitableUI {
    private UnitInventoryCtrl[] _c_Materials;
    private Slider _c_ProgressBar;
    private ResourceItemBase _c_Weapon;
    private PackController _c_PackController;
    private Button _c_Button;

    private UnitInventoryCtrl[] C_Materials {
        get {
            if (_c_Materials == null) {
                _c_Materials = new UnitInventoryCtrl[2];
                for (int i = 0; i < 2; i++) {
                    _c_Materials[i] = transform.Find("Materials/Material" + (i + 1)).GetComponent<UnitInventoryCtrl>();
                }
            }
            return _c_Materials;
        }
    }
    private Slider C_ProgressBar {
        get {
            if (_c_ProgressBar == null) {
                _c_ProgressBar = transform.Find("Slider").GetComponent<Slider>();
            }
            return _c_ProgressBar;
        }
    }
    private ResourceItemBase C_Weapon {
        get {
            if (_c_Weapon == null) {
                _c_Weapon = transform.Find("Weapon").GetComponent<ResourceItemBase>();
            }
            return _c_Weapon;
        }
    }
    private PackController C_PackController {
        get {
            if (_c_PackController == null) {
                _c_PackController = GameObject.Find("Canvas").transform.Find("InventoryPanel").GetComponent<PackController>();
            }
            return _c_PackController;
        }
    }
    private Button C_Button {
        get {
            if (_c_Button == null) {
                _c_Button = transform.Find("Button").GetComponent<Button>();
            }
            return _c_Button;
        }
    }

    private ComposeStructure Structure { get; set; }

    private void OnEnable() {
        C_PackController.gameObject.SetActive(true);

        Structure.OnFinished += Structure_OnFinished;
        Structure.OnProgressUpdate += Structure_OnProgressUpdate;

        for (int i = 0; i < Structure.Materials.Length; i++) {
            if (Structure.Materials[i] != null) {
                C_Materials[i].SetItemData(Structure.Materials[i]);
            } else {
                C_Materials[i].Clear();
            }
        }
        if (Structure.SelectedFormula == -1) {
            C_Weapon.Clear();
        } else {
            C_Weapon.SetItemID(Structure.Formulas[Structure.SelectedFormula].ToWeapon);
        }

        if (Structure.Progress > 0) {
            LockUI(false);
        } else {
            LockUI(true);
        }
    }

    private void OnDisable() {
        Structure.OnFinished -= Structure_OnFinished;
        Structure.OnProgressUpdate -= Structure_OnProgressUpdate;
    }

    public override void Init(CarriageBackend carriage) {
        Structure = carriage.Structures[gameObject.name] as ComposeStructure;
        
        for (int i = 0; i < C_Materials.Length; i++) {
            C_Materials[i].OnChargeIn = OnChargeIn;
        }
    }

    private bool OnChargeIn(DragableAndDropableAssetsItemView sender, int id, int num) {
        ItemInfo item = StaticResource.GetItemInfoByID<ItemInfo>(id);
        if (item.Type == PublicData.ItemType.Component) {
            int index = Array.IndexOf(C_Materials, sender);
            int formulaIndex = ContainsFormula(item.ID, index == 0);
            if (formulaIndex >= 0) {
                Structure.Materials[index] = new ComponentData(id, 1, 0); //TODO 需要取到拖进来物体的品质等信息，物品系统没有提供。
                if (Structure.Materials[1 - index] != null) {
                    Structure.SelectedFormula = formulaIndex;
                    C_Weapon.SetItemID(Structure.Formulas[formulaIndex].ToWeapon);
                }
                return false;
            } else {
                return true;
            }
        } else {
            return true;
        }
    }

    public void Forge() {
        if (Structure.Forge()) {
            LockUI(false);
        } else {
            LockUI(true);
        }
    }

    private void LockUI(bool isLocked) {
        C_Button.interactable = isLocked;
        for (int i = 0; i < C_Materials.Length; i++) {
            C_Materials[i].enabled = isLocked;
        }
    }

    private void Structure_OnProgressUpdate(float value, float max) {
        C_ProgressBar.value = value;
        C_ProgressBar.maxValue = max;
    }

    private void Structure_OnFinished() {
        LockUI(true);
        for (int i = 0; i < C_Materials.Length; i++) {
            C_Materials[i].Clear();
        }
        C_Weapon.Clear();
        C_PackController.UpdatePack();
    }

    private int ContainsFormula(int componentID, bool component1) {
        if (component1) {
            for (int i = 0; i < Structure.Formulas.Length; i++) {
                if (Array.IndexOf(Structure.Formulas[i].FromComponent1, componentID) >= 0 && 
                    (Structure.Materials[1] == null || Array.IndexOf(Structure.Formulas[i].FromComponent2, Structure.Materials[1].ID) >= 0)) {
                    return i;
                }
            }
        } else {
            for (int i = 0; i < Structure.Formulas.Length; i++) {
                if (Array.IndexOf(Structure.Formulas[i].FromComponent2, componentID) >= 0 &&
                    (Structure.Materials[0] == null || Array.IndexOf(Structure.Formulas[i].FromComponent1, Structure.Materials[0].ID) >= 0)) {
                    return i;
                }
            }
        }
        return -1;
    }
}
