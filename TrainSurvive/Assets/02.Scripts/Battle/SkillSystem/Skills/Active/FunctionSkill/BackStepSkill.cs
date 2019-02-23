/*
 * 描述：这是后撤步技能
 * 瞬间后移5米
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class BackStepSkill : NormalSkill {

        public readonly float rate;

        public BackStepSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType, 0) {
            this.rate = rate;
        }

        /// <summary>
        /// 瞬间后移5米
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            battleActor.changeRealPos(battleActor.pos - rate);
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new BackStepSkill(curActor, needAp, skillType, rate);
        }
    }
}

