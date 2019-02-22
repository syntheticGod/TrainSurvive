/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/29 15:45:34
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using TTT.UI;
using TTT.Utility;
using TTT.Resource;

namespace WorldMap.UI
{
    public class AttributeInSchoolView : BaseView
    {
        public delegate void OnAddAttributeSuccessfully();
        AttributePanelView attributePanel;
        MoneyPanelView moneyPanel;
        StrategyPanelView strategyPanel;
        OnAddAttributeSuccessfully onAddAttribute;
        Person person;
        public override void CreateView()
        {
            attributePanel = ViewTool.ForceGetComponentInChildren<AttributePanelView>(this, "AttributePanel");
            ViewTool.Anchor(attributePanel, new Vector2(0.0714f, 0.2333f), new Vector2(0.3571f, 0.900f));
            moneyPanel = ViewTool.ForceGetComponentInChildren<MoneyPanelView>(this, "MoneyPanel");
            ViewTool.Anchor(moneyPanel, new Vector2(0.3571f, 0.2667f), new Vector2(0.6429f, 0.9333f));
            strategyPanel = ViewTool.ForceGetComponentInChildren<StrategyPanelView>(this, "StrategyPanel");
            ViewTool.Anchor(strategyPanel, new Vector2(0.7143f, 0.2667f), new Vector2(0.9286f, 0.9333f));
        }
        public void Init(OnAddAttributeSuccessfully onAddAttribute)
        {
            moneyPanel.onAddAttribute = delegate ()
                 {
                     attributePanel.SetNumbers(person.AttriNumbers);
                     onAddAttribute?.Invoke();
                     strategyPanel.SetNumbers();
                 };
            strategyPanel.onAddAttribute = delegate ()
                 {
                     attributePanel.SetNumbers(person.AttriNumbers);
                     onAddAttribute?.Invoke();
                     moneyPanel.SetNumbers();
                 };
        }
        public void SetPerson(Person person)
        {
            attributePanel.SetNumbers(person.AttriNumbers);
            this.person = person;
            moneyPanel.person = person;
            strategyPanel.person = person;
            moneyPanel.SetNumbers();
            strategyPanel.SetNumbers();
        }
        class MoneyPanelView : BaseView
        {
            Image[] moneyIcon;
            Text[] moneyText;
            Button[] plus;
            public OnAddAttributeSuccessfully onAddAttribute;
            public Person person { get; set; }
            public override void CreateView()
            {
                const int attributeCnt = (int)EAttribute.NUM;
                moneyIcon = new Image[attributeCnt];
                moneyText = new Text[attributeCnt];
                plus = new Button[attributeCnt];

                float delta = 1.0f / (int)EAttribute.NUM;
                float currentFloat = 1.0f;
                Text info = ViewTool.CreateText("MoneyInfo", "使用金钱：", this);
                ViewTool.Anchor(info, new Vector2(0f, currentFloat - delta / 2), Vector2.one);
                for (int i = 0; i < attributeCnt; i++, currentFloat -= delta)
                {
                    RectTransform item = new GameObject("Item").AddComponent<RectTransform>();
                    ViewTool.SetParent(item, this);
                    ViewTool.Anchor(item, new Vector2(0, currentFloat - delta), new Vector2(1, currentFloat));

                    moneyIcon[i] = ViewTool.CreateImage("MoneyIcon", item);
                    moneyIcon[i].sprite = StaticResource.GetSprite("ItemSprite/Money");
                    ViewTool.Anchor(moneyIcon[i], new Vector2(0.0f, 0.0f), new Vector2(0.25f, 0.5f));
                    moneyText[i] = ViewTool.CreateText("MoneyNumber", "", item);
                    ViewTool.Anchor(moneyText[i], new Vector2(0.25f, 0.0f), new Vector2(0.75f, 0.5f));
                    plus[i] = ViewTool.CreateBtn("Plus", "+", item);
                    int index = i;
                    plus[i].onClick.AddListener(delegate () { OnPlusClick(index); });
                    ViewTool.Anchor(plus[i], new Vector2(0.75f, 0.0f), new Vector2(1.0f, 0.5f));
                }
            }
            public void SetNumbers()
            {
                for (EAttribute attribute = EAttribute.NONE + 1; attribute < EAttribute.NUM; attribute++)
                {
                    moneyText[(int)attribute].text = person.CalMoneyByAttribute(attribute, 1).ToString();
                }
            }
            public void OnPlusClick(int index)
            {
                EAttribute attribute = EAttribute.NONE + 1 + index;
                const int delta = 1;
                int state = person.AddAttributeWithPay(attribute, delta, 0);
                switch (state)
                {
                    case 1:
                        onAddAttribute?.Invoke();
                        SetNumbers();
                        break;
                    case -1:
                        InfoDialog.Show("金钱不足");
                        break;
                    case -2:
                        InfoDialog.Show("属性已达到上限");
                        break;
                    case -3:
                        Debug.LogError("扣款失败");
                        break;
                }
            }
        }
        class StrategyPanelView : BaseView
        {
            Image[] strategyIcon;
            Text[] strategyText;
            Button[] plus;
            public OnAddAttributeSuccessfully onAddAttribute;
            public Person person { get; set; }
            public override void CreateView()
            {
                const int attributeCnt = (int)EAttribute.NUM;
                strategyIcon = new Image[attributeCnt];
                strategyText = new Text[attributeCnt];
                plus = new Button[attributeCnt];

                float delta = 1.0f / (int)EAttribute.NUM;
                float currentFloat = 1.0f;
                Text info = ViewTool.CreateText("StrategyInfo", "使用战略点：", this);
                ViewTool.Anchor(info, new Vector2(0f, currentFloat - delta / 2), Vector2.one);
                for (int i = 0; i < attributeCnt; i++, currentFloat -= delta)
                {
                    RectTransform item = new GameObject("Item").AddComponent<RectTransform>();
                    ViewTool.SetParent(item, this);
                    ViewTool.Anchor(item, new Vector2(0, currentFloat - delta), new Vector2(1, currentFloat));

                    strategyIcon[i] = ViewTool.CreateImage("MoneyIcon", item);
                    strategyIcon[i].sprite = StaticResource.GetSprite("ItemSprite/Money");
                    ViewTool.Anchor(strategyIcon[i], new Vector2(0f, 0f), new Vector2(0.25f, 0.5f));
                    strategyText[i] = ViewTool.CreateText("MoneyNumber", "", item);
                    ViewTool.Anchor(strategyText[i], new Vector2(0.25f, 0f), new Vector2(0.75f, 0.5f));
                    plus[i] = ViewTool.CreateBtn("Plus", "+", item);
                    int index = i;
                    plus[i].onClick.AddListener(delegate () { OnPlusClick(index); });
                    ViewTool.Anchor(plus[i], new Vector2(0.75f, 0.0f), new Vector2(1.0f, 0.5f));
                }
            }
            public void SetNumbers()
            {
                for (EAttribute attribute = EAttribute.NONE + 1; attribute < EAttribute.NUM; attribute++)
                {
                    strategyText[(int)attribute].text = person.CallStrategyByAttribute(attribute, 1).ToString();
                }
            }
            private void OnPlusClick(int index)
            {
                EAttribute attribute = EAttribute.NONE + 1 + index;
                const int delta = 1;
                int state = person.AddAttributeWithPay(attribute, delta, 1);
                switch (state)
                {
                    case 1:
                        onAddAttribute?.Invoke();
                        SetNumbers();
                        break;
                    case -1:
                        InfoDialog.Show("策略点不足");
                        break;
                    case -2:
                        InfoDialog.Show("属性已达到上限");
                        break;
                    case -3:
                        Debug.LogError("扣除策略点失败");
                        break;
                }
            }
        }
    }
}