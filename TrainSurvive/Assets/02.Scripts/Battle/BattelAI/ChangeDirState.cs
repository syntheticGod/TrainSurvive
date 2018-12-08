/*
 * 描述：
 * 作者：王安鑫
 * 创建时间：2018/12/7 22:01:47
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class ChangeDirState : AISubState {

        //保存转向的终点rotation
        private Quaternion endRotation;
        //设置转身所需的时间
        private const float changeDirAnimationTime = 0.3f;
        //当前等待的时间
        private float curPassTime = 0.0f;
        //记录actor的transform
        private Transform myTransform;

        //初始化转换方向子状态
        public ChangeDirState(BattleActor actor, Animator animator) : base(actor, animator) {
        }

        //播放转身动画（平滑旋转）
        
        public override void executeState() {
            float rotateT = 180 / changeDirAnimationTime / Quaternion.Angle(myTransform.rotation, endRotation) * Time.deltaTime;
            //增加当前等待时间
            curPassTime += Time.deltaTime;

            if (rotateT >= 180.0f) {
                myTransform.rotation = endRotation;
                //如果转向结束，改成下一个状态
                battleActor.changeNextSubState();
            } else {
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, endRotation, rotateT);
            }
        }

        public override void initState() {
            this.myTransform = battleActor.myTransform;
            //从0转到180或者从180转到0
            float start = myTransform.rotation.y;
            float end = (start == 0.0f ? 180.0f : 0.0f);

            //转身的播放速度          
            endRotation = Quaternion.Euler(myTransform.rotation.eulerAngles + new Vector3(0, 180.0f, 0));

            //初始化当前等待时间
            curPassTime = 0.0f;
            //播放转身动画
            //playChangeMotion();
        }
    }
}

