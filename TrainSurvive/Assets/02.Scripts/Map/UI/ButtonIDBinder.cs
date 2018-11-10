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
    public enum BUTTON_ID
    {
        //小队按键时间
        TEAM_NONE,
        TEAM_ACTION,//小队下车行动
        TEAM_RETRUN,//小队回车
        TEAM_NUM,

        TRAIN_NONE,
        TRAIN_RUN,
        TRAIN_STOP,
        TRAIN_NUM,
        NUM
    }
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
            ButtonHandler bc = transform.parent.Find("ButtonHandler").GetComponent<ButtonHandler>();
            Debug.Assert(null != bc, "缺少ButtonHandler游戏对象");
            button.onClick.AddListener(delegate()
            {
                bc.OnClick(ButtonID);
            });
        }
    }
}