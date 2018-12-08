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
        public void AddListener(OnClickListener listener)
        {
            Debug.Log("add listener "+listener.GetName()+" "+listeners.Count + " address:" + listener.GetHashCode());
            listeners.Add(listener);
        }
        public void RemoveListener(OnClickListener listener)
        {
            Debug.Log("remove listener " + listener.GetName() + " " + listeners.Count + " address:" + listener.GetHashCode());
            listeners.Remove(listener);
        } 
        public void OnClick(BUTTON_ID buttonID)
        {
            Debug.Log(ButtonIDBinder.GetButtonName(buttonID) + " clicked");
            List<OnClickListener> accepted = new List<OnClickListener>();
            foreach(var listener in listeners)
            {
                if (listener.IfAccepted(buttonID))
                    accepted.Add(listener);
            }
            foreach(var listener in accepted)
            {
                listener.OnClick(buttonID);
            }
        }
    }
    public enum BUTTON_ID
    {
        NONE,
        
        TEAM_NONE,//小队模式显示的按键
        TEAM_ENTRY_AREA,//小队进入区域
        TEAM_RETRUN,//小队回车
        TEAM_GATHER,//小队采集
        TEAM_PACK,//小队背包
        TEAM_NUM,

        TRAIN_NONE,//列车模式显示的按钮
        TRAIN_ENTRY_AREA,//进入区域
        TRAIN_TEAM_ACTION,//小队下车行动
        TRAIN_RUN_OR_STOP,//开/停车
        TRAIN_CHANGE,//列车内部
        TRAIN_NUM,

        TOWN_NONE,//城镇界面显示的按钮
        TOWN_SHOP,//商店
        TOWN_SCHOOL,//学校
        TOWN_TAVERN,//酒馆
        TOWN_NUM,

        TAVERN_NONE,//酒馆界面
        TAVERN_BUTTON3,
        TAVERN_BUTTON2,
        TAVERN_BUTTON1,
        TAVERN_NUM,

        TEAM_SELECT_DIALOG_NONE,//探险队选择框
        TEAM_SELECT_FOOD_PLUS,//增加食物
        TEAM_SELECT_FOOD_SUBTRCT,//减少食物
        TEAM_SELECT_DIALOG_NUM,
        NUM,
    }
}