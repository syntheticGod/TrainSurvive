/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/28 22:31:13
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageButton : Image, IPointerClickHandler
{
    private const int UNUSED = -1;
    public delegate void OnClick(int id, object tag);

    public Color BaseColor { set; get; } = Color.clear;
    public Color ClickedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F, 0.5F);
    public object Tag { set; get; }
    public int ID { set; get; } = UNUSED;
    public OnClick onClick;
    private Text textView;
    protected override void Awake()
    {
        base.Awake();
        color = BaseColor;
        textView = GetComponentInChildren<Text>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("条款被点击了");
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ID == UNUSED)
                return;
            onClick(ID, Tag);
            color = ClickedColor;
        }
    }
    public void normal()
    {
        color = BaseColor;
    }
    public void ShowText(string content)
    {
        textView.text = content;
    }
    public void Clean()
    {
        ID = UNUSED;
        textView.text = "";
        color = BaseColor;
        Tag = null;
    }
}
