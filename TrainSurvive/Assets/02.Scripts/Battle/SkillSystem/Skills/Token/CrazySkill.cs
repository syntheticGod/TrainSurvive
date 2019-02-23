/*
 * 描述：这是狂化技能
 * 开启后持续消耗AP，技能开启期间攻速增加100%，攻击力增加50%，但受到伤害增加20%
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class CrazySkill : TokenSkill {
        //增加攻击速度的参数
        public readonly float atkSpeedRate;
        //增加攻击力的参数
        public readonly float attackRate;
        //增加受伤比例的参数
        public readonly float injuredRate;

        public CrazySkill(BattleActor battleActor, int needAp, SkillType skillType
            , float atkSpeedRate, float attackRate, float injuredRate)
            : base(battleActor, needAp, skillType) {
            this.atkSpeedRate = atkSpeedRate;
            this.attackRate = attackRate;
            this.injuredRate = injuredRate;
        }

        /// <summary>
        /// 技能开启期间攻速增加100%，攻击力增加50%，但受到伤害增加20%
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            // 技能开启期间攻速增加100%
            battleActor.atkNeedTime /= atkSpeedRate;
            //攻击力增加50%
            battleActor.atkDamage *= attackRate;
            //受到伤害增加20%
            battleActor.damageRate *= injuredRate;
        }

        /// <summary>
        /// 关闭效果
        /// </summary>
        protected override void closeSkillEffect() {
            // 恢复攻速
            battleActor.atkNeedTime *= atkSpeedRate;
            //恢复攻击力
            battleActor.atkDamage /= attackRate;
            //恢复受伤比例
            battleActor.damageRate /= injuredRate;
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new CrazySkill(curActor, needAp, skillType, atkSpeedRate, attackRate, injuredRate);
        }
    }
}

