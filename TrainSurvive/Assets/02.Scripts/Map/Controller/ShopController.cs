/*
 * 描述：商店界面控制类
 * 作者：项叶盛
 * 创建时间：2018/12/1 12:30:05
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using WorldMap.UI;
using WorldMap.Model;

namespace WorldMap.Controller
{
    public class ShopController : WindowsController
    {
        private string[] btnStrs = { "退出" };
        private GoodsListView goodsInShopLV;
        private GoodsListView goodsInPackLV;
        private Model.Town currentTown;
        protected override void CreateModel()
        {
            base.CreateModel();
            WinSizeType = WindowSizeType.BIG;
            //ListView
            goodsInShopLV = new GameObject("GoodsInShopLayout").AddComponent<GoodsListView>();
            ConfigListView(goodsInShopLV, 0.25F);
            
            goodsInShopLV.callBackGoodsAction = CallBackGoodsBuy;
            goodsInPackLV = new GameObject("GoodsInPackLayout").AddComponent<GoodsListView>();
            ConfigListView(goodsInPackLV, 0.75F);
            goodsInPackLV.ActionBtnString = "售卖";
            goodsInPackLV.callBackGoodsAction = CallBackGoodsSell;
            //Buttons
            RectTransform btns = new GameObject("Btns").AddComponent<RectTransform>();
            Utility.SetParent(btns, this);
            Utility.Anchor(btns, Vector2.zero, new Vector2(1F, 0.2F));

            Vector2 btnSize = new Vector2(120, 80);
            Button cancel = Utility.CreateBtn("Cancel", btnStrs[0]);
            Utility.SetParent(cancel, btns);
            Utility.CenterAt(cancel, new Vector2(0.5F, 0.5F), btnSize);
            cancel.onClick.AddListener(delegate () { HideWindow(); });
        }
        private void ConfigListView(GoodsListView listView, float xAnchor)
        {
            listView.SetCellSize(new Vector2(500F, 100F));
            listView.ScrollDirection = ScrollType.Vertical;
            listView.m_selectable = false;
            listView.StartAxis = GridLayoutGroup.Axis.Horizontal;
            Utility.SetParent(listView, this);
            Utility.VLineAt(comp: listView, anchor: xAnchor, top: 0.95F, bottom: 0.2F, width: 502F);
        }
        protected override bool PrepareDataBeforeShowWindow()
        {
            return true;
        }

        protected override void AfterShowWindow()
        {
            goodsInShopLV.Datas = new List<Good>(currentTown.Goods);
            if(world.IfTeamOuting)
                goodsInPackLV.Datas = new List<Good>(world.GetGoodsInTeam());
            else
                goodsInPackLV.Datas = new List<Good>(world.GetGoodsInTrain());
        }
        public void SetTown(Model.Town town)
        {
            currentTown = town;
        }
        public void CallBackGoodsBuy(Good good)
        {
            //FOR TEST全部买下
            int numberBuy = good.Number;
            if (!world.Pay(good.Price * numberBuy))
            {
                Debug.Log("商店：金额不足，滚");
                return;
            }
            if(numberBuy > good.Number)
            {
                Debug.Log("商店：物品数量不足，另寻他处");
                return;
            }
            if (world.IfTeamOuting)
            {
                InventoryForTeam inventoryForTeam = Team.Instance.Inventory;
                if (!inventoryForTeam.CanPushItemToPack(good, numberBuy))
                {
                    Debug.Log("系统：购买物品失败");
                    return;
                }
                inventoryForTeam.PushItemFromShop(good, numberBuy);
            }
            else
            {
                if (!world.PushItemToTrain(good, numberBuy))
                {
                    Debug.Log("列车：仓库已满");
                    return;
                }
            }
            //全部买下，不应该失败
            if (!currentTown.BuyGoods(good, good.Number))
            {
                Debug.LogError("系统：物品购买失败");
            }
            if (!goodsInShopLV.RemoveItem(good))
            {
                Debug.LogError("系统：列表刷新失败");
            }
            goodsInPackLV.AddItem(good);
        }
        public void CallBackGoodsSell(Good good)
        {
            
        }
        protected override bool FocusBehaviour()
        {
            return true;
        }

        protected override void UnfocusBehaviour()
        {

        }
    }
}