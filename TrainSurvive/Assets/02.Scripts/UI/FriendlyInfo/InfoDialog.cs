/*
 * 描述：消息提示框
 * 作者：项叶盛
 * 创建时间：2018/12/9 16:22:55
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using TTT.Utility;

namespace WorldMap.UI
{
    public class InfoDialog : BaseDialog
    {
        private Text InfoText;
        private string infoString;
        protected override void CreateModel()
        {
            SetTitle("提示");
            ColorBlock redColorBlock = GetOKBtn().colors;
            redColorBlock.normalColor = new Color(1, 0, 0);
            redColorBlock.highlightedColor = new Color(0.9F, 0, 0);
            redColorBlock.pressedColor = new Color(0.8F, 0, 0);
            redColorBlock.disabledColor = new Color(0.8F, 0.8F, 0.8F);
            GetOKBtn().colors = redColorBlock;
            SetBGColor(new Color(1F, 0.8F, 0.8F));
            DialogSizeType = EDialogSizeType.SMALL7x6;
            InfoText = ViewTool.CreateText("InfoText");
            ViewTool.SetParent(InfoText, this);
            ViewTool.Anchor(InfoText, new Vector2(0F, 0F), new Vector2(1F, 1F), new Vector2(0F, 60F), new Vector2(0F, -60F));
        }
        protected override void AfterDialogShow()
        {
            InfoText.text = infoString;
        }
        protected override bool OK()
        {
            return true;
        }
        protected override void Cancel()
        { }
        public void SetInfo(string info)
        {
            infoString = info;
        }
        public static InfoDialog Show(string content)
        {
            InfoDialog infoDialog = CreateDialog<InfoDialog>("DEFAULT_INFO_DIALOG_NAME");
            infoDialog.SetInfo(content);
            infoDialog.ShowDialog();
            return infoDialog;
        }
    }
}

