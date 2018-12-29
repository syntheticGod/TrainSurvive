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
using TTT.UI;

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
            m_cellSize.x = -1;
            m_cellSize.y = viewPortSize.y / StaticResource.AttributeCount;
            ConfigCellSize();
        }
        protected override void OnItemView(ListViewItem item, Profession data, int itemIndex)
        {
            ProfessionItemView view = ViewTool.ForceGetComponentInChildren<ProfessionItemView>(item,"ProfessionItem");
            view.SetIcon(data.IconSmall);
            string[] content = new string[3];
            content[0] = StaticResource.GetAttributeName(itemIndex);
            content[1] = data.Name;
            content[2] = data.Info;
            view.SetInfo(content);
        }
    }
    public class ProfessionItemView : BaseItem
    {
        private Image professionIcon;
        private Text[] infos;
        private string[] infoPrefix = { "选择专精：", "发展方向：", "" };
        private const int fontSize = 14;
        protected override void CreateModel()
        {
            professionIcon = ViewTool.CreateImage("Icon");
            infos = new Text[3];
            for(int i = 0; i < infos.Length; i++)
            {
                infos[i] = ViewTool.CreateText("Info" + i);
                infos[i].alignment = TextAnchor.MiddleLeft;
                infos[i].fontSize = fontSize;
            }
        }
        protected override void InitModel()
        {
        }
        protected override void PlaceModel()
        {
            ViewTool.SetParent(professionIcon, this);
            ViewTool.Anchor(professionIcon, Vector2.zero, new Vector2(0.285F, 1F));
            float delta = 1.0f / infos.Length;
            Vector2 maxAnchor = new Vector2(1.0F, 1.0F);
            Vector2 minAnchor = new Vector2(0.285F, 1.0F - delta);
            for (int i = 0; i < infos.Length; i++)
            {
                ViewTool.SetParent(infos[i], this);
                ViewTool.Anchor(infos[i], minAnchor, maxAnchor);
                maxAnchor.y -= delta;
                minAnchor.y -= delta;
            }
        }
        public void SetIcon(Sprite icon)
        {
            professionIcon.sprite = icon;
        }
        public void SetInfo(string[] infosStr)
        {
            for(int i = 0; i < infos.Length; i++)
            {
                infos[i].text = infoPrefix[i] + infosStr[i];
            }
        }
    }
}