/*
 * 描述：物品的UI视图。包括：物品图标、稀有度边框、物品数量
 * 作者：项叶盛
 * 创建时间：2018/12/3 9:25:40
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TTT.Utility;

namespace TTT.UI
{
    public class AssetsItemView : ResourceItemBase
    {
        public int Number { get; private set; }
        protected Text numView;
        //Override
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
        public override void Clear()
        {
            base.Clear();
            if (numView)
                numView.text = "";
            Number = 0;
        }
        //Operations
        /// <summary>
        /// 设置物品视图的数量
        /// </summary>
        /// <param name="number"></param>
        public void SetNumber(int number)
        {
            Number = number;
            numView.text = number.ToString();
        }
        /// <summary>
        /// 根据物品的ID和数量 设置单元格视图
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <param name="number">物品数量</param>
        public void SetItemData(int id, int number)
        {
            SetItemID(id);
            Number = number;
            numView.text = number.ToString();
        }
        /// <summary>
        /// 设置物体的数据，只获取ItemData中的ID和数量
        /// </summary>
        /// <param name="data"></param>
        public void SetItemData(ItemData data)
        {
            SetItemID(data.ID);
            Number = data.Number;
            numView.text = data.Number.ToString();
        }
    }
}