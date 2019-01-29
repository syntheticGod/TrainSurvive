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
    public const int taskMaxIndex = 30;//支持的任务个数容量
    public enum TASKCONDITION
    {
        LOCKED=0,
        CAN_DO,
        DOING,
        CAN_FINISH,
        FINISH
    }

    private static TaskController instance = null;

    public Task[] TaskList = new Task[taskMaxIndex];
    /// <summary>
    /// 前者为battleid,后者为taskid
    /// </summary>
    private SerializableDictionary<int, int> battleid_taskid_dic = new SerializableDictionary<int, int>();

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
                    task.id= int.Parse(taskElement.Attributes["id"].Value);
                    task.name = taskElement.Attributes["name"].Value;
                    task.description = taskElement.Attributes["description"].Value;
                    task.condition = (TASKCONDITION)int.Parse(taskElement.Attributes["condition"].Value);
                    //特殊战斗生成
                    XmlNode influenceNode = taskElement.SelectSingleNode("influenceList");
                    if (influenceNode != null)
                    {
                        XmlNodeList influenceList = influenceNode.ChildNodes;
                        foreach (XmlElement influence in influenceList)
                        {
                            int battleId = int.Parse(influence.Attributes["id"].Value);
                            task.task_battle = SpecialBattleInitializer.getInstance().loadBattle(battleId);
                            con.battleid_taskid_dic.Add(battleId, task.id);
                        }
                    }

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
                                    MoneyRequirement r1 = new MoneyRequirement(int.Parse(requirement.Attributes["num"].Value));
                                    task.reqList.Add(r1);
                                    break;
                                case "giveItem":
                                    ItemRequirement r2 = new ItemRequirement(int.Parse(requirement.Attributes["itemId"].Value), int.Parse(requirement.Attributes["num"].Value));
                                    task.reqList.Add(r2);
                                    break;
                                case "kill":
                                    KillRequirement r3 = new KillRequirement(int.Parse(requirement.Attributes["monsterId"].Value), int.Parse(requirement.Attributes["num"].Value));
                                    task.reqList.Add(r3);
                                    break;
                                case "hasItem":
                                    HasItemRequirement r4 = new HasItemRequirement(int.Parse(requirement.Attributes["itemId"].Value), int.Parse(requirement.Attributes["num"].Value));
                                    task.reqList.Add(r4);
                                    break;
                                case "special":
                                    SpecialRequirement_1 r5 = new SpecialRequirement_1();
                                    task.reqList.Add(r5);
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

                    XmlNode rewardNode = taskElement.SelectSingleNode("rewardList");
                    if (rewardNode != null)
                    {
                        XmlNodeList rewardList = rewardNode.ChildNodes;
                        foreach (XmlElement node in rewardList)
                        {
                            switch (node.Attributes["type"].Value)
                            {
                                case "item":
                                    task.rewardList.Add(new System.ValueTuple<int, int>(int.Parse(node.Attributes["id"].Value), int.Parse(node.Attributes["num"].Value)));
                                    break;
                                case "money":
                                    task.rewardMoney = int.Parse(node.Attributes["num"].Value);
                                    break;
                            }
                        }
                    }

                    con.TaskList[task.id] = task;
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
   /// <param name="taskId">任务id</param>
   /// <returns></returns>
    public Task getTask(int taskId)
    {
        return TaskList[taskId];
    }
    /// <summary>
    /// 返回特殊战斗，不存在返回null
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    public SpecialBattle getBattle(int battleId)
    {
        if (battleid_taskid_dic.ContainsKey(battleId))
            return null;

        return TaskList[battleid_taskid_dic[battleId]].task_battle;
    }
}
