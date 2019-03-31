/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/3/30 21:39:49
 * 版本：v0.7
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TTT.Utility;

namespace TTT.UI.ListView
{
    public class ListViewItem : Image, IPointerClickHandler
    {
        /// <summary>
        /// 点击item之后的灰色遮掩层
        /// </summary>
        private Image selectedMask;
        public delegate void CallBackItemEvent(ListViewItem item);
        private static int IncreaseingID = 0;
        public static int GetNewIDUnsafely()
        {
            return IncreaseingID++;
        }
        private const int UNSET = -1;
        //鼠标点击Item回调代理
        public CallBackItemEvent callBackItemClick;
        public Color BaseColor { set; get; } = new Color(1F, 1F, 1F, 0F);
        public Color SelectedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F, 0.5F);
        public Color ClickedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F, 0.5F);
        public object Tag { set; get; }
        public int ID { private set; get; }
        public ListViewItem()
        {
            ID = GetNewIDUnsafely();
        }
        void Awake()
        {
            selectedMask = ViewTool.CreateImage("Mask");
            ViewTool.SetParent(selectedMask, this);
            ViewTool.FullFillRectTransform(selectedMask);
        }

        public void ShowBaseColor()
        {
            selectedMask.color = BaseColor;
        }
        public void ShowSelectedColor()
        {
            selectedMask.transform.SetAsLastSibling();
            selectedMask.color = SelectedColor;
        }
        public void Recycle()
        {
            selectedMask.color = BaseColor;
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
}