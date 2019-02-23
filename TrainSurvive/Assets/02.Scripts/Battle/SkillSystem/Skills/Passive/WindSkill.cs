/*
 * 描述：这是御风技能
 * 消耗0 被动技能
 * 初阶风系魔法，小队全体增加（技能携带者智力*0.5）%移速，（技能携带者智力*1）%攻速
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class WindSkill : NormalSkill {
        //移速参数
        public readonly float moveRate;
        //攻速参数
        public readonly float atkSpeedRate;

        public WindSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float moveRate, float atkSpeedRate)
            : base(battleActor, needAp, skillType) {
            this.moveRate = moveRate;
            this.atkSpeedRate = atkSpeedRate;
        }

        /// <summary>
        /// 小队全体增加（技能携带者智力*0.5）%移速，（技能携带者智力*1）%攻速
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            foreach (BattleActor friend in battleActor.playerActors) {
                //如果角色已死亡跳过
                if (friend.isAlive == false) {
                    continue;
                }

                //小队全体增加（技能携带者智力*0.5）%移速
                friend.moveSpeed *= (1 + battleActor.intelligence * moveRate * 0.01f);

                //小队全体增加（技能携带者智力*1）%攻速
                friend.atkNeedTime /= (1 + battleActor.intelligence * atkSpeedRate * 0.01f);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new WindSkill(curActor, needAp, skillType, moveRate, atkSpeedRate);
        }
    }
}

