/*
 * 描述：物品的UI视图。包括：物品图标、稀有度边框、物品数量
 * 作者：项叶盛
 * 创建时间：2018/12/3 9:25:40
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;
using TTT.UI;
using UnityEngine.EventSystems;
using TTT.Item;
using TTT.Resource;

namespace WorldMap.UI
{
    public class AssetsItemView : ResourceItemBase, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //public delegate void OnItemDragCallBack(AssetsItemView item);
        //public OnItemDragCallBack onItemBeginDrag { set; get; }
        //public OnItemDragCallBack onItemDragOutCallBack { set; get; }
        private AssetsItemView dragingItem;
        public int Number { get; private set; }
        protected Text numView;
        private Canvas canvas;
        public IDropMessageReceiver DropMsgReceriver { get; set; }
        protected override void CreateModel()
        {
            base.CreateModel();
            numView = ViewTool.CreateText("Number");
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        protected override void InitModel()
        {
            base.InitModel();
            ViewTool.SetParent(numView, this);
        }
        protected override void PlaceModel()
        {
            base.PlaceModel();
            RectTransform rect = numView.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.6F, 0.2F);
            rect.anchorMax = new Vector2(0.9F, 0.4F);
        }
        public void SetNumber(int number)
        {
            Number = number;
            numView.text = number.ToString();
        }
        /// <summary>
        /// 根据物品的ID和数量 设置单元格视图
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <param name="number">物品数量</param>
        public void SetItemData(int id, int number)
        {
            SetItemID(id);
            Number = number;
            numView.text = number.ToString();
        }
        /// <summary>
        /// 设置物体的数据
        /// </summary>
        /// <param name="data"></param>
        public void SetItemData(ItemData data)
        {
            SetItemID(data.ID);
            Number = data.Number;
            numView.text = data.Number.ToString();
        }
        public override void Clear()
        {
            base.Clear();
            numView.text = "";
            Number = 0;
        }
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag id:" + ItemID + " number:" + Number);
            dragingItem = ViewTool.ForceGetComponentInChildren<AssetsItemView>(canvas, "TempDragObject");
            dragingItem.SetItemData(ItemID, Number);
            CompTool.ForceGetComponent<CanvasGroup>(dragingItem).blocksRaycasts = false;
            RectTransform rect = dragingItem.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.offsetMin = new Vector2(-50f, -50f);
            rect.offsetMax = new Vector2(50f, 50f);
            rect.position = Input.mousePosition;
            dragingItem.gameObject.SetActive(true);
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (dragingItem != null)
                dragingItem.GetComponent<RectTransform>().position = Input.mousePosition;
            //Debug.Log("OnDrag id:" + ItemID + " number:" + Number + " " + dragingItem.name);
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("ItemVIew OnEndDrag");
            if (dragingItem.gameObject.activeSelf)
            {
                dragingItem.gameObject.SetActive(false);
            }
        }
        public void DropSucess()
        {
            if (dragingItem.gameObject.activeSelf)
            {
                dragingItem.gameObject.SetActive(false);
            }
        }
    }
}