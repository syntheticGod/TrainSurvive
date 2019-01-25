/*
 * 描述：英雄选择框
 * 作者：项叶盛
 * 创建时间：2018/12/10 22:42:38
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;
using TTT.UI.ListView;
using System.Collections.Generic;

namespace WorldMap.UI
{
    public class HeroSelectDialog : BaseDialog
    {
        private HeroListView heroListView;
        private List<Person> datas;
        protected override void CreateModel()
        {
            SetTitle("请选择人物");
            DialogSizeType = EDialogSizeType.MIDDLE12x12;
            heroListView = ViewTool.ForceGetComponentInChildren<HeroListView>(this, "HeroListView");
            heroListView.GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            heroListView.GridConstraintCount = 7;
            heroListView.ScrollDirection = ScrollType.Vertical;
            ViewTool.SetParent(heroListView, this);
            ViewTool.FullFillRectTransform(heroListView, new Vector2(0, 60F), new Vector2(0, -60F));
        }
        public void SetDatas(List<Person> persons)
        {
            datas = persons;
        }
        protected override void AfterDialogShow()
        {
            heroListView.Datas = datas;
        }
        public Person GetSelectedHero()
        {
            return datas[heroListView.SelectIndex];
        }
        protected override void Cancel()
        {
        }
        protected override bool OK()
        {
            if (heroListView.IsSelectNothing)
            {
                InfoDialog.Show("未选择任何英雄");
                return false;
            }
            return true;
        }
    }
}
