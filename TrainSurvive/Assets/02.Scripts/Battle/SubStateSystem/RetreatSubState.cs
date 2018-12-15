using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class RetreatSubState : AISubState {
        //设置撤退淡出所需的时间
        private const float retreatAnimationTime = 1.0f;
        //当前等待的时间
        private float curPassTime = 0.0f;

        //初始化撤退子状态
        public RetreatSubState(BattleActor actor, Animator animator) : base(actor, animator) {
        }

        //执行撤退状态
        public override void executeState() {
            //增加等待时间
            curPassTime += Time.deltaTime;

            //如果已经全部淡出，销毁gameObject
            if (curPassTime > retreatAnimationTime) {
                //删除当前的gameObject
                GameObject.Destroy(battleActor.playerPrefab);
                //设置当前角色停止
                battleActor.isActorStop = true;
            }
        }

        //初始化撤退子状态
        public override void initState() {
            //播放撤退动画
            animator.SetTrigger("dead");
            //设置正常的动画播放速度
            animator.speed = 1.0f;

            //显示玩家已死亡
            battleActor.nameText.text += "(撤退)";

            //该角色撤退，未存活
            battleActor.isAlive = false;

            //每次有角色撤退调用battleConroller判断本场战斗是否结束
            battleActor.battleController.isBattleEnd(battleActor.isPlayer);

            //设置当前撤退的等待时间
            curPassTime = 0.0f;
        }
    }
}

