/*
 * 描述：进阶水系魔法
 * 将目标冰冻5秒
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class IceImprisonSkill : NormalSkill {
        //时长参数
        public readonly float timeRate;

        public IceImprisonSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float timeRate)
            : base(battleActor, needAp, skillType) {
            this.timeRate = timeRate;
        }

        /// <summary>
        /// 将目标冰冻5秒
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //将目标冰冻5秒（如果目标存在）
            if (target != -1) {
                targetActor = battleActor.enemyActors[target];
                //眩晕5s，上眩晕buff
                //按给定技能初始化buff
                Buff buff = new Buff(targetActor, "IceImprisonSkill");
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
            return new IceImprisonSkill(curActor, needAp, skillType, timeRate);
        }
    }
}

