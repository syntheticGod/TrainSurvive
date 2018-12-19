/*
 * 描述：这是一个帮助类，帮助gameObject居中等
 * 作者：王安鑫
 * 创建时间：2018/12/19 22:14:57
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class HelpGameObjectAnchor {
        /// <summary>
        /// 帮助物体居中
        /// </summary>
        /// <param name="gameObject"></param>
        public static void setCenter(GameObject gameObject) {
            //获得rectTransform
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            //设置anchor min max
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            //设置其position为0,0
            rectTransform.anchoredPosition = new Vector3();
        }
    }
}

