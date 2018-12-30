/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/7 18:47:40
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using TTT.Utility;
using TTT.UI;

namespace WorldMap.UI
{
    public class TownChatListView : BaseListView<NPCChat>
    {
        protected override void OnItemView(ListViewItem item, NPCChat data, int itemIndex)
        {
            TownChatItem view = CompTool.ForceGetComponent<TownChatItem>(item);
            view.SetProfile(data.Name);
            view.SetContent(data.Content);
        }
    }
    public class TownChatItem : BaseItem
    {
        protected Text profile;
        protected Text content;
        protected override void CreateModel()
        {
            profile = ViewTool.CreateText("Profile");
            content = ViewTool.CreateText("Content");
        }

        protected override void InitModel()
        {
            ViewTool.SetParent(profile, this);
            ViewTool.SetParent(content, this);
        }

        protected override void PlaceModel()
        {
            ViewTool.Anchor(profile, new Vector2(0F, 0F), new Vector2(0.2F, 1F));
            ViewTool.Anchor(content, new Vector2(0.2F, 0F), new Vector2(1F, 1F));
        }
        public void SetProfile(string name)
        {
            profile.text = name;
        }
        public void SetContent(string content)
        {
            this.content.text = content;
        }
    }
}