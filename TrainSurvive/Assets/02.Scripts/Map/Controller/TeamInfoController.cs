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
using TTT.Resource;

namespace WorldMap.Controller
{
    public class TeamInfoController : WindowsController
    {
        HeroListView heroListView;
        Button[] btns;
        static string[] btnString = { "人物属性", "技能", "故事背景" };
        AttributePanelView attributePanelView;
        Image skill1Image, skill2Image;
        Vector2 selectedSkillSize = new Vector2(110f, 110f);
        protected override void CreateModel()
        {
            base.CreateModel();
            {
                //左边人物选择列表
                heroListView = ViewTool.ForceGetComponentInChildren<HeroListView>(this, "HeroListView");
                heroListView.ScrollDirection = ScrollType.Vertical;
                heroListView.GridConstraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
                heroListView.GridConstraintCount = 1;
                ViewTool.Anchor(heroListView, new Vector2(0.0714f, 0.0833f), new Vector2(0.3929f, 0.9167f));
            }
            {
                //右边信息框
                RectTransform rightInfo = ViewTool.CreateImage("RightInfoLayout").rectTransform;
                ViewTool.SetParent(rightInfo, this);
                ViewTool.Anchor(rightInfo, new Vector2(0.4643f, 0.0833f), new Vector2(0.9286f, 0.9167f));
                RectTransform infoContentLayout = ViewTool.CreateImage("InfoContentLayout").rectTransform;
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
                        RectTransform topLayout = new GameObject("TopLayout").AddComponent<RectTransform>();
                        ViewTool.SetParent(topLayout, infoContentLayout);
                        ViewTool.Anchor(topLayout, new Vector2(0f, 0.8125f), new Vector2(1f, 1f));

                        Vector2 closeBtnSize = new Vector2(30f, 30f);
                        //技能1
                        skill1Image = ViewTool.CreateImage("Skill1", topLayout);
                        ViewTool.CenterAt(skill1Image, new Vector2(0.3182f, 0.5f), selectedSkillSize);
                        Button close1 = ViewTool.CreateBtn("Close", "X", skill1Image);
                        close1.onClick.AddListener(delegate ()
                        {
                            SetSkill1(-1);
                        });
                        ViewTool.RightTop(close1, Vector2.zero, closeBtnSize);
                        //技能2
                        skill2Image = ViewTool.CreateImage("Skill2", topLayout);
                        ViewTool.CenterAt(skill2Image, new Vector2(0.8182f, 0.5f), selectedSkillSize);
                        Button close2 = ViewTool.CreateBtn("Close", "X", skill2Image);
                        close2.onClick.AddListener(delegate ()
                        {
                            SetSkill2(-1);
                        });
                        ViewTool.RightTop(close1, Vector2.zero, closeBtnSize);
                        //交换按钮
                        Button exchangeBtn = ViewTool.CreateBtn("ExchangeBtn", "→\n←", topLayout);
                        ViewTool.CenterAt(exchangeBtn, new Vector2(0.5f, 0.5f), new Vector2(60f, 80f));
                    }
                    {
                        //所有技能
                    }
                }
                {
                    //背景故事
                }
            }
        }
        protected override void AfterShowWindow()
        {

        }

        protected override bool PrepareDataBeforeShowWindow()
        {
            return true;
        }
        /// <summary>
        /// 设置第一技能槽
        /// </summary>
        /// <param name="skillID">技能id，当skillID=-1时，表示清空第一技能槽</param>
        private void SetSkill1(int skillID)
        {
            if (skillID == -1)
                skill1Image.sprite = null;
            else
                skill1Image.sprite = StaticResource.GetSkillByID(skillID).BigSprite;
        }
        private void SetSkill2(int skillID)
        {
            if (skillID == -1)
                skill2Image.sprite = null;
            else
                skill2Image.sprite = StaticResource.GetSkillByID(skillID).BigSprite;
        }
        private void ExchangeSkills()
        {
        }
    }
}