/*
 * 描述：这是空袭技能
 * 呼叫毁灭性飞弹袭击，对敌方全体造成 7x技巧 伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class AirStrikeSkill : NormalSkill {
        //伤害参数（技巧倍数）
        public readonly float attackRate;

        public AirStrikeSkill(BattleActor battleActor, int needAp, SkillType skillType
            ,float attackRate)
            : base(battleActor, needAp, skillType) {
            this.attackRate = attackRate;
        }

        /// <summary>
        /// 呼叫毁灭性飞弹袭击，对敌方全体造成 7x技巧 伤害
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            foreach (BattleActor enemy in battleActor.enemyActors) {
                //如果角色已死亡跳过
                if (enemy.isAlive == false) {
                    continue;
                }
                //造成7x技巧的伤害
                enemy.getDamage(battleActor.myId,
                    battleActor.technical * attackRate * battleActor.skillPara);
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new AirStrikeSkill(curActor, needAp, skillType, attackRate);
        }
    }
}

