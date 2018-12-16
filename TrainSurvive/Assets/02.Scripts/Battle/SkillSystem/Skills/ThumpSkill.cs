/*
 * 描述：这是重击技能
 * 消耗5AP，立即对敌人中当前生命值最低的单位造成100伤害
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class ThumpSkill : Skill {

        //当前技能消耗的AP
        public new const int needAp = 5;
        //当前技能是不是被动技能
        public new const bool isPassive = false;

        public ThumpSkill(BattleActor battleActor)
            : base(battleActor, needAp, isPassive) {
        }

        /// <summary>
        /// 找到生命值最少的敌方目标对其造成伤害
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            //获取生命值最少的目标
            int target = HelpSelectTarget.getHPMinestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //对敌方目标造成100点伤害
                battleActor.enemyActors[target].addHealthPoint(battleActor.myId, -100);
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override Skill Clone(BattleActor curActor) {
            return new ThumpSkill(curActor);
        }
    }
}

