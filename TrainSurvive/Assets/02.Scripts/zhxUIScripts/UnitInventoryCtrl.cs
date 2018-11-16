/*
 * 描述：
 * 作者：����
 * 创建时间：2018/11/6 23:29:41
 * 版本：v0.1
 */
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

public class UnitInventoryCtrl : MonoBehaviour, IDropHandler{
    private GameObject grid;        //包含的物品格实例
    private ItemGridCtrl gridCtrl;
    public PublicData.Charge ChargeIn;
    public GameObject Prefab;



    public void OnDrop(PointerEventData eventData)                                    //仅在本空间为空的情况下触发
    {
        //if (!ChargeIn(eventData.pointerDrag.GetComponent<ItemGridCtrl>().item))
        //    return;
        grid = eventData.pointerDrag;
        gridCtrl = grid.GetComponent<ItemGridCtrl>();
        
        

        InventoryCtrl tempController = grid.GetComponent<ItemGridCtrl>().GetController();
        Debug.Log(tempController);
        tempController.coreInventory.PopItem(grid.GetComponent<ItemGridCtrl>().item);
        tempController.RemoveGrid(grid);

        grid.GetComponent<ItemGridCtrl>().BindContainer(gameObject);
        grid.transform.SetParent(gameObject.transform);
        //grid.GetComponent<RectTransform>().position = Vector2.zero;
    }

    public void Clear()
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
        if (gridCtrl.item.currPileNum - consumeNum < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool Consume(int consumeNum)
    {   //若剩余物品不够一次性扣除，则直接返回FALSE，否则返回TRUE，物品是否存在由外部判断
        if(gridCtrl.item.currPileNum - consumeNum < 0)
        {
            return false;
        }
        else if(gridCtrl.item.currPileNum - consumeNum == 0)
        {
            Destroy(grid);
            grid = null;
            gridCtrl = null;
        }
        else
        {
            gridCtrl.item.currPileNum -= consumeNum;
        }
        return true;
    }

    public bool GeneratorItem(int id,int num)
    {
        if(grid != null)
        {
            if(gridCtrl.item.id != id || gridCtrl.item.currPileNum + num > gridCtrl.item.maxPileNum)
            {
                return false;
            }
        }
        else
        {
            Item demoItem = new Assets._02.Scripts.zhxUIScripts.Material(id);
            grid = Instantiate(Prefab);
            gridCtrl = grid.GetComponent<ItemGridCtrl>();
            demoItem.belongGrid = gridCtrl;
            gridCtrl.BindContainer(gameObject);
            gridCtrl.BindItem(demoItem);
            grid.transform.SetParent(gameObject.transform);
        }
        return true;
    }
}
