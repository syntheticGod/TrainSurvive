/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 9:38:09
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TTT.Utility;
<<<<<<< .merge_file_a24348
=======
using TTT.UI;
>>>>>>> .merge_file_a15364
using WorldMap.UI;
using WorldMap.Model;
using Assets._02.Scripts.zhxUIScripts;

namespace WorldMap.Controller
{
    public class PackController : WindowsController
    {
        private TeamPackListView packLV;
        private List<Good> items;
        private string[] rightUpBtnsName = { "AllItem", "Equipments", "Materials", "Specials" };
        private string[] rightUpBtnsContent = { "全部", "装备", "材料", "特殊" };
        private Button[] rightTopBtns;
        private string[] rightBottomBtnsName = { "Discard", "Tind" };
        private string[] rightBottomBtnsContent = { "托此丢弃", "整理物品" };
        private Button[] rightBottomBtns;
        private string[] topLeftBtnsName = { "Close" };
        private string[] topLeftBtnsContent = { "关 闭" };
        private Button[] topLeftBtns;
        protected override void Start()
        { }
        protected override void Update()
        { }
        protected override void CreateModel()
        {
            enableTitileBar = false;
            base.CreateModel();
            //PackListView
            packLV = ViewTool.ForceGetComponentInChildren<TeamPackListView>(gameObject, "PackListViewLayout");
            packLV.SetBackgroudColor(containerColor);
            packLV.GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            packLV.GridConstraintCount = 8;
            packLV.ScrollDirection = ScrollType.Vertical;
            packLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            ViewTool.FullFillRectTransform(packLV, new Vector2(20, 20), new Vector2(-20, -20));
            packLV.gameObject.SetActive(true);
            //Buttons
            RectTransform btns = new GameObject("Btns", typeof(RectTransform)).GetComponent<RectTransform>();
            ViewTool.SetParent(btns, this);
            ViewTool.FullFillRectTransform(btns, Vector2.zero, Vector2.zero);
            Vector2 pivotOfRightUp = new Vector2(0, 1);
            Vector2 sizeOfRightUp = new Vector2(80, 50);
            Vector2 direction = Vector2.zero;
            rightTopBtns = new Button[rightUpBtnsName.Length];
            for (int i = 0; i < rightUpBtnsName.Length; i++)
            {
                rightTopBtns[i] = ViewTool.CreateBtn(rightUpBtnsName[i], rightUpBtnsContent[i], btns);
                ViewTool.RightTop(rightTopBtns[i], pivotOfRightUp, sizeOfRightUp, direction);
                direction.y -= sizeOfRightUp.y;
            }
            rightTopBtns[0].onClick.AddListener(delegate () { OnRightTopBtnClick(0); });
            rightTopBtns[1].onClick.AddListener(delegate () { OnRightTopBtnClick(1); });
            rightTopBtns[2].onClick.AddListener(delegate () { OnRightTopBtnClick(2); });
            rightTopBtns[3].onClick.AddListener(delegate () { OnRightTopBtnClick(3); });
            Vector2 pivotOfRightBottom = new Vector2(0, 0);
            Vector2 sizeOfRightBottom = new Vector2(120, 50);
            direction = Vector2.zero;
            rightBottomBtns = new Button[rightBottomBtnsName.Length];
            for (int i = 0; i < rightBottomBtnsName.Length; i++)
            {
                rightBottomBtns[i] = ViewTool.CreateBtn(rightBottomBtnsName[i], rightBottomBtnsContent[i], btns);
                ViewTool.RightBottom(rightBottomBtns[i], pivotOfRightBottom, sizeOfRightBottom, direction);
                direction.y += sizeOfRightBottom.y;
            }
            rightBottomBtns[0].onClick.AddListener(delegate () { OnRightBottomBtnClick(0); });
            Vector2 pivotOfTopLeft = Vector2.zero;
            Vector2 sizeOfTopLeft = new Vector2(100, 50);
            direction = Vector2.zero;
            topLeftBtns = new Button[topLeftBtnsName.Length];
            for (int i = 0; i < topLeftBtnsName.Length; i++)
            {
                topLeftBtns[i] = ViewTool.CreateBtn(topLeftBtnsName[i], topLeftBtnsContent[i], btns);
                ViewTool.LeftTop(topLeftBtns[i], pivotOfTopLeft, sizeOfTopLeft, direction);
                direction.x += sizeOfTopLeft.x;
                int index = i;
                topLeftBtns[i].onClick.AddListener(delegate () { OnTopLeftBtnClick(index); });
            }
        }
        public void OnRightTopBtnClick(int index)
        {
            switch (index)
            {
                case 0:
                    packLV.onItemFilter = null;
                    break;
                case 1:
                    packLV.onItemFilter = (Good item) => { return item.ItemType != PublicData.ItemType.Weapon; };
                    break;
                case 2:
                    packLV.onItemFilter = (Good item) => { return item.ItemType != PublicData.ItemType.Material; };
                    break;
                case 3:
                    packLV.onItemFilter = (Good item) => { return item.ItemType != PublicData.ItemType.SpecialItem; };
                    break;
            }
            packLV.Refresh();
        }
        public void OnRightBottomBtnClick(int index)
        {
            switch (index)
            {
                case 1:
                    break;
            }
        }
        public void OnTopLeftBtnClick(int index)
        {
            Hide();
        }

        protected override bool PrepareDataBeforeShowWindow()
        {
            //FOR TEST：背包测试数据
            ////items.Add(new Weapon(000));
            //items.Add(new Material(201));
            //items.Add(new Material(211));
            //items.Add(new Material(212));
            //items[1].currPileNum = 100;
            //items[2].currPileNum = 999;
            //items[3].currPileNum = 555;
            items = new List<Good>(world.GetGoodsInTeam());
            return true;
        }
        protected override void AfterShowWindow()
        {
            packLV.Datas = items;
        }
    }
}