/*
 * 描述：玩家与NPC对话剧情的的前提条件
 *          必须满足对话剧情中的所有前提条件，才能够触发该对话剧情。
 * 作者：项叶盛
 * 创建时间：2019/2/11 22:03:25
 * 版本：v0.7
 */
namespace WorldMap.Model
{
    public abstract class DialogueCondition
    {
        /// <summary>
        /// 是否满足条件
        /// </summary>
        /// <returns></returns>
        public abstract bool IfSatisfy();
    }
    public class OtherCondition : DialogueCondition
    {
        /// <summary>
        /// 前置对话ID
        /// </summary>
        public int[] OtherDialogueID { get; private set; }
        public OtherCondition(string[] words)
        {
            char[] charsToTrim = { '(', ')' };
            string[] OtherID = words[2].Trim(charsToTrim).Split(',');
            OtherDialogueID = new int[OtherID.Length];
            for (int i = 0; i < OtherID.Length; i++)
                OtherDialogueID[i] = int.Parse(OtherID[i]);
        }
        public override bool IfSatisfy()
        {
            return World.getInstance().Dialogues.IfTalked(OtherDialogueID);
        }
    }
    public class TaskCondition : DialogueCondition
    {
        /// <summary>
        /// 类型：{0 正在进行任务| 1 完成任务}
        /// </summary>
        public int Type { get; private set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskID { get; private set; }
        public TaskCondition(string[] cmd)
        {
            switch (cmd[0])
            {
                case "doing": Type = 0; break;
                case "finish": Type = 1; break;
            }
            TaskID = int.Parse(cmd[2]);
        }
        public override bool IfSatisfy()
        {
            Task task = World.getInstance().taskCon.getTask(TaskID);
            switch (Type)
            {
                case 0: if (task.condition == TaskController.TASKCONDITION.DOING) return true; break;
                case 1: if (task.condition == TaskController.TASKCONDITION.FINISH) return true; break;
            }
            return false;
        }
    }
}
