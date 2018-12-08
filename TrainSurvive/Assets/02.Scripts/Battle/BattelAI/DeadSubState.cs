/*
 * 描述：这是死亡子状态
 * 作者：王安鑫
 * 创建时间：2018/12/7 22:46:57
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class DeadSubState : AISubState {
        //设置死亡淡出所需的时间
        private const float deadAnimationTime = 1.0f;
        //当前等待的时间
        private float curPassTime = 0.0f;

        //初始化死亡子状态
        public DeadSubState(BattleActor actor, Animator animator) : base(actor, animator) {
        }

        //执行死亡子状态
        public override void executeState() {
            //增加等待时间
            curPassTime += Time.deltaTime;

            //如果已经全部淡出，销毁gameObject
            if (curPassTime > deadAnimationTime) {
                //删除当前的gameObject
                GameObject.Destroy(battleActor.playerPrefab);
                //设置当前角色停止
                battleActor.isActorStop = true;
            }
        }

        //初始化死亡子状态
        public override void initState() {
            //播放死亡动画
            animator.SetTrigger("dead");

            //该角色死亡，未存活
            battleActor.isAlive = false;

            //每次有角色死亡调用battleConroller判断本场战斗是否结束
            battleActor.battleController.isBattleEnd(battleActor.isPlayer);

            //设置当前死亡的等待时间
            curPassTime = 0.0f;
        }
    }
}

