/*
 * 描述：英雄选择框
 * 作者：项叶盛
 * 创建时间：2018/12/10 22:42:38
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class HeroSelectDialog : BaseDialog
    {
        private HeroListView heroListView;
        protected override void CreateModel()
        {
            SetTitle("请选择人物");
            DialogSizeType = EDialogSizeType.MIDDLE12x12;
            heroListView = Utility.ForceGetComponentInChildren<HeroListView>(this, "HeroListView");
            heroListView.GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            heroListView.GridConstraintCount = 7;
            Utility.SetParent(heroListView, this);
            Utility.FullFillRectTransform(heroListView, new Vector2(0, 60F), new Vector2(0, -60F));
        }
        protected override void AfterDialogShow()
        {
            heroListView.Datas = world.GetHeros();
        }
        public Person GetSelectedHero()
        {
            return world.GetHeros()[heroListView.SelectIndex];
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
