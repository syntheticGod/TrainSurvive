/*
 * 描述：资源的UI视图。包括：物品图标、稀有度边框
 *          当鼠标移到视图上时，会出现详细信息框
 * 作者：项叶盛
 * 创建时间：2018/12/2 21:05:17
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TTT.Utility;
using TTT.Item;
using TTT.Resource;
using Assets._02.Scripts.zhxUIScripts;

namespace TTT.UI
{
    public class ResourceItemBase : BaseItem, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void OnItemHoverCallBack(ResourceItemBase item);
        public OnItemHoverCallBack OnItemEnter { set; get; }
        public OnItemHoverCallBack OnItemExit { set; get; }

        protected Image backgroudImage;
        protected Image markImage;
        protected Image targetImage;

        public int ItemID { get; protected set; } = -1;
        
        // TODO:应该从资源文件中读取
        protected static Color[] markColors = new Color[]
        {
            new Color(1, 1, 1),
            new Color(0.2824f, 0.8824f, 0.2627f),
            new Color(0.2627f, 0.7569f, 0.8784f),
            new Color(0.7373f, 0.2627f, 0.8706f),
            new Color(0.8706f, 0.2706f, 0.2706f)
        };
        protected override void CreateModel()
        {
            backgroudImage = ViewTool.CreateImage("Backgroud");
            markImage = ViewTool.CreateImage("Mark");
            markImage.sprite = StaticResource.GetSprite("Sprite/Item/RarityMark");
            targetImage = ViewTool.CreateImage("TargetImage");
        }
        protected override void InitModel()
        {
            ViewTool.SetParent(backgroudImage, this);
            ViewTool.SetParent(targetImage, this);
            ViewTool.SetParent(markImage, this);
        }
        protected override void PlaceModel()
        {
            ViewTool.FullFillRectTransform(backgroudImage, Vector2.zero, Vector2.zero);
            ViewTool.FullFillRectTransform(markImage, Vector2.zero, Vector2.zero);
            ViewTool.FullFillRectTransform(targetImage, Vector2.zero, Vector2.zero);
        }
        /// <summary>
        /// 通过物品的ID设置 图标 稀有度
        /// </summary>
        /// <param name="id">物品ID</param>
        public void SetItemID(int id)
        {
            ItemID = id;
            ItemInfo item = StaticResource.GetItemInfoByID<ItemInfo>(id);
            targetImage.sprite = item.ItemSprite;
            markImage.color = markColors[(int)item.Rarity];
        }
        /// <summary>
        /// 通过物品的ID设置 图标 稀有度
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <param name="rarity">物品稀有度</param>
        public void SetItemIDAndRarity(int id, PublicData.Rarity rarity) {
            ItemID = id;
            ItemInfo item = StaticResource.GetItemInfoByID<ItemInfo>(id);
            targetImage.sprite = item.ItemSprite;
            markImage.color = markColors[(int)rarity];
        }
        /// <summary>
        /// 清除所有东西：id=-1、图片变为默认、稀有度变为最低
        /// </summary>
        public virtual void Clear()
        {
            if (markImage)
                markImage.color = markColors[(int)PublicData.Rarity.Poor];
            if (targetImage)
                targetImage.sprite = null;
            ItemID = -1;
        }
        /// <summary>
        /// 判断是否为空物品
        /// </summary>
        /// <returns></returns>
        public bool IfEmpty()
        {
            return ItemID == -1;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnItemEnter?.Invoke(this);
            //Debug.Log("ItemBase PointerEnter");
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            OnItemExit?.Invoke(this);
            //Debug.Log("ItemBase PointerExit");
        }
    }
}
