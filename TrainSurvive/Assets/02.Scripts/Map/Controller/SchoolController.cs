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
using TTT.Resource;
using System.Text;

namespace WorldMap.Controller
{
    public class SchoolController : WindowsController, DialogCallBack
    {
        private string moneyInfoStr = "所需金钱：";
        private string payBtnStr = "确定";
        private string resetBtnStr = "重置";
        //属性视图
        private int[] deltaAttri;
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
        private ProfessionListView professionListView;
        private Profession currentProfession;
        private Profession[] nextProfessions;
        private int[] heroAttribute;
        private int cost;
        protected override void CreateModel()
        {
            m_windowSizeType = EWindowSizeType.MIDDLE14x12;
            m_titleString = "学校";
            base.CreateModel();
            SetBackground("tavern_bg_01");
            int attriCount = StaticResource.AttributeCount;
            attriViews = new Text[attriCount];
            attriViewsNew = new Text[attriCount];
            deltaAttri = new int[attriCount];
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
                        if (world.IfTeamOuting)
                            dialog.SetDatas(world.GetPersonInTeam());
                        else
                            dialog.SetDatas(world.GetPersonInTrain());
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
                    ViewTool.Anchor(attributes, new Vector2(0F, 0.125F), new Vector2(1F, 0.815F));
                    float delta = 1F / attriCount;
                    for (int i = 0; i < attriCount; i++)
                    {
                        RectTransform attribute = CreateAttribute(i);
                        ViewTool.SetParent(attribute, attributes);
                        float tempDelta = delta * (attriCount - i);
                        attribute.anchorMax = new Vector2(1, tempDelta);
                        tempDelta -= delta;
                        attribute.anchorMin = new Vector2(0, tempDelta);
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
                    professionListView = ViewTool.ForceGetComponentInChildren<ProfessionListView>(advanceLayout, "ProfessionsLV");
                    ViewTool.SetParent(professionListView, advanceLayout);
                    ViewTool.Anchor(professionListView, new Vector2(0F, 0.133F), new Vector2(1F, 0.800F));
                }
                {
                    //确定按钮
                    Button ok = ViewTool.CreateBtn("OK", "确定");
                    ok.onClick.AddListener(OnProfessionOKBtn);
                    ViewTool.SetParent(ok, advanceLayout);
                    ViewTool.Anchor(ok, new Vector2(0F, 0F), new Vector2(1F, 0.133F));
                }
            }
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
            attriInfoView.text = StaticResource.GetAttributeName(index);
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
            professionListView.SetData(new List<Profession>());
            List<Person> heros;
            if (world.IfTeamOuting)
                heros = world.GetPersonInTeam();
            else
                heros = world.GetPersonInTrain();
            if (heros.Count > 0)
            {
                ShowHero(heros[0]);
            }
        }
        private void ShowHero(Person person)
        {
            heroProfile.ShowPerson(person);
            heroChoosed = person;
            InitAttribute();
            ShowAttributes();
            ShowMoney();
            //专精
            InitProfessions();
            ShowProfession();
        }
        /// <summary>
        /// 计算所需金钱，并显示
        /// </summary>
        //TODO：有细节问题，训练次数是每点一次属性加一，还是点一次训练加一。
        private void ShowMoney()
        {
            int origin = 0, discount = 0;
            GetMoney(ref origin, ref discount);
            moneyView.text = string.Format("{0:D}", discount);
        }
        public void GetMoney(ref int origin, ref int discount)
        {
            float originF = 0F;
            float discountF = 0F;
            for (EAttribute iAttribute = EAttribute.NONE + 1; iAttribute < EAttribute.NUM; iAttribute++)
            {
                int i = (int)iAttribute;
                float oneAttributeMoney = deltaAttri[i] * 1000F * (1 + heroAttribute[i] * 0.05F) * (1 + heroChoosed.trainCnt * 0.05F);
                originF += oneAttributeMoney;
                if (currentProfession != null)
                    oneAttributeMoney *= currentProfession.GetDiscountByAttri(iAttribute);
                discountF += oneAttributeMoney;
            }
            origin = (int)originF;
            discount = (int)discountF;
        }
        /// <summary>
        /// 将加完之后的所有属性显示在属性板上
        /// </summary>
        private void ShowAttributes()
        {
            for (int i = 0; i < StaticResource.AttributeCount; i++)
            {
                attriViews[i].text = heroAttribute[i].ToString();
                attriViewsNew[i].text = (heroAttribute[i] + deltaAttri[i]).ToString();
            }
        }
        private void ShowHeroInfo()
        {
            string content = heroChoosed.name + (heroChoosed.ismale ? "，男" : "，女") + "\n";
            if (currentProfession != null)
                content += currentProfession.Name;
            heroInfoContent.text = content;
        }
        /// <summary>
        /// 初始化专精所需要的数组
        /// currentProfession和nextProfessions
        /// </summary>
        private void InitProfessions()
        {
            currentProfession = heroChoosed.getTopProfession();
            nextProfessions = StaticResource.GetNextProfessions(currentProfession);
        }
        /// <summary>
        /// 根据专精所需要的数据显示专精
        /// </summary>
        private void ShowProfession()
        {
            professionListView.RemoveAllDatas();
            if (nextProfessions != null)
            {
                for (int i = 0; i < nextProfessions.Length; i++)
                {
                    professionListView.AddItem(nextProfessions[i]);
                }
            }
            else
            {
                Debug.Log("已到达顶级专精");
            }
            //更新英雄信息
            ShowHeroInfo();
        }
        private void LearnSKills()
        {
            SkillInfo[] skills = StaticResource.GetAvailableSkills(heroAttribute);
            List<SkillInfo> newSkills = new List<SkillInfo>();
            for(int i= 0; i < skills.Length; i++)
            {
                if (heroChoosed.IfHaveGotTheSkill(skills[i]) == false)
                {
                    newSkills.Add(skills[i]);
                    heroChoosed.AddGotSkill(skills[i].ID);
                }
            }
            if(newSkills.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("学习到新技能：");
                foreach (SkillInfo skill in newSkills)
                {
                    sb.AppendFormat("{0},", skill.Name);
                }
                sb.Remove(sb.Length - 1, 1);
                InfoDialog.Show(sb.ToString());
            }
        }
        private void OnProfessionOKBtn()
        {
            if (professionListView.IsSelectNothing)
            {
                InfoDialog.Show("未选择");
                return;
            }
            if (nextProfessions == null)
            {
                InfoDialog.Show("已到达顶级专精");
                return;
            }
            Profession wanted = nextProfessions[professionListView.SelectIndex];
            if (wanted.State == EProfessionState.DEVELOPING)
            {
                InfoDialog.Show("该专精未开放");
                return;
            }
            if (wanted.State == EProfessionState.EMPTY)
            {
                InfoDialog.Show("该专精不存在");
                return;
            }
            if (!heroChoosed.IfProfessionAvailable())
            {
                InfoDialog.Show("无更多专精槽，专精槽可以通过剧情解锁");
                return;
            }
            heroChoosed.setProfession(wanted);
            InitProfessions();
            ShowProfession();
            //重新计算金钱
            ShowMoney();
        }
        /// <summary>
        /// 初始化数值变量
        /// 1、在初始化属性板时调用
        /// 2、在成功训练后调用
        /// </summary>
        private void InitAttribute()
        {
            for (EAttribute itr = EAttribute.NONE + 1; itr < EAttribute.NUM; itr++)
            {
                heroAttribute[(int)itr] = heroChoosed.GetAttriNumber(itr);
            }
            for (int i = 0; i < StaticResource.AttributeCount; i++)
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
            if (deltaAttri[index] <= 0)
                return;
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
            int afterAttribute = deltaAttri[index] + 1 + heroAttribute[index];
            if (afterAttribute > heroChoosed.GetAttriMaxNumber((EAttribute)index))
                return;
            deltaAttri[index]++;
            attriViewsNew[index].text = afterAttribute.ToString();
            ShowMoney();
        }
        /// <summary>
        /// 训练按钮响应函数
        /// </summary>
        public void OnOKBtnClick()
        {
            int origin = 0;
            int money = 0;
            GetMoney(ref origin, ref money);
            if (!world.IfMoneyEnough(money))
            {
                InfoDialog.Show("金钱不足");
                return;
            }
            if (!world.Pay(money))
            {
                Debug.LogError("扣款失败");
                return;
            }
            Debug.Log(string.Format("训练成功：原价{0:D} 折扣后{1:D}", origin, money));
            for (EAttribute itr = EAttribute.NONE + 1; itr < EAttribute.NUM; itr++)
            {
                heroChoosed.AddAttriNumber(itr, deltaAttri[(int)itr]);
            }
            InitAttribute();
            ShowAttributes();
            ShowMoney();
            LearnSKills();
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