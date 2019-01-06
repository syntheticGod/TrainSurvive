/*
 * 描述：屏幕左边的流动提示信息
 * 作者：项叶盛
 * 创建时间：2019/1/7 1:37:24
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using TTT.Utility;
using System.Collections.Generic;
using TTT.Item;
using TTT.Resource;

namespace TTT.UI
{
    public class FlowInfoData
    {
        public FlowInfoData(int itemID, string title, string detail, string time)
        {
            ItemID = itemID;
            Title = title;
            Detail = detail;
            Time = time;
        }
        public int ItemID { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Time { get; set; }
    }
    public class FlowInfo : BaseListView<FlowInfoData>
    {
        protected override void Awake()
        {
            m_cellSize.x = 360f;
            m_cellSize.y = 120f;
            m_scrollType = ScrollType.Vertical;
            m_gridConstraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
            m_gridConstraintCount = 1;
            m_datas = new List<FlowInfoData>();
            base.Awake();
            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.1111f);
            rect.anchorMax = new Vector2(0.1875f, 0.6667f);
        }
        protected override void OnItemView(ListViewItem item, FlowInfoData data, int itemIndex)
        {
            FlowInfoItem view = ViewTool.ForceGetComponentInChildren<FlowInfoItem>(item, "FlowInfoItem");
            view.SetData(data);
        }
        public override void AddItem(FlowInfoData data)
        {
            base.AddItem(data);
            StartCoroutine(Timer(data, 2f + Datas.Count * 0.2f));
        }
        IEnumerator Timer(FlowInfoData data, float time)
        {
            yield return new WaitForSeconds(time);
            Datas.Remove(data);
            Refresh();
        }
        public static void ShowItem(string title, int itemID, int number)
        {
            FlowInfo flowInfo = ViewTool.ForceGetComponentInChildren<FlowInfo>(GameObject.Find("Canvas"), "FlowInfo");
            string time = "";
            ItemInfo itemInfo = StaticResource.GetItemInfoByID<ItemInfo>(itemID);
            string detail = "获得" + number + "个稀有度为" + itemInfo.Rarity + "的" + itemInfo.Name + "。";
            flowInfo.AddItem(new FlowInfoData(itemID, title, detail, time));
        }
    }
}
