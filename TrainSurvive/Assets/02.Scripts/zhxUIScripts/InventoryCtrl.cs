/*
 * 描述：物品栏（前端控制区）
 * 作者：张皓翔
 * 创建时间：2018/10/31 18:41:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._02.Scripts.zhxUIScripts;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCtrl : MonoBehaviour, IDropHandler {

    public Inventory coreInventory;
    public GameObject itemGrid;
    public List<GameObject> itemGridInst;
    public Canvas belongCanvas;
    public Text sizeShow;

    private void Awake()
    { 
        coreInventory = new Inventory(300, this);              //测试临时MaxSize
        itemGridInst = new List<GameObject>();
        ReFreshMaxSize();
    }

    private void Update()
    {
        //---------------------
    }

    public void ReFreshMaxSize()                        //重新计算最大容量
    {
        sizeShow.text = string.Format("{0:f1}/{1:f0}", coreInventory.currSize, coreInventory.maxSize);
        //sizeShow.text = coreInventory.currSize.ToString("#.#") + "/" + coreInventory.maxSize.ToString("#.#");
    }

    public void ReFreshShowGrid()                       //刷新/同步
    {
        foreach (GameObject grid in itemGridInst)
        {
            grid.GetComponent<ItemGridCtrl>().Refresh();
        }
    }

    public void ReFreshInvalidItem()
    {
        for(int i=0; i<itemGridInst.Count; ++i)
        {
            if(itemGridInst[i] == null)
            {
                itemGridInst.RemoveAt(i);
            }
        }
    }

    public void AddGrid(Item item)                      //添加一个物品格实例
    {
        GameObject tempGrid = Instantiate(itemGrid);
        tempGrid.GetComponent<ItemGridCtrl>().BindItem(item);
        tempGrid.GetComponent<ItemGridCtrl>().BindController(this);
        itemGridInst.Add(tempGrid);
        tempGrid.transform.SetParent(transform);
        tempGrid.transform.SetAsLastSibling();
        tempGrid.name = item.name;
        //coreInventory.PushItem(item);                //是Inventory的pushitem....先调用再传入该前台，所以这样会造成无限递归
        
    }

    public void RemoveGrid(GameObject grid)             //用于外部销毁某个Grid实例后将List中对应的实例剔除，以免出现空指针错误
    {
        for(int i=0; i<itemGridInst.Count; ++i)
        {
            if(itemGridInst[i] == grid)
            {
                itemGridInst.RemoveAt(i);
                return;
            }
        }
        Debug.Log("找不到对应Grid，无法剔除");
        return;
    }

    public void OnDrop(PointerEventData eventData)     //收到拖拽释放消息
    {
        Destroy(GameObject.Find("tempDragImg"));
        GameObject oriGrid = eventData.pointerDrag;
        if(oriGrid.tag != "ITEMGRID")
        {
            return;
        }
        int restNum = coreInventory.PushItemToLast(oriGrid.GetComponent<ItemGridCtrl>().item);
        oriGrid.SendMessage("SetRestNum", restNum);
        
    }

    //  仅测试用代码  ---------------------------------------------------------
    public void AddWeapon()
    {
        Item temp = new Weapon(Random.Range(0, 100));
        coreInventory.PushItem(temp);

    }

    public void AddConsumable()
    {
        Item temp = new Consumable(Random.Range(100, 200));
        coreInventory.PushItem(temp);
    }

    public void AddMaterial()
    {
        Item temp = new Assets._02.Scripts.zhxUIScripts.Material(Random.Range(200, 205));
        coreInventory.PushItem(temp);

    }

    
}
