/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/21 10:54:40
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;


[System.Serializable]
public class TaskController  {
    public enum TASKCONDITION
    {
        LOCKED=1,
        CAN_DO,
        Doing
    }

    private static TaskController instance = null;
    /// <summary>
    /// 未解锁的任务
    /// </summary>
    public SerializableDictionary<int, Task> Task_locked = new SerializableDictionary<int, Task>();
    /// <summary>
    /// 已解锁但未接取的任务
    /// </summary>
    public SerializableDictionary<int, Task> Task_canDo = new SerializableDictionary<int, Task>();
    /// <summary>
    /// 已接取的任务
    /// </summary>
    public SerializableDictionary<int, Task> Task_doing = new SerializableDictionary<int, Task>();
    public SerializableDictionary<int, Task> Task_finish = new SerializableDictionary<int, Task>();
    private TaskController()
    {

    }
    public static TaskController getInstance()
    {
        if (instance == null)
        {
            if (World.getInstance().taskCon == null)
            {
                //读取xml
                TaskController con = new TaskController();

                string xmlString = Resources.Load("xml/Task").ToString();
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlString);
                XmlNode root = document.SelectSingleNode("tasklist");
                XmlNodeList taskList = root.ChildNodes;
                foreach (XmlElement taskElement in taskList)
                {
                    Task task = new Task();
                    XmlNode requirementNode= taskElement.SelectSingleNode("requimentList");
                    //初始化任务条件
                    if (requirementNode != null)
                    {
                        XmlNodeList requirementList = requirementNode.ChildNodes;
                        foreach (XmlElement requirement in requirementList)
                        {
                            switch (requirement.Attributes["type"].Value)
                            {
                                case "giveMoney":
                                    task.reqList.Add(new MoneyRequirement(int.Parse(requirement.Attributes["num"].Value)));
                                    break;
                                case "giveItem":
                                    task.reqList.Add(new ItemRequirement(int.Parse(requirement.Attributes["itemId"].Value),int.Parse(requirement.Attributes["num"].Value)));
                                    break;
                                case "kill":
                                    task.reqList.Add(new KillRequirement(int.Parse(requirement.Attributes["monsterId"].Value), int.Parse(requirement.Attributes["num"].Value)));
                                    break;
                            }
                        }
                    }


                    XmlNode unlockNode = taskElement.SelectSingleNode("unlockList");
                    if (unlockNode != null)
                    {
                        XmlNodeList unlockList = unlockNode.ChildNodes;
                        foreach (XmlElement node in unlockList)
                        {
                            task.LatterTaskIDList.Add(int.Parse(node.Attributes["taskId"].Value));
                        }
                    }
                   
                    if (taskElement.Attributes["islocked"].Value == "true")
                        con.Task_locked.Add(task.id,task);
                    else
                    {
                        con.Task_canDo.Add(task.id, task);
                        //需求接口： NPC getNPC(int npcId)
                        //给对应npc加上任务id和当前任务状态：已解锁未接取或者已接取
                    }
                        
                }
                World.getInstance().taskCon = con;
            }
            instance = World.getInstance().taskCon;
        }
        return instance;
    }
   /// <summary>
   /// 获取对应任务,不存在返回null
   /// </summary>
   /// <param name="taskId"></param>
   /// <param name="taskCondition"></param>
   /// <returns></returns>
    public Task getTask(int taskId, TASKCONDITION taskCondition)
    {
        Task task = null;
        switch (taskCondition)
        {
            case TASKCONDITION.LOCKED:
                task = Task_locked[taskId];
                break;
            case TASKCONDITION.Doing:
                task = Task_doing[taskId];
                break;
            case TASKCONDITION.CAN_DO:
                task = Task_canDo[taskId];
                break;
        }
        return task;
    }
}
