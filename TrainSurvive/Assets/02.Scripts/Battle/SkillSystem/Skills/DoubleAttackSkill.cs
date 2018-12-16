/*
 * 描述：这是二连击技能
 * 消耗30
 * 增加500%攻速，直至两次攻击后结束
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class DoubleAttackSkill : Skill {

        //当前技能消耗的AP
        public new const int needAp = 0;
        //当前技能是不是被动技能
        public new const bool isPassive = false;

        public DoubleAttackSkill(BattleActor battleActor)
            : base(battleActor, needAp, isPassive) {
        }

        /// <summary>
        /// 给自己加一个攻击加速buff，持续时间两次攻击
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            //给自己加一个移速加速buff
            Buff buff = BuffFactory.getBuff(
                    BuffFactory.BuffEnum.ATTACK_SPEED_UP_BUFF,
                    battleActor);

            //获取当中的攻击增加
            BuffBase buffBase = buff.buffList[0];

            //修改当前最大持续时间为两次攻击的时间
            buffBase.maxDurationTime = battleActor.atkNeedTime / 5 * 2;

            //设置buff
            battleActor.setBuffEffect(buff);
        }
    }
}

