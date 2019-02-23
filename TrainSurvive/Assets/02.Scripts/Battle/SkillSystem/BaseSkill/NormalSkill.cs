/*
 * 描述：这是技能基类
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:13:17
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public abstract class NormalSkill : BaseSkill {
        public NormalSkill(BattleActor battleActor, int needAp, SkillType skillType, float skillReleaseTime = 0.5F)
            : base(battleActor, needAp, skillType, skillReleaseTime) {
        }

        //释放本次技能
        public override bool release(BattleActor targetActor = null) {
            //如果不可以释放该技能，返回false
            if (canReleaseSkill() == false) {
                return false;
            }

            //减去本次所需要的AP
            battleActor.addActionPoint(battleActor.myId, -needAp);
            //释放本次技能
            skillEffect(targetActor);

            return true;
        }
    }
}

