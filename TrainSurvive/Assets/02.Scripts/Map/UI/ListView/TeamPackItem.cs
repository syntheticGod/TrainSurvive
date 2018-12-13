/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 9:25:40
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using TTT.Utility;

namespace WorldMap.UI
{
    public class TeamPackItem : ResourceItemBase
    {
        protected Text numView;
        protected override void CreateModel()
        {
            base.CreateModel();
            numView = ViewTool.CreateText("Number");
        }
        protected override void InitModel()
        {
            base.InitModel();
            ViewTool.SetParent(numView, this);
        }
        
        protected override void PlaceModel()
        {
            base.PlaceModel();
            RectTransform rect = numView.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.6F, 0.2F);
            rect.anchorMax = new Vector2(0.9F, 0.4F);
        }
        public void SetNumber(int number)
        {
            numView.text = number.ToString();
        }
    }
}