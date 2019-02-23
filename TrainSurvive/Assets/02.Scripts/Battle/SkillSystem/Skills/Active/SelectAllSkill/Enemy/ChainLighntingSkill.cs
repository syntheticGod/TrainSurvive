/*
 * 描述：这是闪电链技能
 * 初阶雷系魔法，对敌方全体造成 2x智力 伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class ChainLighntingSkill : NormalSkill {

        public readonly float damageRate;

        public ChainLighntingSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.damageRate = rate;
        }

        /// <summary>
        /// 对敌方全体造成 2x智力 伤害
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            foreach (BattleActor enemy in battleActor.enemyActors) {
                //如果角色已死亡跳过
                if (enemy.isAlive == false) {
                    continue;
                }

                //对敌方全体造成 2x智力 伤害
                enemy.getDamage(battleActor.myId,
                    battleActor.intelligence * damageRate * battleActor.skillPara);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new ChainLighntingSkill(curActor, needAp, skillType, damageRate);
        }
    }
}

