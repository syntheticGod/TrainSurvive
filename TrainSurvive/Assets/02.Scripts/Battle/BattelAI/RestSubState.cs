/*
 * 描述：这是休息子状态
 * 作者：王安鑫
 * 创建时间：2018/12/7 21:17:58
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class RestSubState : AISubState {

        //休息所需等待的时间
        private const float restNeedTime = 0.5f;
        //已经等待了休息的时间
        private float curPassTime = 0.0f;

        //初始化休息子状态
        public RestSubState(BattleActor actor, Animator animator) : base(actor, animator) {
        }

        /// <summary>
        /// 进行休息子状态
        /// </summary>
        /// <returns></returns>
        public override void executeState() {
            //增加已休息的时间
            curPassTime += Time.deltaTime;

            //如果当前时间小于休息等待时间
            if (curPassTime < restNeedTime) {
                //继续等待，不做操作
                return;
            }

            //回复一次生命值
            battleActor.addHealthPoint(battleActor.myId, battleActor.hpRecovery * 0.5f);

            //当前时间清空
            curPassTime = 0.0f;
        }

        /// <summary>
        /// 开始进入休息状态
        /// </summary>
        public override void initState() {
            //如果当前已处于休息状态，返回
            //if (actionState == ActionStateEnum.REST) {
            //    return;
            //}

            //时间清空，开始记录休息时间
            curPassTime = 0.0f;
            //设置休息子状态
            //actionState = ActionStateEnum.REST;

            //播放休息动画

        }
    }
}

