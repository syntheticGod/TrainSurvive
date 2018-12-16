/*
 * 描述：这是一个空技能
 * 作者：王安鑫
 * 创建时间：2018/12/15 10:47:37
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class NoneSkill : Skill {
        //收成默认值
        public NoneSkill(BattleActor battleActor)
            : base(battleActor, 0, false) {
        }

        //技能不执行
        protected override void skillEffect(BattleActor targetActor = null) {
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override Skill Clone(BattleActor curActor) {
            return new NoneSkill(curActor);
        }
    }
}

