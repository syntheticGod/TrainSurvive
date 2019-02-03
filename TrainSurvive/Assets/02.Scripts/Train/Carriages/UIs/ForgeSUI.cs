/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/2/3 13:47:03
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

public class ForgeSUI : InitableUI {
    private UnitInventoryCtrl[] _c_Materials;
    private Slider _c_ProgressBar;
    private ResourceItemBase _c_Component;
    private SelectionsUI _c_SelectionsUI;
    private PackController _c_PackController;
    private Button _c_Button;
    private Button _c_ComponentButton;

    private UnitInventoryCtrl[] C_Materials {
        get {
            if (_c_Materials == null) {
                _c_Materials = new UnitInventoryCtrl[6];
                for (int i = 0; i < 6; i++) {
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
    private ResourceItemBase C_Component {
        get {
            if (_c_Component == null) {
                _c_Component = transform.Find("Component").GetComponent<ResourceItemBase>();
            }
            return _c_Component;
        }
    }
    private SelectionsUI C_SelectionsUI {
        get {
            if (_c_SelectionsUI == null) {
                _c_SelectionsUI = transform.Find("Selections").GetComponent<SelectionsUI>();
            }
            return _c_SelectionsUI;
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
    private Button C_ComponentButton {
        get {
            if (_c_ComponentButton == null) {
                _c_ComponentButton = transform.Find("Component").GetComponent<Button>();
            }
            return _c_ComponentButton;
        }
    }

    private ForgeStructure Structure { get; set; }
    private int SelectedComponentID {
        get {
            return Structure.SelectedComponentID;
        }
        set {
            if (value == -1) {
                C_Component.Clear();
            } else {
                C_Component.SetItemIDAndRarity(value, (PublicData.Rarity)Math.Round(Structure.GetResultRarity()));
            }
            Structure.SelectedComponentID = value;
        }
    }

    private void OnEnable() {
        C_PackController.gameObject.SetActive(true);

        Structure.OnFinished += Structure_OnFinished;
        Structure.OnProgressUpdate += Structure_OnProgressUpdate;

        for (int i = 0; i < Structure.Materials.Length; i++) {
            if (Structure.Materials[i] != null && Structure.UnlockMaterials[i]) {
                C_Materials[i].SetItemData(Structure.Materials[i]);
            } else {
                C_Materials[i].Clear();
            }
            C_Materials[i].gameObject.SetActive(Structure.UnlockMaterials[i]);
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
        Structure = carriage.Structures[gameObject.name] as ForgeStructure;

        C_SelectionsUI.SetData(Structure.ComponentIDs, (id) => SelectedComponentID = id);
        C_SelectionsUI.gameObject.SetActive(false);

        for (int i = 0; i < C_Materials.Length; i++) {
            C_Materials[i].gameObject.SetActive(Structure.UnlockMaterials[i]);
            C_Materials[i].OnChargeIn = OnChargeIn;
        }
    }
    
    private bool OnChargeIn(DragableAndDropableAssetsItemView sender, int id, int num) {
        ItemInfo item = StaticResource.GetItemInfoByID<ItemInfo>(id);
        if (item.Type == PublicData.ItemType.Material) {
            int index = Array.IndexOf(C_Materials, sender);
            Structure.Materials[index] = new ItemData(id, 1);
            if (SelectedComponentID != -1) {
                C_Component.SetItemIDAndRarity(SelectedComponentID, (PublicData.Rarity)Math.Round(Structure.GetResultRarity()));
            }
            if (num > 1) {
                sender.SetItemData(id, 1);
                PublicMethod.ConsumeItems(new ItemData[] { new ItemData(id, 1) });
                C_PackController.UpdatePack();
                return true;
            } else {
                return false;
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
        C_ComponentButton.interactable = isLocked;
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
        C_Component.SetItemIDAndRarity(SelectedComponentID, (PublicData.Rarity)Math.Round(Structure.GetResultRarity()));
        C_PackController.UpdatePack();
    }
}
