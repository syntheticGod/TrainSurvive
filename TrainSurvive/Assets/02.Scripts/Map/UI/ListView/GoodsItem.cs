/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/2 20:54:40
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

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
            numView = Utility.CreateText("Number");
            priceView = Utility.CreateText("Price");
            actionBtn = Utility.CreateBtn("Action", "", transform);
            itemView = new GameObject("Item", typeof(RectTransform)).transform;
        }
        protected override void InitModel()
        {
            Utility.SetParent(backgroudImage, itemView);
            Utility.SetParent(targetImage, itemView);
            Utility.SetParent(markImage, itemView);
            Utility.SetParent(itemView, this);
            Utility.SetParent(numView, this);
            Utility.SetParent(priceView, this);
            Utility.SetParent(actionBtn, this);
        }
        protected override void PlaceModel()
        {
            base.PlaceModel();
            Utility.CenterAt(itemView, new Vector2(0.2F, 0.5F), new Vector2(100F, 100F));
            Utility.CenterAt(priceView, new Vector2(0.4F, 0.5F), new Vector2(80F, 40F));
            Utility.CenterAt(numView, new Vector2(0.6F, 0.5F), new Vector2(80F, 40F));
            Utility.CenterAt(actionBtn, new Vector2(0.8F, 0.5F), new Vector2(90F, 40F));
            Utility.CenterAt(this, new Vector2(0.5F, 0.5F), defaultSize);
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
            Utility.SetBtnContent(actionBtn, content);
        }
    }
}