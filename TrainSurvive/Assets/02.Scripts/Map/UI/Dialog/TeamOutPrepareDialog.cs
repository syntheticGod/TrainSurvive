/*
 * 描述：小队外出的弹出框
 * 作者：项叶盛
 * 创建时间：2018/11/26 22:05:50
 * 版本：v0.1
 */
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class TeamOutPrepareDialog : BaseDialog
    {
        public GameObject personProfile;

        private InputField foodEditUI;
        private HeroListView herosChoosedLV;
        private HeroListView herosGetReadyLV;
        private int foodInTrain;
        private int foodSelected = 1000;
        private const int deltaFood = 100;
        protected override void CreateModel()
        {
            SetTitle("选择英雄");
            //上部分
            RectTransform topLayout = new GameObject("TopLayout", typeof(Image)).GetComponent<RectTransform>();
            Utility.SetParent(topLayout, this);
            Utility.Anchor(topLayout, new Vector2(0F, 0.7F), new Vector2(1F, 1F), Vector2.zero, new Vector2(0F, -60F));
            //FoodEdit
            foodEditUI = Utility.CreateInputField("FoodEdit");
            foodEditUI.onEndEdit.AddListener(delegate
            {
                Debug.Assert(int.TryParse(foodEditUI.text, out foodSelected));
                TryShowFood();
            });
            Utility.SetParent(foodEditUI, topLayout);
            Utility.Anchor(foodEditUI, Vector2.zero, new Vector2(0.2F, 1F));
            //Plus
            Button plus = Utility.CreateBtn("Plus", "+");
            plus.onClick.AddListener(delegate () { OnClick(BUTTON_ID.TEAM_SELECT_FOOD_PLUS); });
            Utility.SetParent(plus, topLayout);
            Utility.Anchor(plus, new Vector2(0.2F, 0F), new Vector2(0.3F, 1F));
            //Utility.Anchor(plus, )
            Button minus = Utility.CreateBtn("Minus", "-");
            minus.onClick.AddListener(delegate () { OnClick(BUTTON_ID.TEAM_SELECT_FOOD_SUBTRCT); });
            Utility.SetParent(minus, topLayout);
            Utility.Anchor(minus, new Vector2(0.3F, 0F), new Vector2(0.4F, 1F));
            //已选
            herosChoosedLV = new GameObject("TeamChoosedLV").AddComponent<HeroListView>();
            herosChoosedLV.GridConstraint = GridLayoutGroup.Constraint.FixedRowCount;
            herosChoosedLV.SetCellSize(new Vector2(200F, 200F));
            herosChoosedLV.GridConstraintCount = 1;
            herosChoosedLV.m_selectable = false;
            herosChoosedLV.ScrollDirection = ScrollType.Horizontal;
            herosChoosedLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            Utility.SetParent(herosChoosedLV, topLayout);
            Utility.Anchor(herosChoosedLV, new Vector2(0.4F, 0F), new Vector2(1F, 1F));
            herosChoosedLV.onItemClick = delegate (ListViewItem item, Person person)
            {
                if (herosChoosedLV.RemoveItem(person))
                    herosGetReadyLV.AddItem(person);
            };
            //下部分 待选
            herosGetReadyLV = new GameObject("TeamGetReadyLV").AddComponent<HeroListView>();
            herosGetReadyLV.GridConstraint = GridLayoutGroup.Constraint.FixedRowCount;
            herosGetReadyLV.GridConstraintCount = 2;
            herosGetReadyLV.m_selectable = false;
            herosGetReadyLV.ScrollDirection = ScrollType.Horizontal;
            herosGetReadyLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            Utility.SetParent(herosGetReadyLV, this);
            Utility.Anchor(herosGetReadyLV, new Vector2(0F, 0.2F), new Vector2(1F, 0.6F), new Vector2(0F, 60F), Vector2.zero);
            Debug.Assert(herosGetReadyLV != null);
            herosGetReadyLV.onItemClick = delegate (ListViewItem item, Person person)
            {
                if (herosGetReadyLV.RemoveItem(person))
                    herosChoosedLV.AddItem(person);
            };
        }
        protected override void AfterDialogShow()
        {
            herosChoosedLV.Datas = new List<Person>();
            herosGetReadyLV.Datas = new List<Person>(world.GetHeros());
            foodInTrain = world.GetFoodIn();
            TryShowFood();
        }
        public List<Person> GetSelectedPerson()
        {
            return herosChoosedLV.Datas;
        }
        private int GetSelectedCount()
        {
            return herosChoosedLV.Datas.Count;
        }
        public int GetSelectedFood()
        {
            return foodSelected;
        }
        private void TryShowFood()
        {
            if (foodSelected >= foodInTrain)
                foodSelected = foodInTrain;
            if (foodSelected < 0)
                foodSelected = 0;
            if (foodSelected >= world.GetFootOutMax())
                foodSelected = world.GetFootOutMax();
            foodEditUI.text = foodSelected + "";
        }
        private void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TEAM_SELECT_FOOD_PLUS:
                    Debug.Assert(int.TryParse(foodEditUI.text, out foodSelected));
                    foodSelected += deltaFood;
                    TryShowFood();
                    break;
                case BUTTON_ID.TEAM_SELECT_FOOD_SUBTRCT:
                    Debug.Assert(int.TryParse(foodEditUI.text, out foodSelected));
                    foodSelected -= deltaFood;
                    TryShowFood();
                    break;
            }
        }

        public string GetName()
        {
            return "TeamOutPrepareDialog";
        }

        protected override bool OK()
        {
            if (GetSelectedCount() == 0)
            {
                InfoDialog dialog = BaseDialog.CreateDialog<InfoDialog>("InfoDialog");
                dialog.SetInfo("未选择任何人");
                dialog.ShowDialog();
                return false;
            }
            if (foodSelected == 0)
            {
                InfoDialog dialog = BaseDialog.CreateDialog<InfoDialog>("InfoDialog");
                dialog.SetInfo("请选择食物");
                dialog.ShowDialog();
                return false;
            }
            if(GetSelectedCount() > 5)
            {
                InfoDialog dialog = BaseDialog.CreateDialog<InfoDialog>("InfoDialog");
                dialog.SetInfo("至多选择5个英雄");
                dialog.ShowDialog();
                return false;
            }
            if (DialogCallBack != null)
                DialogCallBack.OK(this);
            return true;
        }

        protected override void Cancel()
        {
            if (DialogCallBack != null)
                DialogCallBack.Cancel();
        }
    }
}
