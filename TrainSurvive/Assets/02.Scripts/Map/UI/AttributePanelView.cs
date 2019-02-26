/*
 * 描述：属性榜
 * 作者：项叶盛
 * 创建时间：2018/12/26 21:41:50
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TTT.Resource;
using TTT.Utility;
using TTT.UI;
using TTT.Common;

namespace WorldMap.UI
{
    public class AttributePanelView : BaseView
    {
        Text[] attributeInfoText;
        Text[] attributeNumberText;
        public override void CreateView()
        {
            int attriCount = (int)EAttribute.NUM;
            attributeInfoText = new Text[attriCount];
            attributeNumberText = new Text[attriCount];
            float delta = 1f / attriCount;
            float currentFloat = 1F;
            for (int i = 0; i < attriCount; i++)
            {
                attributeInfoText[i] = ViewTool.CreateText("attributeInfo" + i, AttriTool.NameC[i]);
                ViewTool.SetParent(attributeInfoText[i], this);
                ViewTool.Anchor(attributeInfoText[i], new Vector2(0F, currentFloat - delta), new Vector2(0.5F, currentFloat));
                attributeNumberText[i] = ViewTool.CreateText("attributeNumber" + i);
                ViewTool.SetParent(attributeNumberText[i], this);
                ViewTool.Anchor(attributeNumberText[i], new Vector2(0.5F, currentFloat - delta), new Vector2(1F, currentFloat));
                currentFloat -= delta;
            }
        }
        public void SetNumber(EAttribute attribute, int number)
        {
            attributeNumberText[(int)attribute].text = number + "";
        }
        public void SetNumbers(int[] numbers)
        {
            for (int i = 0; i < numbers.Length && i < attributeNumberText.Length; i++)
            {
                attributeNumberText[i].text = numbers[i] + "";
            }
        }
    }
}

