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

public class PersonListContent : MonoBehaviour {
    public GameObject content;
    public UnitInventoryCtrl EquipmentCtrl;
    
    // Use this for initialization
    void Start () {
        reloadData();
        EquipmentCtrl.ChargeIn = (item) => item.itemType == Assets._02.Scripts.zhxUIScripts.PublicData.ItemType.Weapon;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void reloadData()
    {
        World world = World.getInstance();
        int index = 0;
        foreach (Person person in world.persons)
        {
            GameObject personCell = Resources.Load("Prefabs/PersonList/cell") as GameObject;
            GameObject cellInstance = Instantiate(personCell);
            PersonCell cell = (PersonCell)cellInstance.GetComponent("PersonCell");
            cell.index = index;
            index++;
            cell.setCellText("人物名：" + person.name);
            cellInstance.transform.parent = content.transform;
        }
    }
}
