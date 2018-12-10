/*
 * 描述：这是移动子状态
 * 作者：王安鑫
 * 创建时间：2018/12/7 21:03:29
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class MotionSubState : AISubState {
        //初始化移动子状态
        public MotionSubState(BattleActor actor, Animator animator) : base(actor, animator) {
        }

        /// <summary>
        /// 进行移动子状态
        /// </summary>
        /// <returns></returns>
        public override void executeState() {
            //播放移动动画
            playMoveAnimation();

            //对逻辑位置开始移动
            battleActor.pos += battleActor.getMotionDir() * battleActor.moveSpeed * Time.deltaTime;
            //将坐标限制为0到最大的mapLen中
            battleActor.pos = Mathf.Clamp(battleActor.pos, 0, battleActor.battleMapLen);

            //将当前位置赋予角色上
            battleActor.changeRealPos();
        }

        public override void initState() {

           // throw new System.NotImplementedException();
        }

        //播放移动动画
        private void playMoveAnimation() {
            //animator.SetTrigger("idle");
            animator.SetTrigger("run");

            if (battleActor.isPlayer)
            Debug.Log("播放跑步动画");
            //获取跑步的动画播放速度
            //float length = animator.GetCurrentAnimatorStateInfo(0).length;
            //按照移速更改跑步的播放速度
            animator.speed = 1.0f * battleActor.moveSpeed / 1.0f;
        }

    }
}

