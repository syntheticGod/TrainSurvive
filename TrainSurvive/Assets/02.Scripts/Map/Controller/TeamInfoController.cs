/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/26 18:10:41
 * 版本：v0.7
 */
using UnityEngine;
using UnityEngine.UI;

using WorldMap.UI;

using TTT.Utility;
using TTT.UI;
using TTT.Controller;

namespace WorldMap.Controller
{
    public class TeamInfoController : WindowsController
    {
        TeamListView teamListView;
        Button[] btns;
        static string[] btnString = { "人物属性", "技能", "故事背景" };
        int tabIndex = 0;
        AttributePanelView attributePanelView;
        GameObject skillPanelGameO;
        Text backgroundStore;
        Image infoContentBG;

        Image skill1Image, skill2Image;
        Vector2 selectedSkillSize = new Vector2(110f, 110f);
        Person targetPerson;
        protected override void CreateModel()
        {
            m_titleString = "探险队信息";
            base.CreateModel();
            {
                //左边人物选择列表
                teamListView = ViewTool.ForceGetComponentInChildren<TeamListView>(this, "HeroListView");
                teamListView.ScrollDirection = ScrollType.Vertical;
                teamListView.GridConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
                teamListView.GridConstraintCount = 1;
                teamListView.onItemClick = OnItemClick;
                ViewTool.Anchor(teamListView, new Vector2(0.0714f, 0.0833f), new Vector2(0.3929f, 0.9167f));
            }
            {
                //右边信息框
                RectTransform rightInfo = ViewTool.CreateImage("RightInfoLayout").rectTransform;
                ViewTool.SetParent(rightInfo, this);
                ViewTool.Anchor(rightInfo, new Vector2(0.4643f, 0.0833f), new Vector2(0.9286f, 0.9167f));
                {
                    RectTransform btnsRect = new GameObject("Btns").AddComponent<RectTransform>();
                    ViewTool.SetParent(btnsRect, rightInfo);
                    ViewTool.LeftTop(btnsRect, new Vector2(0, 1), Vector2.zero);
                    //按钮
                    btns = new Button[btnString.Length];
                    Vector2 btnSize = new Vector2(90f, 60f);
                    for (int i = 0; i < btns.Length; i++)
                    {
                        btns[i] = ViewTool.CreateBtn("Btn" + i, btnString[i], btnsRect);
                        int index = i;
                        btns[i].onClick.AddListener(delegate ()
                        {
                            tabIndex = index;
                            OnTabClick(index);
                        });
                        ViewTool.LeftTop(btns[i], new Vector2(0, 1), btnSize, new Vector2(i * btnSize.x, 0));
                    }
                }
                infoContentBG = ViewTool.CreateImage("InfoContentLayout");
                RectTransform infoContentLayout = infoContentBG.rectTransform;
                ViewTool.SetParent(infoContentLayout, rightInfo);
                ViewTool.Anchor(infoContentLayout, new Vector2(0.0769f, 0.05f), new Vector2(0.9231f, 0.85f));
                {
                    //人物属性
                    attributePanelView = ViewTool.ForceGetComponentInChildren<AttributePanelView>(infoContentLayout, "AttributePanel");
                    ViewTool.Anchor(attributePanelView, new Vector2(0f, 0.1875f), new Vector2(1f, 0.8125f));
                }
                {
                    //技能
                    {
                        //已选技能
                        //RectTransform topLayout = new GameObject("TopLayout").AddComponent<RectTransform>();
                        //ViewTool.SetParent(topLayout, infoContentLayout);
                        //ViewTool.Anchor(topLayout, new Vector2(0f, 0.8125f), new Vector2(1f, 1f));

                        //Vector2 closeBtnSize = new Vector2(30f, 30f);
                        ////技能1
                        //skill1Image = ViewTool.CreateImage("Skill1", topLayout);
                        //ViewTool.CenterAt(skill1Image, new Vector2(0.3182f, 0.5f), selectedSkillSize);
                        //Button close1 = ViewTool.CreateBtn("Close", "X", skill1Image);
                        //close1.onClick.AddListener(delegate ()
                        //{
                        //    SetSkill1(-1);
                        //});
                        //ViewTool.RightTop(close1, Vector2.zero, closeBtnSize);
                        ////技能2
                        //skill2Image = ViewTool.CreateImage("Skill2", topLayout);
                        //ViewTool.CenterAt(skill2Image, new Vector2(0.8182f, 0.5f), selectedSkillSize);
                        //Button close2 = ViewTool.CreateBtn("Close", "X", skill2Image);
                        //close2.onClick.AddListener(delegate ()
                        //{
                        //    SetSkill2(-1);
                        //});
                        //ViewTool.RightTop(close1, Vector2.zero, closeBtnSize);
                        ////交换按钮
                        //Button exchangeBtn = ViewTool.CreateBtn("ExchangeBtn", "→\n←", topLayout);
                        //exchangeBtn.onClick.AddListener(delegate ()
                        //{
                        //    ExchangeSkills();
                        //});
                        //ViewTool.CenterAt(exchangeBtn, new Vector2(0.5f, 0.5f), new Vector2(60f, 80f));
                    }
                    {
                        //所有技能
                    }
                }
                {
                    //背景故事
                    backgroundStore = ViewTool.CreateText("BackgroundStore", "", infoContentLayout);
                    ViewTool.FullFillRectTransform(backgroundStore);
                }
            }
        }
        protected override bool PrepareDataBeforeShowWindow()
        {
            return true;
        }
        protected override void AfterShowWindow()
        {
            teamListView.Datas = WorldForMap.Instance.GetAllPersons();
            if (teamListView.Datas.Count > 0)
                teamListView.ClickManually(0);
        }
        private void OnItemClick(ListViewItem item, Person person)
        {
            targetPerson = person;
            OnTabClick(tabIndex);
        }
        private void OnTabClick(int index)
        {
            
            attributePanelView.gameObject.SetActive(false);
            skillPanelGameO?.SetActive(false);
            backgroundStore.gameObject.SetActive(false);
            switch (index)
            {
                case 0:
                    infoContentBG.color = new Color(0.85f, 0.85f, 0.85f);
                    attributePanelView.gameObject.SetActive(true);
                    attributePanelView.SetNumbers(targetPerson.GetAttriNumbers());
                    break;
                case 1:
                    //TODO 显示技能
                    infoContentBG.color = Color.white;
                    skillPanelGameO?.SetActive(true);
                    break;
                case 2:
                    infoContentBG.color = new Color(0.85f, 0.85f, 0.85f);
                    backgroundStore.text = targetPerson.BackgroundStoreInfo;
                    backgroundStore.gameObject.SetActive(true);
                    break;
            }
        }
    }
}