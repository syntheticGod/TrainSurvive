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
    public Text moneyText;
    // Use this for initialization
    void Start () {
        World world = World.getInstance();
        setFoodIn(world.getFoodIn(), world.getFoodInMax());
        setFoodOut(world.getFoodOut(), world.getFoodOutMax());
        setEnergy(world.getEnergy(), world.getEnergyMax());
        world.resourceUI = this;
    }
	
	public void setFoodIn(uint currentNum,uint maxNum)
    {
        foodInSlider.minValue = 0;
        foodInSlider.maxValue = maxNum;
        foodInSlider.value = currentNum;
        foodInText.text = currentNum + "/" + maxNum;
    }
    public void setFoodOut(uint currentNum, uint maxNum)
    {
        foodOutSlider.minValue = 0;
        foodOutSlider.maxValue = maxNum;
        foodOutSlider.value = currentNum;
        foodOutText.text = currentNum + "/" + maxNum;
    }
    public void setEnergy(uint currentNum, uint maxNum)
    {
        energySlider.minValue = 0;
        energySlider.maxValue = maxNum;
        energySlider.value = currentNum;
        energyText.text = currentNum + "/" + maxNum;
    }
    public void setMoney(uint currentNum)
    {
        moneyText.text = currentNum.ToString();
    }

}
