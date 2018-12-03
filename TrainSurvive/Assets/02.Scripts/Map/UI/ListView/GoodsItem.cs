/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/2 20:54:40
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class GoodsItem : ItemBase
    {
        private Text numView;
        private Text priceView;
        private Button buyBtn;
        private Transform itemView;
        private float defualtWidth = 500F;
        private float defaultHeight = 200F;
        void Start()
        {
            //For Test
            //MarkLevel(2);
            //SetTargetByName("Weapon_img");
            //SetNumber(1000);
            //SetPrice(99999);
        }
        protected override void CreateModel()
        {
            base.CreateModel();
            numView = CreateText("Number");
            priceView = CreateText("Price");
            buyBtn = CreateBtn("Buy", "购买");
            itemView = new GameObject("Item", typeof(RectTransform)).transform;
        }
        protected override void InitModel()
        {
            SetParent(backgroudImage, itemView);
            SetParent(targetImage, itemView);
            SetParent(markImage, itemView);
            SetParent(itemView, this);
            SetParent(numView, this);
            SetParent(priceView, this);
            SetParent(buyBtn, this);
        }
        protected override void PlaceModel()
        {
            base.PlaceModel();
            SetAnchor(itemView, 0.2F, 0.5F, 0.2F, 0.5F);
            SetAnchor(priceView, 0.4F, 0.5F, 0.4F, 0.5F);
            SetAnchor(numView, 0.6F, 0.5F, 0.6F, 0.5F);
            SetAnchor(buyBtn, 0.8F, 0.5F, 0.8F, 0.5F);
            SetAnchor(this, 0.5F, 0.5F, 0.5F, 0.5F);
            SetSize(itemView, 100F, 100F);
            SetSize(priceView, 80F, 40F);
            SetSize(numView, 80F, 40F);
            SetSize(buyBtn, 90F, 40F);
            SetSize(this, defualtWidth, defaultHeight);
        }
        public void SetNumber(int num)
        {
            numView.text = "x" + num.ToString();
        }
        public void SetPrice(int price)
        {
            priceView.text = "$" + price.ToString();
        }
    }
}