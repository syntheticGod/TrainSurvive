/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 16:47:51
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldMap
{
    public class ButtonHandler : MonoBehaviour
    {

        public void OnClick(BUTTON_ID buttonID)
        {
            Debug.Log(ButtonIDBinder.GetButtonName(buttonID) + " clicked");
        }
    }
}