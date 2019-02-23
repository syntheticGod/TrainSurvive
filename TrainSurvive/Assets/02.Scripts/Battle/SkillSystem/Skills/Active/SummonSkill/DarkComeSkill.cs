/*
 * 描述：这是黑暗将至技能
 * 消耗150AP，在身后1米，召唤一支骷髅军队【怪物ID=11、12、13、14各一只】
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class DarkComeSkill : NormalSkill {
        //范围参数
        public readonly float rangeRate;
        //怪物Id
        public readonly int monsterId;
        //怪物数量
        public readonly int monsterNum;

        public DarkComeSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float rangeRate, int monsterId, int monsterNum)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
            this.monsterId = monsterId;
            this.monsterNum = monsterNum;
        }

        /// <summary>
        /// 在身后1米，召唤一支骷髅军队【怪物ID=11、12、13、14各一只】
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            //获取施法人朝向
            int motionDir = battleActor.getMotionDir();

            for (int i = 0; i < monsterNum; i++) {
                //身后1m召唤一只野狼
                HelpSummonSkill.generateMonster(monsterId + i, battleActor.pos - motionDir * rangeRate, battleActor.isPlayer);
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new DarkComeSkill(curActor, needAp, skillType, rangeRate, monsterId, monsterNum);
        }
    }
}

