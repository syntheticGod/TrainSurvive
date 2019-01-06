/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/1/6 10:17:23
 * 版本：v0.7
 */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace TTT.UI
{
    public class HoverRange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
}

