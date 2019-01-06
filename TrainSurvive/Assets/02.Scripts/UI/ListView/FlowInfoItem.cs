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
    public class FlowInfoItem : BaseItem
    {
        private Text titleView;
        private Text detailView;
        private Text timeView;
        private ResourceItemBase itemView;
        protected override void CreateModel()
        {
            titleView = ViewTool.CreateText("Title");
            detailView = ViewTool.CreateText("Detail");
            timeView = ViewTool.CreateText("Time");
            itemView = ViewTool.ForceGetComponentInChildren<ResourceItemBase>(this, "ItemImage");
        }

        protected override void InitModel()
        {
            titleView.alignment = TextAnchor.MiddleLeft;
            titleView.fontSize = 15;
            detailView.alignment = TextAnchor.UpperLeft;
            detailView.fontSize = 20;
            timeView.alignment = TextAnchor.MiddleLeft;
            timeView.fontSize = 14;
        }

        protected override void PlaceModel()
        {
            ViewTool.SetParent(titleView, this);
            ViewTool.SetParent(detailView, this);
            ViewTool.SetParent(timeView, this);
            ViewTool.Anchor(titleView, new Vector2(0.3333f, 0.75f), Vector2.one);
            ViewTool.Anchor(detailView, new Vector2(0.3333f, 0.25f), new Vector2(1f, 0.75f));
            ViewTool.Anchor(timeView, new Vector2(0.3333f, 0f), new Vector2(1, 0.25f));
            ViewTool.Anchor(itemView, new Vector2(0.0278f, 0.0834f), new Vector2(0.3056f, 0.9167f));
        }
        public void SetData(FlowInfoData data)
        {
            titleView.text = data.Title;
            detailView.text = data.Detail;
            timeView.text = data.Time;
            itemView.SetItemID(data.ItemID);
        }
    }
}