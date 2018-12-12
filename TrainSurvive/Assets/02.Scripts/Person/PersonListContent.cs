/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/11/24 11:46:53
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Assets._02.Scripts.zhxUIScripts;

public class PersonListContent : MonoBehaviour {
    public UnitInventoryCtrl EquipmentCtrl;
    public Toggle togTrain;
    public Toggle togTeam;
    public PersonTextPanel textPanel;
    // Use this for initialization
    void Start () {
        reloadData();
        Debug.Log("asdf");
        //EquipmentCtrl.ChargeIn = (item) => item.itemType == Assets._02.Scripts.zhxUIScripts.PublicData.ItemType.Weapon;
    }
	

    //public void WeaponChargeIn()

    public void reloadData()
    {
        clearCells();
        textPanel.clearPanel();
        bool trainPerson_display = togTrain.isOn;
        bool teamPerson_display = togTeam.isOn;
        World world = World.getInstance();
        int index = 0;
        foreach (Person person in world.persons)
        {
            if((trainPerson_display&&!person.ifOuting)|| (teamPerson_display && person.ifOuting))
            {
                GameObject personCell = Resources.Load("Prefabs/PersonList/cell") as GameObject;
                GameObject cellInstance = Instantiate(personCell);
                PersonCell cell = (PersonCell)cellInstance.GetComponent("PersonCell");
                cell.index = index;                
                cell.setCellText("人物名：" + person.name);
                cellInstance.transform.parent = gameObject.transform;
            }
            index++;
        }
    }

    public void clearCells()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
