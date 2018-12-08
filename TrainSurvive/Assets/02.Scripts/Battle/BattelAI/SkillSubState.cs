/*
 * 描述：这是释放技能子状态
 * 作者：王安鑫
 * 创建时间：2018/12/7 21:38:29
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldBattle.BattleActor;

namespace WorldBattle {
    public class SkillSubState : AISubState {
        //释放技能所需等待的时间(设立统一的固定CD)
        private const float skillNeedTime = 1.06f;

        //释放技能已等待的时间
        private float curPassTime = 0.0f;

        //初始化释放技能子状态
        public SkillSubState(BattleActor actor, Animator animator) : base(actor, animator) {
        }

        /// <summary>
        /// 进行释放技能子状态
        /// </summary>
        /// <returns></returns>
        public override void executeState() {
            //如果此时是首次进入释放技能状态，播放技能动画
            if (curPassTime == 0.0f) {

            }

            //增加释放技能等待时间
            curPassTime += Time.deltaTime;

            //如果当前时间小于释放技能等待时间
            if (curPassTime < skillNeedTime) {
                //继续等待，不做操作
                return;
            }

            //释放此次技能（效果）

            //结束此次技能的释放

            //等待时间置空
            curPassTime = 0.0f;

            //如果当前角色还存活，更改其状态
            battleActor.changeNextSubState();
        }

        public override void initState() {
            //初始化当前的已等待时间为0
            curPassTime = 0.0f;

            //播放技能动画
            animator.SetTrigger("skill");
        }
    }
}

