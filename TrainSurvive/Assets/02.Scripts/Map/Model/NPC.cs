/*
 * 描述：对外部Person类的封装，用于WorldMap内部。
 * 作者：项叶盛
 * 创建时间：2018/11/21 23:06:19
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using TTT.Resource;
using TTT.Utility;

namespace WorldMap.Model
{
    [Serializable]
    public class NPC
    {
        public List<int> taskId_canDo=new List<int>();
        public List<int> taskId_doing= new List<int>();
        public List<int> taskId_locked = new List<int>();
        public List<int> taskId_finish = new List<int>();
        public Person PersonInfo { private set; get; }
        public string Name { get { return PersonInfo.name; } }
        public int Strength { get { return PersonInfo.strength; } }
        public int ID { get; private set; }
        private NPC()
        {
            PersonInfo = Person.RandomPerson();
            ID = MathTool.GenerateID();
        }
        public static NPC Random()
        {
            return new NPC();
        }
        public string Info
        {
            get
            {
                return "名字：" + PersonInfo.name + "\n" +
                    "力量：" + PersonInfo.strength + 
                    " 体力：" + PersonInfo.vitality + 
                    " 敏捷：" + PersonInfo.agile + 
                    "\n技巧：" + PersonInfo.technique + 
                    " 智力：" + PersonInfo.intelligence;
            }
        }
        /// <summary>
        /// 给NPC添加能开始的或未解锁的任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="taskCondition">取值范围[LOCKED, CAN_DO]</param>
        /// <returns>
        /// TRUE：添加成功
        /// FALSE：该任务状态的任务不允许被填加
        /// </returns>
        public bool AddTask(int taskId, TaskController.TASKCONDITION taskCondition)
        {
            switch (taskCondition)
            {
                case TaskController.TASKCONDITION.LOCKED:
                    taskId_locked.Add(taskId);
                    break;
                case TaskController.TASKCONDITION.CAN_DO:
                    taskId_canDo.Add(taskId);
                    break;
                default:
                    return false;
            }
            return true;
        }
        //调用taskController的getTask获取任务


        /// <summary>
        /// 接取某个任务后调用
        /// </summary>
        /// <param name="taskId"></param>
        private void receiveTask(int taskId)
        {
            taskId_canDo.Remove(taskId);
            taskId_doing.Add(taskId);
            TaskController con = TaskController.getInstance();
            Task task = con.getTask(taskId, TaskController.TASKCONDITION.CAN_DO);
            con.Task_canDo.Remove(taskId);
            con.Task_doing.Add(taskId,task);
        }


        /// <summary>
        /// 完成某个任务后调用
        /// </summary>
        /// <param name="taskId"></param>
        private void finishTask(int taskId)
        {
            taskId_doing.Remove(taskId);
            TaskController con = TaskController.getInstance();
            Task task = con.getTask(taskId, TaskController.TASKCONDITION.Doing);
            foreach(int latterTaskId in task.LatterTaskIDList)
            {
                Task latter_task = con.getTask(latterTaskId, TaskController.TASKCONDITION.LOCKED);
                con.Task_locked.Remove(latterTaskId);
                con.Task_canDo.Add(latterTaskId,latter_task);
            }       
            con.Task_doing.Remove(taskId);
            con.Task_finish.Add(taskId, task);
        } 

    }
}