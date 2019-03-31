/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 14:39:08
 * 版本：v0.1
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using TTT.Resource;
using TTT.Utility;

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
        public Vector2 PosTeam { private set; get; }
        public Vector2Int MapPosTeam
        {
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
                switch (state)
                {
                    case STATE.STOP_OUT:
                    case STATE.STOP_TOWN:
                        World.getInstance().ifTeamMoving = false;
                        break;
                    case STATE.MOVING_TOP:
                    case STATE.MOVING_RIGHT:
                    case STATE.MOVING_BOTTOM:
                    case STATE.MOVING_LEFT:
                        World.getInstance().ifTeamMoving = true;
                        break;
                    case STATE.IN_TRAIN://改变事物
                        World.getInstance().ifTeamOuting = false;
                        World.getInstance().ifTrainMoving = false;
                        //探险队放回食物
                        int remain = (int)World.getInstance().getFoodOut();
                        World.getInstance().setFoodOut(0);
                        if (World.getInstance().addFoodIn(remain) != 1)
                        {
                            Debug.LogWarning("探险队增加内部食物不正常");
                        }
                        Debug.Log("探险队：我们（人数：" + World.getInstance().numOut + "）回车了。" +
                            "带回食物：" + remain + "，列车现在有食物：" + World.getInstance().getFoodIn());
                        World.getInstance().numIn = World.getInstance().Persons.Count;
                        World.getInstance().numOut = 0;
                        break;
                }
                Notify((int)state);
            }
        }
        //探险队是否可移动
        public bool IsMovable { get; set; } = true;
        //探险队的移动速度
        private float velocity = 0.0F;
        //移动时下一临近块
        private Vector2 nextStopPosition;
        public PassBlockCenterCallBack OnPassBlockCenter;
        public static Team Instance { get; } = new Team();
        private Team() : base()
        {
            state = STATE.NONE;
        }
        /// <summary>
        /// 仅没设置探险队位置的初始化
        /// </summary>
        public void Init()
        { }
        /// <summary>
        /// 配置探险队的初始坐标
        /// </summary>
        /// <param name="initPosition"></param>
        public void Config(Vector2Int initPosition)
        {
            PosTeam = StaticResource.BlockCenter(initPosition);
        }
        /// <summary>
        /// 探险队是上下左右，四个方向移动
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool Run(ref Vector2 current)
        {
            if (!IsMoving || !IsMovable) return false;
            Vector2 currentNext = current;
            Vector2 direction = nextStopPosition - current;
            IMapForTrain map = Map.GetInstance();
            if (MathTool.ApproximatelyInView(direction, Vector2.zero))
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
            Map map = Map.GetInstance();
            map.MoveToThisSpawn(position);
            World.getInstance().PMarker.TeamMapPos = position;
            OnPassBlockCenter?.Invoke(position);
            if (map.IfMonsterArea(position))
            {
                TimeController.getInstance()?.changeScene(true);
                //触发战斗
                SceneManager.LoadScene("BattleScene");
            }
        }
        /// <summary>
        /// 探险队外出时，探险队该做的准备
        /// </summary>
        public void OutPrepare()
        {
            PosTeam = StaticResource.BlockCenter(World.getInstance().PMarker.TrainMapPos);
            //准备事物
            int food = World.getInstance().Persons.Count * 200;
            if (food > World.getInstance().getFoodIn())
                food = (int)World.getInstance().getFoodIn();
            if (World.getInstance().addFoodIn(-food) != 1)
            {
                Debug.LogWarning("列车食物减少不正常——探险队外出！");
            }
            World.getInstance().numIn = 0;
            World.getInstance().numOut = World.getInstance().Persons.Count;
            Debug.Log("列车：探险队外出了，剩下" + World.getInstance().numIn + "人，剩下" + World.getInstance().getFoodIn() + "食物");
            if (!World.getInstance().setFoodOut((uint)food))
            {
                Debug.LogWarning("探险队携带外出食物不正常！");
            }
            Debug.Log("探险队：我们（一共" + World.getInstance().numOut + "人）外出了，带走了" + World.getInstance().getFoodOut() + "点食物");
            World.getInstance().FullOutVit();
            World.getInstance().ifTeamOuting = true;

            State = World.getInstance().PMarker.IfInTown() ? STATE.STOP_TOWN : STATE.STOP_OUT;
            OnPassBlockCenter?.Invoke(MapPosTeam);
        }
        /// <summary>
        /// 判断探险队是否能回车
        /// </summary>
        /// <returns>
        /// TRUE：其他
        /// FALSE：探险队不在列车方块上
        /// </returns>
        public bool CanTeamGoBack()
        {
            if (StaticResource.BlockIndex(Train.Instance.PosTrain) != StaticResource.BlockIndex(PosTeam))
            {
                Debug.Log("探险队不在列车上");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 探险队回车数据处理函数
        /// </summary>
        /// <returns>
        /// TRUE：探险队成功回车
        /// FALSE：探险队不在列车上
        /// </returns>
        public bool GoBackToTrain()
        {
            State = STATE.IN_TRAIN;
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
            if (!Map.GetInstance().IfInter(target)) return false;
            //判断目标是否正在移动、或者不可移动
            if (IsMoving || !IsMovable) return false;
            nextStopPosition = StaticResource.BlockCenter(target);
            //Debug.Log("Position：当前位置：" + PosTeam + " 移动到：" + nextStopPosition);
            //Debug.Log("Index：当前位置：" + StaticResource.BlockIndex(PosTeam) + " 移动到：" + target);
            velocity = 0.0F;
            return true;
        }
        public void MoveTop()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.y++;
            if (WalkTo(teamIndex) && State != STATE.MOVING_TOP)
                State = STATE.MOVING_TOP;

        }
        public void MoveBottom()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.y--;
            if (WalkTo(teamIndex) && State != STATE.MOVING_BOTTOM)
                State = STATE.MOVING_BOTTOM;
        }
        public void MoveLeft()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.x--;
            if (WalkTo(teamIndex) && State != STATE.MOVING_LEFT)
                State = STATE.MOVING_LEFT;
        }
        public void MoveRight()
        {
            Vector2Int teamIndex = StaticResource.BlockIndex(PosTeam);
            teamIndex.x++;
            if (WalkTo(teamIndex) && State != STATE.MOVING_RIGHT)
                State = STATE.MOVING_RIGHT;
        }
        public bool IsMoving
        {
            get
            {
                return MathTool.IfBetweenBoth((int)STATE.MOVING_TOP, (int)STATE.MOVING_LEFT, (int)state);
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
            //相对运动
            MOVING_TOP,
            MOVING_RIGHT,
            MOVING_BOTTOM,
            MOVING_LEFT,
            NUM
        }
    }
}