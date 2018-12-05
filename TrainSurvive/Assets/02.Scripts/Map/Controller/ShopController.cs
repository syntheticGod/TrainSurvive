/*
 * 描述：商店界面控制类
 * 作者：项叶盛
 * 创建时间：2018/12/1 12:30:05
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections.Generic;
using WorldMap.UI;
using WorldMap.Model;
using UnityEngine.UI;

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
            goodsInPackLV.Datas = new List<Good>();
            foreach(Item item in Team.Instance.Inventory.items){

            }
        }
        public void SetTown(Model.Town town)
        {
            currentTown = town;
        }
        public void CallBackGoodsBuy(Good good)
        {
            if (!world.Pay(good.Price))
            {
                Debug.Log("商店：金额不足，滚");
                return;
            }
            if (world.IfTeamOuting)
            {
                int originNumber = good.Number;
                int remain = Team.Instance.Inventory.PushItem(good.item);
                if (originNumber == remain)
                {
                    Debug.Log("探险队：我的背包已满");
                    return;
                }
                else if(remain > 0)
                {
                    Debug.Log("探险队：我的背包快满，只能购买了一部分 数量：" + (originNumber - remain));
                    //买下部分
                    if (!currentTown.BuyGoods(good, originNumber- remain))
                    {
                        Debug.Log("系统：物品购买失败");
                    }
                    good.item.currPileNum = remain;
                    return;
                }
            }
            else
            {
                if (!world.PushItemToTrain(good.item))
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