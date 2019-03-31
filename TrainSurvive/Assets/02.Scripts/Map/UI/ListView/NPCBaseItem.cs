/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/3/30 21:08:03
 * 版本：v0.7
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TTT.UI;
using TTT.Utility;
using WorldMap.Model;

public class NPCBaseItem : BaseItem
{
    private Image icon;
    private Text npcName;
    private Image npcNameBG;
    protected override void CreateModel()
    {
        icon = ViewTool.CreateImage("Icon");
        npcName = ViewTool.CreateText("Name");
        npcNameBG = ViewTool.CreateImage("NameBG");
        npcNameBG.color = new Color(0.8f, 0.8f, 0.8f);
    }

    protected override void InitModel()
    {
        ViewTool.SetParent(icon, this);
        ViewTool.SetParent(npcNameBG, this);
        ViewTool.SetParent(npcName, this);
    }

    protected override void PlaceModel()
    {
        ViewTool.FullFillRectTransform(icon, Vector2.zero, Vector2.zero);
        ViewTool.CenterAt(npcNameBG, new Vector2(0.5f, 0f), new Vector2(0.5f, 0.1f), new Vector2(90f, 30f));
        ViewTool.CenterAt(npcName, new Vector2(0.5f, 0f), new Vector2(0.5f, 0.1f), new Vector2(90f, 30f));
    }
    public void ShowNpc(NpcInfo npc)
    {
        icon.sprite = npc.Icon;
        npcName.text = npc.Name;
    }
}
