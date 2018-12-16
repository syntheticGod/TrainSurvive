/*
 * 描述：这是每个子状态的基类
 * 每次第一次切换状态（上个状态和当前状态不一致）的时候会执行initState
 * 每一帧执行的时候BattleActor的update会调用executeState函数
 * 
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

