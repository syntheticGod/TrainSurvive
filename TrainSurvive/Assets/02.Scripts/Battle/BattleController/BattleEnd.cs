/*
 * 描述：这是战斗结束场景类
 * 掉落战利品
 * 作者：王安鑫
 * 创建时间：2018/12/14 21:50:05
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Collections;
using System.Collections.Generic;
using TTT.Utility;
using UnityEngine;
using WorldMap.UI;

namespace WorldBattle {

    public class DropSpoils {
        //战利品材料图片的高度
        private const float picHeight = 100.0f;
        //战利品材料图片的宽度
        private const float picWidth = 100.0f;
        //设置一排最多显示多少个
        private const int widthMax = 5;
        //设置图片初始的位置
        private static Vector2 orign = new Vector2(-200, 100);

        public static void setItem(Transform parent, Item item, int index) {
            //创建一个空的gameObject
            GameObject gameObject = new GameObject(item.name);
            //将当前图片显示依附于此object
            ItemDisplay view = CompTool.ForceGetComponent<ItemDisplay>(gameObject);
            //设置相应的图片
            view.setItem(item);

            //更改gameObject的parent
            gameObject.transform.parent = parent;

            //添加RectTransform
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            //设置其瞄点居中
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            //获取Rect
            Rect rect = rectTransform.rect;
            //设置图片的大小
            rect.width = picWidth;
            rect.height = picHeight;

            //设置图片的位置
            rectTransform.anchoredPosition = new Vector2(
                orign.x + index % widthMax * (picWidth + 1),
                orign.y - index / widthMax * (picHeight + 1)
                );
        }
    }

    /// <summary>
    /// 这个是用来显示图片的类
    /// </summary>
    public class ItemDisplay : TeamPackItem {
        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="item">物品类型</param>
        /// <param name="itemNum">物品数量</param>
        public void setItem(Item item, int itemNum = 1) {
            //设置物品稀有度
            SetMarkLevel((int)item.rarity);
            //设置物品的图片
            SetTarget(item);
            //设置物品数量
            SetNumber(itemNum);
        }
    }
}

