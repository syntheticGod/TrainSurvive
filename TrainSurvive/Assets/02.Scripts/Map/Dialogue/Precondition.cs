/*
 * 描述：对话剧情的的前提条件
 *          必须满足对话剧情中的所有前提条件，才能够触发该对话剧情，或句子。
 * 作者：项叶盛
 * 创建时间：2019/2/11 22:03:25
 * 版本：v0.7
 */
namespace WorldMap.Model
{
    public abstract class Precondition
    {
        /// <summary>
        /// 根据上下文判断
        /// </summary>
        /// <example>
        /// 例如：
        ///     判断城镇中NPC的人数，则需要传入城镇对象上下文
        /// </example>
        /// <param name="context">上下文</param>
        /// <returns>
        /// TRUE：满足条件 或 上下文不为空时，上下文不匹配。
        /// FLASE：不满足条件
        /// </returns>
        public abstract bool IfSatisfy();
        public static Precondition[] Compile(string condition)
        {
            Precondition[] preconditions;
            if (condition != null && condition.Length != 0)
            {
                string[] precon = condition.Split(';');
                preconditions = new Precondition[precon.Length];
                for (int i = 0; i < precon.Length; i++)
                {
                    string[] words = precon[i].Split(' ');
                    switch (words[1])
                    {
                        case "task": preconditions[i] = new TaskCondition(words); break;
                        case "other": preconditions[i] = new OtherCondition(words); break;
                        case "npc": preconditions[i] = new NpcCondition(words); break;
                    }
                }
            }
            else
            {
                preconditions = new Precondition[0];
            }
            return preconditions;
        }
    }
    public class OtherCondition : Precondition
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
    public class TaskCondition : Precondition
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
    public class NpcCondition : Precondition
    {
        /// <summary>
        /// 类型：{0 当前城镇的数量}
        /// </summary>
        public int Type { get; private set; }
        public int Count { get; private set; }
        private string m_comparer;
        public NpcCondition(string[] cmd)
        {
            switch (cmd[0])
            {
                case "count":
                    Type = 0;
                    m_comparer = cmd[2];
                    Count = int.Parse(cmd[3]);
                    break;
                default:Type = -1;break;
            }
            
        }
        public override bool IfSatisfy()
        {
            switch (Type)
            {
                case 0:
                    TownData town = World.getInstance().PMarker.GetCurrentTown();
                    if (town == null) return false;
                    switch (m_comparer)
                    {
                        case ">": return town.Npcs.Count > Count;
                        case ">=": return town.Npcs.Count >= Count;
                        case "<": return town.Npcs.Count < Count;
                        case "<=": return town.Npcs.Count <= Count;
                        case "==": return town.Npcs.Count == Count;
                        default:return false;
                    }
            }
            return false;
        }
    }
}
