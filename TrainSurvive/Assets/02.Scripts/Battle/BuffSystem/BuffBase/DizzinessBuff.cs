/*
 * 描述：这是一个眩晕类型的buff，包括眩晕，冰冻
 * 作者：王安鑫
 * 创建时间：2018/12/11 17:10:55
 * 版本：v0.1
 */
using System;
using System.Collections;
using UnityEngine;

namespace WorldBattle {
    public class DizzinessBuff : BuffBase {
        //更改的类型
        public enum TypePropertyEnum {
            NONE = -1,

            //眩晕
            DIZZINESS,

            //冰冻
            FROZEN,

            NUM
        }

        //眩晕的类型
        TypePropertyEnum typeProperty;

        public DizzinessBuff(BattleActor battleActor,
            TypePropertyEnum typeProperty,
            float maxDurationTime, bool isCanOverlay, int maxFloorNum = 1)
            : base(battleActor, isCanOverlay, maxDurationTime, maxFloorNum) {

            //设置当前的层数为0（还未开始）
            floorNum = 0;
            //设置当前所要更改的角色属性
            this.typeProperty = typeProperty;
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
            //设置玩家眩晕，不行动
            battleActor.isActorStop = true;
            //当前眩晕buff数加1
            battleActor.actorStopNum++;

            //等待对应的时间
            while (curPassTime < maxDurationTime) {
                curPassTime += Time.deltaTime;
                yield return 0;
            }

            //当前眩晕buff数减1
            battleActor.actorStopNum--;
            //如果当前眩晕buff数为0且当前角色还存活，恢复行动数
            if (battleActor.actorStopNum == 0 && battleActor.isAlive == true) {
                battleActor.isActorStop = false;
            }
        }

        //返回一个新的类
        public override object Clone(BattleActor battleActor) {
            return new DizzinessBuff(
                battleActor, 
                typeProperty,
                maxDurationTime,
                isCanOverlay,
                maxFloorNum);
        }
    }
}
