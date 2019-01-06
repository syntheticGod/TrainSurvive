/*
 * 描述：资源的UI视图。包括：物品图标、稀有度边框
 * 作者：项叶盛
 * 创建时间：2018/12/2 21:05:17
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TTT.Utility;
using TTT.UI;

using TTT.Item;
using TTT.Resource;
using Assets._02.Scripts.zhxUIScripts;

namespace WorldMap.UI
{
    public class ResourceItemBase : BaseItem, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void OnItemHoverCallBack(ResourceItemBase item);
        public OnItemHoverCallBack onItemEnter { set; get; }
        public OnItemHoverCallBack onItemExit { set; get; }

        protected Image backgroudImage;
        protected Image markImage;
        protected Image targetImage;

        public int Index { get; set; }
        public int ItemID { get; protected set; } = -1;
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
            markImage.sprite = StaticResource.GetSprite("ItemSprite/RarityMark");
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
        /// 设置物品的稀有度
        /// </summary>
        /// <param name="level">取值范围[0,1,2,3,4] 4是最高稀有度</param>
        public void SetMarkLevel(int level)
        {
            if (level < markColors.Length)
                markImage.color = markColors[level];
        }
        /// <summary>
        /// 通过物品的ID设置 图标 稀有度
        /// </summary>
        /// <param name="id">物品ID</param>
        public void SetItemID(int id)
        {
            ItemID = id;
            ItemInfo item = StaticResource.GetItemInfoByID<ItemInfo>(id);
            targetImage.sprite = item.BigSprite;
            markImage.color = markColors[(int)item.Rarity];
        }
        public virtual void Clear()
        {
            markImage.color = markColors[(int)PublicData.Rarity.Poor];
            targetImage.sprite = null;
            Index = -1;
            ItemID = -1;
        }
        public bool IfEmpty()
        {
            return ItemID == -1;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            onItemEnter?.Invoke(this);
            //Debug.Log("ItemBase PointerEnter");
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            onItemExit?.Invoke(this);
            //Debug.Log("ItemBase PointerExit");
        }
    }
}
