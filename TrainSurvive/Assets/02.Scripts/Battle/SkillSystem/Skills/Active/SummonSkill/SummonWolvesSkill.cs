/*
 * 描述：这是召唤狼群技能
 * 消耗60AP，在身前1米、身后1米处各召唤1只野狼  【怪物ID=6】
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class SummonWolvesSkill : NormalSkill {
        //范围参数
        public readonly float rangeRate;
        //怪物Id
        public readonly int monsterId;

        public SummonWolvesSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float rangeRate, int monsterId)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
            this.monsterId = monsterId;
        }

        /// <summary>
        /// 在身前1米、身后1米处各召唤1只野狼  【怪物ID=6】
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            //获取施法人朝向
            int motionDir = battleActor.getMotionDir();

            //身前1m召唤一只野狼
            HelpSummonSkill.generateMonster(monsterId, battleActor.pos + motionDir * rangeRate, battleActor.isPlayer);
            //身后1m召唤一只野狼
            HelpSummonSkill.generateMonster(monsterId, battleActor.pos - motionDir * rangeRate, battleActor.isPlayer);
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new SummonWolvesSkill(curActor, needAp, skillType, rangeRate, monsterId);
        }
    }
}

