/*
 * 描述：这是圣光术技能
 * 进阶光系魔法，对小队全体造成 3x智力 治疗
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class HolyLightSkill : NormalSkill {

        public readonly float restoreRate;

        public HolyLightSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.restoreRate = rate;
        }

        /// <summary>
        /// 进阶光系魔法，对小队全体造成 3x智力 治疗
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            foreach (BattleActor friend in battleActor.playerActors) {
                //如果角色已死亡跳过
                if (friend.isAlive == false) {
                    continue;
                }

                //对小队全体造成 3x智力 治疗
                friend.addHealthPoint(battleActor.myId,
                    battleActor.intelligence * restoreRate * battleActor.skillPara);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new HolyLightSkill(curActor, needAp, skillType, restoreRate);
        }
    }
}

