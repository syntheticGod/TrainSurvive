/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 20:14:18
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task  {

    public int id;
    public int npcId;
    public string name;

    //完成任务的条件，包含了任务进度信息
    public List<TaskRequirement> reqList=new List<TaskRequirement>();
    /// <summary>
    /// 完成后可解锁的任务
    /// </summary>
    public List<int> LatterTaskIDList=new List<int>();
   
    /// <summary>
    /// 只有所有条件都满足时才有效
    /// </summary>
    public void finish_task()
    {
        bool all_finish = true;
        foreach(TaskRequirement req in reqList)
        {
            if (req.isfinish == false)
            {
                all_finish = false;
                break;
            }
        }
        if (all_finish)
        {
            TaskController con = TaskController.getInstance();
            foreach(int taskId in LatterTaskIDList)
            {
                Task temp = con.Task_locked[taskId];
                con.Task_locked.Remove(taskId);
                con.Task_canDo.Add(taskId, temp);
                World.getInstance().FindNPCByID(temp.npcId).taskId_canDo.Add(taskId);
            }
            con.Task_finish.Add(id, this);
            World.getInstance().FindNPCByID(npcId).taskId_doing.Remove(id);          
            con.Task_doing.Remove(id);
        }
    }
}
