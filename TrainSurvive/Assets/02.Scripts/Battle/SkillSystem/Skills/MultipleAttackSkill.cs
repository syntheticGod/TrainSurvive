/*
 * 描述：这是多连击技能
 * 增加500%攻速，直至多次攻击后结束
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class MultipleAttackSkill : Skill {

        //当前连击的次数
        public int attackTime;

        public MultipleAttackSkill(BattleActor battleActor, int needAp, SkillType skillType, int attackTime)
            : base(battleActor, needAp, skillType, 0) {
            this.attackTime = attackTime;
        }

        /// <summary>
        /// 给自己加一个攻击加速buff，持续时间多次攻击
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            //给自己加一个移速加速buff
            Buff buff = BuffFactory.getBuff(
                    "AttackSpeedUp",
                    battleActor);

            //获取当中的攻击增加
            BuffBase buffBase = buff.buffList[0];

            //修改当前最大持续时间为两次攻击的时间
            buffBase.maxDurationTime = (battleActor.atkNeedTime / 5 + 0.05f) * attackTime;

            //设置buff
            battleActor.setBuffEffect(buff);
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override Skill Clone(BattleActor curActor) {
            return new MultipleAttackSkill(curActor, needAp, skillType, attackTime);
        }
    }
}

