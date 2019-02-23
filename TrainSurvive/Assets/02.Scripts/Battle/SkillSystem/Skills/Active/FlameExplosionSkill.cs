/*
 * 描述：这是火焰爆轰技能
 * 初阶火系魔法，在当前攻击目标身上引爆火球，造成 8x智力 伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class FlameExplosionSkill : NormalSkill {

        public readonly float damageRate;

        public FlameExplosionSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.damageRate = rate;
        }

        /// <summary>
        /// 在当前攻击目标身上引爆火球，造成 8x智力 伤害
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //造成 8x智力 伤害
                float damage = battleActor.enemyActors[target].getDamage(battleActor.myId,
                    battleActor.intelligence * damageRate * battleActor.skillPara);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new FlameExplosionSkill(curActor, needAp, skillType, damageRate);
        }
    }
}

