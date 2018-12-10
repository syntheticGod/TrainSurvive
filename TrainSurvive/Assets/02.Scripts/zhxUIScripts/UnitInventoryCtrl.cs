/*
 * 描述：专门为只能嵌入一个物品的容器设计的附加类
 * 作者：张皓翔
 * 创建时间：2018/11/6 19:53:41
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets._02.Scripts.zhxUIScripts;
using System;

public class UnitInventoryCtrl : MonoBehaviour, IDropHandler{

    public GameObject grid;        //包含的物品格实例
    private ItemGridCtrl gridCtrl;
    public Action<Item> OnItemIn;
    public bool isEquipmentGrid = false;
    public PublicData.Charge ChargeIn;  //需要玩家自己绑定准入函数
    public GameObject Prefab;


    private void Awake()
    {
    }

    public void OnDrop(PointerEventData eventData)                                    //仅在本空间为空的情况下触发
    {
        if (eventData.pointerDrag.GetComponent<ItemGridCtrl>().GetController() == null || !(ChargeIn?.Invoke(eventData.pointerDrag.GetComponent<ItemGridCtrl>().item) ?? false))
            return; 
        Destroy(GameObject.Find("tempDragImg"));

        grid = eventData.pointerDrag;
        Debug.Log(grid.name);
        gridCtrl = grid.GetComponent<ItemGridCtrl>();
        if (isEquipmentGrid)
        {
            int personID = GameObject.Find("gcTextPanel").GetComponent<PersonTextPanel>().getIndexOfpersonUsed();
            Person curPerson = World.getInstance().persons[personID];
            curPerson.equipWeapon((Weapon)gridCtrl.item);
            GameObject.Find("gcTextPanel").GetComponent<PersonTextPanel>().updatePanel(personID,false);    //这句话存在问题，导致原来的grid直接变成了itemGrid
        }
        InventoryCtrl tempController = gridCtrl.GetController();
        tempController.coreInventory.PopItem(gridCtrl.item);
        tempController.RemoveGrid(grid);
        
        gridCtrl.BindContainer(gameObject);
        grid.transform.SetParent(gameObject.transform);
        //grid.GetComponent<RectTransform>().position = Vector2.zero;

        OnItemIn?.Invoke(gridCtrl.item);
    }

    public void Clear()
    {
        if (grid) {
            gridCtrl.DestroyMyself();
            grid = null;
            gridCtrl = null;
        }
        
    }

    public void ClearWithNotDestroyMyself()
    {
        grid = null;
        gridCtrl = null;
    }

    private bool AddNum(int num)    
    {   //若增加这些物品后达超过堆叠上限，则直接返回FALSE，否则返回TRUE
        if(gridCtrl.item.currPileNum + num > gridCtrl.item.maxPileNum)
        {
            return false;
        }
        gridCtrl.item.currPileNum += num;
        return true;
    }

    public bool CanConsume(int consumeNum)
    {   //预判该物品是否足够量的消耗
        if (grid == null || gridCtrl.item.currPileNum - consumeNum < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CanGeneratorItem(int id, int num) {
        if (grid != null) {
            if (gridCtrl.item.id != id || gridCtrl.item.currPileNum + num > gridCtrl.item.maxPileNum) {
                return false;
            }
        }
        return true;
    }

    public void Consume(int consumeNum)
    {   
        if(gridCtrl.item.currPileNum - consumeNum == 0)
        {
            Destroy(grid);
            grid = null;
            gridCtrl = null;
        }
        else
        {
            gridCtrl.item.currPileNum -= consumeNum;
        }
    }

    public bool GeneratorItem(int id,int num = 1)
    {
        if(grid != null)
        {
            return false;
        }
        else
        {
            Item item = PublicMethod.GenerateItem(id, num)[0];
            grid = Instantiate(Prefab);
            gridCtrl = grid.GetComponent<ItemGridCtrl>();
            item.belongGrid = gridCtrl;
            gridCtrl.BindContainer(gameObject);
            gridCtrl.BindItem(item);
            grid.transform.SetParent(gameObject.transform);
            //Item demoItem = new Assets._02.Scripts.zhxUIScripts.Material(id);
            //grid = Instantiate(Prefab);
            //gridCtrl = grid.GetComponent<ItemGridCtrl>();
            //demoItem.belongGrid = gridCtrl;
            //gridCtrl.BindContainer(gameObject);
            //gridCtrl.BindItem(demoItem);
            //grid.transform.SetParent(gameObject.transform);
            //gridCtrl.item.currPileNum = num;
        }
        return true;
    }
    
    public Item GetItem() {
        if (grid != null) {
            return gridCtrl.item;
        }
        return null;
    }
}
