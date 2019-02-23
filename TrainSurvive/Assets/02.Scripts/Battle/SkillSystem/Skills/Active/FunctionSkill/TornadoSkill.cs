/*
 * 描述：这是龙卷风技能
 * 进阶风系魔法，推出一道飓风至面前20米的位置，并击退路径上所有敌人。
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:40:11
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class TornadoSkill : NormalSkill {

        //龙卷风前进的范围
        public readonly float rangeRate;
        //龙卷风前进的速度
        public readonly float speedRate = 1.0f;

        //当前等待时间
        private float curPassTime;

        public TornadoSkill(BattleActor battleActor, int needAp, SkillType skillType
            , float rangeRate)
            : base(battleActor, needAp, skillType) {
            this.rangeRate = rangeRate;
        }

        /// <summary>
        /// 推出一道飓风至面前20米的位置，并击退路径上所有敌人
        /// </summary>
        protected override void skillEffect(BattleActor targetActor = null) {
            //获取龙卷风最终停留位置
            float endPos = battleActor.pos + 20 * battleActor.getMotionDir();
            //限制在地图内
            endPos = Mathf.Clamp(endPos, 0, BattleController.getInstance().battleMapLen);

            //设置当前等待时间为0
            curPassTime = 0.0f;

            //在玩家的朝向下，释放龙卷风技能
            battleActor.StartCoroutine(
                startEffect(battleActor.pos, endPos, battleActor.getMotionDir(), battleActor)
                );
        }

        /// <summary>
        /// 推出一道飓风至面前20米的位置，并击退路径上所有敌人
        /// </summary>
        /// <returns></returns>
        private IEnumerator startEffect(float pos, float endPos, int motionDir, BattleActor battleActor) {
            while (pos < endPos) {
                //等待对应的时间
                while (curPassTime < 0.5f) {
                    curPassTime += Time.deltaTime;
                    yield return 0;
                }

                //过去0.5s
                curPassTime -= 0.5f;
                //算出0.5s内龙卷风前进的距离
                float newPos = pos + motionDir * speedRate;

                //如果在此距离内有敌人在，将其往后退
                foreach (BattleActor enemy in battleActor.enemyActors) {
                    if (enemy.pos >= pos && enemy.pos <= newPos) {
                        //把敌人往同方向推速度的一半
                        enemy.changeRealPos(enemy.pos + motionDir * speedRate);
                    }
                }

                //赋予pos新值
                pos = newPos;
            }
        }


        /// <summary>
        /// 实现克隆方法
        /// </summary>
        /// <param name="curActor"></param>
        /// <returns></returns>
        public override BaseSkill Clone(BattleActor curActor) {
            return new TornadoSkill(curActor, needAp, skillType, rangeRate);
        }
    }
}

