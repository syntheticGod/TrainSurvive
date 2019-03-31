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
using WorldMap.Controller;

namespace WorldMap.UI
{
    public class TavernNPCListView : BaseListView<int>
    {
        public TavernController Context { get; set; }
        private static string[] persistentStrs = new string[] { "大厅" };
        protected override int GetPersistentCount()
        {
            return persistentStrs.Length;
        }
        protected override void Start()
        {
            GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GridConstraintCount = 2;
            StartAxis = GridLayoutGroup.Axis.Horizontal;
            ScrollDirection = ScrollType.Vertical;
            onItemClick = OnItemClick;
            onPersistentItemClick = OnPersistentClick;
        }
        protected override void OnItemView(ListViewItem item, int id, int itemIndex)
        {
            NpcInfo info = NpcInfoLoader.Instance.Find(id);
            ViewTool.ForceGetComponentInChildren<NPCBaseItem>(item, "NPC").ShowNpc(info);
            item.Tag = info;
        }
        protected override void OnPersistentItemView(ListViewItem item, int index)
        {
            Text text = ViewTool.CreateText("Persistent" + index, persistentStrs[index]);
            ViewTool.SetParent(text, item);
            ViewTool.FullFillRectTransform(text, Vector2.zero, Vector2.zero);
        }

        public void OnItemClick(ListViewItem item, int npc)
        {
            Context.ShowSelectedNpc(SelectIndex);
        }
        public void OnPersistentClick(ListViewItem item, int index)
        {
            Context.ShowSelectedNpc(index);
        }
        /// <summary>
        /// 判断当前是否在大厅
        /// </summary>
        /// <returns></returns>
        public bool IfInHill()
        {
            return SelectIndex == 0;
        }
    }
}