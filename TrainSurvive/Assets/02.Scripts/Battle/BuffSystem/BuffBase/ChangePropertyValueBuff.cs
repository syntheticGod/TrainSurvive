/*
 * 描述：这是一个按值更改角色属性的buff基类（只加减）
 * 作者：王安鑫
 * 创建时间：2018/12/12 15:39:36
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class ChangePropertyValueBuff : BuffBase {
        //更改的角色类型
        public enum ValuePropertyEnum {
            NONE = -1,

            //更改生命值
            HP,

            //更改行动力
            AP,

            NUM
        }

        //更改的值（每个层数改的值）
        protected readonly float changeValue;
        //所要改变角色的属性
        protected readonly ValuePropertyEnum actorProperty;
        //每次数值变化间隔的时间
        protected readonly float intervalTime;

        //记录当前执行的次数
        private int executeNum;
        //记录当前最大的执行次数
        private int maxExecuteNum;

        public ChangePropertyValueBuff(BattleActor battleActor,
            float changeValue, float maxDurationTime, float intervalTime,
            ValuePropertyEnum actorProperty,
            bool isCanOverlay, int maxFloorNum = 1)
            : base(battleActor, isCanOverlay, maxDurationTime, maxFloorNum) {

            //设置当前的层数为0（还未开始）
            floorNum = 0;
            //设置当前属性修改的值
            this.changeValue = changeValue;
            //设置当前每隔多长时间执行一次
            this.intervalTime = intervalTime;
            //设置当前所要更改的角色属性
            this.actorProperty = actorProperty;

            //计算出最大执行次数
            maxExecuteNum = (int)Mathf.Floor(maxDurationTime / intervalTime);
        }

        /// <summary>
        /// 刷新当前buf的时间
        /// </summary>
        /// <param name="addFloorNum">如果是那种可叠加的</param>
        public override void setBuff(int addFloorNum = 1) {
            //设置当前执行次数为0（刷新最初的执行时间）
            executeNum = 0;

            //如果当前层数为0
            if (floorNum == 0) {
                //增加相应的层数
                floorNum = Mathf.Min(addFloorNum + floorNum, maxFloorNum);
                //通过绑定的对象启动线程(当前没有执行)
                battleActor.StartCoroutine(effectActor());
            } else {
                //增加相应的层数
                floorNum = Mathf.Min(addFloorNum + floorNum, maxFloorNum);
            }
        }

        /// <summary>
        /// 对角色的属性产生暂时的影响
        /// 一段时间后恢复
        /// </summary>
        /// <returns></returns>
        private IEnumerator effectActor() {
            //等待执行到最大执行次数
            while (executeNum < maxExecuteNum) {
                //等待对应的间隔时间
                while (curPassTime < intervalTime) {
                    curPassTime += Time.deltaTime;

                    yield return 0;
                }
                //按值增加相应的角色属性
                changeActorProperty(actorProperty, changeValue * floorNum);
                //增加执行次数
                executeNum++;
                //当前执行时间置空
                curPassTime = 0.0f;
            }
            
            //设置当前不执行（经过最大执行次数后，不执行）
            floorNum = 0;
        }

        /// <summary>
        /// 根据相应修改的值，更改角色的属性
        /// </summary>
        /// <param name="actorProperty">对应角色的属性</param>
        /// <param name="value">对应修改的值</param>
        private void changeActorProperty(ValuePropertyEnum actorProperty, float value) {
            switch (actorProperty) {
                //更改移动速度
                case ValuePropertyEnum.HP:
                    battleActor.addHealthPoint(battleActor.myId, value);
                    break;

                //更改攻击速度(因为玩家计算的是攻击间隔时间所以要除以)
                case ValuePropertyEnum.AP:
                    battleActor.addActionPoint(battleActor.myId, value);
                    break;
            }
        }

        //返回一个新的类
        public override object Clone(BattleActor battleActor) {
            return new ChangePropertyValueBuff(
                battleActor,
                changeValue,
                maxDurationTime,
                intervalTime,
                actorProperty,
                isCanOverlay,
                maxFloorNum);
        }
    }
}

