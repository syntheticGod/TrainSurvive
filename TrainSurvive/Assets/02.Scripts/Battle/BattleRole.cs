///*
// * 描述：这是一个战斗角色类
// * 负责角色的属性
// * 角色的属性显示
// * 以及暴露对角色属性的修改
// * 
// * 作者：王安鑫
// * 创建时间：2018/12/2 9:20:23
// * 版本：v0.1
// */
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace WorldBattle {
//    public class BattleRole : MonoBehaviour {

//        //当前角色最大的生命值（假设出场是满的）
//        public float maxHealthPoint;
//        //当前角色最大的行动值
//        public float maxActionPoint;
//        //HP恢复值（休息的时候可恢复）
//        public float hpRecovery;
//        //AP恢复值（行动值恢复值）
//        public float apRecovery;
//        //移动速度（每1s移动的距离）
//        public float moveSpeed;
//        //攻击力
//        public float atkDamage;
//        //攻击范围
//        public float atkRange;
//        //当前所处的逻辑位置
//        public float pos;
//        //当前角色的受伤比例（类似于防御值）
//        public float damageRate;
//        //当前角色的命中率
//        public float hitRate;
//        //当前角色的闪避率
//        public float dodgeRate;
//        //当前角色的暴击率
//        public float critRate;
//        //当前角色的暴击伤害比例
//        public float critDamage;

//        //攻击所需的时间(受攻速影响)
//        public float atkNeedTime;

//        //本次战斗中本角色的id
//        public int myId;
//        //本次战斗中是否是玩家角色
//        public bool isPlayer;

//        //生命值Slider
//        public Slider hpSlider;
//        //行动力Slider
//        public Slider apSlider;
//        //撤退按钮
//        public Button retreatBtn;

//        //释放技能所需等待的时间(设立统一的固定CD)
//        private float skillNeedTime = 0.5f;
//        //休息所需等待的时间
//        private float restNeedTime = 0.5f;
//        //撤退所需的等待时间
//        protected float retreatNeedTime = 5.0f;

//        //当前攻击\释放技能\休息已等待的时间
//        private float curPassTime = 0.0f;
//        //当前撤退的等待时间
//        protected float curRetreatWaitTime = 0.0f;

//        //生命值
//        private float curHealthPoint;
//        //行动值
//        private float curActionPoint;

//        //当前战斗地图的起始位置
//        private Vector3 orign;
//        //战斗地图的卷轴长度（假设战斗地图以x,z为平面）
//        private int battleMapLen = 40;

//        //角色当前的攻击目标
//        public int atkTarget;
//        //玩家当前选中的攻击目标
//        private int selectedAtkTarget;

//        //当前角色的朝向，是否处于前进状态
//        private int isForward = 1;
//        //角色的走向（玩家正为往前走，敌人负为往前走）
//        protected int forwardDir;

//        //记录本次移动的方向
//        protected int curMotionDir = 0;

//        //当前角色是否存活(死亡或撤退都属于不存活)
//        public bool isAlive { get; protected set; }

//        //设置控制射程的范围参数
//        public float controlRangePara = 0.8f;

//        //获取当前的BattleController
//        protected BattleController battleController;


//        // Use this for initialization
//        void Start() {
//            //获取当前的BattleController
//            battleController = BattleController.getInstance();
//            //初始化地图的参数（用来计算人物的显示位置）
//            orign = battleController.orign;
//            battleMapLen = battleController.battleMapLen;
//        }

//        // Update is called once per frame
//        void Update() {

//        }

//        /// <summary>
//        /// 判断本次伤害是否被闪避
//        /// </summary>
//        /// <param name="id">造成伤害的id</param>
//        /// <param name="hitRate">命中率</param>
//        /// <returns>返回是否命中</returns>
//        public bool isHit(int id, float hitRate) {
//            //计算本次攻击命中率
//            float curHitRage = hitRate - dodgeRate;

//            //返回随机数[0.0f, 1.0f]是否在命中率范围之内，在则命中
//            return Random.value <= curHitRage;
//        }

//        /// <summary>
//        /// 受到伤害
//        /// </summary>
//        /// <param name="id">造成此次伤害的id</param>
//        /// <param name="damagePoint">此次伤害的数值</param>
//        /// <returns>返回造成本次伤害的数值</returns>
//        public float getDamage(int id, float damagePoint) {
//            //真实受到的伤害先乘上受伤比例
//            float realDamage = damagePoint * damageRate;

//            //如果当前处于休息状态，受到的伤害加倍
//            realDamage *= 2.0f;

//            //受到伤害后，撤退时间为0
//            curRetreatWaitTime = 0.0f;

//            //如果当前生命值高于本次伤害，减去本次伤害
//            if (curHealthPoint > realDamage) {
//                addHealthPoint(id, -realDamage);
//            }
//            //否则生命值变为0，进入死亡播放状态
//            else {
//                addHealthPoint(id, -curHealthPoint);
//                startDead();
//            }

//            return realDamage;
//        }

//        /// <summary>
//        /// 增加生命值
//        /// </summary>
//        /// <param name="id">造成此次生命值增加的id</param>
//        /// <param name="addHp">此次增加的数值</param>
//        public void addHealthPoint(int id, float addHp) {
//            curHealthPoint = Mathf.Clamp(curHealthPoint + addHp, 0.0f, maxHealthPoint);

//            //更新hp的显示
//            hpSlider.value = curHealthPoint / maxHealthPoint;
//        }

//        /// <summary>
//        /// 增加行动力
//        /// </summary>
//        /// <param name="id">造成此次行动力增加的id</param>
//        /// <param name="addAp">此次增加的数值</param>
//        public void addActionPoint(int id, float addAp) {
//            curActionPoint = Mathf.Clamp(curActionPoint + addAp, 0.0f, maxActionPoint);

//            //更新ap的显示
//            apSlider.value = curActionPoint / maxActionPoint;
//        }
//    }
//}

