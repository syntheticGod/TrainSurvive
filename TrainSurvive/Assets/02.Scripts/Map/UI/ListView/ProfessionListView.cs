/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/13 15:51:13
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Resource;
using TTT.Utility;

namespace WorldMap.UI
{
    public class ProfessionListView : BaseListView<Profession>
    {
        protected override void Awake()
        {
            base.Awake();
            ScrollDirection = ScrollType.DISABLE;
            GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GridConstraintCount = 1;
        }
        protected override void Start()
        {
            base.Start();
            cellSize.x = -1;
            cellSize.y = viewPortSize.y / StaticResource.AttributeCount;
            ConfigCellSize();
        }
        protected override void OnItemView(ListViewItem item, Profession data, int itemIndex)
        {
            ProfessionItemView view = ViewTool.ForceGetComponentInChildren<ProfessionItemView>(item,"ProfessionItem");
            view.SetIcon(data.IconSmall);
            string content = StaticResource.GetAttributeName(itemIndex) + " " +
                data.Name + "\n" + data.Info;
            view.SetInfo(content);
        }
    }
    public class ProfessionItemView : BaseItem
    {
        private Image professionIcon;
        private Text professionInfo;
        private const int fontSize = 14;
        protected override void CreateModel()
        {
            professionIcon = ViewTool.CreateImage("Icon");
            professionInfo = ViewTool.CreateText("Info");

        }
        protected override void InitModel()
        {
            professionInfo.alignment = TextAnchor.MiddleLeft;
            professionInfo.fontSize = fontSize;
        }
        protected override void PlaceModel()
        {
            ViewTool.SetParent(professionIcon, this);
            ViewTool.Anchor(professionIcon, Vector2.zero, new Vector2(0.285F, 1F));
            ViewTool.SetParent(professionInfo, this);
            ViewTool.Anchor(professionInfo, new Vector2(0.285F, 0F), Vector2.one);
        }
        public void SetIcon(Sprite icon)
        {
            professionIcon.sprite = icon;
        }
        public void SetInfo(string info)
        {
            professionInfo.text = info;
        }
    }
}