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

	public int id
    {
        get
        {
            return id;
        }
    }
    public int name
    {
        get
        {
            return name;
        }
    }
    //完成任务的条件，包含了任务进度信息
    public List<TaskRequirement> reqList=new List<TaskRequirement>();
    /// <summary>
    /// 完成后可解锁的任务
    /// </summary>
    public List<int> LatterTaskIDList=new List<int>();
   

}
