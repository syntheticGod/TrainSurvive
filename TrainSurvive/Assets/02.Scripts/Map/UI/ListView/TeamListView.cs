/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/28 14:57:43
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using TTT.UI;
using TTT.UI.ListView;
using TTT.Utility;
using TTT.Team;

namespace WorldMap.UI
{
    public class TeamListView : BaseListView<Person>
    {
        protected override void Awake()
        {
            m_cellSize = new Vector2(-1f, 120f);
            base.Awake();
        }
        protected override void OnItemView(ListViewItem item, Person data, int itemIndex)
        {
            TeamItem itemView = ViewTool.ForceGetComponentInChildren<TeamItem>(item, "TeamItem");
            itemView.Config(data);
        }

        public class TeamItem : BaseItem
        {
            /// <summary>
            /// 头像 120*120
            /// </summary>
            Image profile;
            /// <summary>
            /// 文字信息
            /// </summary>
            Text infoText;
            /// <summary>
            /// 出战/休息 按钮
            /// </summary>
            Button action;
            static string[] btnString = { "出战", "休息" };
            Person currentData;
            protected override void CreateModel()
            {
                profile = ViewTool.CreateImage("Profile", this);
                infoText = ViewTool.CreateText("Info", "", this);
                infoText.alignment = TextAnchor.UpperLeft;
                action = ViewTool.CreateBtn("Action", btnString[0], this);
                action.onClick.AddListener(delegate ()
                {
                    bool swithFight = !currentData.ifReadyForFighting;
                    if (World.getInstance().Persons.ConfigFight(currentData, swithFight))
                    {
                        SetActionBtn(currentData.ifReadyForFighting);
                    }
                    else
                    {
                        InfoDialog.Show(swithFight ? "出战人数已经达到上限" + PersonSet.MAX_NUMBER_FIGHER + "人" : "必须有一人出战");
                    }
                });
            }

            protected override void InitModel()
            {
            }

            protected override void PlaceModel()
            {
                ViewTool.Anchor(profile, Vector2.zero, new Vector2(0.4444f, 1f));
                ViewTool.Anchor(infoText, new Vector2(0.4444f, 0f), Vector2.one);
                ViewTool.LeftBottom(action, new Vector2(1, 0), new Vector2(60f, 60f));
            }
            public void Config(Person person)
            {
                profile.sprite = person.IconBig;
                infoText.text = string.Format("{0}\n{1}", person.SimpleInfo, person.ProfessionInfo);
                SetActionBtn(person.ifReadyForFighting);
                currentData = person;
            }
            private void SetActionBtn(bool readyForFighting)
            {
                ViewTool.SetBtnContent(action, btnString[readyForFighting ? 1 : 0]);
                ViewTool.SetBtnColor(action, readyForFighting ? Color.red : Color.green);
            }
        }
    }
}