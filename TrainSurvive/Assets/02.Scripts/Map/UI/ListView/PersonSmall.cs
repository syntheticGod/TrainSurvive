/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/12/5 5:01:55
 * 版本：v0.1
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WorldMap.UI
{
    public class PersonSmall : MonoBehaviour
    {
        private void Awake()
        {
            Text text = Utility.CreateText("Name");
            Utility.SetParent(text, this);
            Utility.FullFillRectTransform(text, Vector2.zero, Vector2.zero);
        }
    }
}
