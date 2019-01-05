/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/8 22:04:08
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TTT.Utility;
using TTT.UI;
using TTT.Item;

namespace WorldMap.UI
{
    public abstract class ResourceListViewBase : BaseListView<ItemData>
    {
        private RectTransform hoverPanel;
        private RectTransform panelContent;
        private Text infoText;
        private Text detailText;
        protected override void Awake()
        {
            base.Awake();

            //Hover Panel
            hoverPanel = new GameObject("ListViewItemPanel").AddComponent<RectTransform>();
            ViewTool.SetParent(hoverPanel, this);
            hoverPanel.pivot = new Vector2(0, 1);
            ViewTool.Anchor(hoverPanel, new Vector2(0, 1), new Vector2(0, 1));
            hoverPanel.gameObject.SetActive(false);
            //Hover Panel Content
            panelContent = new GameObject("ResourcePanel", typeof(Image), typeof(VerticalLayoutGroup)).GetComponent<RectTransform>();
            ViewTool.SetParent(panelContent, hoverPanel);
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
        protected sealed override void OnItemView(ListViewItem item, ItemData data, int itemIndex)
        {
            ResourceItemBase view = GetResourceItemBase(item, data);
            view.onItemEnter = delegate (ResourceItemBase i) {
                ItemData temp = data;
                StartCoroutine(ShowPanel(temp));
            };
            view.onItemExit = CallbackItemExit;
        }
        protected abstract ResourceItemBase GetResourceItemBase(ListViewItem item, ItemData data);
        
        IEnumerator ShowPanel(ItemData data)
        {
            yield return new WaitForSeconds(0.5f);
            //防止出现闪烁现象
            hoverPanel.position = Input.mousePosition + new Vector3(0.1F, -0.1F, 0F);
            hoverPanel.DetachChildren();
            infoText.text = data.Name;
            detailText.text = data.Description;
            hoverPanel.gameObject.SetActive(true);
        }
        public void CallbackItemExit(ResourceItemBase item)
        {
            StopAllCoroutines();
            hoverPanel.gameObject.SetActive(false);
        }
    }
}