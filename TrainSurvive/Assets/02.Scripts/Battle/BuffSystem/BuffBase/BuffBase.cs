/*
 * 描述：这是buff的基类
 * 作者：王安鑫
 * 创建时间：2018/12/11 15:41:13
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public abstract class BuffBase {
        
        //是否可叠加
        public readonly bool isCanOverlay;
        //当前层数
        public int floorNum;
        //最大的层数（如果可叠加，最大层数默认999，如果不可叠加，最大层数默认为1）
        public readonly int maxFloorNum;

        //当前buff效果绑定的actor
        protected BattleActor battleActor;

        //当前buff的持续时间
        public float curPassTime;

        //持续的时间
        public float maxDurationTime;

        //绑定相应的角色对象，以及当前buff是否可以叠加
        public BuffBase(BattleActor battleActor, bool isCanOverlay, int maxFloorNum = 1) {
            this.battleActor = battleActor;
            this.isCanOverlay = isCanOverlay;
            this.maxFloorNum = maxFloorNum;
        }

        /// <summary>
        /// 给当前角色设置buff
        /// 默认设置的层数为1
        /// </summary>
        public abstract void setBuff(int floor = 1);

        public abstract object Clone(BattleActor battleActor);
    }
}

