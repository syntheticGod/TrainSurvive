/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/6 15:50:49
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeBaseUI : MonoBehaviour {
    private float game_time_scale;
    //以下均0基
    private int weeksNum;
    private int daysAtWeek;
    private int hour;
    private int minute;
    private bool isAm;
    private float minuteLeft;

    public Text weekNums;
    public Text weeksAt;
    public Text curTime;
    public Text ampm;
    // Use this for initialization
    void Start () {
        TimeController timeCon = TimeController.getInstance();
        game_time_scale = timeCon.getGame_time_scale();
        double gameTime=timeCon.getGameTime();
        int gameMinutes = (int)(gameTime * game_time_scale);
        //日0基
        int days = gameMinutes /1440;
        int todayMinutes= gameMinutes- days*1440;
        weeksNum = days/7;
        daysAtWeek = days%7;
        hour = todayMinutes / 60;
        minute = todayMinutes - hour * 60;
        isAm = true;
        if (hour >= 12)
            isAm = false;
        minuteLeft = 0;
        setCurTimeText(hour, minute);
        setDaysAtWeekText(daysAtWeek);
        setWeekText(weeksNum);
        setAmpm();
    }
	
	// Update is called once per frame
	void Update () {
        float minuteAdd = Time.deltaTime * game_time_scale+ minuteLeft;
        int tempAdd= (int)Mathf.Floor(minuteAdd);
        minuteLeft = minuteAdd - tempAdd;
        minute += tempAdd;
        setCurTimeText(hour, minute);
        if (minute >= 60)
        {
            minute -= 60;
            hour += 1;
            setAmpm();
            setCurTimeText(hour, minute);
            if (hour >= 24)
            {
                hour = 0;
                daysAtWeek += 1;
                setAmpm();
                setCurTimeText(hour, minute);
                setDaysAtWeekText(daysAtWeek);
                if (daysAtWeek >=7)
                {
                    daysAtWeek = 0;
                    weeksNum += 1;
                    setCurTimeText(hour, minute);
                    setDaysAtWeekText(daysAtWeek);
                    setWeekText(weeksNum);
                }
            }
        }
    }

    private void setWeekText(int weeknum)
    {
        weekNums.text = "第" + (weeknum + 1) + "周";
    }
    private void setDaysAtWeekText(int weeksAtnum)
    {
        string str = "";
        switch (weeksAtnum)
        {
            case 0:
                str = "一";
                break;
            case 1:
                str = "二";
                break;
            case 2:
                str = "三";
                break;
            case 3:
                str = "四";
                break;
            case 4:
                str = "五";
                break;
            case 5:
                str = "六";
                break;
            case 6:
                str = "日";
                break;
        }
        weeksAt.text = "星期" + str;
    }
    //参数为24小时制
    private void setCurTimeText(int hour,int minute)
    {
        if (hour >= 12)
            hour -= 12;
        curTime.text = hour+"："+ minute;
    }
    private void setAmpm()
    {
        string str = "a.m.";
        if(hour>=12)
            str= "p.m.";
        ampm.text = str;
    }
}
