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
    public Color BaseColor { set; get; } = Color.clear;
    public Color ClickedColor { set; get; } = new Color(0.5F, 0.5F, 0.5F, 0.5F);
    public int ID;
    public delegate void OnClick(int id);
    public OnClick onClick;
    protected override void Awake()
    {
        base.Awake();
        color = BaseColor;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("条款"+ID+"被点击了");
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            onClick(ID);
            color = ClickedColor;
        }
    }
    public void normal()
    {
        color = BaseColor;
    }
}
