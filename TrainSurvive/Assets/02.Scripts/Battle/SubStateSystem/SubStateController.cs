/*
 * 描述：这个是控制每个战斗角色的子状态的类
 * 作者：王安鑫
 * 创建时间：2018/12/7 20:32:13
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldBattle.BattleActor;

namespace WorldBattle {
    public class SubStateController : MonoBehaviour {
        //当前角色的状态
        public ActionStateEnum curActionState;
        //当前角色的下一个状态
        public ActionStateEnum nextActionState;

        //当前战斗角色
        private BattleActor actor;
        //当前角色的动画器
        private Animator animator;

        //角色当前的状态
        private AISubState subActionState;

        //初始化当前的所有子状态
        public SubStateController(BattleActor actor, Animator animator) {
            //当前角色的状态为不动
            curActionState = ActionStateEnum.NONE;
            //当前的状态行为为空
            subActionState = null;

            //初始化当前的角色和动画器
            this.actor = actor;
            this.animator = animator;
        }

        /// <summary>
        /// 转换为下一个状态
        /// </summary>
        public void changeNextSubState() {
            curActionState = ActionStateEnum.NONE;
            changeSubState(nextActionState);
            nextActionState = ActionStateEnum.NONE;
        }

        /// <summary>
        /// 提供子状态之间切换的方法
        /// NONE优先级最低，MOTION
        /// 转向和释放技能前摇为原子性
        /// </summary>
        /// <param name="nextActionState"></param>
        public void changeSubState(ActionStateEnum nextActionState) {
            //if (actor.isPlayer)
            //Debug.Log((actor.isPlayer ? "玩家" : "敌人")
            //    + actor.myId
            //    + "当前状态为：" + curActionState
            //    + " 下一个状态为：" + nextActionState);

            //如果当前状态等于上一个状态，不做转变
            if (curActionState == nextActionState) {
                return;
            }

            do {
                bool isHighestPriority = false;
                //如果下一个状态为死亡、撤退、胜利
                //（优先级最高的状态）
                switch (nextActionState) {
                    //如果为NONE或WIN，直接返回，不做操作
                    case ActionStateEnum.NONE:
                    case ActionStateEnum.WIN:
                        subActionState = null;
                        curActionState = nextActionState;
                        return;
                    
                    case ActionStateEnum.DEAD:
                        isHighestPriority = true;
                        subActionState = new DeadSubState(actor, animator);
                        break;
                    case ActionStateEnum.RETREAT:
                        isHighestPriority = true;
                        subActionState = new RetreatSubState(actor, animator);
                        break;
                }

                //如果当前状态为优先级最高的状态，直接跳出
                if (isHighestPriority) {
                    break;
                }

                //查看当前状态是否为优先级最高的状态或是原子状态
                switch (curActionState) {
                    //如果当前状态为死亡、撤退、胜利
                    //不能修改当前状态
                    case ActionStateEnum.DEAD:
                    case ActionStateEnum.WIN:
                    case ActionStateEnum.RETREAT:
                        return;

                    //如果当前状态是改变方向、释放技能状态（原子状态）
                    //不能修改
                    case ActionStateEnum.CHANGE_DIR:
                    case ActionStateEnum.SKILL:
                        //如果下一个状态已经保存了其中一个原子状态，则不保存其它状态
                        if (this.nextActionState != ActionStateEnum.CHANGE_DIR
                            && this.nextActionState != ActionStateEnum.SKILL) {
                            //保存下一个状态
                            this.nextActionState = nextActionState;
                        }
                        return;
                }

                //根据下一个子状态做相应的转换
                switch (nextActionState) {
                    //转换为攻击状态
                    case ActionStateEnum.ATTACK:
                        subActionState = new AttackSubState(actor, animator);
                        break;

                    //转换为移动状态
                    case ActionStateEnum.MOTION:
                        subActionState = new MotionSubState(actor, animator);
                        break;

                    //转换为休息状态
                    case ActionStateEnum.REST:
                        subActionState = new RestSubState(actor, animator);
                        break;

                    //转换为释放技能状态
                    case ActionStateEnum.SKILL:
                        subActionState = new SkillSubState(actor, animator);
                        break;

                    //转换为改变方向状态
                    case ActionStateEnum.CHANGE_DIR:
                        subActionState = new ChangeDirState(actor, animator);
                        break;
                }
            } while (false);
            

            //设置状态变为下一个子状态
            curActionState = nextActionState;
            //初始化当前的状态
            subActionState.initState();
        }

        //执行当前子状态
        public void executeCurState() {
            //如果当前子状态不为空
            if (subActionState != null) {
                //Debug.Log(curActionState);
                subActionState.executeState();
            }
        }
    }
}

