/*
 * 描述：ListView控制器，使用起来非常方便，只需要在对象中添加该脚本即可简单使用。
 *          不过目前只支持水平的方向
 *          Selectable属性 bool类型：如果启用，则点击Item之后会显示选中状态。
 *          Item Width属性 float类型：设置Item的宽度
 *          
 *          onItemClick属性 delegate类型：点击Item的点击回调函数。
 *                  示例：
 *                  onItemClick = delegate(ListViewItem item, int index){
 *                      Debug.Log("item:"+index+" is clicked");
 *                  }
 *          onItemView属性 delegate类型：每次Item显示的时候都会调用该函数。可以用来给Item填充数据
 *                  示例：
 *                  onItemView = delegate(ListViewItem item, int index){
 *                      //在item中显示英雄的名字
 *                      item.GetComponentInChildren<Text>().text = heros[index].name;
 *                  }
 * 作者：项叶盛
 * 创建时间：2018/11/30 17:14:34
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace WorldMap.UI
{
    public class ListViewController : MonoBehaviour
    {
        public delegate void OnItemClick(ListViewItem item, int index);
        public delegate void OnItemView(ListViewItem item, int index);
        public OnItemClick onItemClick { set; get; }
        public OnItemView onItemView { set; get; }
        
        private GameObject content;
        private List<ListViewItem> items;
        private List<ListViewItem> recycal;
        private ListViewItem lastClickedItem;

        public GameObject m_itemContentPrefab;
        public bool m_selectable = true;
        public float m_itemWidth = 100;
        private float viewPortHeight;
        public int ItemCount { get { return items.Count; } }

        void Awake()
        {
            //ScrollRect
            ScrollRect scrollRect = gameObject.GetComponent<ScrollRect>();
            if (scrollRect == null)
                scrollRect = gameObject.AddComponent<ScrollRect>();
            //Viewport
            RectTransform viewport = new GameObject("Viewport", typeof(Mask), typeof(Image)).GetComponent<RectTransform>();
            SetParent(viewport, scrollRect.GetComponent<RectTransform>());
            viewPortHeight = viewport.rect.height;
            //Content
            content = new GameObject("Content", typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
            HorizontalLayoutGroup hlg = content.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 6;
            hlg.childControlHeight = hlg.childControlWidth = false;
            hlg.childForceExpandWidth = hlg.childForceExpandHeight = false;
            content.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            RectTransform contentRectTransform = content.GetComponent<RectTransform>();
            SetParent(contentRectTransform, viewport);
            //Config ScrollRect
            scrollRect.content = contentRectTransform;
            scrollRect.viewport = viewport;
            scrollRect.scrollSensitivity = 30;
            scrollRect.vertical = false;
            scrollRect.horizontal = true;

            recycal = new List<ListViewItem>();
            items = new List<ListViewItem>();
        }
        void Start()
        {

        }

        void Update()
        {

        }
        private ListViewItem CreateItem()
        {
            GameObject item;
            item = new GameObject("Item", typeof(RectTransform));
            if (m_itemContentPrefab != null)
            {
                GameObject contentGameObject = Instantiate(m_itemContentPrefab);
                contentGameObject.name = "Content";
                RectTransform rectContent = contentGameObject.GetComponent<RectTransform>();
                if (rectContent == null)
                    rectContent = contentGameObject.AddComponent<RectTransform>();
                SetParent(rectContent, item.GetComponent<RectTransform>());
            }
            ListViewItem listViewItem = item.GetComponent<ListViewItem>();
            if (listViewItem == null)
                listViewItem = item.AddComponent<ListViewItem>();
            return listViewItem;
        }
        private void ShowItem(ListViewItem item, int index)
        {
            RectTransform rectOfItem = item.GetComponent<RectTransform>();
            SetParent(rectOfItem, content.GetComponent<RectTransform>());
            rectOfItem.sizeDelta = new Vector2(m_itemWidth, viewPortHeight);
            onItemView?.Invoke(item, index);
        }
        private void RecyleItem(int index)
        {
            if (index >= items.Count)
            {
                Debug.LogWarning("回收的Item不存在");
                return;
            }
            ListViewItem item = items[index];
            items.RemoveAt(index);
            item.Recycle();
            item.gameObject.SetActive(false);
            recycal.Add(item);
        }
        public ListViewItem AppendItem()
        {
            ListViewItem itemView;
            if (recycal.Count == 0)
            {
                itemView = CreateItem();
            }
            else
            {
                itemView = recycal[0];
                recycal.RemoveAt(0);
                itemView.gameObject.SetActive(true);
            }
            itemView.Controller = this;
            items.Add(itemView);
            ShowItem(itemView, items.Count-1);
            return itemView;
        }
        public bool RemoveItem(int index)
        {
            if (index >= items.Count) return false;
            if (lastClickedItem == items[index])
                lastClickedItem = null;
            RecyleItem(index);
            return true;
        }
        public void RemoveAllItem()
        {
            int CountOfItem = items.Count;
            for (int i = CountOfItem - 1; i >=0 ; --i)
            {
                RecyleItem(i);
            }
        }
        public ListViewItem GetItem(int index)
        {
            return items[index];
        }
        /// <summary>
        /// 代码逻辑点击Item，适用于默认显示指定item
        /// </summary>
        /// <param name="index"></param>
        /// <returns>
        /// TRUE：显示成功
        /// FALSE：index超过条款的数量
        /// </returns>
        public bool ClickManually(int index)
        {
            if (index >= items.Count) return false;
            CallbackItemClick(items[index]);
            return true;
        }
        public void CallbackItemClick(ListViewItem item)
        {
            onItemClick?.Invoke(item, items.IndexOf(item));
            if (m_selectable)
            {
                if (lastClickedItem != null && item != lastClickedItem)
                    lastClickedItem.ShowBaseColor();
                item.ShowSelectedColor();
            }
            lastClickedItem = item;
        }
        private void SetParent(RectTransform child,RectTransform parent)
        {
            child.SetParent(parent);
            child.anchorMin = new Vector2(0, 0);
            child.anchorMax = new Vector2(1, 1);
            //默认左上
            child.pivot = new Vector2(0, 1);
            child.localEulerAngles = Vector3.zero;
            child.localScale = Vector3.one;
            child.localPosition = Vector3.zero;
            child.offsetMin = Vector2.zero;
            child.offsetMax = Vector2.zero;
        }
    }
    public class ListViewItem : Image, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        private static int IncreaseingID = 0;
        public static int GetNewIDUnsafely()
        {
            return IncreaseingID++;
        }
        private const int UNSET = -1;
        public Color BaseColor { set; get; } = Color.clear;
        public Color SelectedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F, 0.5F);
        public Color ClickedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F);
        public object Tag { set; get; }
        public int ID { private set; get; }
        public ListViewController Controller { set; get; }
        public ListViewItem()
        {
            ID = GetNewIDUnsafely();
        }
        public void ShowBaseColor()
        {
            color = BaseColor;
        }
        public void ShowSelectedColor()
        {
            color = SelectedColor;
        }
        public void Recycle()
        {
            color = BaseColor;
            ID = UNSET;
            Controller = null;
            Tag = null;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("条款PointerUp");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("条款PointerDown");
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("条款被点击了");
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Controller.CallbackItemClick(this);
            }
        }
    }

}