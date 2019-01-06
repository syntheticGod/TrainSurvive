/*
 * 描述：这是攻击子状态
 * 作者：王安鑫
 * 创建时间：2018/12/7 19:47:59
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldBattle.BattleActor;

namespace WorldBattle {
    public class AttackSubState : AISubState{
        //当前攻击前摇已等待的时间
        private float curAtkWindUpPassTime = 0.0f;       

        //初始化攻击子状态
        public AttackSubState(BattleActor actor, Animator animator) : base(actor, animator) {
        }

        /// <summary>
        /// 进行攻击子状态
        /// 开始攻击动画
        /// 攻击动画结束后，对目标进行攻击
        /// </summary>
        /// <returns></returns>
        public override void executeState() {
            //获取本次的攻击目标
            int atkTarget = battleActor.atkTarget;

            //如果当前目标已经死亡
            if (atkTarget == -1 || battleActor.enemyActors[atkTarget].isAlive == false) {
                //停止本次攻击，返回
                stopThisAttack();

                return;
            }

            //如果当前攻击时间小于攻击等待时间（攻速影响）不攻击
            if (battleActor.curAtkPassTime < battleActor.atkNeedTime) {
                return;
            }

            //如果此时是首次进入攻击状态，播放攻击动画
            if (curAtkWindUpPassTime == 0.0f) {
                //播放攻击动画
                playAttackAnimation();
            }

            //增加当前攻击等待时间
            curAtkWindUpPassTime += Time.deltaTime;

            //如果当前时间小于攻击前摇等待时间
            if (curAtkWindUpPassTime < battleActor.atkWindUpTime) {
                //继续等待，不做操作
                return;
            }

            //先判断本次攻击是否命中
            if (battleActor.enemyActors[atkTarget].isHit(battleActor.myId, battleActor.hitRate) == true) {
                //如果此次攻击命中
                //对敌人进行攻击操作
                float damage = battleActor.enemyActors[atkTarget].getDamage(battleActor.myId, battleActor.atkDamage);

                //增加攻击上浮数值
                HelpGenerateInfo.generateInfo(battleActor.enemyActors[atkTarget], "" + (int)damage);

                //每次攻击后增加行动值，保证不大于最大值
                battleActor.addActionPoint(battleActor.myId, battleActor.apRecovery);

                //给敌人加一个攻击减速buff
                battleActor.enemyActors[atkTarget].setBuffEffect(
                    BuffFactory.getBuff(
                        "Attack",
                        battleActor.enemyActors[atkTarget])
                );

                //给每个人增加普攻击退
                battleActor.enemyActors[atkTarget].changeRealPos(
                    battleActor.enemyActors[atkTarget].pos + battleActor.getMotionDir() * 0.5f
                    );

                //给攻击目标执行被动技能
                battleActor.releasePassiveBuff(battleActor.enemyActors[atkTarget]);
            } else {
                //如果此次攻击被敌人闪避
                Debug.Log((battleActor.isPlayer ? "玩家" : "敌人")
                    + battleActor.myId
                    + "攻击"
                    + (battleActor.enemyActors[atkTarget].isPlayer ? "玩家" : "敌人")
                    + battleActor.enemyActors[atkTarget].myId
                    + "被闪避!");
            }

            //本次攻击完，停止本次攻击
            stopThisAttack();
        }

        /// <summary>
        /// 开始进入攻击状态
        /// </summary>
        public override void initState() {
            //攻击前摇时间清空
            curAtkWindUpPassTime = 0.0f;
        }      

        //播放攻击动画
        private void playAttackAnimation() {
            //Debug.Log("开始攻击！");
            //播放攻击动画
            animator.SetTrigger("attack");
            //按攻击速度设置动画速度
            battleActor.setAtkSpeedAnimate();
        }

        /// <summary>
        /// 停止本次攻击
        /// </summary>
        private void stopThisAttack() {
            //停止攻击动画？
            //animator.SetTrigger("idle");
            animator.SetTrigger("run");

            //攻击等待时间置空
            battleActor.curAtkPassTime = 0.0f;
            //攻击前摇等待时间置空
            curAtkWindUpPassTime = 0.0f;

            //如果当前角色还存活，更改其状态
            battleActor.changeSubState(ActionStateEnum.NONE);

            //本次攻击完后，更换攻击目标
            battleActor.atkTarget = -1;
        }
    }
}

