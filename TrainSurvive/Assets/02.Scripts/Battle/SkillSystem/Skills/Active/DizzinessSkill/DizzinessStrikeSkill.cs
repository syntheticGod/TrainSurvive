/*
 * 描述：这是眩晕击技能
 * 消耗30AP，对当前攻击目标造成200%攻击力的伤害，并晕眩5秒
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class DizzinessStrikeSkill : NormalSkill {
        //伤害参数
        public readonly float attackRate;
        //时长参数
        public readonly float timeRate;

        public DizzinessStrikeSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float attackRate, float timeRate)
            : base(battleActor, needAp, skillType) {
            this.attackRate = attackRate;
            this.timeRate = timeRate;
        }

        /// <summary>
        /// 对当前攻击目标造成200%攻击力的伤害，并晕眩5秒
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //对目标造成300%攻击力的伤害（如果目标存在）
            if (target != -1) {
                targetActor = battleActor.enemyActors[target];
                //对当前攻击目标造成200%攻击力的伤害
                targetActor.getDamage(battleActor.myId,
                battleActor.atkDamage * attackRate * battleActor.skillPara);

                //眩晕5s，上眩晕buff
                //按给定技能初始化buff
                Buff buff = new Buff(targetActor, "DizzinessStrikeSkill");
                buff.buffList.Add(new DizzinessBuff(
                    targetActor, DizzinessBuff.TypePropertyEnum.DIZZINESS, timeRate, false, 1));
                //上buff
                targetActor.setBuffEffect(buff);
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new DizzinessStrikeSkill(curActor, needAp, skillType, attackRate, timeRate);
        }
    }
}

