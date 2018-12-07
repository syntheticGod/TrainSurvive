/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/1 1:35:30
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using WorldMap.UI;

namespace WorldMap.Controller
{
    public class SchoolController : WindowsController
    {
        private string[] attributeInfoStrs = new string[] { "体力", "力量", "敏捷", "技巧", "智力" };
        private string moneyInfoStr = "所需金额：";
        private string payBtnStr = "训练";
        private string cancelBtnStr = "退出";
        //属性视图
        private uint[] deltaAttri;
        private Text[] attriViews;
        //花费金额视图
        private Text moneyView;
        //英雄显示列表
        private HeroListView herosLayout;
        //被选中的英雄
        private Person heroChoosed;
        private int[] heroAttribute;
        private int cost;
        protected override void CreateModel()
        {
            base.CreateModel();
            WinSizeType = WindowSizeType.FULL;
            int attriCount = attributeInfoStrs.Length;
            attriViews = new Text[attriCount];
            deltaAttri = new uint[attriCount];
            heroAttribute = new int[attriCount];
            //属性窗口
            RectTransform attributes = new GameObject("Attributes", typeof(RectTransform)).GetComponent<RectTransform>();
            Utility.SetParent(attributes, this);
            Utility.CenterAt(attributes, anchor:new Vector2(0.2F, 0.4F), size:new Vector2(300F, 300F));
            for (int i = 0; i < attributeInfoStrs.Length; i++)
            {
                RectTransform attribute = CreateAttribute(i);
                Utility.SetParent(attribute, attributes);
                attribute.anchorMin = new Vector2(0, 1 - (i + 1) / (float)attriCount);
                attribute.anchorMax = new Vector2(1, 1 - i / (float)attriCount);
                attribute.offsetMin = Vector2.zero;
                attribute.offsetMax = Vector2.zero;
            }
            //结账区块
            RectTransform payRect = new GameObject("Pay").AddComponent<RectTransform>();
            Utility.SetParent(payRect, this);
            Utility.CenterAt(payRect, anchor:new Vector2(0.6F, 0.4F), size:new Vector2(380F, 100F), vector:new Vector2(0F, -100F));

            Text moneyInfo = Utility.CreateText("MoneyInfo");
            Utility.SetParent(moneyInfo, payRect);
            Utility.Anchor(moneyInfo, Vector2.zero, new Vector2(0.3F, 1.0F));
            moneyInfo.text = moneyInfoStr;

            moneyView = Utility.CreateText("Money");
            Utility.SetParent(moneyView, payRect);
            Utility.Anchor(moneyView, new Vector2(0.3F, 0.0F), new Vector2(0.6F, 1.0F));

            Button payBtn = Utility.CreateBtn("PayBtn", payBtnStr);
            Utility.SetParent(payBtn, payRect);
            Utility.Anchor(payBtn, new Vector2(0.6F, 0.0F), new Vector2(0.8F, 1.0F));
            payBtn.onClick.AddListener(delegate () { OnOKBtnClick(); });

            Button cancelBtn = Utility.CreateBtn("CancelBtn", cancelBtnStr);
            Utility.SetParent(cancelBtn, payRect);
            Utility.Anchor(cancelBtn, new Vector2(0.8F, 0.0F), new Vector2(1.0F, 1.0F));
            cancelBtn.onClick.AddListener(delegate () { OnCancelBtnClick(); });
            //注意：不能对ListView进行添加删除行为
            herosLayout = Utility.ForceGetComponentInChildren<HeroListView>(this, "HerosLayout");
            herosLayout.StartAxis = GridLayoutGroup.Axis.Horizontal;
            RectTransform herosLayoutRect = herosLayout.GetComponent<RectTransform>();
            Utility.HLineAt(herosLayoutRect, anchor:0.8F, height:100F);
            herosLayout.onItemClick = delegate (ListViewItem item, Person person)
            {
                heroChoosed = person;
                InitAttribute();
                ShowAttributes();
                ShowMoney();
            };

        }
        private RectTransform CreateAttribute(int index)
        {
            RectTransform attribute = new GameObject("abi" + index).AddComponent<RectTransform>();

            //属性名
            Text attriInfoView = Utility.CreateText("Info");
            Utility.SetParent(attriInfoView, attribute);
            RectTransform attriInfo = attriInfoView.GetComponent<RectTransform>();
            attriInfo.anchorMin = Vector2.zero;
            attriInfo.anchorMax = new Vector2(0.4F, 1F);
            attriInfoView.text = attributeInfoStrs[index];

            //数字
            attriViews[index] = Utility.CreateText("abi" + index);
            Utility.SetParent(attriViews[index], attribute);
            RectTransform attriNum = attriViews[index].GetComponent<RectTransform>();
            attriNum.anchorMin = new Vector2(0.4F, 0);
            attriNum.anchorMax = new Vector2(0.6F, 1F);

            //减号按钮
            Vector2 btnSize = new Vector2(50F, 50F);
            Button minus = Utility.CreateBtn("Minus", "-");
            minus.GetComponentInChildren<Text>().fontSize = 30;
            Utility.SetParent(minus, attribute);
            Utility.RightCenter(minus, new Vector2(1F, 0.5F), btnSize, new Vector2(-btnSize.x, 0));
            minus.onClick.AddListener(delegate () { OnAttributeMinusBtnClick(index); });
            //加号按钮
            Button plus = Utility.CreateBtn("Plus", "+");
            plus.GetComponentInChildren<Text>().fontSize = 30;
            Utility.SetParent(plus, attribute);
            Utility.RightCenter(plus, new Vector2(1F, 0.5F), btnSize, new Vector2(0, 0));
            plus.onClick.AddListener(delegate () { OnAttributePlusBtnClick(index); });
            return attribute;
        }
        protected override void Start()
        {
            base.Start();
        }

        protected override bool PrepareDataBeforeShowWindow()
        {
            return true;
        }

        protected override void AfterShowWindow()
        {
            herosLayout.Datas = world.GetHeros();
            if (herosLayout.Datas.Count != 0)
                herosLayout.ClickManually(0);
            ShowAttributes();
        }

        protected override bool FocusBehaviour()
        {
            return true;
        }

        protected override void UnfocusBehaviour()
        {

        }
        public void Show()
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
        }
        private void Hide()
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 计算所需金钱，并显示
        /// </summary>
        //TODO：有细节问题，训练次数是每点一次属性加一，还是点一次训练加一。
        private void ShowMoney()
        {
            float money = 0F;
            for (int i = 0; i < attributeInfoStrs.Length; i++)
            {
                money += deltaAttri[i] * 1000F * (1 + heroAttribute[i] * 0.05F) * (1 + heroChoosed.trainCnt * 0.05F);
            }
            cost = (int)money;
            moneyView.text = cost.ToString();
        }
        /// <summary>
        /// 将加完之后的所有属性显示在属性板上
        /// </summary>
        private void ShowAttributes()
        {
            for (int i = 0; i < attributeInfoStrs.Length; i++)
            {
                attriViews[i].text = (heroAttribute[i] + deltaAttri[i]).ToString();
            }
        }
        /// <summary>
        /// 初始化数值变量
        /// 1、在初始化属性板时调用
        /// 2、在成功训练后调用
        /// </summary>
        private void InitAttribute()
        {
            heroAttribute[0] = heroChoosed.vitality;
            heroAttribute[1] = heroChoosed.strength;
            heroAttribute[2] = heroChoosed.agile;
            heroAttribute[3] = heroChoosed.technique;
            heroAttribute[4] = heroChoosed.intelligence;
            for (int i = 0; i < attributeInfoStrs.Length; i++)
            {
                deltaAttri[i] = 0;
            }
        }
        /// <summary>
        /// 减属性响应函数
        /// </summary>
        /// <param name="index"></param>
        public void OnAttributeMinusBtnClick(int index)
        {
            if (deltaAttri[index] != 0)
                deltaAttri[index]--;
            attriViews[index].text = (heroAttribute[index] + deltaAttri[index]).ToString();
            ShowMoney();
        }
        /// <summary>
        /// 加属性响应函数
        /// </summary>
        /// <param name="index"></param>
        public void OnAttributePlusBtnClick(int index)
        {
            deltaAttri[index]++;
            attriViews[index].text = (heroAttribute[index] + deltaAttri[index]).ToString();
            ShowMoney();
        }
        /// <summary>
        /// 训练按钮响应函数
        /// </summary>
        public void OnOKBtnClick()
        {
            //TODO：检查金额是否足够
            heroChoosed.vitality += (int)deltaAttri[0];
            heroChoosed.strength += (int)deltaAttri[1];
            heroChoosed.agile += (int)deltaAttri[2];
            heroChoosed.technique += (int)deltaAttri[3];
            heroChoosed.intelligence += (int)deltaAttri[4];
            InitAttribute();
            ShowMoney();
        }
        public void OnCancelBtnClick()
        {
            Hide();
        }

    }
}