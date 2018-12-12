/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/8 22:04:08
 * 版本：v0.1
 */
using UnityEngine;
using WorldMap.Model;
using UnityEngine.UI;
using TTT.Utility;

namespace WorldMap.UI
{
    public abstract class ResourceListViewBase : MergableListView<Good>
    {
        private RectTransform panelContent;
        private Text infoText;
        private Text detailText;
        protected override void Awake()
        {
            base.Awake();
            panelContent = new GameObject("ResourcePanel", typeof(Image), typeof(VerticalLayoutGroup)).GetComponent<RectTransform>();
            panelContent.pivot = new Vector2(0F, 1F);
            VerticalLayoutGroup verticalLayoutGroup = panelContent.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = 10;
            ContentSizeFitter contentSizeFitter = panelContent.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            infoText = ViewTool.CreateText("InfoText");
            ViewTool.SetParent(infoText, panelContent);
            infoText.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            infoText.alignment = TextAnchor.UpperLeft;

            detailText = ViewTool.CreateText("DetailText");
            ViewTool.SetParent(detailText, panelContent);
            detailText.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            detailText.alignment = TextAnchor.UpperLeft;
        }
        protected override void OnHoverPanelShow(RectTransform panel, Good data)
        {
            base.OnHoverPanelShow(panel, data);
            panel.DetachChildren();
            ViewTool.SetParent(panelContent, panel);
            infoText.text = data.item.ToString();
            detailText.text = data.item.description;
        }
    }
}