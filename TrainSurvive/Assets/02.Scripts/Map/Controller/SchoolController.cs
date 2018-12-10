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
    public class SchoolController : WindowsController, DialogCallBack
    {
        private string[] attributeInfoStrs = new string[] { "体力", "力量", "敏捷", "技巧", "智力" };
        private string moneyInfoStr = "所需金钱：";
        private string payBtnStr = "确定";
        private string resetBtnStr = "重置";
        //属性视图
        private uint[] deltaAttri;
        private Text[] attriViews;
        private Text[] attriViewsNew;
        //花费金额视图
        private Text moneyView;
        //被选中的英雄
        private Person heroChoosed;
        private PersonBaseItem heroProfile;
        private Text heroInfoContent;
        private int[] heroAttribute;
        private int cost;
        protected override void CreateModel()
        {
            m_windowSizeType = EWindowSizeType.MIDDLE14x12;
            m_titleString = "学校";
            base.CreateModel();
            SetBackground("tavern_bg_01");
            int attriCount = attributeInfoStrs.Length;
            attriViews = new Text[attriCount];
            attriViewsNew = new Text[attriCount];
            deltaAttri = new uint[attriCount];
            heroAttribute = new int[attriCount];
            //英雄信息窗口
            RectTransform heroInfoBorad = new GameObject("HeroInfoBorad", typeof(Image)).GetComponent<RectTransform>();
            Utility.SetParent(heroInfoBorad, this);
            Utility.Anchor(heroInfoBorad, new Vector2(0.083F, 0.1F), new Vector2(0.625F, 0.9F));
            //英雄信息条
            RectTransform heroInfoLayout = new GameObject("HeroInfoLayout").AddComponent<RectTransform>();
            Utility.SetParent(heroInfoLayout, heroInfoBorad);
            Utility.Anchor(heroInfoLayout, new Vector2(0F, 0.815F), new Vector2(1F, 1F));
            //英雄头像框
            heroProfile = new GameObject("HeroProfile", typeof(RectTransform)).AddComponent<PersonBaseItem>();
            Utility.SetParent(heroProfile, heroInfoLayout);
            Utility.LeftTop(heroProfile, new Vector2(0, 1), new Vector2(100F, 100F));
            heroProfile.gameObject.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                HeroSelectDialog dialog = BaseDialog.CreateDialog<HeroSelectDialog>("HeroSelectDialog");
                dialog.DialogCallBack = this;
                dialog.ShowDialog();
            });
            //英雄简介
            heroInfoContent = Utility.CreateText("HeroInfoContent");
            heroInfoContent.alignment = TextAnchor.MiddleLeft;
            Utility.SetParent(heroInfoContent, heroInfoLayout);
            Utility.FullFillRectTransform(heroInfoContent, new Vector2(100F, 0), Vector2.zero);
            //属性
            RectTransform attributes = new GameObject("Attributes").AddComponent<RectTransform>();
            Utility.SetParent(attributes, heroInfoBorad);
            Utility.Anchor(attributes, new Vector2(0F, 0F), new Vector2(1F, 0.815F));
            float delta = 1F / attributeInfoStrs.Length;
            for (int i = 0; i < attributeInfoStrs.Length; i++)
            {
                RectTransform attribute = CreateAttribute(i);
                Utility.SetParent(attribute, attributes);
                float tempDelta = delta * i;
                attribute.anchorMin = new Vector2(0, tempDelta);
                attribute.anchorMax = new Vector2(1, tempDelta + delta);
                attribute.offsetMin = Vector2.zero;
                attribute.offsetMax = Vector2.zero;
            }
            //结账区块
            RectTransform payRect = new GameObject("Pay", typeof(Image)).GetComponent<RectTransform>();
            Utility.SetParent(payRect, heroInfoBorad);
            Utility.Anchor(payRect, new Vector2(0F, 0F), new Vector2(1F, 0.125F));

            Text moneyInfo = Utility.CreateText("MoneyInfo");
            Utility.SetParent(moneyInfo, payRect);
            Utility.Anchor(moneyInfo, Vector2.zero, new Vector2(0.4F, 1.0F));
            moneyInfo.text = moneyInfoStr;

            moneyView = Utility.CreateText("Money");
            Utility.SetParent(moneyView, payRect);
            Utility.Anchor(moneyView, new Vector2(0.4F, 0.0F), new Vector2(0.6F, 1.0F));

            Button payBtn = Utility.CreateBtn("PayBtn", payBtnStr);
            Utility.SetParent(payBtn, payRect);
            Utility.Anchor(payBtn, new Vector2(0.6F, 0.0F), new Vector2(0.8F, 1.0F));
            payBtn.onClick.AddListener(delegate () { OnOKBtnClick(); });

            Button resetBtn = Utility.CreateBtn("ResetBtn", resetBtnStr);
            Utility.SetParent(resetBtn, payRect);
            Utility.Anchor(resetBtn, new Vector2(0.8F, 0.0F), new Vector2(1.0F, 1.0F));
            resetBtn.onClick.AddListener(delegate () { InitAttribute(); ShowAttributes(); ShowMoney(); });
            //注意：不能对ListView进行添加删除行为
            //herosLayout = Utility.ForceGetComponentInChildren<HeroListView>(this, "HerosLayout");
            //herosLayout.StartAxis = GridLayoutGroup.Axis.Horizontal;
            //herosLayout.GridConstraint = GridLayoutGroup.Constraint.FixedRowCount;
            //herosLayout.GridConstraintCount = 1;
            //RectTransform herosLayoutRect = herosLayout.GetComponent<RectTransform>();
            //Utility.HLineAt(herosLayoutRect, anchor:0.8F, height:100F);
            //herosLayout.onItemClick = delegate (ListViewItem item, Person person)
            //{
            //};

        }
        private RectTransform CreateAttribute(int index)
        {
            RectTransform attribute = new GameObject("abi" + index).AddComponent<RectTransform>();
            //属性名
            Text attriInfoView = Utility.CreateText("Info");
            Utility.SetParent(attriInfoView, attribute);
            RectTransform attriInfo = attriInfoView.GetComponent<RectTransform>();
            attriInfo.anchorMin = Vector2.zero;
            attriInfo.anchorMax = new Vector2(0.2F, 1F);
            attriInfoView.text = attributeInfoStrs[index];
            //数字
            attriViews[index] = Utility.CreateText("abi" + index);
            Utility.SetParent(attriViews[index], attribute);
            Utility.Anchor(attriViews[index], new Vector2(0.2F, 0), new Vector2(0.35F, 1F));
            //数字
            Text arrow = Utility.CreateText("abi" + index, " -> ");
            Utility.SetParent(arrow, attribute);
            Utility.Anchor(arrow, new Vector2(0.35F, 0), new Vector2(0.45F, 1F));
            //数字
            attriViewsNew[index] = Utility.CreateText("abi" + index);
            Utility.SetParent(attriViewsNew[index], attribute);
            Utility.Anchor(attriViewsNew[index], new Vector2(0.45F, 0), new Vector2(0.6F, 1F));
            //加号按钮
            Vector2 btnSize = new Vector2(50F, 50F);
            Button plus = Utility.CreateBtn("Plus", "+");
            plus.GetComponentInChildren<Text>().fontSize = 30;
            Utility.SetParent(plus, attribute);
            Utility.RightCenter(plus, new Vector2(1F, 0.5F), btnSize, new Vector2(-btnSize.x, 0));
            plus.onClick.AddListener(delegate () { OnAttributePlusBtnClick(index); });
            //减号按钮
            Button minus = Utility.CreateBtn("Minus", "-");
            minus.GetComponentInChildren<Text>().fontSize = 30;
            Utility.SetParent(minus, attribute);
            Utility.RightCenter(minus, new Vector2(1F, 0.5F), btnSize, new Vector2(0, 0));
            minus.onClick.AddListener(delegate () { OnAttributeMinusBtnClick(index); });

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
            //herosLayout.Datas = world.GetHeros();
            //if (herosLayout.Datas.Count != 0)
            //    herosLayout.ClickManually(0);
            List<Person> heros = world.GetHeros();
            if (heros.Count > 0)
            {
                ShowHero(heros[0]);
            }
        }
        private void ShowHero(Person person)
        {
            heroProfile.ShowPerson(person);
            heroInfoContent.text = person.name + "，其他简介待填充";
            heroChoosed = person;
            InitAttribute();
            ShowAttributes();
            ShowMoney();
        }

        protected override bool FocusBehaviour()
        {
            return true;
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
            moneyView.text = GetMoney().ToString();
        }
        public int GetMoney()
        {
            float money = 0F;
            for (int i = 0; i < attributeInfoStrs.Length; i++)
            {
                money += deltaAttri[i] * 1000F * (1 + heroAttribute[i] * 0.05F) * (1 + heroChoosed.trainCnt * 0.05F);
            }
            return (int)money;
        }
        /// <summary>
        /// 将加完之后的所有属性显示在属性板上
        /// </summary>
        private void ShowAttributes()
        {
            for (int i = 0; i < attributeInfoStrs.Length; i++)
            {
                attriViews[i].text = heroAttribute[i].ToString();
                attriViewsNew[i].text = (heroAttribute[i] + deltaAttri[i]).ToString();
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
            attriViewsNew[index].text = (heroAttribute[index] + deltaAttri[index]).ToString();
            ShowMoney();
        }
        /// <summary>
        /// 加属性响应函数
        /// </summary>
        /// <param name="index"></param>
        public void OnAttributePlusBtnClick(int index)
        {
            deltaAttri[index]++;
            attriViewsNew[index].text = (heroAttribute[index] + deltaAttri[index]).ToString();
            ShowMoney();
        }
        /// <summary>
        /// 训练按钮响应函数
        /// </summary>
        public void OnOKBtnClick()
        {
            int money = GetMoney();
            if (!world.IfMoneyEnough(money))
            {
                Debug.Log("金钱不足");
                return;
            }
            //TODO：检查金额是否足够
            heroChoosed.vitality += (int)deltaAttri[0];
            heroChoosed.strength += (int)deltaAttri[1];
            heroChoosed.agile += (int)deltaAttri[2];
            heroChoosed.technique += (int)deltaAttri[3];
            heroChoosed.intelligence += (int)deltaAttri[4];
            InitAttribute();
            ShowAttributes();
            ShowMoney();
        }

        public void OK(BaseDialog dialog)
        {
            if (!(dialog is HeroSelectDialog)) return;
            Person person = (dialog as HeroSelectDialog).GetSelectedHero();
            ShowHero(person);
        }

        public void Cancel()
        {
        }
    }
}