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
        //探险队的最多人数
        public const int MAX_NUMBER_OF_TEAM_MEMBER = 5;
        //最大运动速度
        public float MaxSpeed { set; get; } = 1.0F;
        //最小移动距离
        public float MinDeltaStep { set; get; } = 0.01F;
        public float SmoothTime { set; get; } = 0.3F;
        //探险队位置
        public Vector2 PosTeam { private set; get; }
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
                switch (state)
                {
                    case STATE.GATHERING:
                        world.DoGather();
                        break;
                    case STATE.STOP_OUT:
                    case STATE.STOP_TOWN:
                        world.TeamStandeBy();
                        break;
                    case STATE.MOVING_TOP:
                    case STATE.MOVING_RIGHT:
                    case STATE.MOVING_BOTTOM:
                    case STATE.MOVING_LEFT:
                        world.TeamMoving();
                        break;
                    case STATE.IN_TRAIN:
                        world.TeamGetIn();
                        break;
                }
            }
        }
        //探险队是否可移动
        public bool IsMovable { get; set; } = true;
        //探险队的移动速度
        private float velocity = 0.0F;
        //移动时下一临近块
        private Vector2 nextStopPosition;
        //背包
        public InventoryForTeam Inventory { private set; get; }
        //外部引用
        private Train train;
        private IMapForTrain map;
        private WorldForMap world;
        public override int MaxState()
        {
            return (int)STATE.NUM;
        }
        public static Team Instance { get; } = new Team();
        private Team() : base()
        {
            state = STATE.NONE;
        }
        /// <summary>
        /// 仅没设置探险队位置的初始化
        /// </summary>
        public void Init()
        {
            map = Map.GetIntanstance();
            train = Train.Instance;
            world = WorldForMap.Instance;
            Inventory = new InventoryForTeam(float.MaxValue);
        }
        /// <summary>
        /// 夹带探险队位置的初始化
        /// </summary>
        /// <param name="initPosition"></param>
        public void Init(Vector2 initPosition)
        {
            Init();
            PosTeam = initPosition;
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
            map.MoveToThisSpawn(StaticResource.BlockIndex(position));
            world.TeamSetMapPos(position);
            if (MathTool.RandomInt(5) == 0)
            {
                //
                TimeController.getInstance()?.changeScene(true);
                //触发战斗
                SceneManager.LoadScene("BattleScene");
            }
        }
        /// <summary>
        /// 探险队外出时，探险队该做的准备
        /// </summary>
        /// <param name="initPosition">出现的世界坐标</param>
        /// <param name="selectedFood">外带的食物</param>
        /// <param name="selectedPersons">选择的成员</param>
        public void OutPrepare(Vector2 initPosition, int selectedFood, List<Person> selectedPersons)
        {
            PosTeam = initPosition;
            world.TeamGetOut(selectedFood, selectedPersons);
            State = IfInTown()? STATE.STOP_TOWN : STATE.STOP_OUT;
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
            if (StaticResource.BlockIndex(train.PosTrain) != StaticResource.BlockIndex(PosTeam))
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
        /// 探险队招募到英雄的回调函数
        /// </summary>
        /// <param name="theOne"></param>
        public void CallBackRecruit(Person theOne)
        {
            Debug.Log("探险队：招募到" + theOne.name);
            world.TeamRecruit(theOne);
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
            if(WalkTo(teamIndex) && State != STATE.MOVING_BOTTOM)
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
        public bool IsGathering
        {
            get
            {
                return state == STATE.GATHERING;
            }
        }
        private bool IfInTown()
        {
            return map.IfTown(StaticResource.BlockIndex(PosTeam));
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
            MOVING_TOP,
            MOVING_RIGHT,
            MOVING_BOTTOM,
            MOVING_LEFT,
            NUM
        }
    }
}