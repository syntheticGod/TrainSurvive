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
    public class ButtonHandler
    {
        private List<OnClickListener> listeners;
        private static ButtonHandler buttonHandler;
        public static ButtonHandler Instance
        {
            get
            {
                if (null == buttonHandler)
                    buttonHandler = new ButtonHandler();
                return buttonHandler;
            }
        }

        private ButtonHandler()
        {
            listeners = new List<OnClickListener>();
        }
        public void AddListeners(OnClickListener listener)
        {
            listeners.Add(listener);
        }
        public void OnClick(BUTTON_ID buttonID)
        {
            Debug.Log(ButtonIDBinder.GetButtonName(buttonID) + " clicked");
            foreach(var listener in listeners)
            {
                if(listener.IfAccepted(buttonID))
                    listener.OnClick(buttonID);
            }
        }
    }
    public enum BUTTON_ID
    {
        NONE,
        
        TEAM_NONE,//小队模式显示的按键
        TEAM_RETRUN,//小队回车
        TEAM_NUM,

        TRAIN_NONE,//列车模式显示的按钮
        TRAIN_RUN,
        TRAIN_STOP,
        TRAIN_TEAM_ACTION,//小队下车行动
        TRAIN_CHANGE,
        TRAIN_NUM,

        TOWN_NONE,//城镇界面显示的按钮
        TOWN_TAVERN,//酒馆
        TOWN_SCHOOL,//学校
        TOWN_SHOP,//商店
        TOWN_NUM,

        TAVERN_NONE,//酒馆界面
        TAVERN_RECRUIT,//招募NPC
        TAVERN_NUM,

        NUM,
    }
}