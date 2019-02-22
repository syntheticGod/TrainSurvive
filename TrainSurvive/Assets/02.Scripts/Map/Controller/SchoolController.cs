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
using TTT.Controller;
using TTT.Resource;

using WorldMap.UI;

namespace WorldMap.Controller
{
    public class SchoolController : WindowsController, DialogCallBack
    {
        private string moneyInfoStr = "所需金钱：";
        private string payBtnStr = "确定";
        private string resetBtnStr = "重置";
        //属性视图
        private AttributeInSchoolView attributePanelView;
        //折扣信息
        private Text discountInfoView;
        //被选中的英雄
        private Person heroChoosed;
        private PersonBaseItem heroProfile;
        private Text heroInfoContent;
        //专精
        private Text advanceTitle;
        private ProfessionListView professionListView;
        private Profession currentProfession;
        private Profession[] nextProfessions;
        protected override void CreateModel()
        {
            m_windowSizeType = EWindowSizeType.MIDDLE14x12;
            m_titleString = "学校";
            base.CreateModel();
            SetBackground("tavern_bg_01");
            int attriCount = StaticResource.AttributeCount;
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
                        dialog.SetDatas(World.getInstance().Persons.CopyAll());
                        dialog.DialogCallBack = this;
                        dialog.ShowDialog();
                    });
                    //英雄简介
                    heroInfoContent = ViewTool.CreateText("HeroInfoContent");
                    heroInfoContent.alignment = TextAnchor.MiddleLeft;
                    ViewTool.SetParent(heroInfoContent, heroInfoLayout);
                    ViewTool.FullFillRectTransform(heroInfoContent, new Vector2(100F, 0), Vector2.zero);
                }
                {
                    //属性面板
                    attributePanelView = ViewTool.ForceGetComponentInChildren<AttributeInSchoolView>(heroInfoBorad, "AttributePanel");
                    ViewTool.Anchor(attributePanelView, new Vector2(0F, 0.125F), new Vector2(1F, 0.815F));
                    attributePanelView.Init(ShowMoneyIncreaseInfo);
                }
                {
                    //折扣信息区块
                    discountInfoView = ViewTool.CreateText("DiscountInfo", "", heroInfoBorad);
                    discountInfoView.alignment = TextAnchor.MiddleLeft;
                    ViewTool.Anchor(discountInfoView, new Vector2(0.0714f, 0.0556f), new Vector2(0.9296F, 0.1112F));
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
            heros = World.getInstance().Persons.CopyAll();
            if (heros.Count > 0)
            {
                ShowHero(heros[0]);
            }
        }
        private void ShowHero(Person person)
        {
            heroProfile.ShowPerson(person);
            heroChoosed = person;
            ShowAttributes();
            //专精
            InitProfessions();
            ShowProfession();
        }
        /// <summary>
        /// 将加完之后的所有属性显示在属性板上
        /// </summary>
        private void ShowAttributes()
        {
            attributePanelView.SetPerson(heroChoosed);
            ShowMoneyIncreaseInfo();
        }
        private void ShowHeroInfo()
        {
            string content = heroChoosed.name + (heroChoosed.ismale ? "，男" : "，女") + "\n";
            if (currentProfession != null)
                content += currentProfession.Name;
            heroInfoContent.text = content;
        }
        private void ShowMoneyIncreaseInfo()
        {
            discountInfoView.text = string.Format("总训练次数：{0} -> 所有训练花费增加：{1}%", heroChoosed.trainCnt, heroChoosed.CallMoneyIncreaseByTrainCnt());
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
        /// <summary>
        /// 专精按钮的触发事件
        /// </summary>
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
            heroChoosed.SetProfession(wanted, EAttribute.NONE + 1 + professionListView.SelectIndex);
            InitProfessions();
            ShowProfession();
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