/*
 * 描述：物品转能源建筑UI
 * 作者：刘旭涛
 * 创建时间：2018/12/4 14:40:29
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using UnityEngine.UI;

public class Item2EnergySUI : FacilityUI {

    public UnitInventoryCtrl Gas;
    public Slider Slider;
    public AutomataUI AutomataUI;

    private new Item2EnergyStructure Structure {
        get {
            return base.Structure as Item2EnergyStructure;
        }
    }

    private void Awake() {
        Gas.ChargeIn = (item) => Structure.Conversions.ContainsKey(item.id);
        AutomataUI.OnChangeState = (state, value) => {
            Structure.AutomataCount = value;
            Structure.AutomataEnabled = state;
        };
    }

    private void OnEnable() {
        UpdateUI();
        Structure.OnAcquireGas = OnAcquireGas;
        Structure.OnGasUpdate = OnGasUpdate;
        Structure.OnProgressChange += OnProgressChange;
        Structure.OnAutomataCountChange = OnAutomataCountChange;

        UIManager.Instance?.ToggleInventoryPanel(true);
    }

    private void OnDisable() {
        Structure.OnAcquireGas = null;
        Structure.OnGasUpdate = null;
        Structure.OnProgressChange -= OnProgressChange;
        Structure.OnAutomataCountChange = null;
    }

    private void UpdateUI() {
        Gas.Clear();
        if (Structure.Gas != null)
            Gas.GeneratorItem(Structure.Gas.id, Structure.Gas.num);
        Slider.value = Structure.Progress;
        AutomataUI.gameObject.SetActive(World.getInstance().automata);
        AutomataUI.Value = Structure.AutomataCount;
        AutomataUI.Enabled = Structure.AutomataEnabled;
    }
    
    private void OnAcquireGas(ref ItemData old) {
        Item item = Gas.GetItem();
        if (item == null) { // item空
            old = null;
        } else {
            if (old == null) { // item不空，old空
                old = new ItemData(item.id, item.currPileNum);
            } else {
                if (old.id == item.id) {  // item不空，old不空，item与old的id相同
                    old.num = item.currPileNum;
                } else {  // item不空，old不空，item与old的id不同
                    old = new ItemData(item.id, item.currPileNum);
                }
            }
        }
    }

    private void OnGasUpdate(ItemData newItem) {
        if(newItem == null) {
            Gas.Clear();
        } else {
            if (Gas.GetItem() == null) {
                Gas.GeneratorItem(newItem.id, newItem.num);
            } else {
                Gas.GetItem().currPileNum = newItem.num;
            }
        }
    }
    
    private void OnProgressChange(float min, float max, float value) {
        Slider.minValue = min;
        Slider.maxValue = max;
        Slider.value = value;
    }

    private void OnAutomataCountChange(int count) {
        AutomataUI.Value = count;
    }
}
