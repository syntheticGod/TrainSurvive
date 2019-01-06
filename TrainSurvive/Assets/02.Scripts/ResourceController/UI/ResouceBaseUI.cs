/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/6 14:18:15
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResouceBaseUI : MonoBehaviour {
    public Text foodInText;
    public Slider foodInSlider;
    public Text foodOutText;
    public Slider foodOutSlider;
    public Text energyText;
    public Slider energySlider;
    public Text electricityText;
    public Slider electricitySlider;
    public Text moneyText;
    // Use this for initialization
    void Start () {
        World world = World.getInstance();
        setFoodIn(world.getFoodIn(), world.getFoodInMax());
        setFoodOut(world.getFoodOut(), world.getFoodOutMax());
        setEnergy(world.getEnergy(), world.getEnergyMax());
        setElectricity(world.getElectricity(), world.getElectricityMax());
        setMoney(world.getMoney());
        world.resourceUI = this;
    }
	
	public void setFoodIn(float currentNum,float maxNum)
    {
        foodInSlider.minValue = 0;
        foodInSlider.maxValue = maxNum;
        foodInSlider.value = currentNum;
        foodInText.text = (int)currentNum + "/" + (int)maxNum;
    }
    public void setFoodOut(uint currentNum, uint maxNum)
    {
        foodOutSlider.minValue = 0;
        foodOutSlider.maxValue = maxNum;
        foodOutSlider.value = currentNum;
        foodOutText.text = currentNum + "/" + maxNum;
    }
    public void setEnergy(float currentNum, float maxNum)
    {
        energySlider.minValue = 0;
        energySlider.maxValue = maxNum;
        energySlider.value = currentNum;
        energyText.text = (int)currentNum + "/" + (int)maxNum;
    }
    public void setElectricity(float currentNum, float maxNum)
    {
        electricitySlider.minValue = 0;
        electricitySlider.maxValue = maxNum;
        electricitySlider.value = currentNum;
        electricityText.text = (int)currentNum + "/" + (int)maxNum;
    }
    public void setMoney(uint currentNum)
    {
        moneyText.text = currentNum.ToString();
    }

}
