/*
 * 描述：这是致命一击技能
 * 对敌人中当前生命值百分比最低的单位造成1000%攻击力的伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class CritStrikeSkill : NormalSkill {

        public readonly float damageRate;

        public CritStrikeSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.damageRate = rate;
        }

        /// <summary>
        /// 对敌人中当前生命值百分比最低的单位造成1000%攻击力的伤害
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.getHPRateMinestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //对敌人中当前生命值百分比最低的单位造成1000%攻击力的伤害
                battleActor.enemyActors[target].getDamage(battleActor.myId,
                    battleActor.atkDamage * damageRate * battleActor.skillPara);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new CritStrikeSkill(curActor, needAp, skillType, damageRate);
        }
    }
}

