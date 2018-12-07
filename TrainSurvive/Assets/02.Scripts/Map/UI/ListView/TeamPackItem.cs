/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/3 9:25:40
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace WorldMap.UI
{
    public class TeamPackItem : ItemBase
    {
        protected Text numView;
        void Start()
        { }

        void Update()
        { }
        protected override void CreateModel()
        {
            base.CreateModel();
            numView = Utility.CreateText("Number");
        }
        protected override void InitModel()
        {
            base.InitModel();
            Utility.SetParent(numView, this);
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