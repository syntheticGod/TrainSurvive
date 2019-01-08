/*
 * 描述：屏幕左边的流动提示信息的条款视图
 * 作者：项叶盛
 * 创建时间：2019/1/7 1:40:34
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TTT.Utility;
using WorldMap.UI;

namespace TTT.UI
{
    public class FlowItemData : FlowBaseData
    {
        public FlowItemData(int itemID, string title, string detail, string time)
            : base(title, detail, time)
        {
            ItemID = itemID;
        }
        public int ItemID { get; set; }
        public override FlowBaseItem CreateView(Component parent)
        {
            FlowInfoItem view = ViewTool.ForceGetComponentInChildren<FlowInfoItem>(parent, "FlowBaseItem");
            view.SetData(this);
            return view;
        }
    }
    public class FlowInfoItem : FlowBaseItem
    {
        private ResourceItemBase itemView;
        protected override void CreateModel()
        {
            base.CreateModel();
            itemView = ViewTool.ForceGetComponentInChildren<ResourceItemBase>(this, "ItemImage");
            ViewTool.SetParent(itemView, this);
        }

        protected override void InitModel()
        {
            base.InitModel();
        }

        protected override void PlaceModel()
        {
            ViewTool.Anchor(titleView, new Vector2(0.3333f, 0.75f), Vector2.one);
            ViewTool.Anchor(detailView, new Vector2(0.3333f, 0.25f), new Vector2(1f, 0.75f));
            ViewTool.Anchor(timeView, new Vector2(0.3333f, 0f), new Vector2(1, 0.25f));
            ViewTool.Anchor(itemView, new Vector2(0.0278f, 0.0834f), new Vector2(0.3056f, 0.9167f));
        }
        public override void SetData(FlowBaseData data)
        {
            base.SetData(data);
            FlowItemData infoData = data as FlowItemData;
            if (infoData != null)
                itemView.SetItemID(infoData.ItemID);
        }
    }
}