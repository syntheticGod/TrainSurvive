/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/8 13:53:07
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;

namespace WorldMap.UI
{

    public interface DialogCallBack
    {
        void OK(BaseDialog dialog);
        void Cancel();
    }
    public abstract class BaseDialog : MonoBehaviour
    {
        private static string[] btnStrs = { "确定", "取消" };
        public DialogCallBack DialogCallBack { set; get; }
        protected Vector2 m_dialogSize = new Vector2(840F, 720F);
        public enum EDialogSizeType
        {
            SMALL7x6,     //420*360
            MIDDLE12x12, //720*720
            MIDDLE14x12   //840*720
        }
        protected EDialogSizeType m_dialogSizeType = EDialogSizeType.MIDDLE14x12;
        protected EDialogSizeType DialogSizeType
        {
            set
            {
                m_dialogSizeType = value;
                float scale = 60;
                if (bgImage != null)
                {
                    switch (value)
                    {
                        case EDialogSizeType.SMALL7x6:
                            m_dialogSize.x = scale * 7;
                            m_dialogSize.y = scale * 6;
                            break;
                        case EDialogSizeType.MIDDLE12x12:
                            m_dialogSize.x = scale * 12;
                            m_dialogSize.y = m_dialogSize.x;
                            break;
                        default:
                        case EDialogSizeType.MIDDLE14x12:
                            m_dialogSize.x = scale * 14;
                            m_dialogSize.y = scale * 12;
                            break;
                    }
                    ViewTool.CenterAt(this, new Vector2(0.5F, 0.5F), new Vector2(0.5F, 0.5F), m_dialogSize);
                }
            }
            get
            {
                return m_dialogSizeType;
            }
        }
        protected WorldForMap world;
        private Image bgImage;
        private Button[] btns;
        private Text titleText;
        protected virtual void Awake()
        {
            world = WorldForMap.Instance;
            CreateBG();
            CreateModel();
        }
        protected virtual void Start()
        {
            AfterDialogShow();
        }
        protected virtual void Update()
        { }
        public static T CreateDialog<T>(string name)
            where T : BaseDialog
        {
            T dialog = ViewTool.ForceGetComponentInChildren<T>(GameObject.Find("Canvas"), name, false);
            return dialog;
        }
        public Button GetOKBtn()
        {
            return btns[0];
        }
        private void CreateBG()
        {
            //背景
            bgImage = CompTool.ForceGetComponent<Image>(this);
            DialogSizeType = m_dialogSizeType;
            //标题
            titleText = ViewTool.CreateText("Title", "TTT");
            ViewTool.SetParent(titleText, this);
            ViewTool.TopFull(titleText, 60F);
            //按钮
            RectTransform btnsRect = new GameObject("Btns").AddComponent<RectTransform>();
            ViewTool.SetParent(btnsRect, this);
            ViewTool.Anchor(btnsRect, new Vector2(0, 0), new Vector2(1, 0));
            btns = new Button[btnStrs.Length];
            Vector2 btnSize = new Vector2(120F, 60F);
            float mid = (btns.Length - 1) / 2F;
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i] = ViewTool.CreateBtn("Btn" + i, btnStrs[i]);
                ViewTool.SetParent(btns[i], btnsRect);
                //居中排序
                ViewTool.CenterAt(btns[i], new Vector2(0.5F, 0.5F), new Vector2(0.5F, 0.5F), btnSize, new Vector2((i - mid) * btnSize.x, 0));
                btns[i].GetComponent<RectTransform>().pivot = new Vector2(0.5F, 0F);
            }
            btns[0].onClick.AddListener(delegate () { if (OK()) { DialogCallBack?.OK(this); CloseDialog(); } });
            btns[1].onClick.AddListener(delegate () { Cancel(); DialogCallBack?.Cancel(); CloseDialog(); });
        }
        public void SetTitle(string title)
        {
            titleText.text = title;
        }
        public void SetBGColor(Color color)
        {
            bgImage.color = color;
        }
        public void ShowDialog()
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
        }
        public void CloseDialog()
        {
            Destroy(gameObject);
        }
        protected abstract void CreateModel();
        protected abstract void AfterDialogShow();
        protected abstract bool OK();
        protected abstract void Cancel();
    }
}