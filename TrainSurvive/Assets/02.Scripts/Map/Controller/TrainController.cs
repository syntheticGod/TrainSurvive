/*
 * 描述：玩家控制列车移动的控制器，
 *          该脚本应该绑定到列车对象上。
 * 作者：项叶盛
 * 创建时间：2018/10/31 0:29:36
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using WorldMap.Model;
using WorldMap.UI;
using TTT.Resource;
using TTT.Utility;
using TTT.Controller;

namespace WorldMap.Controller
{
    public class TrainController : BaseController, Observer
    {
        private const int levelOfTrain = -1;
        //列车状态映射显示信息
        private static string[] trainActionBtnStrs = { "开车", "开车", "停车中...", "停车" };
        private const int ECHO_FROM_TRAIN = 1;
        private static string[] bottomBtnsStrs = { "进入区域", "小队行动", "开车", "列车内部" };
        private Button[] bottomBtns;
        private Text trainActionBtn;
        //主摄像机焦点控制器
        private ICameraFocus cameraFocus;
        //外部引用
        private RectTransform trainModeBTs;
        protected override void CreateModel()
        {
            //Buttons
            trainModeBTs = new GameObject("TrainMode").AddComponent<RectTransform>();
            ViewTool.SetParent(trainModeBTs, GameObject.Find("/Canvas").transform);
            ViewTool.RightBottom(trainModeBTs, new Vector2(1, 0), Vector2.zero, Vector2.zero);
            Vector2 btnPivot = new Vector2(1, 0);
            Vector2 btnSize = new Vector2(120, 120);
            float btnSpace = 6;
            bottomBtns = new Button[bottomBtnsStrs.Length];
            for (int i = 0; i < bottomBtnsStrs.Length; i++)
            {
                bottomBtns[i] = ViewTool.CreateBtn("Btn" + i, bottomBtnsStrs[i]);
                ViewTool.SetParent(bottomBtns[i], trainModeBTs);
                ViewTool.RightBottom(bottomBtns[i], btnPivot, btnSize, new Vector2((btnSize.x + btnSpace) * -i, 0));
                BUTTON_ID btnID = BUTTON_ID.TRAIN_NONE + i + 1;
                bottomBtns[i].onClick.AddListener(delegate () { OnClick(btnID); });
            }
            ActiveBTs(!WorldForMap.Instance.IfTeamOuting);
            trainActionBtn = bottomBtns[2].transform.Find("Text").GetComponent<Text>();
            //列车图标
            GOTool.CreateSpriteRenderer("Train", transform).sprite = StaticResource.GetSprite(ESprite.TRAIN);
            cameraFocus = Camera.main.GetComponent<ICameraFocus>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Train.Instance.Attach(this, ECHO_FROM_TRAIN);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            Train.Instance.Detach(obs: this);
        }
        protected override void Start()
        {
            Debug.Log("TrainController Start");
            base.Start();
            transform.position = StaticResource.MapPosToWorldPos(Train.Instance.PosTrain, levelOfTrain);
        }
        protected override void Update()
        {
            base.Update();
            MouseEventDetecter();
            Train train = Train.Instance;
            if (train.IsMovable == false) return;
            //点击事件处理
            if (LeftMouseState == MouseState.Click)
            {
                //如果检测到是UI层，则不处理。
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                Ray ray = Camera.main.GetComponent<Camera>()
                    .ScreenPointToRay(Input.mousePosition);
                //因为摄像机的Projection 为 Orthographic，所以Ray的方向都是平行的
                Debug.Log("origin of ray:" + ray.origin + " dire:" + ray.direction);
                Debug.Log("mouse position " + Input.mousePosition);
                Vector2 clickedPosition = StaticResource.WorldPosToMapPos(ray.origin);
                if (Map.GetInstance().isSpawnVisible(StaticResource.BlockIndex(clickedPosition)))
                {
                    if (!train.StartRun(clickedPosition))
                    {
                        Debug.Log("列车行动失败");
                        return;
                    }
                }
                else
                {
                    InfoDialog.Show("点击处被迷雾环绕");
                    return;
                }
            }
            Vector2 current = StaticResource.WorldPosToMapPos(transform.position);
            if (train.Run(ref current))
                transform.position = StaticResource.MapPosToWorldPos(current, levelOfTrain);
        }
        /// <summary>
        /// UI按钮的点击事件，地图的鼠标事件在Update中处理。
        /// </summary>
        /// <param name="id">按钮的ID</param>
        public void OnClick(BUTTON_ID id)
        {
            Train train = Train.Instance;
            switch (id)
            {
                case BUTTON_ID.TRAIN_RUN_OR_STOP:
                    if (train.IsRunning)
                    {
                        Debug.Log("停车指令");
                        if (!train.StopTemporarily())
                        {
                            Debug.Log("开车指令失败");
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("开车指令");
                        if (!train.ContinueRun())
                        {
                            Debug.Log("开车指令失败");
                            break;
                        }
                    }
                    break;
                case BUTTON_ID.TRAIN_ENTRY_AREA:
                    Debug.Log("进入区域指令");
                    //TODO：目前只有城镇
                    Model.Town town;
                    if (WorldForMap.Instance.FindTown(train.MapPosTrain, out town))
                    {
                        //进入城镇后不能操作列车
                        Train.Instance.IsMovable = false;
                        TownController townController = ControllerManager.GetWindow<TownController>("TownViewer");
                        townController.SetTown(town);
                        townController.Show(this);
                    }
                    else
                    {
                        InfoDialog.Show("该区域不可进入，目前只能进城镇。");
                    }
                    break;
                case BUTTON_ID.TRAIN_TEAM_ACTION:
                    Debug.Log("探险队行动");
                    if (train.IsRunning)
                    {
                        InfoDialog.Show("列车正在运行，无法出队");
                    }
                    else
                    {

                        Train.Instance.IsMovable = false;
                        ActiveBTs(false);
                        Team.Instance.OutPrepare(Train.Instance.PosTrain);
                        ControllerManager.ShowController("Team", "Character");
                        UnFocus();
                    }
                    break;
                case BUTTON_ID.TRAIN_CHANGE:
                    SceneManager.LoadScene("TrainScene");
                    break;
            }
        }
        public void OK(BaseDialog baseDialog)
        {
            if (!(baseDialog is TeamOutPrepareDialog)) return;
            TeamOutPrepareDialog dialog = baseDialog as TeamOutPrepareDialog;
            //险队准备
        }
        public void Cancel()
        {
            //取消区域后
            Train.Instance.IsMovable = true;
        }
        private bool ActiveBTs(bool active)
        {
            if (trainModeBTs.gameObject.activeSelf == active)
            {
                Debug.Log("重复激活按钮，无效");
                return false;
            }
            trainModeBTs.gameObject.SetActive(active);
            return true;
        }
        protected override bool FocusBehaviour()
        {
            ActiveBTs(true);
            Train.Instance.IsMovable = true;
            cameraFocus.focusLock(transform);
            return true;
        }
        protected override bool UnfocusBehaviour()
        {
            ActiveBTs(false);
            Train.Instance.IsMovable = false;
            return true;
        }
        protected override bool ShowBehaviour()
        {
            return false;
        }
        protected override bool HideBehaviour()
        {
            return false;
        }
        public void ObserverUpdate(int state, int echo)
        {
            switch (echo)
            {
                case ECHO_FROM_TRAIN:
                    if (state < trainActionBtnStrs.Length)
                        trainActionBtn.text = trainActionBtnStrs[state];
                    else
                        Debug.LogError("系统：该列车状态无对应的显示信息");
                    break;
            }
        }

    }
}
