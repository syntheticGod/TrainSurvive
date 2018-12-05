/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/2 21:05:17
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using UnityEngine;
using UnityEngine.UI;
using WorldMap;

namespace WorldMap.UI
{
    public class ItemBase : MonoBehaviour
    {
        protected static string spriteFloder = "ItemSprite/";
        protected Image backgroudImage;
        protected Image markImage;
        protected Image targetImage;
        private string targetImageFN;
        private string markImageFN;
        private string backgroudImageFN;
        protected static Color[] markColors = new Color[] { new Color(1, 1, 1), new Color(0.2824f, 0.8824f, 0.2627f), new Color(0.2627f, 0.7569f, 0.8784f), new Color(0.7373f, 0.2627f, 0.8706f), new Color(0.8706f, 0.2706f, 0.2706f) };
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
            backgroudImage = Utility.CreateImage("Backgroud");
            markImage = Utility.CreateImage("Mark");
            markImage.sprite = Resources.Load<Sprite>(spriteFloder + "RarityMark");
            targetImage = Utility.CreateImage("Target");
        }
        protected virtual void InitModel()
        {
            Utility.SetParent(backgroudImage, this);
            Utility.SetParent(targetImage, this);
            Utility.SetParent(markImage, this);
        }
        protected virtual void PlaceModel()
        {
            Utility.FullFillRectTransform(backgroudImage, Vector2.zero, Vector2.zero);
            Utility.FullFillRectTransform(markImage, Vector2.zero, Vector2.zero);
            Utility.FullFillRectTransform(targetImage, Vector2.zero, Vector2.zero);
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
