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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using TTT.Utility;

namespace TTT.UI.ListView
{
    public enum ScrollType
    {
        DISABLE,//禁止滑动
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
        private RectTransform viewport;
        /// <summary>
        /// 存放正在显示的Item脚本（GameObject）
        /// </summary>
        private List<ListViewItem> items;
        /// <summary>
        /// 被回收的Item，删除添加时会再次利用之前创建的GameObject。
        /// 不过位置不一定在最后，这个以后再修改吧。
        /// </summary>
        private List<ListViewItem> recycal;
        /// <summary>
        /// 上一次点击的item
        /// 用途：当点击其他item时，将上一次点击的item颜色变为正常。
        /// </summary>
        private ListViewItem lastClickedItem;
        /// <summary>
        /// 存放item的GridLayoutGroup
        /// </summary>
        private GridLayoutGroup gridLayout;
        /// <summary>
        /// 上下左右滑动的脚本
        /// </summary>
        private ScrollRect scrollRect;
        /// <summary>
        /// Content的大小根据item数量自适应
        /// </summary>
        private ContentSizeFitter contentSizeFitter;
        /// <summary>
        /// 也可以手动设置Item的Prefab，如果未设置就创建
        /// </summary>
        public GameObject m_itemContentPrefab;
        private const int UNSELECTED = -1;
        /// <summary>
        /// 是否允许item被选择（点一下item会显示灰色，点其他item时变回正常色）
        /// 当IfSelectable设值成False时，所有的item会变为正常色。
        /// 如果再次开启选择功能时，前一次选择的会再次变为灰色。
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
        private bool m_ifSelectable = true;
        protected bool m_ifRecycle = true;
        /// <summary>
        /// 选择的索引
        /// </summary>
        public int SelectIndex { get; set; } = UNSELECTED;
        /// <summary>
        /// 判断是否没有选择
        /// </summary>
        public bool IsSelectNothing { get { return !IfSelectable || SelectIndex == UNSELECTED; } }
        /// <summary>
        /// item的大小
        /// </summary>
        protected Vector2 m_cellSize = new Vector2(100.0F, 100.0F);
        protected void ConfigCellSize()
        {
            if (m_cellSize.x < 0)
            {
                m_cellSize.x = viewPortSize.x;
            }
            if (m_cellSize.y < 0)
            {
                m_cellSize.y = viewPortSize.y;
            }
            gridLayout.cellSize = m_cellSize;
        }
        public void SetCellSize(Vector2 cellSize)
        {
            this.m_cellSize = cellSize;
        }
        /// <summary>
        /// 视图的大小
        /// </summary>
        protected Vector2 viewPortSize;
        /// <summary>
        /// 默认的滑动灵敏度
        /// </summary>
        public float defaultScrollRectSensitivity = 15F;
        /// <summary>
        /// 静态不变的Item数量。（再item前面加若干条一直存在的item，不会被销毁）
        /// 子类继承设置个数。意味着这个个数不一开始需要设置的参数。
        /// </summary>
        private int m_persistentCount;
        protected virtual int GetPersistentCount()
        { return 0; }
        /// <summary>
        /// 存放数据引用，内部不会重新拷贝一份
        /// </summary>
        protected List<D> m_datas;
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
        /// <summary>
        /// 设置滑动方向
        ///DISABLE,//禁止滑动
        ///Horizontal,//水平滑动
        ///Vertical,//垂直滑动
        ///Both//四周滑动
        /// </summary>
        public ScrollType m_scrollType = ScrollType.Horizontal;
        public ScrollType ScrollDirection
        {
            set
            {
                m_scrollType = value;
                if (scrollRect != null)
                {
                    scrollRect.vertical = m_scrollType == ScrollType.Vertical;
                    scrollRect.horizontal = m_scrollType == ScrollType.Horizontal;
                }
            }
        }
        public GridLayoutGroup.Axis m_startAxis = GridLayoutGroup.Axis.Vertical;
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
        protected int m_gridConstraintCount = 1;
        /// <summary>
        /// 固定列或者固定行的数量
        /// </summary>
        public int GridConstraintCount
        {
            set
            {
                m_gridConstraintCount = value;
                if (gridLayout != null)
                    gridLayout.constraintCount = value;
            }
            get
            {
                return m_gridConstraintCount;
            }
        }
        public GridLayoutGroup.Constraint m_gridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
        /// <summary>
        /// 固定列或者固定行
        /// </summary>
        public GridLayoutGroup.Constraint GridConstraint
        {
            set
            {
                m_gridConstraint = value;
                if (gridLayout != null)
                    gridLayout.constraint = value;
            }
            get
            {
                return m_gridConstraint;
            }
        }
        public int ItemCount { get { return items.Count; } }
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
            //gameObject.AddComponent<RectTransform>();
            //ScrollRect
            scrollRect = CompTool.ForceGetComponent<ScrollRect>(this);
            //Viewport
            Transform viewportTransform = transform.Find("Viewport");
            if (viewportTransform == null)
            {
                viewport = new GameObject("Viewport", typeof(Mask), typeof(Image)).GetComponent<RectTransform>();
                viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0.1F);
                ViewTool.SetParent(viewport, scrollRect);
            }
            else
            {
                viewport = viewportTransform.GetComponent<RectTransform>();
            }
            ViewTool.FullFillRectTransform(viewport);
            //Content
            Transform contentTransform = viewport.Find("Content");
            if (contentTransform == null)
            {
                content = new GameObject("Content", typeof(GridLayoutGroup), typeof(ContentSizeFitter)).GetComponent<RectTransform>();
                contentSizeFitter = content.GetComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                ViewTool.SetParent(content, viewport);
            }
            else
            {
                content = CompTool.ForceGetComponent<RectTransform>(contentTransform);
                contentSizeFitter = CompTool.ForceGetComponent<ContentSizeFitter>(contentTransform);
            }

            ViewTool.FullFillRectTransform(content);
            content.pivot = new Vector2(0, 1);

            //Config ScrollRect
            scrollRect.content = content;
            scrollRect.viewport = viewport;
            scrollRect.scrollSensitivity = defaultScrollRectSensitivity;
            //Config GridLayoutGroup
            gridLayout = content.GetComponent<GridLayoutGroup>();
            recycal = new List<ListViewItem>();
            items = new List<ListViewItem>();
            //设置默认值
            GridConstraint = m_gridConstraint;
            GridConstraintCount = m_gridConstraintCount;
            StartAxis = m_startAxis;
            ScrollDirection = m_scrollType;
        }
        protected virtual void Start()
        {
            viewPortSize = viewport.rect.size;
            //Debug.Log("vireport:" + viewport.rect.size + " width:" + viewport.rect.width + " height:" + viewport.rect.height);
            ConfigCellSize();
        }
        protected void Update()
        { }
        public void SetBackgroudColor(Color color)
        {
            CompTool.ForceGetComponent<Image>(gameObject).color = color;
        }
        public void Refresh()
        {
            RemoveAllItems();
            RefreshData();
        }
        /// <summary>
        /// 刷新数据时的操作，示例见MergableListView。
        /// 因为MergableListView会把删除数量小于等于0的item删去
        /// </summary>
        protected virtual void RefreshData()
        {
            for (int i = 0; i < Datas.Count; i++)
            {
                if (onItemFilter == null || !onItemFilter(Datas[i]))
                    OnItemView(AppendItem(), Datas[i], i);
            }
        }
        /// <summary>
        /// 添加Item
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddItem(D data)
        {
            Datas.Add(data);
            if (onItemFilter == null || !onItemFilter(data))
                OnItemView(AppendItem(), data, Datas.Count - 1);
        }
        /// <summary>
        /// 删去item。注意存放数据的数组是引用，并非拷贝，也就意味着连通外面的一起删掉。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool RemoveData(D data)
        {
            if (!Datas.Remove(data))
                return false;
            Refresh();
            return true;
        }
        public bool RemoveDataAt(int index)
        {
            if (index >= Datas.Count || index < 0)
                return false;
            Datas.RemoveAt(index);
            Refresh();
            return true;
        }
        /// <summary>
        /// 除去所有的数据
        /// </summary>
        public void RemoveAllDatas()
        {
            Datas.Clear();
            Refresh();
        }
        /// <summary>
        /// 除去所有的item，但不除去数据
        /// </summary>
        public void RemoveAllItems()
        {
            for (int i = m_persistentCount; i < items.Count; ++i)
            {
                ListViewItem item = items[i];
                if (m_ifRecycle)
                {
                    item.Recycle();
                    //将回收的item移到最后
                    item.GetComponent<Transform>().SetSiblingIndex(content.childCount - 1);
                    item.gameObject.SetActive(false);
                    recycal.Add(item);
                }
                else
                {
                    Destroy(item.gameObject);
                }
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
        /// <summary>
        /// Item点击的回调函数
        /// </summary>
        /// <param name="item"></param>
        public void CallbackItemClick(ListViewItem item)
        {
            int index = items.IndexOf(item);
            SelectIndex = index;
            if (IfSelectable)
            {
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
        /// <summary>
        /// 创建一个新的item，并把它追加到视图（GameObject）中。
        /// </summary>
        /// <returns></returns>
        protected ListViewItem AppendItem()
        {
            ListViewItem itemView;
            if (recycal.Count == 0)
            {
                itemView = CreateItem();
                itemView.callBackItemClick = CallbackItemClick;
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
        /// <summary>
        /// 创建一个Item
        /// </summary>
        /// <returns></returns>
        private ListViewItem CreateItem()
        {
            RectTransform rectOfItem = new GameObject("Item", typeof(RectTransform)).GetComponent<RectTransform>();
            if (m_itemContentPrefab != null)
            {
                RectTransform rectContent = CompTool.ForceGetComponent<RectTransform>(Instantiate(m_itemContentPrefab));
                ViewTool.SetParent(rectOfItem, rectContent);
            }
            ListViewItem listViewItem = CompTool.ForceGetComponent<ListViewItem>(rectOfItem);
            ViewTool.SetParent(rectOfItem, content);
            return listViewItem;
        }
        /// <summary>
        /// Item的具体显示方法
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        /// <param name="itemIndex"></param>
        protected abstract void OnItemView(ListViewItem item, D data, int itemIndex);
        /// <summary>
        /// 一直存在的item，具体显示方法
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        protected virtual void OnPersistentItemView(ListViewItem item, int index)
        { }
        /// <summary>
        /// 根据索引返回item的对象，不包括PersistentItem
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public D this[int index]
        {
            get { return m_datas[index]; }
        }
        /// <summary>
        /// 搜索指定的对象，并返回第一个匹配的项
        /// </summary>
        /// <param name="data">要在 List<T> 中定位的对象。 对于引用类型，该值可以为 null。</param>
        /// <returns>
        /// -1：不存在
        /// 不为-1：第一个匹配项的从零开始的索引</returns>
        public int IndexOf(D data)
        {
            return m_datas.IndexOf(data);
        }
    }
    public class ListViewItem : Image, IPointerClickHandler
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
            //Debug.Log("条款被点击了");
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                callBackItemClick?.Invoke(this);
            }
        }
    }
    public class NullListView : BaseListView<int>
    {
        protected override void OnItemView(ListViewItem item, int data, int itemIndex)
        {
        }
    }
}