/*
 * 描述：这是枪林弹雨技能
 * 增加自身80%攻速与200%击退距离，但降低50%攻击力，持续5秒
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class BulletSkill : NormalSkill {

        public BulletSkill(BattleActor battleActor, int needAp, SkillType skillType)
            : base(battleActor, needAp, skillType) {
        }

        /// <summary>
        /// 给自己加一个增加自身80%攻速与200%击退距离，但降低50%攻击力，持续5秒
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            //给自己加一个增加自身80%攻速与200%击退距离，但降低50%攻击力，持续5秒
            battleActor.setBuffEffect(
                BuffFactory.getBuff(
                    "Bullets",
                    battleActor)
            );
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new BulletSkill(curActor, needAp, skillType);
        }
    }
}

