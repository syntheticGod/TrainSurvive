/*
 * 描述：这是裂地击技能
 * 对自身3米内所有敌人造成300%攻击力的伤害，并晕眩3秒
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class CrackStrikeSkill : Skill {
        //范围参数
        public readonly float rangeRate;
        //伤害参数
        public readonly float attackRate;
        //时长参数
        public readonly float timeRate;

        public CrackStrikeSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float rangeRate, float attackRate, float timeRate)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
            this.attackRate = attackRate;
            this.timeRate = timeRate;
        }

        /// <summary>
        /// 对自身3米内所有敌人造成300%攻击力的伤害，并晕眩3秒
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {

            foreach (BattleActor enemy in battleActor.enemyActors) {
                //如果角色已死亡跳过
                if (enemy.isAlive == false) {
                    continue;
                }
                //如果与当前角色超过3m，跳过
                if (Mathf.Abs(enemy.pos - battleActor.pos) > rangeRate) {
                    continue;
                }

                //造成300%攻击力的伤害
                enemy.getDamage(battleActor.myId,
                    battleActor.atkDamage * attackRate * battleActor.skillPara);

                //眩晕3s，上眩晕buff
                //按给定技能初始化buff
                Buff buff = new Buff(battleActor, "CrackStrikeSkill");
                buff.buffList.Add(new DizzinessBuff(
                    enemy, DizzinessBuff.TypePropertyEnum.DIZZINESS, timeRate, false, 1));
                //上buff
                enemy.setBuffEffect(buff);
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override Skill Clone(BattleActor curActor) {
            return new CrackStrikeSkill(curActor, needAp, skillType, rangeRate, attackRate, timeRate);
        }
    }
}

