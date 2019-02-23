/*
 * 描述：这是雷击技能
 * 对敌方当前生命值最高的单位造成 15x智力 伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class LightningStrikeSkill : NormalSkill {

        public readonly float damageRate;

        public LightningStrikeSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.damageRate = rate;
        }

        /// <summary>
        /// 对敌方当前生命值最高的单位造成 15x智力 伤害
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.getHPMaxestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //对敌方当前生命值最高的单位造成 15x智力 伤害
                battleActor.enemyActors[target].getDamage(battleActor.myId,
                    battleActor.intelligence * damageRate * battleActor.skillPara);
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new LightningStrikeSkill(curActor, needAp, skillType, damageRate);
        }
    }
}

