/*
 * 描述：这是一个组合buff的基类
 * 通过构造方法，生成各种buff
 * 再通过setBuff将list中的buff都执行
 * 
 * 作者：王安鑫
 * 创建时间：2018/12/12 23:54:56
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldBattle.BuffFactory;

namespace WorldBattle {
    public class Buff{
        //当前buff类型
        public BuffEnum buffType { get; private set; }
        //构建组合Buff列表
        public List<BuffBase> buffList;
        //绑定的battleActor
        public BattleActor battleActor;

        public Buff(BattleActor battleActor, BuffEnum buffType) {
            //初始化buff效果队列
            buffList = new List<BuffBase>();
            //初始化buff类型
            this.buffType = buffType;
        }

        /// <summary>
        /// 返回Buff类库的深拷贝
        /// </summary>
        /// <param name="battleActor"></param>
        /// <returns></returns>
        public object Clone(BattleActor battleActor) {
            //在buff链表中增加绑定指定对象的clone对象
            Buff cloneBuff = new Buff(battleActor, buffType);
            foreach (BuffBase buff in buffList) {
                cloneBuff.buffList.Add((BuffBase)buff.Clone(battleActor));
            }
            return cloneBuff;
        }

        /// <summary>
        /// 执行当前列表中的所有buff效果
        /// </summary>
        /// <param name="floor"></param>
        public void setBuff(int floor = 1) {
            foreach (BuffBase buff in buffList) {
                buff.setBuff(floor);
            }
        }
    }
}

