/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/1 1:35:30
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using WorldMap.UI;

namespace WorldMap
{
    public class SchoolController : MonoBehaviour
    {
        private int m_attributeCount = 5;
        //属性视图板
        public GameObject m_attributeBoard;
        //记账板
        public GameObject m_counterBoard;
        //属性视图
        private Text[] attriViews;
        private uint[] deltaAttri;
        //花费金额视图
        private Text moneyView;
        //英雄显示列表
        private ListViewController herosLayout;
        private List<Person> heros;
        //被选中的英雄
        private int heroChoosedIndex;
        private Person heroChoosed;
        private int[] heroAttribute;
        private int cost;
        void Awake()
        {
            heros = World.getInstance().persons;
            attriViews = new Text[m_attributeCount];
            deltaAttri = new uint[m_attributeCount];
            heroAttribute = new int[m_attributeCount];
            for (int i = 0; i < m_attributeCount; i++)
            {
                attriViews[i] = m_attributeBoard.transform.Find("abi" + i).Find("Number").GetComponentInChildren<Text>();
            }
            herosLayout = GetComponentInChildren<ListViewController>();
            herosLayout.onItemView = delegate (ListViewItem item, int index)
            {
                item.GetComponentInChildren<Text>().text = heros[index].name;
            };
            herosLayout.onItemClick = delegate (ListViewItem item, int index)
            {
                if (index >= heros.Count)
                {
                    Debug.LogError("选择英雄的Item不存在");
                    return;
                }
                CallBackChooseHero(index);
            };
            moneyView = m_counterBoard.transform.Find("Money").GetComponent<Text>();
        }
        void Start()
        {
            int i;
            for (i = 0; i < heros.Count; i++)
            {
                herosLayout.AppendItem();
            }
            if(i != 0)
                herosLayout.ClickManually(0);
            ShowAttributes();
        }
        
        void Update()
        { }
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
        /// 点击英雄ListView时的回调函数
        /// 显示设值
        /// </summary>
        /// <param name="index"></param>
        private void CallBackChooseHero(int index)
        {
            heroChoosedIndex = index;
            heroChoosed = heros[index];
            InitAttribute();
            ShowAttributes();
            ShowMoney();
        }
        /// <summary>
        /// 计算所需金钱，并显示
        /// </summary>
        //TODO：有细节问题，训练次数是每点一次属性加一，还是点一次训练加一。
        private void ShowMoney()
        {
            float money = 0F;
            for(int i = 0;i < m_attributeCount; i++)
            {
                money += deltaAttri[i] * 1000F * (1 + heroAttribute[i] * 0.05F) * (1 + heroChoosed.trainCnt * 0.05F);
            }
            cost = (int)money;
            moneyView.text = cost.ToString();
        }
        /// <summary>
        /// 将加完之后的所有属性显示在属性板上
        /// </summary>
        private void ShowAttributes()
        {
            for(int i = 0; i < m_attributeCount; i++)
            {
                attriViews[i].text = (heroAttribute[i] + deltaAttri[i]).ToString();
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
            heroAttribute[4] = heroChoosed.intellgence;
            for (int i = 0; i < m_attributeCount; i++)
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
            attriViews[index].text = (heroAttribute[index] + deltaAttri[index]).ToString();
            ShowMoney();
        }
        /// <summary>
        /// 加属性响应函数
        /// </summary>
        /// <param name="index"></param>
        public void OnAttributePlusBtnClick(int index)
        {
            deltaAttri[index]++;
            attriViews[index].text = (heroAttribute[index] + deltaAttri[index]).ToString();
            ShowMoney();
        }
        /// <summary>
        /// 训练按钮响应函数
        /// </summary>
        public void OnOKBtnClick()
        {
            //TODO：检查金额是否足够
            heroChoosed.vitality += (int)deltaAttri[0];
            heroChoosed.strength += (int)deltaAttri[1];
            heroChoosed.agile += (int)deltaAttri[2];
            heroChoosed.technique += (int)deltaAttri[3];
            heroChoosed.intellgence += (int)deltaAttri[4];
            InitAttribute();
            ShowMoney();
        }
        public void OnCancelBtnClick()
        {
            Hide();
        }
    }
}