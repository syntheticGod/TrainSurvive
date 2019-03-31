/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/3/30 21:47:49
 * 版本：v0.7
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.UI;
using TTT.Utility;

namespace WorldMap.UI
{
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