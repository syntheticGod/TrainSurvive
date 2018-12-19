/*
 * 描述：城镇控制器，控制城市界面的显示与用户操作
 * 作者：项叶盛
 * 创建时间：2018/11/20 23:14:19
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;

namespace WorldMap.Controller
{
    public class TownController : WindowsController
    {
        private const int ECHO_CODE_TRAIN = 1;
        private const int ECHO_CODE_TEAM = 2;
        private static string[] btnsStrs = { "商店", "学校", "酒馆" };
        private TavernController tavernController;
        private Model.Town currentTown;
        private Text townInfoText;
        protected override void OnEnable()
        { }
        protected override void OnDisable()
        { }
        protected override void CreateModel()
        {
            m_windowSizeType = EWindowSizeType.BIG26x14;
            m_titleString = "城镇";
            base.CreateModel();
            SetBackground("town_bg_02");
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
            Button[] btns = new Button[btnsStrs.Length];
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i] = ViewTool.CreateBtn("Btn" + i, btnsStrs[i]);
                ViewTool.SetParent(btns[i], btnsRect);
                ViewTool.Anchor(btns[i], new Vector2(0F, (float)i / btns.Length), new Vector2(1.0F, (float)(i + 1) / btns.Length));
                BUTTON_ID bid = BUTTON_ID.TOWN_NONE + i + 1;
                btns[i].onClick.AddListener(delegate () { OnClick(bid); });
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
            townInfoText.text = currentTown.Info;
        }
        public bool TryShowTown(Vector2Int mapPos)
        {
            Model.Town town;
            if (!world.FindTown(mapPos, out town))
            {
                Debug.Log("当前列车位置不在城镇");
                return false;
            }
            ShowTwon(town);
            return true;
        }
        private void ShowTwon(Model.Town town)
        {
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("城镇界面已经显示");
                return;
            }
            gameObject.SetActive(true);
            Debug.Log("城镇：" + town.Info);
            currentTown = town;
        }
        public void OnClick(BUTTON_ID id)
        {
            switch (id)
            {
                case BUTTON_ID.TOWN_TAVERN:
                    Debug.Log("进入酒馆");
                    TavernController tavernController = ControllerManager.Instance.GetWindow<TavernController>("TavernViewer");
                    tavernController.SetTown(currentTown);
                    tavernController.Show(this);
                    break;
                case BUTTON_ID.TOWN_SCHOOL:
                    Debug.Log("进入学校");
                    ControllerManager.Instance.GetWindow<SchoolController>("SchoolViewer").Show(this);
                    break;
                case BUTTON_ID.TOWN_SHOP:
                    Debug.Log("进入商店");
                    ShopController shopController = ControllerManager.Instance.GetWindow<ShopController>("ShopController");
                    shopController.SetTown(currentTown);
                    shopController.Show(this);
                    break;
            }
        }

        public string GetName()
        {
            return "TownController";
        }
    }
}