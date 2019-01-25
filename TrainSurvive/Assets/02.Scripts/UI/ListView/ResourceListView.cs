/*
 * 描述：物品列表
            功能：
            1、鼠标移到物品网格上时，显示物品面板信息
 * 作者：项叶盛
 * 创建时间：2018/12/8 22:04:08
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TTT.Utility;
using TTT.Item;
using TTT.Resource;

namespace TTT.UI.ListView
{
    public abstract class ResourceListView : BaseListView<ItemData>
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

            hoverPanel.gameObject.SetActive(false);
        }
        protected sealed override void OnItemView(ListViewItem item, ItemData data, int itemIndex)
        {
            ResourceItemBase view = GetView(item, data, itemIndex);
            view.OnItemEnter = delegate (ResourceItemBase i)
            {
                ItemData temp = data;
                StartCoroutine(ShowPanel(temp));
            };
            view.OnItemExit = delegate (ResourceItemBase i)
            {
                StopAllCoroutines();
                hoverPanel.gameObject.SetActive(false);
            };
        }
        protected virtual ResourceItemBase GetView(ListViewItem item, ItemData data, int index)
        {
            AssetsItemView view = ViewTool.ForceGetComponentInChildren<AssetsItemView>(item, "AssetsItem");
            view.SetItemData(data);
            return view;
        }
        private IEnumerator ShowPanel(ItemData data)
        {
            yield return new WaitForSeconds(0.5f);
            //防止出现闪烁现象
            hoverPanel.position = Input.mousePosition + new Vector3(25F, -30F, 0F);
            infoText.text = data.Name;
            detailText.text = data.Description;
            hoverPanel.gameObject.SetActive(true);
        }
    }
}