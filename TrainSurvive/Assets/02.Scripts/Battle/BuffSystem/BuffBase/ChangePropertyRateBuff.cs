/*
 * 描述：这是一个按比率更改角色属性的buff基类（只乘除）
 * 作者：王安鑫
 * 创建时间：2018/12/11 17:10:55
 * 版本：v0.1
 */
using System;
using System.Collections;
using UnityEngine;

namespace WorldBattle {
    public class ChangePropertyRateBuff : BuffBase {
        //更改的角色类型
        public enum RatePropertyEnum {
            NONE = -1,

            //更改移动速度
            MOVE_SPEED,

            //更改攻击速度
            ATTACK_SPEED,

            NUM
        }

        //更改的比率
        public readonly float changeRate;
        //所要改变角色的属性
        public readonly RatePropertyEnum actorProperty;
        //持续的时间
        //public float maxDurationTime;
       
        public ChangePropertyRateBuff(BattleActor battleActor,
            float changeRate, float maxDurationTime, RatePropertyEnum actorProperty,
            bool isCanOverlay, int maxFloorNum = 1)
            : base(battleActor, isCanOverlay, maxFloorNum) {

            //设置当前的层数为0（还未开始）
            floorNum = 0;
            //设置当前属性修改的比率
            this.changeRate = changeRate;
            //设置当前最大的持续时间
            this.maxDurationTime = maxDurationTime;
            //设置当前所要更改的角色属性
            this.actorProperty = actorProperty;
        }

        /// <summary>
        /// 刷新当前buf的时间
        /// </summary>
        /// <param name="addFloorNum">如果是那种可叠加的</param>
        public override void setBuff(int addFloorNum = 1) {
            //设置当前开始时间为0（刷新，或者初始化）
            curPassTime = 0.0f;

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
            //按比率更改相应的角色属性
            changeActorProperty(actorProperty, changeRate * floorNum);

            //等待对应的时间
            while (curPassTime < maxDurationTime) {
                curPassTime += Time.deltaTime;
                yield return 0;
            }

            //恢复原本的角色属性
            changeActorProperty(actorProperty, 1.0f / (changeRate * floorNum));
            //设置当前不执行
            floorNum = 0;
        }

        /// <summary>
        /// 根据相应的比率，更改角色的属性
        /// </summary>
        /// <param name="actorProperty">对应角色的属性</param>
        /// <param name="rate">对应的比率</param>
        private void changeActorProperty(RatePropertyEnum actorProperty, float rate) {
            switch (actorProperty) {
                //更改移动速度
                case RatePropertyEnum.MOVE_SPEED:
                    battleActor.moveSpeedChangeRate *= rate;
                    //battleActor.moveSpeed *= rate;
                    break;

                //更改攻击速度(因为玩家计算的是攻击间隔时间所以要除以)
                case RatePropertyEnum.ATTACK_SPEED:
                    battleActor.atkNeedTime /= rate;
                    Debug.Log(battleActor.atkNeedTime);
                    break;
            }
        }

        //返回一个新的类
        public override object Clone(BattleActor battleActor) {
            return new ChangePropertyRateBuff(
                battleActor, 
                changeRate,
                maxDurationTime,
                actorProperty,
                isCanOverlay,
                maxFloorNum);
        }
    }
}
