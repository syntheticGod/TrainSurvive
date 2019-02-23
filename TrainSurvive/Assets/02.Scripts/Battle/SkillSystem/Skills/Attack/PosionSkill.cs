/*
 * 描述：这是毒素技能
 * 消耗0 被动技能
 * 攻击给目标叠加中毒状态：每0.5s失去5*层数的HP，并降低99%攻速与移速
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class PosionSkill : NormalSkill {

        public PosionSkill(BattleActor battleActor, int needAp, SkillType skillType)
            : base(battleActor, needAp, skillType) {
        }

        /// <summary>
        /// 攻击给目标叠加中毒状态：每0.5s失去5*层数的HP，并降低99%攻速与移速
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            //如果目标存活
            if (targetActor != null && targetActor.isAlive == true) {
                //给目标叠加中毒状态：每0.5s失去5*层数的HP，并降低99%攻速与移速
                targetActor.setBuffEffect(
                    BuffFactory.getBuff(
                        "Posion",
                        targetActor)
                );
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new PosionSkill(curActor, needAp, skillType);
        }
    }
}

