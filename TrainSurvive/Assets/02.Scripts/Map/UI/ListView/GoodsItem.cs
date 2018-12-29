/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/2 20:54:40
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using TTT.Utility;
using UnityEngine.EventSystems;
using System.Collections;
using WorldMap.Model;

namespace WorldMap.UI
{
    public class GoodsItem : ResourceItemBase
    {
        private Text numView;
        private Text priceView;
        //按钮可以为购买，或者是售卖。
        private Button actionBtn;
        private Transform itemView;
        private Vector2 defaultSize = new Vector2(500F, 200F);
        protected override void CreateModel()
        {
            base.CreateModel();
            numView = ViewTool.CreateText("Number");
            priceView = ViewTool.CreateText("Price");
            actionBtn = ViewTool.CreateBtn("Action", "", transform);
            itemView = new GameObject("Item", typeof(RectTransform)).transform;
        }
        protected override void InitModel()
        {
            ViewTool.SetParent(backgroudImage, itemView);
            ViewTool.SetParent(targetImage, itemView);
            ViewTool.SetParent(markImage, itemView);
            ViewTool.SetParent(itemView, this);
            ViewTool.SetParent(numView, this);
            ViewTool.SetParent(priceView, this);
            ViewTool.SetParent(actionBtn, this);
        }
        protected override void PlaceModel()
        {
            base.PlaceModel();
            ViewTool.CenterAt(itemView, new Vector2(0.5f, 0.5f),new Vector2(0.2F, 0.5F), new Vector2(100F, 100F));
            ViewTool.CenterAt(priceView, new Vector2(0.5f, 0.5f), new Vector2(0.4F, 0.5F), new Vector2(80F, 40F));
            ViewTool.CenterAt(numView, new Vector2(0.5f, 0.5f), new Vector2(0.6F, 0.5F), new Vector2(80F, 40F));
            ViewTool.CenterAt(actionBtn, new Vector2(0.5f, 0.5f), new Vector2(0.8F, 0.5F), new Vector2(90F, 100F));
            ViewTool.CenterAt(this, new Vector2(0.5f, 0.5f), new Vector2(0.5F, 0.5F), defaultSize);
        }
        public void SetNumber(int num)
        {
            numView.text = "x" + num.ToString();
        }
        public void SetPrice(int price)
        {
            priceView.text = "$" + price.ToString();
        }
        public void BindActionBtnEvent(Button.ButtonClickedEvent onclick)
        {
            actionBtn.onClick = onclick;
        }
        public void SetActionBtnContent(string content)
        {
            ViewTool.SetBtnContent(actionBtn, content);
        }
    }
}