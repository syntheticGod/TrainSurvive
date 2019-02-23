/*
 * 描述：这是强力击技能
 * 将目标击退10米
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class PowerBlowSkill : NormalSkill {

        public readonly float rangeRate;

        public PowerBlowSkill(BattleActor battleActor, int needAp, SkillType skillType, float rangeRate)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
        }

        /// <summary>
        /// 将目标击退10米
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //将目标击退10米（如果目标存在）
            if (target != -1) {
                //获取施法人朝向
                int motionDir = battleActor.getMotionDir();
                //改变目标的位置，为施法人前方10m
                battleActor.enemyActors[target].
                    changeRealPos(battleActor.enemyActors[target].pos + motionDir * rangeRate);
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new PowerBlowSkill(curActor, needAp, skillType, rangeRate);
        }
    }
}

