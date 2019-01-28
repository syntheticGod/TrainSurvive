/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 20:14:18
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using TTT.Item;
using UnityEngine;

[System.Serializable]
public class Task  {

    public int id;
    public string name;
    public string description;
    /// <summary>
    /// 任务状态
    /// </summary>
    public TaskController.TASKCONDITION condition;

    //完成任务的条件，包含了任务进度信息
    public List<TaskRequirement> reqList=new List<TaskRequirement>();
    /// <summary>
    /// 完成后可解锁的任务
    /// </summary>
    public List<int> LatterTaskIDList=new List<int>();
    /// <summary>
    /// 奖励列表，元组前者为id，后者为数量
    /// </summary>
    public List<ValueTuple<int, int>> rewardList = new List<ValueTuple<int, int>>();
    /// <summary>
    /// 奖励金钱，目前例子里暂时都为0
    /// </summary>
    public int rewardMoney=0;
    /// <summary>
    /// 尝试交付任务，只有当前任务状态处于进行中调用才有效，如果满足任务完成条件则任务状态变成可交付CAN_FINISH，有时交付意味着上交任务物品
    /// </summary>
    public void achieve_task()
    {
        if (condition != TaskController.TASKCONDITION.DOING)
            return;

        bool all_finish = true;
        foreach (TaskRequirement req in reqList)
        {
            if (req.achieveGoal() == false)
            {
                all_finish = false;
                break;
            }
        }
        if (all_finish)
            condition = TaskController.TASKCONDITION.CAN_FINISH;
    }


    /// <summary>
    /// 完成任务，只有当前任务状态处于可交付CAN_FINISH才有效，解锁后续任务，发放奖励，任务状态变成结束FINISH
    /// </summary>
    public void finish_task()
    {
        if (condition != TaskController.TASKCONDITION.CAN_FINISH)
            return;


        TaskController con = TaskController.getInstance();
        foreach(int taskId in LatterTaskIDList)
        {
              if (con.TaskList[taskId] != null)
                  con.TaskList[taskId].condition = TaskController.TASKCONDITION.CAN_DO;
        }
        Storage bag = World.getInstance().storage;
        foreach (ValueTuple<int, int> reward in rewardList)
        {           
            bag.AddItem(reward.Item1, reward.Item2);
        }
        if (rewardMoney > 0)
            World.getInstance().addMoney(rewardMoney);
        condition = TaskController.TASKCONDITION.FINISH;
    }
}
