/*
 * 描述：这是每个子状态的基类
 * 作者：王安鑫
 * 创建时间：2018/12/7 19:54:04
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public abstract class AISubState {
        //当前子状态所对应的角色
        protected BattleActor battleActor = null;
        //获取当前的Animator
        protected Animator animator;

        //获取当前对应的角色
        public AISubState(BattleActor actor, Animator animator) {
            this.battleActor = actor;
            this.animator = animator;
        }

        //开始这个状态前的初始化
        public abstract void initState();

        //执行当前的状态
        public abstract void executeState();
    }
}

