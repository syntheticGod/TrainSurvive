/*
 * 描述：这是陨石术技能
 * 消耗100AP，在目标位置落下陨石，对半径2米内敌人造成 10x智力 伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class MeteoriteSkill : NormalSkill {
        //范围参数
        public readonly float rangeRate;
        //伤害参数
        public readonly float damageRate;

        public MeteoriteSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float rangeRate, float attackRate)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
            this.damageRate = attackRate;
        }

        /// <summary>
        /// 在目标位置落下陨石，对半径2米内敌人造成 10x智力 伤害
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //获取目标对象
                targetActor = battleActor.enemyActors[target];

                //获取当前目标的位置
                float pos = targetActor.pos;

                foreach (BattleActor enemy in battleActor.enemyActors) {
                    //如果角色已死亡跳过
                    if (enemy.isAlive == false) {
                        continue;
                    }

                    //如果与当前角色超过2m，跳过
                    if (Mathf.Abs(enemy.pos - pos) > rangeRate) {
                        continue;
                    }

                    //造成 10x智力 伤害
                    enemy.getDamage(battleActor.myId,
                        battleActor.intelligence * damageRate * battleActor.skillPara);
                }
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new MeteoriteSkill(curActor, needAp, skillType, rangeRate, damageRate);
        }
    }
}

