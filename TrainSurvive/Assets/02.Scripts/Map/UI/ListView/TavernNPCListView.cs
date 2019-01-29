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
using TTT.Xml;

namespace WorldMap.UI
{
    public class TavernNPCListView : BaseListView<int>
    {
        private static string[] persistentStrs = new string[] { "大厅" };
        protected override int GetPersistentCount()
        {
            return persistentStrs.Length;
        }
        protected override void OnItemView(ListViewItem item, int id, int itemIndex)
        {
            NpcInfo info = NpcInfoLoader.Instance.Find(id);
            ViewTool.ForceGetComponentInChildren<PersonBaseItem>(item, "NPC").GetComponentInChildren<Text>().text = info.Name;
            item.Tag = info;
        }
        protected override void OnPersistentItemView(ListViewItem item, int index)
        {
            Text text = ViewTool.CreateText("Persistent" + index, persistentStrs[index]);
            ViewTool.SetParent(text, item);
            ViewTool.FullFillRectTransform(text, Vector2.zero, Vector2.zero);
        }

    }
}