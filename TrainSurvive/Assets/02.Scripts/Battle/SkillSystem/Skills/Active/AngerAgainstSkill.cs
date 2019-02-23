/*
 * 描述：这是怒意打击技能
 * 对当前攻击目标造成使用者自身20%最大生命值的伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class AngerAgainstSkill : NormalSkill {

        public readonly float damageRate;

        public AngerAgainstSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.damageRate = rate;
        }

        /// <summary>
        /// 对当前攻击目标造成使用者自身20%最大生命值的伤害
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //造成使用者自身20%最大生命值的伤害
                battleActor.enemyActors[target].getDamage(battleActor.myId,
                    battleActor.maxHealthPoint * damageRate * battleActor.skillPara);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new AngerAgainstSkill(curActor, needAp, skillType, damageRate);
        }
    }
}

