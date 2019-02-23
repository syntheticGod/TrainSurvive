/*
 * 描述：这是背刺技能
 * 消耗50AP，瞬间位移至目标身后1尺处，并增加30%攻速，持续5秒
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class BackStabSkill : NormalSkill {
        //范围参数
        public readonly float rangeRate;
        //攻速加成参数
        public readonly float atkSpeedRate;
        //时长参数
        public readonly float timeRate;

        public BackStabSkill(BattleActor battleActor, int needAp, SkillType skillType
            ,float rangeRate, float atkSpeedRate, float timeRate)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
            this.atkSpeedRate = atkSpeedRate;
            this.timeRate = timeRate;
        }

        /// <summary>
        /// 瞬间位移至目标身后1尺处，并增加30%攻速，持续5秒
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //获取对应目标
                targetActor = battleActor.enemyActors[target];

                //获取目标人朝向
                int motionDir = targetActor.getMotionDir();
                //改变自身的位置，为目标后方1m
                battleActor.changeRealPos(targetActor.pos - motionDir * rangeRate);

                //给自己加一个攻速加速buff
                //按给定技能初始化buff
                Buff buff = new Buff(battleActor, "BackStabSkill");
                buff.buffList.Add(new ChangePropertyRateBuff(
                    targetActor, atkSpeedRate, timeRate,
                    ChangePropertyRateBuff.RatePropertyEnum.ATTACK_SPEED,
                    false, 1));
                //上buff
                targetActor.setBuffEffect(buff);
            }
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new BackStabSkill(curActor, needAp, skillType, rangeRate, atkSpeedRate, timeRate);
        }
    }
}

