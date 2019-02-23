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
    public abstract class BaseSkill {

        //设置技能类型
        public enum SkillType {
            NONE = -1,

            //主动型
            ACTIVE,

            //攻击触发型
            ATTACK,

            //开关型
            SWITCH,

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
        public BaseSkill(BattleActor battleActor, int needAp, SkillType skillType, float skillReleaseTime = 0.5f) {
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
        public abstract bool release(BattleActor targetActor = null);

        //释放本次技能的效果
        protected abstract void skillEffect(BattleActor targetActor = null);

        //实现克隆
        public abstract BaseSkill Clone(BattleActor curActor);
    }
}

