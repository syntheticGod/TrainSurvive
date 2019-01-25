/*
 * 描述：酒馆中NPC头像列表
 * 作者：项叶盛
 * 创建时间：2018/12/8 14:21:37
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;
using TTT.UI.ListView;
using WorldMap.Model;

namespace WorldMap.UI
{
    public class TavernNPCListView : BaseListView<NPC>
    {
        private static string[] persistentStrs = new string[] { "大厅" };
        protected override int GetPersistentCount()
        {
            return persistentStrs.Length;
        }
        protected override void OnItemView(ListViewItem item, NPC data, int itemIndex)
        {
            ViewTool.ForceGetComponentInChildren<PersonBaseItem>(item, "NPC").GetComponentInChildren<Text>().text = data.Name;
            item.Tag = data;
        }
        protected override void OnPersistentItemView(ListViewItem item, int index)
        {
            Text text = ViewTool.CreateText("Persistent" + index, persistentStrs[index]);
            ViewTool.SetParent(text, item);
            ViewTool.FullFillRectTransform(text, Vector2.zero, Vector2.zero);
        }

    }
}