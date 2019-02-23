/*
 * 描述：这是诅咒技能
 * 初阶暗系魔法，目标ap恢复降低80%，受到伤害增加20%，持续5秒
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class CurseSkill : NormalSkill {

        public CurseSkill(BattleActor battleActor, int needAp, SkillType skillType)
            : base(battleActor, needAp, skillType) {
        }

        /// <summary>
        /// 初阶暗系魔法，目标ap恢复降低80%，受到伤害增加20%，持续5秒
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.getNearestEnemy(battleActor);
            if (target != -1) {
                //给目标ap恢复降低80%，受到伤害增加20%，持续5秒
                battleActor.enemyActors[target].setBuffEffect(
                    BuffFactory.getBuff(
                        "Curse",
                        battleActor.enemyActors[target])
                );
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new CurseSkill(curActor, needAp, skillType);
        }
    }
}

