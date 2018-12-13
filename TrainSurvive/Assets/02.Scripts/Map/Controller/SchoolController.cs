/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/1 1:35:30
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TTT.Utility;
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
        //专精
        private Text advanceTitle;
        private Image[] professionIcon;
        private Text[] professionInfo;
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
            {
                RectTransform heroInfoBorad = new GameObject("HeroInfoBorad", typeof(Image)).GetComponent<RectTransform>();
                ViewTool.SetParent(heroInfoBorad, this);
                ViewTool.Anchor(heroInfoBorad, new Vector2(0.107F, 0.125F), new Vector2(0.571F, 0.875F));
                //英雄信息条
                {
                    RectTransform heroInfoLayout = new GameObject("HeroInfoLayout").AddComponent<RectTransform>();
                    ViewTool.SetParent(heroInfoLayout, heroInfoBorad);
                    ViewTool.Anchor(heroInfoLayout, new Vector2(0F, 0.815F), new Vector2(1F, 1F));
                    //英雄头像框
                    heroProfile = new GameObject("HeroProfile", typeof(RectTransform)).AddComponent<PersonBaseItem>();
                    ViewTool.SetParent(heroProfile, heroInfoLayout);
                    ViewTool.LeftTop(heroProfile, new Vector2(0, 1), new Vector2(100F, 100F));
                    heroProfile.gameObject.AddComponent<Button>().onClick.AddListener(delegate ()
                    {
                        HeroSelectDialog dialog = BaseDialog.CreateDialog<HeroSelectDialog>("HeroSelectDialog");
                        dialog.DialogCallBack = this;
                        dialog.ShowDialog();
                    });
                    //英雄简介
                    heroInfoContent = ViewTool.CreateText("HeroInfoContent");
                    heroInfoContent.alignment = TextAnchor.MiddleLeft;
                    ViewTool.SetParent(heroInfoContent, heroInfoLayout);
                    ViewTool.FullFillRectTransform(heroInfoContent, new Vector2(100F, 0), Vector2.zero);
                }
                //属性
                {
                    RectTransform attributes = new GameObject("Attributes").AddComponent<RectTransform>();
                    ViewTool.SetParent(attributes, heroInfoBorad);
                    ViewTool.Anchor(attributes, new Vector2(0F, 0F), new Vector2(1F, 0.815F));
                    float delta = 1F / attributeInfoStrs.Length;
                    for (int i = 0; i < attributeInfoStrs.Length; i++)
                    {
                        RectTransform attribute = CreateAttribute(i);
                        ViewTool.SetParent(attribute, attributes);
                        float tempDelta = delta * i;
                        attribute.anchorMin = new Vector2(0, tempDelta);
                        attribute.anchorMax = new Vector2(1, tempDelta + delta);
                        attribute.offsetMin = Vector2.zero;
                        attribute.offsetMax = Vector2.zero;
                    }
                }
                //结账区块
                {
                    RectTransform payRect = new GameObject("Pay", typeof(Image)).GetComponent<RectTransform>();
                    ViewTool.SetParent(payRect, heroInfoBorad);
                    ViewTool.Anchor(payRect, new Vector2(0F, 0F), new Vector2(1F, 0.125F));

                    Text moneyInfo = ViewTool.CreateText("MoneyInfo");
                    ViewTool.SetParent(moneyInfo, payRect);
                    ViewTool.Anchor(moneyInfo, Vector2.zero, new Vector2(0.4F, 1.0F));
                    moneyInfo.text = moneyInfoStr;

                    moneyView = ViewTool.CreateText("Money");
                    ViewTool.SetParent(moneyView, payRect);
                    ViewTool.Anchor(moneyView, new Vector2(0.4F, 0.0F), new Vector2(0.6F, 1.0F));

                    Button payBtn = ViewTool.CreateBtn("PayBtn", payBtnStr);
                    ViewTool.SetParent(payBtn, payRect);
                    ViewTool.Anchor(payBtn, new Vector2(0.6F, 0.0F), new Vector2(0.8F, 1.0F));
                    payBtn.onClick.AddListener(delegate () { OnOKBtnClick(); });

                    Button resetBtn = ViewTool.CreateBtn("ResetBtn", resetBtnStr);
                    ViewTool.SetParent(resetBtn, payRect);
                    ViewTool.Anchor(resetBtn, new Vector2(0.8F, 0.0F), new Vector2(1.0F, 1.0F));
                    resetBtn.onClick.AddListener(delegate () { InitAttribute(); ShowAttributes(); ShowMoney(); });
                }
            }
            //专精
            {
                RectTransform advanceLayout = new GameObject("AdvanceLayout", typeof(Image)).GetComponent<RectTransform>();
                ViewTool.SetParent(advanceLayout, this);
                ViewTool.Anchor(advanceLayout, new Vector2(0.642F, 0.250F), new Vector2(0.893F, 0.875F));
                {
                    //标题
                    advanceTitle = ViewTool.CreateText("AdvanceTitle");
                    ViewTool.SetParent(advanceTitle, advanceLayout);
                    ViewTool.Anchor(advanceTitle, new Vector2(0F, 0.800F), new Vector2(1F, 1F));
                    advanceTitle.text = "选择专精";
                }
                {
                    professionIcon = new Image[attributeInfoStrs.Length];
                    professionInfo = new Text[attributeInfoStrs.Length];
                    float delta = 0.667F / attributeInfoStrs.Length;
                    for(int i = 0; i < attributeInfoStrs.Length; i++)
                    {
                        Vector2 maxAnchor = new Vector2(0.285F, 0.800F - delta * i);
                        Vector2 minAnchor = new Vector2(0F, maxAnchor.y - delta);
                        professionIcon[i] = ViewTool.CreateImage("Icon" + i);
                        ViewTool.SetParent(professionIcon[i], advanceLayout);
                        ViewTool.Anchor(professionIcon[i], minAnchor, maxAnchor);
                        minAnchor.x = maxAnchor.x;
                        maxAnchor.x = 1F;
                        professionInfo[i] = ViewTool.CreateText("Info" + i);
                        professionInfo[i].alignment = TextAnchor.MiddleLeft;
                        ViewTool.SetParent(professionInfo[i], advanceLayout);
                        ViewTool.Anchor(professionInfo[i], minAnchor, maxAnchor);
                    }
                }
                {
                    //确定按钮
                    Button ok = ViewTool.CreateBtn("OK", "确定");
                    ViewTool.SetParent(ok, advanceLayout);
                    ViewTool.Anchor(ok, new Vector2(0F, 0F), new Vector2(1F, 0.133F));
                }
            }
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
            Text attriInfoView = ViewTool.CreateText("Info");
            ViewTool.SetParent(attriInfoView, attribute);
            RectTransform attriInfo = attriInfoView.GetComponent<RectTransform>();
            attriInfo.anchorMin = Vector2.zero;
            attriInfo.anchorMax = new Vector2(0.2F, 1F);
            attriInfoView.text = attributeInfoStrs[index];
            //数字
            attriViews[index] = ViewTool.CreateText("abi" + index);
            ViewTool.SetParent(attriViews[index], attribute);
            ViewTool.Anchor(attriViews[index], new Vector2(0.2F, 0), new Vector2(0.35F, 1F));
            //数字
            Text arrow = ViewTool.CreateText("abi" + index, " -> ");
            ViewTool.SetParent(arrow, attribute);
            ViewTool.Anchor(arrow, new Vector2(0.35F, 0), new Vector2(0.45F, 1F));
            //数字
            attriViewsNew[index] = ViewTool.CreateText("abi" + index);
            ViewTool.SetParent(attriViewsNew[index], attribute);
            ViewTool.Anchor(attriViewsNew[index], new Vector2(0.45F, 0), new Vector2(0.6F, 1F));
            //加号按钮
            Vector2 btnSize = new Vector2(50F, 50F);
            Button plus = ViewTool.CreateBtn("Plus", "+");
            plus.GetComponentInChildren<Text>().fontSize = 30;
            ViewTool.SetParent(plus, attribute);
            ViewTool.RightCenter(plus, new Vector2(1F, 0.5F), btnSize, new Vector2(-btnSize.x, 0));
            plus.onClick.AddListener(delegate () { OnAttributePlusBtnClick(index); });
            //减号按钮
            Button minus = ViewTool.CreateBtn("Minus", "-");
            minus.GetComponentInChildren<Text>().fontSize = 30;
            ViewTool.SetParent(minus, attribute);
            ViewTool.RightCenter(minus, new Vector2(1F, 0.5F), btnSize, new Vector2(0, 0));
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