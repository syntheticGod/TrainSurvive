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
    public interface DialogCallBack
    {
        void OK(TeamOutPrepareDialog dialog);
        void Cancel();
    }
    public class TeamOutPrepareDialog : MonoBehaviour, OnClickListener
    {
        public GameObject personProfile;

        private World world;

        private InputField foodEditUI;
        private ListLayoutGroup personsSelectedUI;
        private ListLayoutGroup personsForSelectUI;
        private int foodInTrain;
        private int foodSelected = 1000;
        private const int deltaFood = 100;
        public DialogCallBack CallBack { set; get; }
        public void Init()
        {
            ButtonHandler.Instance.AddListeners(this);
            world = World.getInstance();
        }
        void Awake()
        {
            personsSelectedUI = transform.Find("SelectedScroll").GetComponentInChildren<ListLayoutGroup>();
            Debug.Assert(personsSelectedUI != null);
            personsForSelectUI = transform.Find("ReadyForSelectScroll").GetComponentInChildren<ListLayoutGroup>();
            Debug.Assert(personsForSelectUI != null);
            foodEditUI = transform.Find("OperationUI").GetComponentInChildren<InputField>();
            Debug.Assert(foodEditUI != null);
            foodEditUI.onEndEdit.AddListener(delegate
            {
                Debug.Assert(int.TryParse(foodEditUI.text, out foodSelected));
                TryShowFood();
            });
        }
        void Start()
        {
            personsForSelectUI.Init(world.persons, personProfile, this);
            personsSelectedUI.Init(new List<Person>(), personProfile, this);
            foodInTrain = (int)world.getFoodIn();
            TryShowFood();
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
        }
        private void Hide()
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }
        public void OnItemClick(Item item)
        {
            if(item.transform.parent == personsForSelectUI.transform)
            {
                if(!personsForSelectUI.Detach(item))
                {
                    Debug.LogError("备选框中不存在Item:"+item.Person.name);
                    return;
                }
                personsSelectedUI.Append(item);
            }
            else if(item.transform.parent == personsSelectedUI.transform)
            {
                if (!personsSelectedUI.Detach(item))
                {
                    Debug.LogError("已选框中不存在Item:" + item.Person.name);
                    return;
                }
                personsForSelectUI.Append(item);
            }
            else
            {
                Debug.LogError("找不到Item的父类");
            }
        }
        public List<Person> GetSelectedPerson()
        {
            return personsSelectedUI.GetData();
        }
        private int GetSelectedCount()
        {
            return personsSelectedUI.GetData().Count;
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
            if (foodSelected >= world.foodOutMax)
                foodSelected = world.foodOutMax;
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
                    if(foodSelected == 0)
                    {
                        Debug.Log("请选择食物");
                        break;
                    }
                    if (CallBack != null)
                        CallBack.OK(this);
                    Hide();
                    break;
                case BUTTON_ID.TEAM_SELECT_FOOD_CANCEL:
                    if (CallBack != null)
                        CallBack.Cancel();
                    Hide();
                    break;
            }
        }

        public bool IfAccepted(BUTTON_ID id)
        {
            return Utility.IfBetweenBoth((int)BUTTON_ID.TEAM_SELECT_DIALOG_NONE, (int)BUTTON_ID.TEAM_SELECT_DIALOG_NUM, (int)id);
        }
    }
}
