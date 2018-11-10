/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2018/11/10 14:39:08
 * 版本：v0.1
 */
using UnityEngine;
namespace WorldMap
{
    public class Team
    {
        //耐力、体力
        private int vitCapacity;
        private int vitCurrent;
        //精神、心情
        private int spiritCapacity;
        private int spiritCurrent;
        //人数
        private int people;
        //小队状态
        private STATE state;

        public Team(int people, int vitCapacity = 100, int spiritCapacity = 100)
        {
            this.vitCapacity = vitCapacity;
            this.spiritCapacity = spiritCapacity;
            this.people = people;
            Init();
        }
        public void Init()
        {
            vitCurrent = vitCapacity;
            spiritCurrent = spiritCapacity;
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
        public bool IsGathering()
        {
            return state == STATE.GATHERING;
        }
        /// <summary>
        /// 移动到指定坐标
        /// </summary>
        /// <param name="position">地图坐标</param>
        public void WalkTo(Vector2Int position)
        {
        }
        public bool IsMoving()
        {
            return state == STATE.MOVING;
        }
        /// <summary>
        /// 判断小队是否能够到达指定坐标
        /// </summary>
        /// <param name="position">地图坐标</param>
        /// <returns>
        /// TRUE：小队能够到达指定坐标
        /// FALSE：小队不能到达指定坐标
        /// </returns>
        public bool CanReachable(Vector2Int position)
        {
            return true;
        }
        public enum STATE
        {
            INTRAIN,
            MOVING,
            RELEXING,
            GATHERING,
            NUM
        }
    }
}