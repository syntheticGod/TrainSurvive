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
    public class FlowInfo : BaseListView<FlowBaseData>
    {
        protected override void Awake()
        {
            m_cellSize.x = 360f;
            m_cellSize.y = 120f;
            m_scrollType = ScrollType.Vertical;
            m_gridConstraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
            m_gridConstraintCount = 1;
            m_datas = new List<FlowBaseData>();
            m_ifRecycle = false;
            base.Awake();
            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.1111f);
            rect.anchorMax = new Vector2(0.1875f, 0.6667f);
        }
        protected override void OnItemView(ListViewItem item, FlowBaseData data, int itemIndex)
        {
            data.CreateView(item);
        }
        public override void AddItem(FlowBaseData data)
        {
            base.AddItem(data);
            StartCoroutine(Timer(data, 2f + Datas.Count * 0.2f));
        }
        IEnumerator Timer(FlowBaseData data, float time)
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
            flowInfo.AddItem(new FlowItemData(itemID, title, detail, time));
        }
        public static void ShowInfo(string title, string detail)
        {
            FlowInfo flowInfo = ViewTool.ForceGetComponentInChildren<FlowInfo>(GameObject.Find("Canvas"), "FlowInfo");
            string time = "";
            flowInfo.AddItem(new FlowBaseData(title, detail, time));
        }
    }
}
