/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/2 21:05:17
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Assets._02.Scripts.zhxUIScripts;
using TTT.Utility;

namespace WorldMap.UI
{
    public class HoverImage : Image, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void OnHoverCallBack();
        public OnHoverCallBack onItemEnter { set; get; }
        public OnHoverCallBack onItemExit { set; get; }
        public void OnPointerEnter(PointerEventData eventData)
        {
            onItemEnter.Invoke();
            Debug.Log("PointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onItemExit.Invoke();
            Debug.Log("PointerExit");
        }
    }
    public class ResourceItemBase : BaseItem
    {
        public delegate void OnHoverCallBack(ResourceItemBase item);
        public OnHoverCallBack onItemEnter { set; get; }
        public OnHoverCallBack onItemExit { set; get; }

        protected static string spriteFloder = "ItemSprite/";
        protected Image backgroudImage;
        protected Image markImage;
        protected HoverImage targetImage;
        private string targetImageFN;
        private string markImageFN;
        private string backgroudImageFN;
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
            markImage.sprite = Resources.Load<Sprite>(spriteFloder + "RarityMark");
            targetImage = new GameObject("TargetImage").AddComponent<HoverImage>();
        }
        protected override void InitModel()
        {
            ViewTool.SetParent(backgroudImage, this);
            ViewTool.SetParent(targetImage, this);
            ViewTool.SetParent(markImage, this);
            targetImage.onItemEnter = delegate () { onItemEnter?.Invoke(this); };
            targetImage.onItemExit = delegate () { onItemExit?.Invoke(this); };
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
        /// 设置物品的图片，图片需要存在ItemSprite目录下
        /// </summary>
        /// <param name="name">图片文件名，不需要后缀</param>
        public void SetTargetByName(string name)
        {
            if (name.Equals(targetImageFN))
                return;
            targetImageFN = name;
            Sprite sprite = Resources.Load<Sprite>(spriteFloder + name);
            targetImage.sprite = sprite;
        }
        /// <summary>
        /// 通过item设置图片
        /// </summary>
        /// <param name="item"></param>
        public void SetTarget(Item item)
        {
            SetTargetByName(GetSpriteFileName(item));
        }
        /// <summary>
        /// 根据Item的itemType获取图片文件名
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetSpriteFileName(Item item)
        {
            string filename;
            switch (item.itemType)
            {
                case PublicData.ItemType.Weapon:
                    filename = "Weapon_img";
                    break;
                default:
                case PublicData.ItemType.Material:
                    filename = "Material_img";
                    break;
                case PublicData.ItemType.SpecialItem:
                    filename = "Material_img";
                    break;
            }
            return filename;
        }
    }
}
