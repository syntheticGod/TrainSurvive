/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/8 14:55:22
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using TTT.Utility;
using UnityEngine.UI;
using WorldMap.UI;

namespace TTT.UI
{
    public class FlowBaseData
    {
        public FlowBaseData(string title, string detail, string time)
        {
            Title = title;
            Detail = detail;
            Time = time;
        }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Time { get; set; }
        public virtual FlowBaseItem CreateView(Component parent)
        {
            FlowBaseItem view = ViewTool.ForceGetComponentInChildren<FlowBaseItem>(parent, "FlowInfoItem");
            view.SetData(this);
            return view;
        }
    }
    public class FlowBaseItem : BaseItem
    {
        protected Text titleView;
        protected Text detailView;
        protected Text timeView;
        protected override void CreateModel()
        {
            titleView = ViewTool.CreateText("Title");
            detailView = ViewTool.CreateText("Detail");
            timeView = ViewTool.CreateText("Time");
            ViewTool.SetParent(titleView, this);
            ViewTool.SetParent(detailView, this);
            ViewTool.SetParent(timeView, this);
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
            ViewTool.Anchor(titleView, new Vector2(0f, 0.75f), Vector2.one);
            ViewTool.Anchor(detailView, new Vector2(0f, 0.25f), new Vector2(1f, 0.75f));
            ViewTool.Anchor(timeView, new Vector2(0f, 0f), new Vector2(1, 0.25f));
        }
        public virtual void SetData(FlowBaseData data)
        {
            titleView.text = data.Title;
            detailView.text = data.Detail;
            timeView.text = data.Time;
        }
    }
}
