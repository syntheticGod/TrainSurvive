/*
 * 描述：ListView控制器，使用起来非常方便，只需要在对象中添加该脚本即可简单使用。
 *          其中块大小是固定的。
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
using System.Collections;

namespace WorldMap.UI
{
    public enum ScrollType
    {
        Horizontal,//水平滑动
        Vertical,//垂直滑动
        Both//四周滑动
    }
    public abstract class BaseListView<D> : MonoBehaviour
    {
        public delegate void ItemClick(ListViewItem item, D data);
        public delegate void PersistenItemClick(ListViewItem item, int index);
        public delegate bool ItemFilter(D data);
        public ItemClick onItemClick { set; get; }
        public PersistenItemClick onPersistentItemClick { set; get; }
        public ItemFilter onItemFilter { set; get; }

        private RectTransform content;
        private RectTransform hoverPanel;
        private RectTransform viewport;
        private List<ListViewItem> items;
        private List<ListViewItem> recycal;
        private ListViewItem lastClickedItem;
        private GridLayoutGroup gridLayout;
        private ScrollRect scrollRect;
        private ContentSizeFitter contentSizeFitter;

        public ScrollType m_scrollType = ScrollType.Horizontal;
        public GridLayoutGroup.Axis m_startAxis = GridLayoutGroup.Axis.Vertical;
        public GameObject m_itemContentPrefab;
        private const int UNSELECTED = -1;
        /// <summary>
        /// 取消选择功能时，取消之前选择的。
        /// 如果再次开启选择功能时，前一次选择的会再次亮起。
        /// </summary>
        public bool IfSelectable
        {
            set
            {
                m_ifSelectable = value;
                if (!m_ifSelectable)
                    lastClickedItem?.ShowBaseColor();
                else
                    lastClickedItem?.ShowSelectedColor();
            }
            get { return m_ifSelectable; }
        }
        public int SelectIndex { get; set; } = UNSELECTED;
        private bool m_ifSelectable = true;
        public bool IsSelectNothing { get { return !IfSelectable || SelectIndex == UNSELECTED; } }
        protected Vector2 cellSize = new Vector2(100.0F, 100.0F);
        public float defaultScrollRectSensitivity = 15F;
        /// <summary>
        /// 一行的长度
        /// -1：无限长
        /// </summary>
        private Vector2 viewPortSize;
        private List<D> m_datas;
        private int m_persistentCount;
        public List<D> Datas
        {
            set
            {
                m_datas = value;
                Refresh();
            }
            get { return m_datas; }
        }
        public void SetData(List<D> datas)
        {
            m_datas = datas;
        }
        public ScrollType ScrollDirection
        {
            set
            {
                m_scrollType = value;
                if (scrollRect != null)
                {
                    scrollRect.vertical = m_scrollType != ScrollType.Horizontal;
                    scrollRect.horizontal = m_scrollType != ScrollType.Vertical;
                }
            }
        }
        /// <summary>
        /// Horizontal：先水平填充，填充满一行后，再填充下一行
        /// Vertical：先垂直填充，填充满一列后，再填充下一列
        /// </summary>
        public GridLayoutGroup.Axis StartAxis
        {
            set
            {
                m_startAxis = value;
                if (gridLayout != null)
                {
                    gridLayout.startAxis = m_startAxis;
                    if (m_startAxis == GridLayoutGroup.Axis.Horizontal)
                    {
                        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                    }
                    else
                    {
                        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    }
                }
            }
            get
            {
                return m_startAxis;
            }
        }
        public int GridConstraintCount
        {
            set
            {
                gridLayout.constraintCount = value;
            }
            get
            {
                return gridLayout.constraintCount;
            }
        }
        public GridLayoutGroup.Constraint GridConstraint
        {
            set
            {
                gridLayout.constraint = value;
            }
            get
            {
                return gridLayout.constraint;
            }
        }
        protected void ConfigCellSize()
        {
            if (cellSize.x < 0)
            {
                cellSize.x = viewPortSize.x;
            }
            if (cellSize.y < 0)
            {
                cellSize.y = viewPortSize.y;
            }
            gridLayout.cellSize = cellSize;
        }
        public int ItemCount { get { return items.Count; } }
        protected virtual int GetPersistentCount()
        { return 0; }
        protected virtual void Awake()
        {
            CreateBaseModel();
            m_persistentCount = GetPersistentCount();
            for (int i = 0; i < m_persistentCount; i++)
            {
                OnPersistentItemView(AppendItem(), i);
            }
        }
        protected void CreateBaseModel()
        {
            gameObject.AddComponent<RectTransform>();
            //ScrollRect
            scrollRect = Utility.ForceGetComponent<ScrollRect>(this);
            //Viewport
            Transform viewportTransform = transform.Find("Viewport");
            if (viewportTransform == null)
            {
                viewport = new GameObject("Viewport", typeof(Mask), typeof(Image)).GetComponent<RectTransform>();
                viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0.1F);
                Utility.SetParent(viewport, scrollRect);
            }
            else
            {
                viewport = viewportTransform.GetComponent<RectTransform>();
            }
            Utility.FullFillRectTransform(viewport);
            //Content
            Transform contentTransform = viewport.Find("Content");
            if (contentTransform == null)
            {
                content = new GameObject("Content", typeof(GridLayoutGroup), typeof(ContentSizeFitter)).GetComponent<RectTransform>();
                contentSizeFitter = content.GetComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                Utility.SetParent(content, viewport);
            }
            else
            {
                content = Utility.ForceGetComponent<RectTransform>(contentTransform);
                contentSizeFitter = Utility.ForceGetComponent<ContentSizeFitter>(contentTransform);
            }
            Utility.FullFillRectTransform(content);
            content.pivot = new Vector2(0, 1);
            //Hover Panel
            hoverPanel = new GameObject("ListViewItemPanel").AddComponent<RectTransform>();
            Utility.SetParent(hoverPanel, this);
            hoverPanel.pivot = new Vector2(0, 1);
            Utility.Anchor(hoverPanel, new Vector2(0, 1), new Vector2(0, 1));
            hoverPanel.gameObject.SetActive(false);

            //Config ScrollRect
            scrollRect.content = content;
            scrollRect.viewport = viewport;
            scrollRect.scrollSensitivity = defaultScrollRectSensitivity;
            ScrollDirection = m_scrollType;
            //Config GridLayoutGroup
            gridLayout = content.GetComponent<GridLayoutGroup>();
            recycal = new List<ListViewItem>();
            items = new List<ListViewItem>();
        }
        void Start()
        {
            viewPortSize = viewport.rect.size;
            Debug.Log("vireport:" + viewport.rect.size + " width:" + viewport.rect.width + " height:" + viewport.rect.height);
            ConfigCellSize();
            StartAxis = m_startAxis;
        }
        void Update()
        { }
        public void SetCellSize(Vector2 cellSize)
        {
            this.cellSize = cellSize;
        }
        public void SetBackgroudColor(Color color)
        {
            Utility.ForceGetComponent<Image>(gameObject).color = color;
        }
        public void Refresh()
        {
            RemoveAllItem();
            RefreshData();
        }
        protected virtual void RefreshData()
        {
            for (int i = 0; i < Datas.Count; i++)
            {
                if (onItemFilter == null || !onItemFilter(Datas[i]))
                    OnItemView(AppendItem(), Datas[i]);
            }
        }
        public virtual void AddItem(D data)
        {
            Datas.Add(data);
            if (onItemFilter == null || !onItemFilter(data))
                OnItemView(AppendItem(), data);
        }
        public virtual bool RemoveItem(D data)
        {
            if (!Datas.Remove(data))
                return false;
            RemoveAllItem();
            Refresh();
            return true;
        }
        public void RemoveAllItem()
        {
            for (int i = m_persistentCount; i < items.Count; ++i)
            {
                ListViewItem item = items[i];
                item.Recycle();
                item.gameObject.SetActive(false);
                recycal.Add(item);
            }
            items.RemoveRange(m_persistentCount, items.Count - m_persistentCount);
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
            int index = items.IndexOf(item);
            if (IfSelectable)
            {
                SelectIndex = index;
                if (lastClickedItem != null && item != lastClickedItem)
                    lastClickedItem.ShowBaseColor();
                item.ShowSelectedColor();
            }
            if (index < GetPersistentCount())
            {
                onPersistentItemClick?.Invoke(item, index);
            }
            else
            {
                index -= GetPersistentCount();
                onItemClick?.Invoke(item, Datas[index]);
            }
            lastClickedItem = item;
        }
        public void CallbackItemEnter(ListViewItem item)
        {
            int index = items.IndexOf(item);
            //持久的Item不显示Panel
            if (index >= GetPersistentCount())
            {
                index -= GetPersistentCount();
                StartCoroutine(ShowPanel(Datas[index]));
            }
        }
        IEnumerator ShowPanel(D data)
        {
            yield return new WaitForSeconds(0.5f);
            //防止出现闪烁现象
            hoverPanel.position = Input.mousePosition + new Vector3(0.1F, -0.1F, 0F);
            OnHoverPanelShow(hoverPanel, data);
            hoverPanel.gameObject.SetActive(true);
        }
        public void CallbackItemExit(ListViewItem item)
        {
            StopAllCoroutines();
            if (hoverPanel.gameObject.activeSelf)
            {
                hoverPanel.gameObject.SetActive(false);
            }
        }
        private void SetParent(RectTransform child, RectTransform parent)
        {
            child.SetParent(parent);
            child.anchorMin = Vector2.zero;
            child.anchorMax = Vector2.one;
            //默认左上
            child.pivot = new Vector2(0, 1);
            child.localEulerAngles = Vector3.zero;
            child.localScale = Vector3.one;
            child.localPosition = Vector3.zero;
            child.offsetMin = Vector2.zero;
            child.offsetMax = Vector2.zero;
        }
        protected ListViewItem AppendItem()
        {
            ListViewItem itemView;
            if (recycal.Count == 0)
            {
                itemView = CreateItem();
                itemView.callBackItemClick = CallbackItemClick;
                itemView.callBackItemEnter = CallbackItemEnter;
                itemView.callBackItemExit = CallbackItemExit;
            }
            else
            {
                itemView = recycal[0];
                recycal.RemoveAt(0);
                itemView.gameObject.SetActive(true);
            }
            items.Add(itemView);
            return itemView;
        }
        private ListViewItem CreateItem()
        {
            RectTransform rectOfItem = new GameObject("Item", typeof(RectTransform)).GetComponent<RectTransform>();
            if (m_itemContentPrefab != null)
            {
                GameObject contentGameObject = Instantiate(m_itemContentPrefab);
                RectTransform rectContent = contentGameObject.GetComponent<RectTransform>();
                if (rectContent == null)
                    rectContent = contentGameObject.AddComponent<RectTransform>();
                SetParent(contentGameObject.GetComponent<RectTransform>(), rectOfItem);
            }
            ListViewItem listViewItem = rectOfItem.GetComponent<ListViewItem>();
            if (listViewItem == null)
                listViewItem = rectOfItem.gameObject.AddComponent<ListViewItem>();
            SetParent(rectOfItem, content.GetComponent<RectTransform>());
            return listViewItem;
        }
        protected abstract void OnItemView(ListViewItem item, D data);
        protected virtual void OnPersistentItemView(ListViewItem item, int index)
        { }
        protected virtual void OnHoverPanelShow(RectTransform panel, D data)
        {

        }
    }
    public class ListViewItem : Image, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void CallBackItemEvent(ListViewItem item);
        private static int IncreaseingID = 0;
        public static int GetNewIDUnsafely()
        {
            return IncreaseingID++;
        }
        private const int UNSET = -1;
        //鼠标点击Item回调代理
        public CallBackItemEvent callBackItemClick;
        //鼠标进入item回调代理
        public CallBackItemEvent callBackItemEnter;
        //鼠标移出item回调代理
        public CallBackItemEvent callBackItemExit;
        public Color BaseColor { set; get; } = new Color(1F, 1F, 1F, 1F);
        public Color SelectedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F, 0.5F);
        public Color ClickedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F, 0.5F);
        public object Tag { set; get; }
        public int ID { private set; get; }
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
            Tag = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("条款被点击了");
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                callBackItemClick?.Invoke(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("鼠标进入条款");
            callBackItemEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("鼠标退出条款");
            callBackItemExit?.Invoke(this);
        }
    }
    public class NullListView : BaseListView<int>
    {
        protected override void OnItemView(ListViewItem item, int data)
        {
        }
    }
}