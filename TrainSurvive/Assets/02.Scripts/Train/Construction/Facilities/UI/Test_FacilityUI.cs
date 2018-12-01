/*
 * 描述：测试用UI
 * 作者：刘旭涛
 * 创建时间：2018/11/7 14:36:06
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;

public class Test_FacilityUI : FacilityUI {
    
    public UnitInventoryCtrl Raw;
    public UnitInventoryCtrl Gas;
    public UnitInventoryCtrl Food;
    
    private new TEST_Facility Structure {
        get {
            return base.Structure as TEST_Facility;
        }
    }

    private void Awake() {
        Raw.ChargeIn = (item) => true;
        Gas.ChargeIn = (item) => true;
        Food.ChargeIn = (item) => false;
    }

    private void OnEnable() {
        UpdateUI();
        Structure.OnAcquireFood = Food.GetItem;
        Structure.OnFoodUpdate = OnFoodUpdate;
        Structure.OnAcquireGas = Gas.GetItem;
        Structure.OnGasUpdate = Gas.Clear;
        Structure.OnAcquireRaw = Raw.GetItem;
        Structure.OnRawUpdate = Raw.Clear;

        UIManager.Instance?.ToggleInventoryPanel(true);
    }

    private void OnDisable() {
        UpdateStructure();
        Structure.OnAcquireFood = null;
        Structure.OnFoodUpdate = null;
        Structure.OnAcquireGas = null;
        Structure.OnGasUpdate = null;
        Structure.OnAcquireRaw = null;
        Structure.OnRawUpdate = null;
    }

    private void UpdateUI() {
        Food.Clear();
        Raw.Clear();
        Gas.Clear();
        if (Structure.Food != null)
            Food.GeneratorItem(Structure.Food.id, Structure.Food.currPileNum);
        if (Structure.Raw != null)
            Raw.GeneratorItem(Structure.Raw.id, Structure.Raw.currPileNum);
        if (Structure.Gas != null)
            Gas.GeneratorItem(Structure.Gas.id, Structure.Gas.currPileNum);
    }

    private void UpdateStructure() {
        Structure.Raw = Raw.GetItem();
        Structure.Gas = Gas.GetItem();
        Structure.Food = Food.GetItem();
    }

    private void OnFoodUpdate(Item food) {
        Food.GeneratorItem(food.id, food.currPileNum);
    }
}