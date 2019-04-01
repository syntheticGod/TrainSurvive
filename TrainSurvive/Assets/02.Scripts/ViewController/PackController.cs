/*
 * 描述：物品界面
 * 作者：项叶盛
 * 创建时间：2018/12/3 9:38:09
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TTT.Utility;
using TTT.UI.ListView;
using WorldMap.UI;
using Assets._02.Scripts.zhxUIScripts;
using WorldMap;
using TTT.Item;

namespace TTT.Controller
{
    public class PackController : WindowsController, Observer
    {
        //视图
        private DragableResourceLV packLV;
        private string[] rightUpBtnsName = { "AllItem", "Equipments", "Materials", "Specials" };
        private string[] rightUpBtnsContent = { "全部", "装备", "材料", "特殊" };
        private Button[] rightTopBtns;
        private string[] rightBottomBtnsName = { "Discard", "Tind" };
        private string[] rightBottomBtnsContent = { "托此丢弃", "整理物品" };
        private Button[] rightBottomBtns;
        private string[] topLeftBtnsName = { "Close" };
        private string[] topLeftBtnsContent = { "关 闭" };
        private Button[] topLeftBtns;
        public Vector2 minAnchor = defaultMinAnchor;
        public Vector2 maxAnchor = defaultMaxAnchor;
        //数据
        private List<ItemData> items;
        protected override void OnEnable()
        {
            base.OnEnable();
            World.getInstance().storage.Attach(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            World.getInstance().storage.Detach(this);
        }
        protected override void CreateModel()
        {
            Vector2 pivotOfRightBottom = new Vector2(0, 0);
            Vector2 sizeOfRightBottom = new Vector2(120, 50);
            Vector2 directionOfRightBottom = new Vector2(-sizeOfRightBottom.x, 0);
            Vector2 pivotOfTopLeft = Vector2.zero;
            Vector2 sizeOfTopLeft = new Vector2(100, 50);
            Vector2 directionOfTopLeft = new Vector2(0, -sizeOfTopLeft.y);
            Vector2 pivotOfRightTop = new Vector2(0, 1);
            Vector2 sizeOfRightTop = new Vector2(80, 50);
            Vector2 directionOfRightTop = new Vector2(-sizeOfRightBottom.x, -sizeOfTopLeft.y);

            enableBackgroudImage = false;
            enableTitileBar = false;
            WinSizeMaxOffset = new Vector2(sizeOfRightBottom.x, sizeOfTopLeft.y);
            defaultMinAnchor = minAnchor;
            defaultMaxAnchor = maxAnchor;
            base.CreateModel();

            //Backgroud Image
            Image bg = ViewTool.ForceGetComponentInChildren<Image>(gameObject, "Backgroud");
            ViewTool.FullFillRectTransform(bg, new Vector2(0, 0), -sizeOfRightBottom);
            //PackListView
            packLV = ViewTool.ForceGetComponentInChildren<DragableResourceLV>(gameObject, "PackListViewLayout", false);
            packLV.SetBackgroudColor(containerColor);
            packLV.GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            packLV.GridConstraintCount = 8;
            packLV.ScrollDirection = ScrollType.Vertical;
            packLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            ViewTool.FullFillRectTransform(packLV, new Vector2(20, 20), new Vector2(-20, -20) - sizeOfRightBottom);
            packLV.gameObject.SetActive(true);
            //Buttons
            RectTransform btns = new GameObject("Btns", typeof(RectTransform)).GetComponent<RectTransform>();
            ViewTool.SetParent(btns, this);
            ViewTool.FullFillRectTransform(btns, Vector2.zero, Vector2.zero);
            rightTopBtns = new Button[rightUpBtnsName.Length];
            for (int i = 0; i < rightUpBtnsName.Length; i++)
            {
                rightTopBtns[i] = ViewTool.CreateBtn(rightUpBtnsName[i], rightUpBtnsContent[i], btns);
                ViewTool.RightTop(rightTopBtns[i], pivotOfRightTop, sizeOfRightTop, directionOfRightTop);
                directionOfRightTop.y -= sizeOfRightTop.y;
            }
            rightTopBtns[0].onClick.AddListener(delegate () { OnRightTopBtnClick(0); });
            rightTopBtns[1].onClick.AddListener(delegate () { OnRightTopBtnClick(1); });
            rightTopBtns[2].onClick.AddListener(delegate () { OnRightTopBtnClick(2); });
            rightTopBtns[3].onClick.AddListener(delegate () { OnRightTopBtnClick(3); });
            rightBottomBtns = new Button[rightBottomBtnsName.Length];
            for (int i = 0; i < rightBottomBtnsName.Length; i++)
            {
                rightBottomBtns[i] = ViewTool.CreateBtn(rightBottomBtnsName[i], rightBottomBtnsContent[i], btns);
                int index = i;
                rightBottomBtns[i].onClick.AddListener(delegate () { OnRightBottomBtnClick(index); });
                ViewTool.RightBottom(rightBottomBtns[i], pivotOfRightBottom, sizeOfRightBottom, directionOfRightBottom);
                directionOfRightBottom.y += sizeOfRightBottom.y;
            }
            topLeftBtns = new Button[topLeftBtnsName.Length];
            for (int i = 0; i < topLeftBtnsName.Length; i++)
            {
                topLeftBtns[i] = ViewTool.CreateBtn(topLeftBtnsName[i], topLeftBtnsContent[i], btns);
                ViewTool.LeftTop(topLeftBtns[i], pivotOfTopLeft, sizeOfTopLeft, directionOfTopLeft);
                directionOfTopLeft.x += sizeOfTopLeft.x;
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
                    packLV.onItemFilter = (ItemData item) => { return item.Type != PublicData.ItemType.Weapon; };
                    break;
                case 2:
                    packLV.onItemFilter = (ItemData item) => { return item.Type != PublicData.ItemType.Material; };
                    break;
                case 3:
                    packLV.onItemFilter = (ItemData item) => { return item.Type != PublicData.ItemType.SpecialItem; };
                    break;
            }
            packLV.Refresh();
        }
        public void OnRightBottomBtnClick(int index)
        {
            switch (index)
            {
                //托此丢弃
                case 0:
                    break;
                //整理物品
                case 1:
                    World.getInstance().storage.SortStorage();
                    break;
            }
        }
        public void OnTopLeftBtnClick(int index)
        {
            Hide();
        }
        public void ObsUpdate(int state, int echo, object tag = null)
        {
            switch ((Storage.EAction)state)
            {
                case Storage.EAction.ADD_ITEM:
                case Storage.EAction.REMOVE_ITEM:
                case Storage.EAction.SORT_ITEM:UpdatePack(); break;
            }
        }
        protected override bool PrepareDataBeforeShowWindow()
        {
            items = World.getInstance().storage.CloneStorage();
            packLV.SetData(items);
            return true;
        }
        protected override void AfterShowWindow()
        {
            packLV.Refresh();
        }

        public void UpdatePack()
        {
            items = World.getInstance().storage.CloneStorage();
            packLV.Datas = items;
        }
#if DEBUG
        public void AddRandomMaterial()
        {
            World.getInstance().storage.AddItem(ItemData.RandomMaterial());
        }
        public void AddRandomWeapon()
        {
            World.getInstance().storage.AddItem(ItemData.RandomWeapon());
        }
#endif
    }
}