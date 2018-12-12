/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/2 21:05:17
 * 版本：v0.1
 */
using UnityEngine;
using UnityEngine.UI;

using Assets._02.Scripts.zhxUIScripts;
using TTT.Utility;

namespace WorldMap.UI
{
    public class ResourceItemBase : BaseItem
    {
        protected static string spriteFloder = "ItemSprite/";
        protected Image backgroudImage;
        protected Image markImage;
        protected Image targetImage;
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
            targetImage = ViewTool.CreateImage("Target");
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
        public void SetMarkLevel(int level)
        {
            if (level < markColors.Length)
                markImage.color = markColors[level];
        }
        public void SetTargetByName(string name)
        {
            if (name.Equals(targetImageFN)) return;
            Sprite sprite = Resources.Load<Sprite>(spriteFloder + name);
            targetImage.sprite = sprite;
        }
        public void SetTarget(Item item)
        {
            SetTargetByName(GetSpriteFileName(item));
        }
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
