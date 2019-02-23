/*
 * 描述：这是技能基类
 * 作者：王安鑫
 * 创建时间：2018/12/13 14:13:17
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public abstract class TokenSkill : BaseSkill {

        //判断当前技能是否处于开启中
        public bool isOpen;

        //记录等待多长时间
        public float curPassTime;

        //构造函数
        public TokenSkill(BattleActor battleActor, int needAp, SkillType skillType, float skillReleaseTime = 0.5F)
            : base(battleActor, needAp, skillType, skillReleaseTime) {
            //默认当前技能未开启
            isOpen = false;
        }

        //释放本次技能
        public override bool release(BattleActor targetActor = null) {
            if (isOpen == false) {
                //如果不可以释放该技能，返回false
                if (canReleaseSkill() == false) {
                    return false;
                }

                //初始化等待时间为0.0s
                curPassTime = 0.0f;

                //开启协程
                battleActor.StartCoroutine(checkDecreaseAp());
            } else {
                //设置等待时间超过1s
                curPassTime = 1.0f;
                //将技能处于关闭状态
                isOpen = false;
            }

            return true;
        }

        //关闭此次技能的effect
        protected abstract void closeSkillEffect();

        //释放技能，每秒扣除AP，如果AP值不够，则对效果进行关闭
        private IEnumerator checkDecreaseAp() {
            //释放本次技能
            skillEffect();
            //将技能处于开启状态
            isOpen = true;
            //减去本次所需要的AP
            battleActor.addActionPoint(battleActor.myId, -needAp);

            //等待对应的时间
            while (canReleaseSkill() == true && isOpen == true) {
                //等待对应的时间
                while (curPassTime < 1.0f) {
                    //增加每帧流逝的时间
                    curPassTime += Time.deltaTime;
                    yield return 0;
                }
                
                //如果过去1s
                curPassTime -= 1.0f;

                //如果当前技能仍然开启
                if (isOpen == true) {
                    //减去本次所需要的AP
                    battleActor.addActionPoint(battleActor.myId, -needAp);
                }
            }

            //强制让技能关闭
            isOpen = false;
            //将技能效果关闭
            closeSkillEffect();
        }
    }
}

