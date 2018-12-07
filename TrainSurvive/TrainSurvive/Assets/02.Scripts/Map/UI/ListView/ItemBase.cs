/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/2 21:05:17
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour
{
    protected static string spriteFloder = "ItemSprite/";
    protected Image backgroudImage;
    protected Image markImage;
    protected Image targetImage;
    protected static Color[] markColors = new Color[] { new Color(1, 1, 1), new Color(0.2824f, 0.8824f, 0.2627f), new Color(0.2627f, 0.7569f, 0.8784f), new Color(0.7373f, 0.2627f, 0.8706f), new Color(0.8706f, 0.2706f, 0.2706f) };
    protected int defaultFontSize = 20;
    void Awake()
    {
        CreateModel();
        InitModel();
        PlaceModel();

    }
    void Start()
    { }
    protected virtual void CreateModel()
    {
        backgroudImage = CreateImage("Backgroud");
        markImage = CreateImage("Mark");
        markImage.sprite = Resources.Load<Sprite>(spriteFloder + "RarityMark");
        targetImage = CreateImage("Target");
    }
    protected virtual void InitModel()
    {
        SetParent(backgroudImage, this);
        SetParent(targetImage, this);
        SetParent(markImage, this);
    }
    protected virtual void PlaceModel()
    {
        FullFillRectTransform(backgroudImage, this);
        FullFillRectTransform(markImage, this);
        FullFillRectTransform(targetImage, this);
    }
    public void MarkLevel(int level)
    {
        if (level < markColors.Length)
            markImage.color = markColors[level];
    }
    public void SetTarget(Sprite sprite)
    {
        targetImage.sprite = sprite;
    }
    public void SetTargetByName(string name)
    {
        targetImage.sprite = Resources.Load<Sprite>(spriteFloder + name);
    }
    //----------
    protected Image CreateImage(string name)
    {
        return new GameObject(name, typeof(Image)).GetComponent<Image>();
    }
    protected Text CreateText(string name)
    {
        Text text = new GameObject(name, typeof(Text)).GetComponent<Text>();
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = defaultFontSize;
        text.alignment = TextAnchor.MiddleCenter;
        return text;
    }
    protected Button CreateBtn(string name, string content)
    {
        Button btn = new GameObject(name, typeof(Button), typeof(RectTransform), typeof(Image)).GetComponent<Button>();
        Image image = btn.GetComponent<Image>();
        //image.sprite = Resources.Load<Sprite>("unity_builtin_extra/UISprite");
        btn.targetGraphic = image;
        Text text = CreateText("Text");
        text.text = content;
        SetParent(text, btn);
        FullFillRectTransform(text, btn);
        return btn;
    }
    protected void SetParent(Component child, Component parent)
    {
        child.transform.SetParent(parent.transform);
        child.transform.localPosition = Vector2.zero;
    }
    protected void FullFillRectTransform(Component child, Component parent)
    {
        RectTransform rect = parent.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    protected void SetAnchor(Component comp, float minX, float minY, float maxX, float maxY)
    {
        RectTransform rect = comp.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    protected void SetSize(Component comp, float width, float height)
    {
        RectTransform rect = comp.GetComponent<RectTransform>();
        width /= 2;
        height /= 2;
        rect.offsetMin = new Vector2(-width, -height);
        rect.offsetMax = new Vector2(width, height);
    }
}
