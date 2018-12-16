/*
 * 描述：这是加速技能
 * 消耗0 
 * 使自身移动速度增加500%，持续5秒
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class SpeedUpSkill : Skill {

        //当前技能消耗的AP
        public new const int needAp = 0;
        //当前技能是不是被动技能
        public new const bool isPassive = false;

        public SpeedUpSkill(BattleActor battleActor)
            : base(battleActor, needAp, isPassive) {
        }

        /// <summary>
        /// 给自己加一个移速加速buff
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            //给自己加一个移速加速buff
            battleActor.setBuffEffect(
                BuffFactory.getBuff(
                    BuffFactory.BuffEnum.MOVE_SPEED_UP_BUFF,
                    battleActor)
            );
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override Skill Clone(BattleActor curActor) {
            return new SpeedUpSkill(curActor);
        }
    }
}

