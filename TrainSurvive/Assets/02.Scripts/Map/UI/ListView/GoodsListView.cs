/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 4:06:31
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using WorldMap.Model;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class GoodsListView : MergableListView<Good>
    {
        public delegate void CallBackGoodsBuy(Good good);
        public CallBackGoodsBuy callBackGoodsAction;
        public string ActionBtnString { set; get; } = "购买";
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void OnItemView(ListViewItem item, Good data)
        {
            GoodsItem view = Utility.ForceGetComponent<GoodsItem>(item);
            ActionClickEvent buyClickEvent = new ActionClickEvent(data);
            buyClickEvent.callBackGoodsAction = callBackGoodsAction;
            view.SetActionBtnContent(ActionBtnString);
            view.BindActionBtnEvent(buyClickEvent);
            view.SetTarget(data.item);
            view.SetNumber(data.Number);
            view.SetPrice(data.Price);
        }
        public class ActionClickEvent : Button.ButtonClickedEvent
        {
            public Good goods;
            public CallBackGoodsBuy callBackGoodsAction;
            public ActionClickEvent(Good goods)
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