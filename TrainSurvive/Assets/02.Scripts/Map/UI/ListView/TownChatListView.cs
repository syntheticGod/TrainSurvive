/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/7 18:47:40
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using WorldMap.Model;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class TownChatListView : BaseListView<NPCChat>
    {
        protected override void OnItemView(ListViewItem item, NPCChat data)
        {
            TownChatItem view = Utility.ForceGetComponent<TownChatItem>(item);
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
            profile = Utility.CreateText("Profile");
            content = Utility.CreateText("Content");
        }

        protected override void InitModel()
        {
            Utility.SetParent(profile, this);
            Utility.SetParent(content, this);
        }

        protected override void PlaceModel()
        {
            Utility.Anchor(profile, new Vector2(0F, 0F), new Vector2(0.2F, 1F));
            Utility.Anchor(content, new Vector2(0.2F, 0F), new Vector2(1F, 1F));
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