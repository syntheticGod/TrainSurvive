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
    public abstract class BattleActor : MonoBehaviour {

        //当前角色最大的生命值（假设出场是满的）
        public float maxHealthPoint { get; set; }
        //当前角色最大的行动值
        public float maxActionPoint { get; set; }
        //HP恢复值（休息的时候可恢复）
        public float hpRecovery { get; set; }
        //AP恢复值（攻击时行动值恢复值）
        public float apRecovery { get; set; }
        //移动速度（每1s移动的距离）
        public float moveSpeed { get; set; }
        //攻击力
        public float atkDamage { get; set; }
        //攻击范围
        public float atkRange { get; set; }
        //当前所处的逻辑位置
        public float pos { get; set; }
        //当前角色的受伤比例（类似于防御值）
        public float damageRate { get; set; }
        //当前角色的命中率
        public float hitRate { get; set; }
        //当前角色的闪避率
        public float dodgeRate { get; set; }
        //当前角色的暴击率
        public float critRate { get; set; }
        //当前角色的暴击伤害比例
        public float critDamage { get; set; }

        //攻击所需的时间(受攻速影响)
        public float atkNeedTime { get; set; }

        //本次战斗中本角色的id
        public int myId { get; set; }
        //本次战斗中是否是玩家角色
        public bool isPlayer { get; set; }

        //生命值Slider
        public Slider hpSlider;
        //行动力Slider
        public Slider apSlider;
        //撤退按钮
        public Button retreatBtn;
        //获取绑定的gameObject(销毁用)
        public GameObject playerPrefab;
             
        //当前攻击间隔等待时间(攻击一次完后清零，其余时间一直计时)
        public float curAtkPassTime = 0.0f;
        //当前撤退的等待时间
        protected float curRetreatPassTime = 0.0f;

        //生命值
        private float curHealthPoint;
        //行动值
        private float curActionPoint;

        //当前战斗地图的起始位置
        private Vector3 orign;
        //战斗地图的卷轴长度（假设战斗地图以x,z为平面）
        public int battleMapLen = 40;

        //角色当前的攻击目标
        public int atkTarget;
        //玩家当前选中的攻击目标
        protected int selectedAtkTarget;

        //当前角色的朝向，是否处于前进状态
        private int isForward = 1;
        //角色的走向（玩家正为往前走，敌人负为往前走）
        protected int forwardDir;

        //记录本次移动的方向
        protected int curMotionDir = 0;

        //当前角色是否存活(死亡或撤退都属于不存活)
        public bool isAlive { get; set; }
        //当前角色行为是否停止（存活后有可能要播放动画）
        public bool isActorStop;

        //当前角色的状态
        public SubStateController subStateController;

        //当前战斗的玩家角色
        public List<BattleActor> playerActors;
        //当前战斗的敌人角色
        public List<BattleActor> enemyActors;
        //获取当前的BattleController
        public BattleController battleController;
        //获取当前的Animator
        private Animator animator;

        //记录actor的transform
        public Transform myTransform;

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
                playerActors = battleController.playerActors;
                enemyActors = battleController.enemyActors;
            } else {
                playerActors = battleController.enemyActors;
                enemyActors = battleController.playerActors;
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
            //当前角色未停止
            isActorStop = false;

            //初始化子状态控制器
            subStateController = new SubStateController(this, animator);

            //其它子类的继承方法
            otherInit();
        }

        //不同子类有不同的方法
        protected abstract void otherInit();

        // Update is called once per frame
        void Update() {
            //如果当前角色行动停止，返回
            if (isActorStop) {
                return;
            }

            //增加攻击等待时间（一直处于计时状态）
            curAtkPassTime += Time.deltaTime;

            //每个角色不同的AI
            AIStrategy();

            //判断需不需要转向，需要的话开始转向
            checkAndChangeDir();

            //开始执行角色行动的策略
            subStateController.executeCurState();
        }

        //每个角色不同的AI
        protected abstract void AIStrategy();  

        //更改显示物体的pos
        public void changeRealPos() {
            Vector3 curPos = orign;
            orign.x = pos;
            myTransform.position = curPos;
        }      

        //获得当前角色移动的方向，正或负
        //保证必须是1或-1
        public int getMotionDir() {
            return isForward * forwardDir;
        }

        /// <summary>
        /// 改变当前子状态
        /// </summary>
        public void changeSubState(ActionStateEnum actionStateEnum) {
            subStateController.changeSubState(actionStateEnum);
        }

        /// <summary>
        /// 改变到下一个子状态
        /// </summary>
        public void changeNextSubState() {
            subStateController.changeNextSubState();
        }

        /// <summary>
        ///判断角色是否需要转向
        ///如果需要，角色转向，目前只改变逻辑方向
        /// </summary>
        private void checkAndChangeDir() {
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

            //转换为改变转向子状态
            changeSubState(ActionStateEnum.CHANGE_DIR);

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
            curRetreatPassTime = 0.0f;

            //如果当前生命值高于本次伤害，减去本次伤害
            if (curHealthPoint > realDamage) {
                addHealthPoint(id, -realDamage);
            }
            //否则生命值变为0，进入死亡播放状态
            else {
                addHealthPoint(id, -curHealthPoint);
                changeSubState(ActionStateEnum.DEAD);
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
        /// 播放获胜动画
        /// </summary>
        public void startWin() {
            //播放胜利动画？

            //设置胜利状态
            changeSubState(ActionStateEnum.WIN);

            //该角色本场不行动
            isAlive = false;
        }

        /// <summary>
        /// 选中最近的敌人
        /// 如果玩家已经选中了敌人，则不变
        /// 否则找最近的敌人
        /// </summary>
        protected void selectNearestEnemy() {
            //如果当前已经存在玩家选中的目标了，朝着目标行动
            if (selectedAtkTarget != -1 && enemyActors[selectedAtkTarget].isAlive == true) {
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
            foreach (BattleActor enemyActor in enemyActors) {
                //如果当前敌人存活
                if (enemyActor.isAlive) {
                    if (nearestId == -1 || Mathf.Abs(enemyActor.pos - pos) > Mathf.Abs(enemyActors[nearestId].pos - pos)) {
                        nearestId = enemyActor.myId;
                    }
                }
            }

            return nearestId;
        }
    }
}

