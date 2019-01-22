/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 4:06:31
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;
using TTT.UI;
using TTT.UI.ListView;

namespace WorldMap.UI
{
    public class GoodsListView : ResourceListView
    {
        public delegate void CallBackGoodsBuy(ItemData good);
        public CallBackGoodsBuy callBackGoodsAction;
        public string ActionBtnString { set; get; } = "购买";
        protected override ResourceItemBase GetView(ListViewItem item, ItemData data, int index)
        { 
            GoodsItem view = CompTool.ForceGetComponent<GoodsItem>(item);
            ActionClickEvent buyClickEvent = new ActionClickEvent(data);
            buyClickEvent.callBackGoodsAction = callBackGoodsAction;
            view.SetActionBtnContent(ActionBtnString);
            view.BindActionBtnEvent(buyClickEvent);
            view.SetItemID(data.ID);
            view.SetNumber(data.Number);
            view.SetPrice(data.SellPrice);
            return view;
        }
        
        /// <summary>
        /// 商店物品条款中的点击按钮
        /// </summary>
        public class ActionClickEvent : Button.ButtonClickedEvent
        {
            public ItemData goods;
            public CallBackGoodsBuy callBackGoodsAction;
            public ActionClickEvent()
            {

            }
            public ActionClickEvent(ItemData goods)
            {
                this.goods = goods;
                AddListener(OnBtnClick);
            }
            private void OnBtnClick()
            {
                callBackGoodsAction?.Invoke(goods);
            }
        }
    }
}