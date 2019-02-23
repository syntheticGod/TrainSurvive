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
    public class SpeedUpSkill : NormalSkill {

        public SpeedUpSkill(BattleActor battleActor, int needAp, SkillType skillType)
            : base(battleActor, needAp, skillType) {
        }

        /// <summary>
        /// 给自己加一个移速加速buff
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            //给自己加一个移速加速buff
            battleActor.setBuffEffect(
                BuffFactory.getBuff(
                    "MoveSpeedUp",
                    battleActor)
            );
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new SpeedUpSkill(curActor, needAp, skillType);
        }
    }
}

