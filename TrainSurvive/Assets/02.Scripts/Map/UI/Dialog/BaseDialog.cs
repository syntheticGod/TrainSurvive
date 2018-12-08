/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/8 13:53:07
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
        protected DialogCallBack CallBack { set; get; }
        protected WorldForMap world;
        private Image bgImage;
        private Text titleText;
        protected virtual void Awake()
        {
            world = WorldForMap.Instance;
            CreateBG();
            CreateModel();
        }
        private void CreateBG()
        {
            //背景
            bgImage = Utility.ForceGetComponent<Image>(this);
            Utility.FullFillRectTransform(bgImage);
            //标题
            titleText = Utility.CreateText("Title", "TTT");
            Utility.SetParent(titleText, this);
            Utility.TopFull(titleText, 60F);
            //按钮
            RectTransform btnsRect = new GameObject("Btns").AddComponent<RectTransform>();
            Utility.SetParent(btnsRect, this);
            Utility.Anchor(btnsRect, new Vector2(0, 0), new Vector2(1, 0));
            Button[] btns = new Button[btnStrs.Length];
            Vector2 btnSize = new Vector2(120F, 60F);
            float mid = (btns.Length-1) / 2F;
            for(int i = 0; i < btns.Length; i++)
            {
                btns[i] = Utility.CreateBtn("Btn" + i, btnStrs[i]);
                Utility.SetParent(btns[i], btnsRect);
                //居中排序
                Utility.CenterAt(btns[i], new Vector2(0.5F, 0.5F), btnSize, new Vector2((i-mid)*btnSize.x, 0));
            }
        }
        public void SetTitle(string title)
        {
            titleText.text = title;
        }
        protected virtual void Start()
        { }
        protected virtual void Update()
        { }
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
        
    }
}