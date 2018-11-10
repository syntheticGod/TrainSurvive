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
                listener.OnClick(buttonID);
            }
        }
        public static bool IsTeam(BUTTON_ID id)
        {
            return id > BUTTON_ID.TEAM_NONE && id < BUTTON_ID.TEAM_NUM;
        }
        public static bool IsTrain(BUTTON_ID id)
        {
            return id > BUTTON_ID.TRAIN_NONE && id < BUTTON_ID.TRAIN_NUM;
        }
    }
}