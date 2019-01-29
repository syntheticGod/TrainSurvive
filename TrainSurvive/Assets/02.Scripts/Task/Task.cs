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
using UnityEngine.SceneManagement;
using WorldBattle;

[System.Serializable]
public class Task  {

    public int id;
    public string name;
    public string description;
    /// <summary>
    /// 任务状态，可以直接通过判断任务状态进行后续操作
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
    public SpecialBattle task_battle =null;
    /// <summary>
    /// 尝试接取任务，只有当任务处于可接取状态调用才有效，调用后状态变为进行中，有时接取任务意味着地图上会生成特殊战斗
    /// </summary>
    /// <returns>为true代表该任务有对话战斗，需要ui控件调用任务方法进入
    ///          为false说明没有</returns>
    public bool get_task()
    {
        if (condition != TaskController.TASKCONDITION.CAN_DO)
            return has_talk_ballte();

        if(task_battle!=null&& !task_battle.is_talk_battle)
            SpecialBattleInitializer.getInstance().generateSpecialBattle(task_battle);

        condition = TaskController.TASKCONDITION.DOING;
        return has_talk_ballte();
    }
    /// <summary>
    /// 切换场景，进入该任务的对话战斗，只在该任务确实有对话战斗时才有效
    /// </summary>
    public void enter_talk_battle()
    {
        if (has_talk_ballte())
        {
            InitEnemys.setNextTalkBattle(task_battle.id);
            TimeController.getInstance()?.changeScene(true);
            SceneManager.LoadScene("BattleScene");
        }
    }
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

    private bool has_talk_ballte()
    {
        bool result = false;
        if (task_battle != null)
            result = task_battle.is_talk_battle;
        return result;
    }


}
