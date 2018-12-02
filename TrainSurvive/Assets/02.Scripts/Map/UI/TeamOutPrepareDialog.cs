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
        private ListViewController herosChoosedLV;
        private ListViewController herosGetReadyLV;
        private int foodInTrain;
        private int foodSelected = 1000;
        private const int deltaFood = 100;
        private List<Person> herosChoosed = new List<Person>();
        private List<Person> herosGetReady;
        public DialogCallBack CallBack { set; get; }
        public void Init()
        {
            ButtonHandler.Instance.AddListeners(this);
            world = World.getInstance();
        }
        void Awake()
        {
            Transform topLayout = transform.Find("TopLayout");
            foodEditUI = topLayout.Find("OperationUI").GetComponentInChildren<InputField>();
            Debug.Assert(foodEditUI != null);
            herosChoosedLV = topLayout.Find("HerosSelected").GetComponent<ListViewController>();
            Debug.Assert(herosChoosedLV != null);
            herosGetReadyLV = transform.Find("MiddleLayout").Find("HerosReadyForSelect").GetComponent<ListViewController>();
            Debug.Assert(herosGetReadyLV != null);
            foodEditUI.onEndEdit.AddListener(delegate
            {
                Debug.Assert(int.TryParse(foodEditUI.text, out foodSelected));
                TryShowFood();
            });
            herosChoosedLV.onItemClick = delegate (ListViewItem item, int index)
            {
                Person person = herosChoosed[index];
                herosChoosed.RemoveAt(index);
                herosGetReady.Add(person);
                herosChoosedLV.RemoveItem(index);
                herosGetReadyLV.AppendItem();
            };
            herosGetReadyLV.onItemClick = delegate (ListViewItem item, int index)
            {
                Person person = herosGetReady[index];
                herosGetReady.RemoveAt(index);
                herosChoosed.Add(person);
                herosGetReadyLV.RemoveItem(index);
                herosChoosedLV.AppendItem();
            };
            //已选
            herosChoosedLV.onItemView = delegate (ListViewItem item, int index)
            {
                item.GetComponentInChildren<Text>().text = herosChoosed[index].name;
                item.Tag = herosChoosed[index];
            };
            //备选
            herosGetReadyLV.onItemView = delegate (ListViewItem item, int index)
            {
                item.GetComponentInChildren<Text>().text = herosGetReady[index].name;
                item.Tag = herosGetReady[index];
            };
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
            herosGetReadyLV.RemoveAllItem();
            herosGetReady = world.persons;
            for (int i = 0; i < herosGetReady.Count; ++i)
            {
                ListViewItem item = herosGetReadyLV.AppendItem();
            }
            foodInTrain = (int)world.getFoodIn();
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
            return herosChoosed;
        }
        private int GetSelectedCount()
        {
            return herosChoosed.Count;
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

        public bool IfAccepted(BUTTON_ID id)
        {
            return Utility.IfBetweenBoth((int)BUTTON_ID.TEAM_SELECT_DIALOG_NONE, (int)BUTTON_ID.TEAM_SELECT_DIALOG_NUM, (int)id);
        }
    }
}
