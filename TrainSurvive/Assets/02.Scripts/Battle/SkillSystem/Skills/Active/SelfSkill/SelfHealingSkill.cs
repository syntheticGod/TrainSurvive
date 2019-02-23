/*
 * 描述：这是自愈技能
 * 回复自身（体力*2）%最大生命值
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class SelfHealingSkill : NormalSkill {

        //按比例恢复的生命值
        public float restoreRate;
        
        public SelfHealingSkill(BattleActor battleActor, int needAp, SkillType skillType, float rate)
            : base(battleActor, needAp, skillType) {
            this.restoreRate = rate;
        }

        /// <summary>
        /// 回复自身（体力*rate）%最大生命值
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            battleActor.addHealthPoint(battleActor.myId,
                battleActor.maxHealthPoint * battleActor.vitality * restoreRate * battleActor.skillPara);
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new SelfHealingSkill(curActor, needAp, skillType, restoreRate);
        }
    }
}

