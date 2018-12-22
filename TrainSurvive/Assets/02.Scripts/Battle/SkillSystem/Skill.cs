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
    public abstract class Skill {

        //设置技能类型
        public enum SkillType {
            NONE = -1,

            //主动型
            ACTIVE,

            //攻击触发型
            ATTACK,

            //被动型
            PASSIVE,

            NUM
        };

        //当前技能消耗的AP
        public readonly int needAp;
        //当前技能是不是被动技能
        public readonly SkillType skillType;
        //当前技能依附的battleActor
        public BattleActor battleActor;
        //技能释放的时间
        public float skillReleaseTime;

        //对技能类的构造函数
        public Skill(BattleActor battleActor, int needAp, SkillType skillType, float skillReleaseTime = 0.5f) {
            this.skillType = skillType;
            this.needAp = needAp;
            this.battleActor = battleActor;
            this.skillReleaseTime = skillReleaseTime;
        }

        //能否释放此技能
        public bool canReleaseSkill() {
            return battleActor.curActionPoint >= needAp;
        }

        //释放本次技能
        public bool release(BattleActor targetActor = null) {
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

        //释放本次技能的效果
        protected abstract void skillEffect(BattleActor targetActor = null);

        //实现克隆
        public abstract Skill Clone(BattleActor curActor);
    }
}

