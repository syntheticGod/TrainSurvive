/*
 * 描述：这是武器涂毒技能
 * 消耗0 被动技能
 * 攻击给目标叠加中毒状态：每秒失去（层数*5）的生命值，并降低（层数*4）%的攻速与移速，最高叠加至20层
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class WeaponPosionSkill : NormalSkill {

        public WeaponPosionSkill(BattleActor battleActor, int needAp, SkillType skillType)
            : base(battleActor, needAp, skillType) {
        }

        /// <summary>
        /// 每秒失去（层数*5）的生命值，并降低（层数*4）%的攻速与移速，最高叠加至20层
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            //如果目标存活
            if (targetActor != null && targetActor.isAlive == true) {
                //给目标叠加中毒状态：每秒失去（层数*5）的生命值，并降低（层数*4）%的攻速与移速，最高叠加至20层
                targetActor.setBuffEffect(
                    BuffFactory.getBuff(
                        "WeaponPosion",
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
            return new WeaponPosionSkill(curActor, needAp, skillType);
        }
    }
}

