/*
 * 描述：城镇控制器，控制城市界面的显示与用户操作
 * 作者：项叶盛
 * 创建时间：2018/11/20 23:14:19
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;
using TTT.Controller;

using WorldMap.UI;

namespace WorldMap.Controller
{
    public class TownController : WindowsController
    {
        private const int ECHO_CODE_TRAIN = 1;
        private const int ECHO_CODE_TEAM = 2;
        private static string[] btnsStrs = { "酒馆", "学校", "商店", "" };
        private TavernController tavernController;
        private Model.Town currentTown;
        private Text townInfoText;
        private Button[] btns;
        protected override void CreateModel()
        {
            m_windowSizeType = EWindowSizeType.BIG26x14;
            m_titleString = "城镇";
            base.CreateModel();
            SetBackground("town_bg_02");
            //城镇信息容器
            Image townInfoBG = ViewTool.CreateImage("TownInfo");
            ViewTool.SetParent(townInfoBG, this);
            ViewTool.Anchor(townInfoBG, new Vector2(0.2F, 0.2F), new Vector2(0.4F, 0.8F));
            townInfoBG.color = containerColor;
            //城镇信息框
            townInfoText = ViewTool.CreateText("TownInfoText");
            ViewTool.SetParent(townInfoText, townInfoBG);
            ViewTool.Anchor(townInfoText, new Vector2(0, 0.4F), new Vector2(1.0F, 1.0F));
            townInfoText.alignment = TextAnchor.UpperLeft;
            //按钮
            RectTransform btnsRect = new GameObject("Btns").AddComponent<RectTransform>();
            ViewTool.SetParent(btnsRect, townInfoBG);
            ViewTool.Anchor(btnsRect, new Vector2(0, 0), new Vector2(1.0F, 0.4F));
            btns = new Button[btnsStrs.Length];
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i] = ViewTool.CreateBtn("Btn" + i, btnsStrs[i]);
                BUTTON_ID bid = BUTTON_ID.TOWN_NONE + i + 1;
                btns[i].onClick.AddListener(delegate () { OnClick(bid); });
                ViewTool.SetParent(btns[i], btnsRect);
            }
        }
        public void SetTown(Model.Town town)
        {
            currentTown = town;
        }
        protected override bool PrepareDataBeforeShowWindow()
        {
            return currentTown != null;
        }
        protected override void AfterShowWindow()
        {
            RefreshView();
        }
        public void RefreshView()
        {
            SetTitle(currentTown.Name);
            townInfoText.text = currentTown.Info;
            float delta;
            int showCount;
            if (currentTown.TownType != ETownType.COMMON)
            {
                showCount = btnsStrs.Length;
                ViewTool.SetBtnContent(btns[btnsStrs.Length - 1], currentTown.SpecialBuilding);
                delta = 1.0f / btnsStrs.Length;
            }
            else
            {
                showCount = btnsStrs.Length - 1;
                delta = 1.0f / (btnsStrs.Length - 1);
                //隐藏最后一个按钮
                ViewTool.Anchor(btns[btnsStrs.Length - 1], Vector2.zero, Vector2.zero);
            }
            Vector2 maxAnchor = new Vector2(1.0f, 1.0f);
            Vector2 minAnchor = new Vector2(0.0f, 1.0f - delta);
            for (int i = 0; i < showCount; i++)
            {
                ViewTool.Anchor(btns[i], minAnchor, maxAnchor);
                maxAnchor.y -= delta;
                minAnchor.y -= delta;
            }
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TOWN_TAVERN:
                    Debug.Log("进入酒馆");
                    TavernController tavernController = ControllerManager.GetWindow<TavernController>("TavernViewer");
                    tavernController.SetTown(currentTown);
                    tavernController.Show(this);
                    break;
                case BUTTON_ID.TOWN_SCHOOL:
                    Debug.Log("进入学校");
                    ControllerManager.GetWindow<SchoolController>("SchoolViewer").Show(this);
                    break;
                case BUTTON_ID.TOWN_SHOP:
                    Debug.Log("进入商店");
                    ShopController shopController = ControllerManager.GetWindow<ShopController>("ShopController");
                    shopController.SetTown(currentTown);
                    shopController.Show(this);
                    break;
                case BUTTON_ID.TOWN_SPECIAL_BUILDING:
                    InfoDialog.Show("待开发");
                    break;
            }
        }
    }
}