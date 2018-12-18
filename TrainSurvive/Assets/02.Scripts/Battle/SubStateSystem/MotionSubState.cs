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

            //对逻辑位置开始移动(方向，移速，移动改变比率，上一帧的时间)
            battleActor.pos += battleActor.getMotionDir() * battleActor.moveSpeed * battleActor.moveSpeedChangeRate * Time.deltaTime;
            //将坐标限制为0到最大的mapLen中
            battleActor.pos = Mathf.Clamp(battleActor.pos, 0, battleActor.battleMapLen);

            //Debug.Log(battleActor.moveSpeed);

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
            //设置动画播放速度
            battleActor.setMoveSpeedAnimate();
        }

    }
}

