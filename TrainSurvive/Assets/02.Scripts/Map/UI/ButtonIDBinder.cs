/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 17:52:01
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace WorldMap
{
    public class ButtonIDBinder : MonoBehaviour
    {
        public static string[] BUTTON_NAMES =
            { "NONE", "TeamActionBT", "TeamReturnBT", "NUM" ,
            "NONE","TrainRunBT","TrainStopBT","NUM" };
        public static string GetButtonName(BUTTON_ID id)
        {
            return BUTTON_NAMES[(int)id];
        }
        public BUTTON_ID ButtonID { get; private set; }
        void Start()
        {
            //绑定按钮的ID
            for(int i = 0; i < (int)BUTTON_ID.NUM; i++)
            {
                if (BUTTON_NAMES[i].Equals(name))
                    ButtonID = (BUTTON_ID)i;
            }
            Debug.Assert(0 != ButtonID, "未找到相应的ButtonID");
            Button button = GetComponent<Button>();
            Debug.Assert(null != button, "改脚本必须绑定在Button上");
            button.onClick.AddListener(delegate()
            {
                ButtonHandler.Instance.OnClick(ButtonID);
            });
        }
    }
}