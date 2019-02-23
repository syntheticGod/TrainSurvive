/*
 * 描述：初阶水系魔法，将自身冰封5秒，无法行动，减少80%所受伤害，每秒恢复50点生命值
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class IceProtectSkill : NormalSkill {
        //时长参数
        public readonly float timeRate;

        public IceProtectSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float timeRate)
            : base(battleActor, needAp, skillType) {
            this.timeRate = timeRate;
        }

        /// <summary>
        /// 初阶水系魔法，将自身冰封5秒，无法行动，减少80%所受伤害，每秒恢复50点生命值
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            //眩晕5s，上眩晕buff
            //按给定技能初始化buff
            Buff buff = new Buff(battleActor, "IceImprisonSkill");
            buff.buffList.Add(new DizzinessBuff(
                battleActor, DizzinessBuff.TypePropertyEnum.DIZZINESS, timeRate, false, 1));
            //上buff
            battleActor.setBuffEffect(buff);

            //给自己增加buff：减少80%所受伤害，每秒恢复50点生命值
            battleActor.setBuffEffect(
                BuffFactory.getBuff(
                    "IceProtect",
                    battleActor)
            );
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new IceProtectSkill(curActor, needAp, skillType, timeRate);
        }
    }
}

