/*
 * 描述：小队外出的弹出框
 * 作者：项叶盛
 * 创建时间：2018/11/26 22:05:50
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TTT.Utility;
using WorldMap.Model;
<<<<<<< .merge_file_a22384
=======
using TTT.UI;
>>>>>>> .merge_file_a16280

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
            ViewTool.SetParent(topLayout, this);
            ViewTool.Anchor(topLayout, new Vector2(0F, 0.7F), new Vector2(1F, 1F), Vector2.zero, new Vector2(0F, -60F));
            //FoodEdit
            foodEditUI = ViewTool.CreateInputField("FoodEdit");
            foodEditUI.onEndEdit.AddListener(delegate
            {
                Debug.Assert(int.TryParse(foodEditUI.text, out foodSelected));
                TryShowFood();
            });
            ViewTool.SetParent(foodEditUI, topLayout);
            ViewTool.Anchor(foodEditUI, Vector2.zero, new Vector2(0.2F, 1F));
            //Plus
            Button plus = ViewTool.CreateBtn("Plus", "+");
            plus.onClick.AddListener(delegate () { OnClick(BUTTON_ID.TEAM_SELECT_FOOD_PLUS); });
            ViewTool.SetParent(plus, topLayout);
            ViewTool.Anchor(plus, new Vector2(0.2F, 0F), new Vector2(0.3F, 1F));
            //Utility.Anchor(plus, )
            Button minus = ViewTool.CreateBtn("Minus", "-");
            minus.onClick.AddListener(delegate () { OnClick(BUTTON_ID.TEAM_SELECT_FOOD_SUBTRCT); });
            ViewTool.SetParent(minus, topLayout);
            ViewTool.Anchor(minus, new Vector2(0.3F, 0F), new Vector2(0.4F, 1F));
            //已选
            herosChoosedLV = new GameObject("TeamChoosedLV").AddComponent<HeroListView>();
            herosChoosedLV.GridConstraint = GridLayoutGroup.Constraint.FixedRowCount;
            herosChoosedLV.SetCellSize(new Vector2(200F, 200F));
            herosChoosedLV.GridConstraintCount = 1;
            herosChoosedLV.IfSelectable = false;
            herosChoosedLV.ScrollDirection = ScrollType.Horizontal;
            herosChoosedLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            ViewTool.SetParent(herosChoosedLV, topLayout);
            ViewTool.Anchor(herosChoosedLV, new Vector2(0.4F, 0F), new Vector2(1F, 1F));
            herosChoosedLV.onItemClick = delegate (ListViewItem item, Person person)
            {
                if (herosChoosedLV.RemoveData(person))
                    herosGetReadyLV.AddItem(person);
            };
            //下部分 待选
            herosGetReadyLV = new GameObject("TeamGetReadyLV").AddComponent<HeroListView>();
            herosGetReadyLV.GridConstraint = GridLayoutGroup.Constraint.FixedRowCount;
            herosGetReadyLV.GridConstraintCount = 2;
            herosGetReadyLV.IfSelectable = false;
            herosGetReadyLV.ScrollDirection = ScrollType.Horizontal;
            herosGetReadyLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            ViewTool.SetParent(herosGetReadyLV, this);
            ViewTool.Anchor(herosGetReadyLV, new Vector2(0F, 0.2F), new Vector2(1F, 0.6F), new Vector2(0F, 60F), Vector2.zero);
            Debug.Assert(herosGetReadyLV != null);
            herosGetReadyLV.onItemClick = delegate (ListViewItem item, Person person)
            {
                if (herosGetReadyLV.RemoveData(person))
                    herosChoosedLV.AddItem(person);
            };
        }
        protected override void AfterDialogShow()
        {
            herosChoosedLV.Datas = new List<Person>();
            foodInTrain = world.TrainGetFoodIn();
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
            if (foodSelected >= world.TeamGetFootOutMax())
                foodSelected = world.TeamGetFootOutMax();
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
        
        protected override bool OK()
        {
            return true;
        }

        protected override void Cancel()
        {
        }
    }
}
