/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 20:17:47
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TaskRequirement  {
    /// <summary>
    /// 当前任务条件执行情况，用于任务面板展示（简短）
    /// </summary>
    public string condition
    {
        get; protected set;
    }
    /// <summary>
    /// 对于该任务条件的描述，用于NPC对话展示（具体）
    /// </summary>
    public string description
    {
        get; protected set;
    }
    public bool isfinish
    {
        get; protected set;
    }

    /// <summary>
    /// 尝试完成该条件，需对派生类具体实现，若条件完成返回true
    /// </summary>
    /// <returns></returns>
    public abstract bool achieveGoal();
    /// <summary>
    /// 任务情况发生改变的适合调用，目前仅killreq会具体实现
    /// </summary>
    /// <param name="numOrId"></param>
    public abstract void conditionChange(int numOrId);
}
