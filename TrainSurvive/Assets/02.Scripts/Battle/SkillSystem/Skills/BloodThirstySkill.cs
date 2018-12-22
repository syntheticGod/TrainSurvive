/*
 * 描述：这是鲜血渴望技能
 * 对当前攻击目标造成800%攻击力的伤害，并回复等值生命值
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class BloodThirstySkill : Skill {

        public readonly float rate;

        public BloodThirstySkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.rate = rate;
        }

        /// <summary>
        /// 对当前攻击目标造成800%攻击力的伤害，并回复等值生命值
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //对当前攻击目标造成800%攻击力的伤害
                float damage = battleActor.enemyActors[target].getDamage(battleActor.myId,
                    battleActor.atkDamage * rate * battleActor.skillPara);
                //并回复等值生命值
                battleActor.addHealthPoint(battleActor.myId, damage);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override Skill Clone(BattleActor curActor) {
            return new BloodThirstySkill(curActor, needAp, skillType, rate);
        }
    }
}

