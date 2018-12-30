/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/8 14:21:37
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using TTT.Utility;
using TTT.UI;

namespace WorldMap.UI
{
    public class TavernNPCListView : HeroListView
    {
        private static string[] persistentStrs = new string[] { "大厅" };
        protected override int GetPersistentCount()
        {
            return persistentStrs.Length;
        }
        protected override void OnPersistentItemView(ListViewItem item, int index)
        {
            Text text = ViewTool.CreateText("Persistent" + index, persistentStrs[index]);
            ViewTool.SetParent(text, item);
            ViewTool.FullFillRectTransform(text, Vector2.zero, Vector2.zero);
        }

    }
}