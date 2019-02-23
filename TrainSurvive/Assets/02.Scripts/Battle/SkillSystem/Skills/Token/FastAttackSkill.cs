/*
 * 描述：这是迅击技能
 * 开启后持续消耗AP，技能开启期间攻速增加100%
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class FastAttackSkill : TokenSkill {
        //增加攻击力的参数
        public readonly float attackRate;

        public FastAttackSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float attackRate)
            : base(battleActor, needAp, skillType) {
            this.attackRate = attackRate;
        }

        /// <summary>
        /// 技能开启期间攻速增加100%
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            // 攻速增加100%
            battleActor.atkNeedTime /= attackRate;
        }

        /// <summary>
        /// 关闭效果
        /// </summary>
        protected override void closeSkillEffect() {
            //攻速减少100%
            battleActor.atkNeedTime *= attackRate;
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new FastAttackSkill(curActor, needAp, skillType, attackRate);
        }
    }
}

