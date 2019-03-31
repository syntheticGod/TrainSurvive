/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/7 18:47:40
 * 版本：v0.1
 */
using TTT.Utility;
using TTT.UI.ListView;
using WorldMap.Model;

namespace WorldMap.UI
{
    public class TownChatListView : BaseListView<ChatSentence>
    {
        protected override void OnItemView(ListViewItem item, ChatSentence data, int itemIndex)
        {
            TownChatItem view = CompTool.ForceGetComponent<TownChatItem>(item);
            view.SetProfile(data.Name);
            view.SetContent(data.Content);
        }
    }
}