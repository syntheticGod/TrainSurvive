/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 14:39:08
 * 版本：v0.1
 */
using UnityEngine;
using WorldMap;
namespace WorldMap.Model
{
    public class Team : SubjectBase
    {
        //最大运动速度
        public float MaxSpeed { set; get; } = 1.0F;
        //最小移动距离
        public float MinDeltaStep { set; get; } = 0.01F;
        public float SmoothTime { set; get; } = 0.3F;
        //探险队位置
        public Vector2 PosTeam { set; get; }
        public Vector2Int MapPosTeam {
            get { return StaticResource.BlockIndex(PosTeam); }
        }
        //小队状态
        private STATE state;
        private STATE State
        {
            get { return state; }
            set
            {
                state = value;
                this.Notify((int)state);
            }
        }
        //耐力、体力
        private float outVitMax;
        private float outVit;
        //精神、心情
        private float outMoodMax;
        private float outMood;
        //人数
        private int people;
        //探险队的视野范围
        private int distView;
        //探险队的移动速度
        private float velocity = 0.0F;
        //移动时下一临近块
        private Vector2 nextStopPosition;
        //外部引用
        private Train train;
        private IMapForTrain map;
        public override int MaxState()
        {
            return (int)STATE.NUM;
        }
        public static Team Instance { get; } = new Team();
        private Team() : base()
        {

        }
        public void Init(int people, float outVitMax = 100, float outMoodMax = 100, int distView = 1)
        {
            map = Map.GetIntanstance();
            train = Train.Instance;
            this.outVitMax = outVitMax;
            this.outMoodMax = outMoodMax;
            this.people = people;
            this.distView = distView;
            outVit = outVitMax;
            outMood = outMoodMax;
            state = STATE.NONE;
            PosTeam = train.PosTrain;
        }
        /// <summary>
        /// 探险队是上下左右，四个方向移动
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool Run(ref Vector2 current)
        {
            if (!IsMoving) return false;
            Vector2 currentNext = current;
            Vector2 direction = nextStopPosition - current;
            if (Utility.ApproximatelyInView(direction, Vector2.zero))
            {
                //到达目的地
                Debug.Log("到达目的地");
                Vector2Int mapPosTeam = StaticResource.BlockIndex(PosTeam);
                PassCenterCallBack(mapPosTeam);
                State = map.IfTown(mapPosTeam) ? STATE.STOP_TOWN : STATE.STOP_OUT;
                return false;
            }
            float magnitude = direction.magnitude;
            float delta = 0.0F;
            if (!Mathf.Approximately(Time.deltaTime, 0.0F))
                delta = Mathf.Max(MinDeltaStep, magnitude - Mathf.SmoothDamp(magnitude, 0, ref velocity, SmoothTime));
            if (delta >= magnitude)
                currentNext = nextStopPosition;
            else
                currentNext += direction.normalized * delta;
            current = PosTeam = currentNext;
            return true;
        }
        public void PassCenterCallBack(Vector2Int position)
        {
            map.MoveToThisSpawn(StaticResource.BlockIndex(position));

        }
        public void Action()
        {
            PosTeam = train.PosTrain;
        }
        /// <summary>
        /// 小队自我固定消耗函数
        /// </summary>
        public void Consume()
        {
        }
        /// <summary>
        /// 主动采集
        /// </summary>
        /// <returns>
        /// TRUE：成功采集
        /// FALSE：暂时不会返回
        /// </returns>
        public bool Gather()
        {
            return true;
        }
        public bool GoTrain()
        {
            if (StaticResource.BlockIndex(train.PosTrain) != StaticResource.BlockIndex(PosTeam))
            {
                Debug.Log("探险队不在列车上");
                return false;
            }

            return true;
        }
        /// <summary>
        /// 移动到指定坐标
        /// </summary>
        /// <param name="position">地图坐标</param>
        /// <returns></returns>
        private bool WalkTo(Vector2Int target)
        {
            //判断目标坐标是否在地图内
            if (!map.IfInter(target)) return false;
            //判断目标是否正在移动
            if (IsMoving) return false;
            nextStopPosition = StaticResource.BlockCenter(target);
            //Debug.Log("Position：当前位置：" + PosTeam + " 移动到：" + nextStopPosition);
            //Debug.Log("Index：当前位置：" + StaticResource.BlockIndex(PosTeam) + " 移动到：" + target);
            velocity = 0.0F;
            state = STATE.MOVING;
            return true;
        }
        public void MoveTop()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.y++;
            WalkTo(teamIndex);
        }
        public void MoveBottom()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.y--;
            WalkTo(teamIndex);
        }
        public void MoveLeft()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.x--;
            WalkTo(teamIndex);
        }
        public void MoveRight()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.x++;
            WalkTo(teamIndex);
        }
        public bool IsMoving
        {
            get
            {
                return state == STATE.MOVING;
            }
        }
        public bool IsGathering
        {
            get
            {
                return state == STATE.GATHERING;
            }
        }
        public enum STATE
        {
            NONE,
            //相对静止
            IN_TRAIN = 0, //在列车里面
            STOP_OUT, //在外面停止
            STOP_TOWN,//停在城镇
            RELEXING,//休息
            GATHERING,//采集
            //相对运动
            MOVING, //正在移动
            NUM
        }
    }
}