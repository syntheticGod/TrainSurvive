/*
 * 描述：商店界面控制类
 * 作者：项叶盛
 * 创建时间：2018/12/1 12:30:05
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TTT.Utility;
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
            m_titleString = "商店";
            m_windowSizeType = EWindowSizeType.MIDDLE14x12;
            base.CreateModel();
            SetBackground("tavern_bg_01");
            //ListView
            goodsInShopLV = new GameObject("GoodsInShopLayout").AddComponent<GoodsListView>();
            goodsInShopLV.ActionBtnString = "购买";
            goodsInShopLV.callBackGoodsAction = CallBackGoodsBuy;
            ConfigListView(goodsInShopLV, 0.25F);
            ViewTool.Anchor(goodsInShopLV, new Vector2(0.0417F, 0.2F), new Vector2(0.3481F, 0.8F));

            goodsInPackLV = new GameObject("GoodsInPackLayout").AddComponent<GoodsListView>();
            goodsInPackLV.ActionBtnString = "售卖";
            goodsInPackLV.callBackGoodsAction = CallBackGoodsSell;
            ConfigListView(goodsInPackLV, 0.75F);
            ViewTool.Anchor(goodsInPackLV, new Vector2(0.6522F, 0.2F), new Vector2(0.9572F, 0.8F));

            //Buttons
            RectTransform btns = new GameObject("Btns").AddComponent<RectTransform>();
            ViewTool.SetParent(btns, this);
            ViewTool.Anchor(btns, Vector2.zero, new Vector2(1F, 0.2F));

            Vector2 btnSize = new Vector2(120, 80);
            Button cancel = ViewTool.CreateBtn("Cancel", btnStrs[0]);
            ViewTool.SetParent(cancel, btns);
            ViewTool.CenterAt(cancel, new Vector2(0.5F, 0.5F), btnSize);
            cancel.onClick.AddListener(delegate () { HideWindow(); });
        }
        private void ConfigListView(GoodsListView listView, float xAnchor)
        {
            listView.SetCellSize(new Vector2(-1, 100F));
            listView.ScrollDirection = ScrollType.Vertical;
            listView.IfSelectable = false;
            listView.StartAxis = GridLayoutGroup.Axis.Vertical;
            ViewTool.SetParent(listView, this);
        }
        protected override bool PrepareDataBeforeShowWindow()
        {
            return true;
        }

        protected override void AfterShowWindow()
        {
            goodsInShopLV.Datas = new List<Good>(currentTown.Goods);
            if (world.IfTeamOuting)
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
            //TODO：弹出选择窗口
            int numberBuy = good.Number;
            if (!world.Pay(good.Price * numberBuy))
            {
                InfoDialog.Show("你的金额不足");
                return;
            }
            if (numberBuy > good.Number)
            {
                InfoDialog.Show("物品数量不足，另寻他处");
                return;
            }
            Good goodInPack = good.Clone();
            goodInPack.SetNumber(numberBuy);
            if (world.IfTeamOuting)
            {
                InventoryForTeam inventoryForTeam = Team.Instance.Inventory;
                if (!inventoryForTeam.CanPushItemToPack(goodInPack))
                {
                    InfoDialog.Show("背包已满");
                    return;
                }
                inventoryForTeam.PushItemFromShop(goodInPack);
            }
            else
            {
                //TODO：需要列车中的仓库是否满
                world.PushItemToTrain(good.item.id, numberBuy);
            }
            if (!currentTown.BuyGoods(good, numberBuy))
            {
                Debug.LogError("系统：物品购买失败");
            }
            Debug.Log("商店：你成功购买了" + goodInPack.Name + " 花费：" + good.Price * numberBuy + " 剩余：" + world.Money);
            //ListView会自动清楚数量为0的条款
            goodsInShopLV.Refresh();
            goodsInPackLV.AddItem(goodInPack);
        }
        public void CallBackGoodsSell(Good good)
        {
            //TODO：弹出选择窗口
            int numberSell = good.Number;
            if (numberSell > good.Number)
            {
                InfoDialog.Show("库存数量不足");
                return;
            }
            Good goodsInTown = good.Clone();
            goodsInTown.SetNumber(numberSell);
            currentTown.SellGoods(goodsInTown);
            if (world.IfTeamOuting)
            {
                world.SellGoodsFromTeam(good.ItemID, numberSell);
            }
            else
            {
                world.SellGoodsFromTrain(good.ItemID, numberSell);
            }
            world.AddMoney(numberSell * good.Price);
            good.DecreaseNumber(numberSell);
            Debug.Log("商店：你出售了" + numberSell + "个" + good.Name + " 剩余：" + good.Number + " 获得金币：" + numberSell * good.Price + " 现有金币：" + world.Money);
            //ListView会自动清楚数量为0的条款
            goodsInPackLV.Refresh();
            goodsInShopLV.AddItem(goodsInTown);
        }
        protected override bool FocusBehaviour()
        {
            return true;
        }

    }
}