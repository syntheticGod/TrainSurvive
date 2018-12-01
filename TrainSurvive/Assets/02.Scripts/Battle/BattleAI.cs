/*
 * 描述：这是战斗中每个人状态控制的基类
 * 作者：王安鑫
 * 创建时间：2018/11/21 19:06:58
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldBattle {
    public abstract class BattleAI : MonoBehaviour {

        //当前角色最大的生命值（假设出场是满的）
        public float maxHealthPoint;
        //当前角色最大的行动值
        public float maxActionPoint;
        //HP恢复值（休息的时候可恢复）
        public float hpRecovery;
        //AP恢复值（行动值恢复值）
        public float apRecovery;
        //移动速度（每1s移动的距离）
        public float moveSpeed;
        //攻击力
        public float atkDamage;
        //攻击范围
        public float atkRange;
        //当前所处的逻辑位置
        public float pos;
        //当前角色的受伤比例（类似于防御值）
        public float damageRate;
        //当前角色的命中率
        public float hitRate;
        //当前角色的闪避率
        public float dodgeRate;
        //当前角色的暴击率
        public float critRate;
        //当前角色的暴击伤害比例
        public float critDamage;

        //攻击所需的时间(受攻速影响)
        public float atkNeedTime;

        //本次战斗中本角色的id
        public int myId;
        //本次战斗中是否是玩家角色
        public bool isPlayer;

        //生命值Slider
        public Slider hpSlider;
        //行动力Slider
        public Slider apSlider;
        //撤退按钮
        public Button retreatBtn;

        //释放技能所需等待的时间(设立统一的固定CD)
        private float skillNeedTime = 0.5f;
        //休息所需等待的时间
        private float restNeedTime = 0.5f;
        //撤退所需的等待时间
        protected float retreatNeedTime = 5.0f;

        //当前攻击\释放技能\休息已等待的时间
        private float curPassTime = 0.0f;
        //当前撤退的等待时间
        protected float curRetreatWaitTime = 0.0f;

        //生命值
        private float curHealthPoint;
        //行动值
        private float curActionPoint;

        //当前战斗地图的起始位置
        private Vector3 orign;
        //战斗地图的卷轴长度（假设战斗地图以x,z为平面）
        private int battleMapLen = 40;

        //角色当前的攻击目标
        public int atkTarget;
        //玩家当前选中的攻击目标
        private int selectedAtkTarget;

        //当前角色的朝向，是否处于前进状态
        private int isForward = 1;
        //角色的走向（玩家正为往前走，敌人负为往前走）
        protected int forwardDir;

        //记录本次移动的方向
        protected int curMotionDir = 0;

        //当前角色是否存活(死亡或撤退都属于不存活)
        public bool isAlive { get; protected set; }

        //设置控制射程的范围参数
        public float controlRangePara = 0.8f;

        //当前角色的状态
        public ActionStateEnum actionState;

        //当前战斗的玩家角色
        public BattleAI[] person;
        //当前战斗的敌人角色
        public BattleAI[] enemy;
        //获取当前的BattleController
        protected BattleController battleController;
        //获取当前的Animator
        private Animator animator;

        //角色当前动作的状态
        public enum ActionStateEnum {
            NONE = -1,

            //攻击
            ATTACK,

            //移动
            MOTION,

            //休息
            REST,

            //释放技能
            SKILL,

            //转向
            CHANGE_DIR,

            //已死亡
            DEAD,

            //已撤退
            RETREAT,

            //胜利
            WIN,

            NUM
        }

        //保存transform的引用，加快速度
        private Transform myTransform;

        //保存转向的终点rotation
        private Quaternion endRotation;

        // Use this for initialization
        void Start() {
            //保存transform的引用，加快速度
            myTransform = this.transform;
            //获取当前的BattleController
            battleController = BattleController.getInstance();
            //初始化地图的参数（用来计算人物的显示位置）
            orign = battleController.orign;
            battleMapLen = battleController.battleMapLen;

            //初始化玩家和敌人的对象
            if (isPlayer) {
                person = battleController.person;
                enemy = battleController.enemy;
            } else {
                person = battleController.enemy;
                enemy = battleController.person;
            }

            //将当前位置赋予角色上
            changeRealPos();

            //获取当前的Animator
            this.animator = this.GetComponentInChildren<Animator>();

            //初始化当前生命值是满的（更新显示条）
            addHealthPoint(myId, maxHealthPoint);
            //初始化当前行动值为0（更新显示条）
            addActionPoint(myId, 0.0f);

            //初始化当前角色规定的朝向正负（玩家为正，敌人为负）
            forwardDir = isPlayer ? 1 : -1;
            //当前角色往认为自己为正的朝向
            isForward = 1;
            //当前角色的前进方向为1
            curMotionDir = 1;

            //当前没有战斗目标，也没有选中的战斗目标
            atkTarget = -1;
            selectedAtkTarget = -1;
            //当前处于存活状态
            isAlive = true;

            //当前角色的状态为不动
            actionState = ActionStateEnum.NONE;

            //其它子类的继承方法
            otherInit();
        }

        //不同子类有不同的方法
        protected abstract void otherInit();

        // Update is called once per frame
        void Update() {
            //如果当前角色不存活（死亡或撤退，则不做操作）
            if (!this.isAlive) {
                return;
            }

            //如果处于释放技能状态或死亡状态，不做状态切换，直接返回
            switch (actionState) {
                //释放技能状态
                case ActionStateEnum.SKILL:
                    skillSubState();
                    return;

                //转向状态
                case ActionStateEnum.CHANGE_DIR:
                    playChangeMotion();
                    return;

                //死亡状态
                case ActionStateEnum.DEAD:
                    return;
            }

            //每个角色不同的AI
            AIStrategy();

            //判断需不需要转向，需要的话开始转向
            checkAndChangeDir();

            //开始执行角色行动的策略
            switch (actionState) {
                //攻击子状态
                case ActionStateEnum.ATTACK:
                    attackSubState();
                    break;

                //移动子状态
                case ActionStateEnum.MOTION:
                    motionSubState();
                    break;

                //休息子状态
                case ActionStateEnum.REST:
                    restSubState();
                    break;
            }          
        }

        //每个角色不同的AI
        protected abstract void AIStrategy();

        //切换目标
        public void changeSelectTarget(int num) {
            Debug.Log("切换所选的目标" + num);
            selectedAtkTarget = num - 1;
        }

        /// <summary>
        /// 选中最近的敌人
        /// 如果玩家已经选中了敌人，则不变
        /// 否则找最近的敌人
        /// </summary>
        protected void selectNearestEnemy() {
            //如果当前已经存在玩家选中的目标了，朝着目标行动
            if (selectedAtkTarget != -1) {
                atkTarget = selectedAtkTarget;
            } else {
                //获取距离最近的目标
                atkTarget = getNearestEnemy();
            }
        }

        /// <summary>
        /// 查找最近的目标
        /// 距离相等时则找序号最小的
        /// </summary>
        /// <returns>返回目标的id</returns>
        private int getNearestEnemy() {
            int nearestId = -1;

            //寻找距离最近相应目标(距离最近，序号最前)
            for (int i = 0; i < enemy.Length; i++) {
                //如果当前敌人存活
                if (enemy[i].isAlive) {
                    if (nearestId == -1 || Mathf.Abs(enemy[i].pos - pos) > Mathf.Abs(enemy[nearestId].pos - pos)) {
                        nearestId = i;
                    }
                }
            }

            return nearestId;
        }

        /// <summary>
        /// 进行攻击子状态
        /// 开始攻击动画
        /// 攻击动画结束后，对目标进行攻击
        /// </summary>
        /// <returns></returns>
        private void attackSubState() {
            //如果当前目标已经死亡
            if (enemy[atkTarget].isAlive == false) {
                //停止本次攻击，返回
                stopThisAttack();

                return;
            }

            //如果此时是首次进入攻击状态，播放攻击动画
            if (curPassTime == 0.0f) {
                //播放攻击动画
                playAttackAnimation();
            }

            //增加攻击等待时间
            curPassTime += Time.deltaTime;

            //如果当前时间小于攻击等待时间
            if (curPassTime < atkNeedTime) {
                //继续等待，不做操作
                return;
            }

            //先判断本次攻击是否命中
            if (isHit(myId, this.hitRate) == true) {
                //如果此次攻击命中
                //对敌人进行攻击操作
                enemy[atkTarget].getDamage(myId, atkDamage);

                //每次攻击后增加行动值，保证不大于最大值
                addActionPoint(myId, apRecovery);
            } else {
                //如果此次攻击被敌人闪避
            }

            //本次攻击完，停止本次攻击
            stopThisAttack();
        }

        //播放攻击动画
        private void playAttackAnimation() {
            Debug.Log("开始攻击！");
            //播放攻击动画
            animator.SetTrigger("attack");
            //获取攻击的动画播放速度
            //Animation animation = animator.animation.;
            //按照攻击速度更改攻击的播放速度
            //animator.speed = 1.0f * atkNeedTime / animationTime;
        }

        /// <summary>
        /// 停止本次攻击
        /// </summary>
        private void stopThisAttack() {
            //停止攻击动画？
            animator.SetTrigger("run");

            //等待时间置空
            curPassTime = 0.0f;

            //如果当前角色还存活，更改其状态
            if (this.isAlive == true) {
                actionState = ActionStateEnum.NONE;
            }

            //本次攻击完后，更换攻击目标
            atkTarget = -1;
        }

        //播放移动动画
        private void playMoveAnimation() {
            animator.SetTrigger("run");
            //获取跑步的动画播放速度
            //float length = animator.GetCurrentAnimatorStateInfo(0).length;
            //按照移速更改跑步的播放速度
            animator.speed = 1.0f * moveSpeed / 1.0f;
        }

        //播放转身动画（平滑旋转）
        float changeDirAnimationTime = 0.3f;
        private void playChangeMotion() {
            float rotateT =  180 / changeDirAnimationTime / Quaternion.Angle(myTransform.rotation, endRotation) * Time.deltaTime;

            if (rotateT >= 1.0f) {
                actionState = ActionStateEnum.MOTION;
                myTransform.rotation = endRotation;
            } else {
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, endRotation, rotateT);
            }
        }

        /// <summary>
        /// 进行释放技能子状态
        /// </summary>
        /// <returns></returns>
        private void skillSubState() {
            //如果此时是首次进入释放技能状态，播放技能动画
            if (curPassTime == 0.0f) {

            }

            //增加释放技能等待时间
            curPassTime += Time.deltaTime;

            //如果当前时间小于释放技能等待时间
            if (curPassTime < skillNeedTime) {
                //继续等待，不做操作
                return;
            }

            //释放此次技能（效果）

            //结束此次技能的释放

            //等待时间置空
            curPassTime = 0.0f;

            //如果当前角色还存活，更改其状态
            if (this.isAlive == true) {
                actionState = ActionStateEnum.NONE;
            }
        }

        /// <summary>
        /// 进行移动子状态
        /// </summary>
        /// <returns></returns>
        private void motionSubState() {         
            //播放移动动画
            playMoveAnimation();

            //对逻辑位置开始移动
            pos += getMotionDir() * moveSpeed * Time.deltaTime;
            //将坐标限制为0到最大的mapLen中
            pos = Mathf.Clamp(pos, 0, battleMapLen);

            //将当前位置赋予角色上
            changeRealPos();
        }

        //更改显示物体的pos
        private void changeRealPos() {
            Vector3 curPos = orign;
            orign.x = pos;
            myTransform.position = curPos;
        }

        /// <summary>
        /// 进行休息子状态
        /// </summary>
        /// <returns></returns>
        private void restSubState() {
            //增加已休息的时间
            curPassTime += Time.deltaTime;

            //如果当前时间小于休息等待时间
            if (curPassTime < restNeedTime) {
                //继续等待，不做操作
                return;
            }

            //回复一次生命值
            addHealthPoint(myId, hpRecovery * 0.5f);

            //当前时间清空
            curPassTime = 0.0f;
        }

        //获得当前角色移动的方向，正或负
        //保证必须是1或-1
        int getMotionDir() {
            return isForward * forwardDir;
        }

        /// <summary>
        ///判断角色是否需要转向
        ///如果需要，角色转向，目前只改变逻辑方向
        /// </summary>
        void checkAndChangeDir() {
            //如果其中一个为0，出现错误
            if (curMotionDir == 0 || getMotionDir() == 0) {
                Debug.Log("curMotionDir为" + curMotionDir + "  getMotionDir()" + getMotionDir());
                Debug.Break();
            }

            //如果当前朝向一致，不做转向
            if (curMotionDir * getMotionDir() > 0) {
                return;
            }

            //否则需要转向，开始转向
            isForward *= -1;

            //从0转到180或者从180转到0
            float start = myTransform.rotation.y;
            float end = (start == 0.0f ? 180.0f : 0.0f);
            
            //转身的播放速度          
            endRotation = Quaternion.Euler(myTransform.rotation.eulerAngles + new Vector3(0, 180.0f, 0));
            //设置转身子状态
            actionState = ActionStateEnum.CHANGE_DIR;

            //播放转身动画
            //playChangeMotion();

            return;
        }

        /// <summary>
        /// 判断本次伤害是否被闪避
        /// </summary>
        /// <param name="id">造成伤害的id</param>
        /// <param name="hitRate">命中率</param>
        /// <returns>返回是否命中</returns>
        public bool isHit(int id, float hitRate) {
            //计算本次攻击命中率
            float curHitRage = hitRate - dodgeRate;

            //返回随机数[0.0f, 1.0f]是否在命中率范围之内，在则命中
            return Random.value <= curHitRage;
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="id">造成此次伤害的id</param>
        /// <param name="damagePoint">此次伤害的数值</param>
        /// <returns>返回造成本次伤害的数值</returns>
        public float getDamage(int id, float damagePoint) {
            //真实受到的伤害先乘上受伤比例
            float realDamage = damagePoint * damageRate;

            //如果当前处于休息状态，受到的伤害加倍
            realDamage *= 2.0f;

            //受到伤害后，撤退时间为0
            curRetreatWaitTime = 0.0f;

            //如果当前生命值高于本次伤害，减去本次伤害
            if (curHealthPoint > realDamage) {
                addHealthPoint(id, -realDamage);
            }
            //否则生命值变为0，进入死亡播放状态
            else {
                addHealthPoint(id, -curHealthPoint);
                startDead();
            }

            return realDamage;
        }

        /// <summary>
        /// 增加生命值
        /// </summary>
        /// <param name="id">造成此次生命值增加的id</param>
        /// <param name="addHp">此次增加的数值</param>
        public void addHealthPoint(int id, float addHp) {
            curHealthPoint = Mathf.Clamp(curHealthPoint + addHp, 0.0f, maxHealthPoint);

            //更新hp的显示
            hpSlider.value = curHealthPoint / maxHealthPoint;
        }

        /// <summary>
        /// 增加行动力
        /// </summary>
        /// <param name="id">造成此次行动力增加的id</param>
        /// <param name="addAp">此次增加的数值</param>
        public void addActionPoint(int id, float addAp) {
            curActionPoint = Mathf.Clamp(curActionPoint + addAp, 0.0f, maxActionPoint);

            //更新ap的显示
            apSlider.value = curActionPoint / maxActionPoint;
        }

        /// <summary>
        /// 开始进入休息状态
        /// </summary>
        protected void startRestState() {
            //如果当前已处于休息状态，返回
            if (actionState == ActionStateEnum.REST) {
                return;
            }

            //时间清空，开始记录休息时间
            curPassTime = 0.0f;
            //设置休息子状态
            actionState = ActionStateEnum.REST;

            //播放休息动画

        }

        /// <summary>
        /// 开始进入攻击状态
        /// </summary>
        protected void startAttackState() {
            //时间清空，开始记录攻击等待时间
            curPassTime = 0.0f;
            //设置攻击子状态
            actionState = ActionStateEnum.ATTACK;
        }

        /// <summary>
        /// 开始死亡
        /// </summary>
        private void startDead() {
            //播放死亡动画

            //设置死亡状态
            actionState = ActionStateEnum.DEAD;

            //该角色本场不行动
            isAlive = false;

            //每次有角色死亡调用battleConroller判断本场战斗是否结束
            battleController.isBattleEnd(isPlayer);
        }

        /// <summary>
        /// 播放获胜动画
        /// </summary>
        public void startWin() {
            //播放胜利动画？

            //设置胜利状态
            actionState = ActionStateEnum.NONE;

            //该角色本场不行动
            isAlive = false;
        }
    }
}

