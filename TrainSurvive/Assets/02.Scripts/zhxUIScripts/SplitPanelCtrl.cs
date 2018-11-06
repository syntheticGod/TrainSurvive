/*
 * 描述：控制物品数量分割面板及分割信息的传输
 * 作者：张皓翔
 * 创建时间：2018/11/3 16:54:19
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplitPanelCtrl : MonoBehaviour {

    private int splitNum;
    private ItemGridCtrl bindGrid;
    // Use this for initialization
    private void Awake()
    {
        splitNum = 0;
        bindGrid = null;
    }

    public void BindGrid(ItemGridCtrl grid)
    {
        bindGrid = grid;
        GetComponentInChildren<Slider>().maxValue = grid.item.currPileNum - 1;
        GetComponentInChildren<Text>().text = "0/" + GetComponentInChildren<Slider>().maxValue.ToString();
    }

    public void UpdateText(float value)
    {
        splitNum = (int)value;
        GetComponentInChildren<Text>().text = ((int)value).ToString() + "/" + GetComponentInChildren<Slider>().maxValue.ToString();
    }

    // Update is called once per frame
    public void ConfirmSplit()
    {
        if (splitNum == 0)
        {
            Destroy(gameObject);
            return;
        }
        bindGrid.SplitItem(splitNum);
        Destroy(gameObject);
    }
}
