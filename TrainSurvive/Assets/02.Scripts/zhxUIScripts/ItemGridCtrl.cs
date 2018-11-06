/*
 * 描述：控制单个物品在物品栏中的显示与互动
 * 作者：张皓翔
 * 创建时间：2018/11/1 0:05:47
 * 版本：v0.1
 * 注释：主动释放Drag方通过eventData.pointerCurrentRaycast获取对方释放区指向的GameObject
 *      接受Drag释放方通过eventData.pointerDray过去己方原拖拽物体
 *      11/4开始实现拖拽接口
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemGridCtrl : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int _index;
    private Item _item;
    public Sprite[] stateImg;

    public int index
    {
        get
        {
            return _index;
        }
        set
        {
            _index = value;
        }
    }       //疑
    public Item item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
        }
    }

    public Sprite markSprite;
    public Image mark;
    public Image state;
    public Image itemSprite;
    private Canvas belongCanvas;
    private InventoryCtrl belongController;
    private GameObject belongContainer;             //用来判断是否来自同一容器的主要依据
    private GameObject draggingImg;
    public GameObject informPanel;
    private GameObject tempInform;
    public GameObject splitPanel;
    public Text showPileNum;

    private Color[] markColors;

    private void Awake()
    {
        markColors = new Color[] { new Color(1, 1, 1), new Color(0.2824f, 0.8824f, 0.2627f), new Color(0.2627f, 0.7569f, 0.8784f), new Color(0.7373f, 0.2627f, 0.8706f), new Color(0.8706f, 0.2706f, 0.2706f) };
        mark.sprite = markSprite;
        belongController = null;
        _item = null;
        belongCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();                    //Canvas改名的话需要一起修改此句
    }

    public void BindController(InventoryCtrl ctrler)
    {
        belongController = ctrler;
        belongCanvas = ctrler.belongCanvas;
        belongContainer = null;
    }

    public void BindContainer(GameObject container)
    {
        belongController = null;
        belongContainer = container;
    }

    public void BindItem(Item item,int index)
    {
        _item = item;
        _index = index;
        _item.belongGrid = this;
        mark.color = markColors[(int)_item.rarity];
        itemSprite.sprite = _item.sprite;
        state.sprite = null;                                //临时不加
        if (item.itemType == PublicData.ItemType.weapon)
            showPileNum.gameObject.SetActive(false);
        else
            showPileNum.text = item.currPileNum.ToString();
    }

    public void Refresh()
    {
        showPileNum.text = item.currPileNum.ToString();
        mark.color = markColors[(int)_item.rarity];
        itemSprite.sprite = item.sprite;
        state.sprite = null;
        if(item.itemType == PublicData.ItemType.weapon)
        {
            showPileNum.gameObject.SetActive(false);
        }
        else
        {
            showPileNum.gameObject.SetActive(true);
        }
    }

    public void SetRestNum(int restNum)
    {
        if (restNum == item.currPileNum)
            return;
        if(restNum == 0)
        {
            belongController.RemoveGrid(gameObject);
            Destroy(gameObject);
            if(belongController != null)
                belongController.coreInventory.PopItem(item);
        }
        else
        {
            item.currPileNum = restNum;
        }
        
    }

    public InventoryCtrl GetController()
    {
        return belongController;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Destroy(GameObject.Find("tempDragImg"));
        GameObject oriGrid = eventData.pointerDrag;
        if (oriGrid.tag != "ITEMGRID")
            return;
        ItemGridCtrl oriGridCtrl = oriGrid.GetComponent<ItemGridCtrl>();
        int allowNum = oriGridCtrl.item.currPileNum;
        int restNum = 0;
        if(oriGridCtrl.belongContainer == belongContainer)
        {
            if (item.id == oriGridCtrl.item.id)
            {   //同物品容器/同ID ->  堆叠
                allowNum = item.maxPileNum - item.currPileNum >= allowNum ? allowNum : item.maxPileNum - item.currPileNum;
                restNum = oriGridCtrl.item.currPileNum - allowNum;
                item.currPileNum += allowNum;
                restNum = oriGridCtrl.item.currPileNum - allowNum;
                oriGrid.SendMessage("SetRestNum", restNum);
            }
            else                                                    
            {   //同物品容器/不同ID -> 交换内核
                Debug.Log("asdf");
                Item temp = oriGridCtrl.item;
                oriGridCtrl.item = item;
                item = temp;
                Refresh();
                oriGridCtrl.Refresh();
                item.belongGrid = this;
                oriGrid.GetComponent<ItemGridCtrl>().item.belongGrid = oriGrid.GetComponent<ItemGridCtrl>();
            }
        }
        else
        {
            if(belongController != null)
            {   //本格子属于正统物品栏
                float restSize = belongController.coreInventory.maxSize - belongController.coreInventory.currSize;
                allowNum = (restSize >= oriGridCtrl.item.size * oriGridCtrl.item.currPileNum ?
                    allowNum : (int)(restSize / oriGridCtrl.item.size));
                if(allowNum == 0)
                {
                    return;
                }
                if(item.id == oriGridCtrl.item.id)
                {   //正统物品栏的物品被拽入同类型物品 -> 只堆叠本物品，其余放至最后
                    int totalAllowNum = allowNum;
                    int totalRestNum = oriGridCtrl.item.currPileNum - allowNum;
                    allowNum = item.maxPileNum - item.currPileNum >= allowNum ? allowNum : item.maxPileNum - item.currPileNum;
                    restNum = totalAllowNum - allowNum;
                    item.currPileNum += allowNum;
                    oriGrid.SendMessage("SetRestNum", totalRestNum);            //回退
                    Item mappingItem = oriGridCtrl.item.Clone();
                    mappingItem.currPileNum = restNum;
                    belongController.coreInventory.PushItemToLast(mappingItem);
                }
                else
                {   //正统物品栏的物品被拽如不同类型物品 -> 直接计算出允许容量的物品，该回退回退，该放最后的放最后
                    restNum = oriGridCtrl.item.currPileNum - allowNum;
                    oriGrid.SendMessage("SetRestNum", restNum);
                    Item mappingItem = oriGridCtrl.item.Clone();
                    mappingItem.currPileNum = allowNum;
                    belongController.coreInventory.PushItemToLast(mappingItem);
                }
            }
            else
            {   //该物品所属容器不是正统的物品栏，是其他设备类，不同考虑容量，只需考虑堆叠数限制
                if(oriGridCtrl.item.id == item.id)
                {   //非物品栏收到同物品堆叠请求
                    allowNum = item.maxPileNum - item.currPileNum >= oriGridCtrl.item.currPileNum ?
                        oriGridCtrl.item.currPileNum : item.maxPileNum - item.currPileNum;
                    restNum = oriGridCtrl.item.currPileNum - allowNum;
                    item.currPileNum += allowNum;
                    oriGrid.SendMessage("SetRestNum", restNum);
                }
                else
                {   //非物品栏收到不同物品堆叠请求 -> 直接交换双方物品  （有些物品不能放在某些设备上，这个需要设备的脚本去把控）
                    Debug.Log(oriGridCtrl.belongController);
                    float restSize = oriGridCtrl.belongController.coreInventory.maxSize - oriGridCtrl.belongController.coreInventory.currSize;
                    float deltaSize = item.size * item.currPileNum - oriGridCtrl.item.size * oriGridCtrl.item.currPileNum;
                    if(deltaSize > restSize)   //对方是从物品栏中拖拽过来的（需要保证对方就是物品栏）
                    {
                        Debug.Log("容量不够，无法交换");
                        return;
                    }
                    else
                    {   //保持物品栏前后台数据同步
                        oriGridCtrl.belongController.coreInventory.PopItem(oriGridCtrl.item);
                        oriGridCtrl.belongController.coreInventory.PushItemWithNoGrid(item);

                        Item temp = oriGridCtrl.item;
                        oriGridCtrl.item = item;
                        item = temp;
                        item.belongGrid = this;
                        oriGrid.GetComponent<ItemGridCtrl>().item.belongGrid = oriGrid.GetComponent<ItemGridCtrl>();

                        Refresh();
                        oriGridCtrl.Refresh();
                    }
                }
            }
        }
            
        
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggingImg = new GameObject("tempDragImg");

        Image tempImg = draggingImg.AddComponent<Image>();
        CanvasGroup tempGroup = draggingImg.AddComponent<CanvasGroup>();
        tempGroup.blocksRaycasts = false;
        tempImg.sprite = item.sprite;
        draggingImg.transform.SetParent(belongCanvas.transform);
        draggingImg.transform.SetAsLastSibling();
        draggingImg.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)                                     //UI依然会被MASK遮挡
    {
        if(draggingImg != null)
        {
            draggingImg.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) //在触发该事件前物体就被销毁，需要解决(由OnDrop函数负责销毁临时图片)
    {   //这里不用检测对方，所有处理交由被释放方处理
        //Debug.Log("2");
        //Destroy(draggingImg);
        //当自己拖拽到自己上，则不做处理，销毁
        Destroy(GameObject.Find("tempDragImg"));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ShowInform());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        Destroy(tempInform);
    }

    public void SplitItem(int SplitNum)
    {
        belongController.coreInventory.PopItem(item, SplitNum);     //物品栏重量、堆叠数减少
        Refresh();
        Item mappingItem = item.Clone();
        Debug.Log(mappingItem);
        mappingItem.currPileNum = SplitNum;
        belongController.coreInventory.PushItemToLast(mappingItem);
    }

    IEnumerator ShowInform()
    {
        yield return new WaitForSeconds(0.5f);
        tempInform = Instantiate(informPanel, belongCanvas.transform);    //放在
        CanvasGroup group = tempInform.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        Text[] inform = tempInform.GetComponentsInChildren<Text>();
        inform[0].text = item.ToString();
        inform[1].text = item.description;
        tempInform.transform.SetAsLastSibling();
        tempInform.GetComponent<RectTransform>().position = Input.mousePosition;
        tempInform.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
        tempInform.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right && belongController != null)
        {
            GameObject temp = Instantiate(splitPanel);
            temp.GetComponent<SplitPanelCtrl>().BindGrid(this);
            temp.GetComponent<RectTransform>().position = new Vector2(Screen.width / 2, Screen.height / 2);
            temp.transform.SetParent(belongCanvas.transform);
            temp.transform.SetAsLastSibling();
        }
    }
}
