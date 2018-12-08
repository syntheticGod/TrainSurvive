/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 9:38:09
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using WorldMap.UI;
using Assets._02.Scripts.zhxUIScripts;
using Material = Assets._02.Scripts.zhxUIScripts.Material;
using WorldMap.Model;

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
            base.CreateModel();
            //PackListView
            packLV = Utility.ForceGetComponentInChildren<TeamPackListView>(gameObject, "PackListViewLayout");
            packLV.SetBackgroudColor(containerColor);
            packLV.ScrollDirection = ScrollType.Vertical;
            packLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            Utility.FullFillRectTransform(packLV, new Vector2(20, 20), new Vector2(-20, -20));
            packLV.gameObject.SetActive(true);
            //Buttons
            RectTransform btns = new GameObject("Btns", typeof(RectTransform)).GetComponent<RectTransform>();
            Utility.SetParent(btns, this);
            Utility.FullFillRectTransform(btns, Vector2.zero, Vector2.zero);
            Vector2 pivotOfRightUp = new Vector2(0, 1);
            Vector2 sizeOfRightUp = new Vector2(80, 50);
            Vector2 direction = Vector2.zero;
            rightTopBtns = new Button[rightUpBtnsName.Length];
            for (int i = 0; i < rightUpBtnsName.Length; i++)
            {
                rightTopBtns[i] = Utility.CreateBtn(rightUpBtnsName[i], rightUpBtnsContent[i], btns);
                Utility.RightTop(rightTopBtns[i], pivotOfRightUp, sizeOfRightUp, direction);
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
                rightBottomBtns[i] = Utility.CreateBtn(rightBottomBtnsName[i], rightBottomBtnsContent[i], btns);
                Utility.RightBottom(rightBottomBtns[i], pivotOfRightBottom, sizeOfRightBottom, direction);
                direction.y += sizeOfRightBottom.y;
            }
            rightBottomBtns[0].onClick.AddListener(delegate () { OnRightBottomBtnClick(0); });
            Vector2 pivotOfTopLeft = Vector2.zero;
            Vector2 sizeOfTopLeft = new Vector2(100, 50);
            direction = Vector2.zero;
            topLeftBtns = new Button[topLeftBtnsName.Length];
            for (int i = 0; i < topLeftBtnsName.Length; i++)
            {
                topLeftBtns[i] = Utility.CreateBtn(topLeftBtnsName[i], topLeftBtnsContent[i], btns);
                Utility.LeftTop(topLeftBtns[i], pivotOfTopLeft, sizeOfTopLeft, direction);
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
            HideWindow();
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
        delegate bool Filter(Item item);
        protected override bool FocusBehaviour()
        {
            return true;
        }

        protected override void UnfocusBehaviour()
        {
        }
    }
}