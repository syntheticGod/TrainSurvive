/*
 * 描述：物品转能源建筑UI
 * 作者：刘旭涛
 * 创建时间：2018/12/4 14:40:29
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using TTT.Utility;
using UnityEngine.UI;
using WorldMap.UI;

public class Item2EnergySUI : FacilityUI
{

    public UnitInventoryCtrl Gas;
    public Slider Slider;
    public AutomataUI AutomataUI;

    private new Item2EnergyStructure Structure
    {
        get
        {
            return base.Structure as Item2EnergyStructure;
        }
    }

    private void Awake()
    {
        Gas.OnChargeIn = delegate (int itemID, int number)
        {
            if (!Structure.Conversions.ContainsKey(itemID))
            {
                InfoDialog.Show("该材料不允许燃烧");
                return true;
            }
            return false;
        };
        //AutomataUI.OnChangeState = (state, value) =>
        //{
        //    Structure.AutomataCount = value;
        //    Structure.AutomataEnabled = state;
        //};
    }

    private void OnEnable()
    {
        UpdateUI();
        Structure.OnAcquireGas = OnAcquireGas;
        Structure.OnGasUpdate = OnGasUpdate;
        Structure.OnProgressChange += OnProgressChange;
        Structure.OnAutomataCountChange = OnAutomataCountChange;

        UIManager.Instance?.ToggleInventoryPanel(true);
    }

    private void OnDisable()
    {
        Structure.OnAcquireGas = null;
        Structure.OnGasUpdate = null;
        Structure.OnProgressChange -= OnProgressChange;
        Structure.OnAutomataCountChange = null;
    }

    private void UpdateUI()
    {
        //UnitInventoryCtrl 不一定在该脚本之前被初始化
        //Gas.Clear();
        if (Structure.Gas != null)
            Gas.GeneratorItem(Structure.Gas.ID, Structure.Gas.Number);
        Slider.value = Structure.Progress;
        AutomataUI.gameObject.SetActive(World.getInstance().automata);
        AutomataUI.Value = Structure.AutomataCount;
        //AutomataUI.Enabled = Structure.AutomataEnabled;
    }

    private void OnAcquireGas(ref ItemData old)
    {
        if (Gas.IfEmpty())
        { // item被拖出
            old = null;
        }
        else
        {
            if (old == null)
            { // item不空，old空
                old = new ItemData(Gas.ItemID, Gas.Number);
            }
            else
            {
                if (old.ID == Gas.ItemID)
                {  // item不空，old不空，item与old的id相同
                    old.Number = Gas.Number;
                }
                else
                {  // item不空，old不空，item与old的id不同
                    old = new ItemData(Gas.ItemID, Gas.Number);
                }
            }
        }
    }

    private void OnGasUpdate(ItemData newItem)
    {
        if (newItem == null)
            Gas.Clear();
        else
            Gas.GeneratorItem(newItem.ID, newItem.Number);
    }

    private void OnProgressChange(float min, float max, float value)
    {
        Slider.minValue = min;
        Slider.maxValue = max;
        Slider.value = value;
    }

    private void OnAutomataCountChange(int count)
    {
        AutomataUI.Value = count;
    }
}
