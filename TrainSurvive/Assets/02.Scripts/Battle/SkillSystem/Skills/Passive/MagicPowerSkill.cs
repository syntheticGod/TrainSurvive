/*
 * 描述：这是魔能之力技能
 * 消耗0 被动技能
 * 增加自身（智力*5）%攻击力
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class MagicPowerSkill : NormalSkill {
        //增加攻击力的参数
        public readonly float attackRate;

        public MagicPowerSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float attackRate)
            : base(battleActor, needAp, skillType) {
            this.attackRate = attackRate;
        }

        /// <summary>
        /// 增加自身（智力*5）%攻击力
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            // 增加自身（智力*5）%攻击力
            battleActor.atkDamage *= (1 + battleActor.intelligence * attackRate * 0.01f);
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new MagicPowerSkill(curActor, needAp, skillType, attackRate);
        }
    }
}

