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
            Transform topLayout = transform.Find("TopLayout");
            foodEditUI = topLayout.Find("OperationUI").GetComponentInChildren<InputField>();
            Debug.Assert(foodEditUI != null);
            herosChoosedLV = topLayout.Find("HerosSelected").GetComponent<HeroListView>();
            Debug.Assert(herosChoosedLV != null);
            herosGetReadyLV = transform.Find("MiddleLayout").Find("HerosReadyForSelect").GetComponent<HeroListView>();
            Debug.Assert(herosGetReadyLV != null);
            foodEditUI.onEndEdit.AddListener(delegate
            {
                Debug.Assert(int.TryParse(foodEditUI.text, out foodSelected));
                TryShowFood();
            });
            herosChoosedLV.onItemClick = delegate (ListViewItem item, Person person)
            {
                if(herosChoosedLV.RemoveItem(person))
                    herosGetReadyLV.AddItem(person);
            };
            herosGetReadyLV.onItemClick = delegate (ListViewItem item, Person person)
            {
                if(herosGetReadyLV.RemoveItem(person))
                    herosChoosedLV.AddItem(person);
            };
            herosChoosedLV.m_lengthOfLine = 1;
            herosChoosedLV.m_selectable = false;
            herosGetReadyLV.StartAxis = GridLayoutGroup.Axis.Horizontal;
            herosGetReadyLV.ScrollDirection = ScrollType.Vertical;
            herosGetReadyLV.m_selectable = false;
        }
        void Start()
        {
        }

        void Update()
        {

        }
        public void Show()
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            herosChoosedLV.Datas = new List<Person>();
            herosGetReadyLV.Datas = new List<Person>(world.GetHeros());
            foodInTrain = world.GetFoodIn();
            TryShowFood();
        }
        private void HideDialog()
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }
        public void OnItemClick(object tag)
        {
            //if(item.transform.parent == personsForSelectUI.transform)
            //{
            //    if(!personsForSelectUI.Detach(item))
            //    {
            //        Debug.LogError("备选框中不存在Item:"+item.Person.name);
            //        return;
            //    }
            //    personsSelectedUI.Append(item);
            //}
            //else if(item.transform.parent == personsSelectedUI.transform)
            //{
            //    if (!personsSelectedUI.Detach(item))
            //    {
            //        Debug.LogError("已选框中不存在Item:" + item.Person.name);
            //        return;
            //    }
            //    personsForSelectUI.Append(item);
            //}
            //else
            //{
            //    Debug.LogError("找不到Item的父类");
            //}
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
        public void OnClick(BUTTON_ID id)
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
                case BUTTON_ID.TEAM_SELECT_FOOD_OK:
                    if (GetSelectedCount() == 0)
                    {
                        Debug.Log("未选择任何人");
                        break;
                    }
                    if (foodSelected == 0)
                    {
                        Debug.Log("请选择食物");
                        break;
                    }
                    if (CallBack != null)
                        CallBack.OK(this);
                    HideDialog();
                    break;
                case BUTTON_ID.TEAM_SELECT_FOOD_CANCEL:
                    if (CallBack != null)
                        CallBack.Cancel();
                    HideDialog();
                    break;
            }
        }

        public string GetName()
        {
            return "TeamOutPrepareDialog";
        }
    }
}
