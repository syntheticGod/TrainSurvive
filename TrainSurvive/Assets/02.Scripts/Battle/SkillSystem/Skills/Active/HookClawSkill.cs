/*
 * 描述：这是勾爪技能
 * 消耗40AP，抓取当前攻击目标至面前1米处，并晕眩3秒
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class HookClawSkill : NormalSkill {
        //范围参数
        public readonly float rangeRate;
        //时长参数
        public readonly float timeRate;

        public HookClawSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float rangeRate, float timeRate)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
            this.timeRate = timeRate;
        }

        /// <summary>
        /// 抓取当前攻击目标至面前1米处，并晕眩3秒
        /// </summary>

        protected override void skillEffect(BattleActor targetActor = null) {
            int target = HelpSelectTarget.selectNearestEnemy(battleActor);
            //对目标造成伤害（如果目标存在）
            if (target != -1) {
                //获取对应目标
                targetActor = battleActor.enemyActors[target];

                //获取施法人朝向
                int motionDir = battleActor.getMotionDir();
                //改变目标的位置，为前方1m
                targetActor.changeRealPos(battleActor.pos + motionDir * rangeRate);

                //眩晕3s，上眩晕buff
                //按给定技能初始化buff
                Buff buff = new Buff(battleActor, "HookClawSkill");
                buff.buffList.Add(new DizzinessBuff(
                    targetActor, DizzinessBuff.TypePropertyEnum.DIZZINESS, timeRate, false, 1));
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
            return new HookClawSkill(curActor, needAp, skillType, rangeRate, timeRate);
        }
    }
}

