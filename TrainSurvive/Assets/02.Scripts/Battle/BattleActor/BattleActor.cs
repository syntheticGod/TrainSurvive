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

        //角色五大基本属性（跟技能相关）
        //体力
        public int vitality;
        //力量
        public int strength;
        //敏捷
        public int agility;
        //技巧
        public int technical;
        //智力
        public int intelligence;

        //当前角色最大的生命值（假设出场是满的）
        public float maxHealthPoint { get; set; }
        //当前角色最大的行动值
        public float maxActionPoint { get; set; }
        //HP恢复值（休息的时候可恢复）
        public float hpRecovery { get; set; }
        //AP恢复值（攻击时行动值恢复值）
        public float apRecovery { get; set; }
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
        //当前角色的技能系数加成
        public float skillPara { get; set; }
        //当前角色击退距离系数(默认0.5)
        public float repelDistance { get; set; }

        //击败获得经验
        public float exp;
        public float size;
        public string name_str;
        public int rank;
        public int model;//使用模型
        //移动速度（每1s移动的距离）
        public float moveSpeed {
            get {
                return MoveSpeed;
            }
            set {
                //设置移速
                MoveSpeed = value;

                //如果当前处于播放移动动画时，设置动画播放速度
                if (subStateController != null
                    && subStateController.curActionState == ActionStateEnum.MOTION) {
                    //按移动速度设置动画速度
                    setMoveSpeedAnimate();
                }
            }
        }
        //这是实际记录的移动速度的值
        private float MoveSpeed;
        //这是移速改变比率
        public float moveSpeedChangeRate {
            get {
                return MoveSpeedChangeRate;
            }
            set {
                //设置移速
                MoveSpeedChangeRate = value;

                //如果当前处于播放移动动画时，设置动画播放速度
                if (subStateController != null
                    && subStateController.curActionState == ActionStateEnum.MOTION) {
                    //按移动速度设置动画速度
                    setMoveSpeedAnimate();
                }
            }
        }
        //这是实际记录的移速改变比率的值
        private float MoveSpeedChangeRate;

        //攻击所需的时间(受攻速影响)
        public float atkNeedTime {
            get {
                return AtkNeedTime;
            }
            set {
                //改攻速后同时更改攻击前摇时间
                AtkNeedTime = value;
                atkWindUpTime = 0.4f * atkNeedTime;
                //如果当前处于播放攻击动画时，设置动画播放速度
                if (subStateController != null
                    && subStateController.curActionState == ActionStateEnum.ATTACK) {
                    //按攻击速度设置动画速度
                    setAtkSpeedAnimate();
                }
            }
        }
        //这是实际记录的攻速值
        private float AtkNeedTime;
        //攻击所需的前摇的时间
        public float atkWindUpTime { get; private set; }

        public delegate void task_killMonster(int monsterId);
        public task_killMonster task_kill_handler;
        //任务用怪物id
        public int task_monsterId;

        //本次战斗中本角色的id
        public int myId { get; set; }
        //本次战斗中是否是玩家角色
        public bool isPlayer { get; set; }
        //判断是不是召唤物，召唤物不更新Hp，Ap
        public bool isSummon { get; set; }

        //生命值Slider
        public Slider hpSlider;
        //Hp值显示
        public Text hpTxt;
        //行动力Slider
        public Slider apSlider;
        //Ap值显示
        public Text apTxt;
        //撤退按钮
        public Button retreatBtn;
        //获取绑定的gameObject(销毁用)
        public GameObject playerPrefab;

        //当前攻击间隔等待时间(攻击一次完后清零，其余时间一直计时)
        public float curAtkPassTime = 0.0f;
        //当前撤退的等待时间
        protected float curRetreatPassTime = 0.0f;

        //生命值
        public float curHealthPoint { get; private set; }
        //行动值
        public float curActionPoint { get; private set; }

        //当前战斗地图的起始位置
        private Vector3 orign;
        //战斗地图的卷轴长度（假设战斗地图以x,z为平面）
        public int battleMapLen {get; private set; }

        //角色当前的攻击目标
        public int atkTarget;
        //玩家当前选中的攻击目标
        public int selectedAtkTarget;

        //当前角色的朝向，是否处于前进状态
        private int isForward = 1;
        //角色的走向（玩家正为往前走，敌人负为往后走）
        protected int forwardDir;

        //记录本次移动的方向（给下次pos增加值或减小值）
        protected int curMotionDir = 0;

        //当前角色是否存活(死亡或撤退都属于不存活)
        public bool isAlive { get; set; }
        //当前角色行为是否停止（存活后有可能要播放动画）
        public bool isActorStop { get; set; }

        //当前角色的状态
        public SubStateController subStateController { get; set; }

        //当前战斗的玩家角色
        public List<BattleActor> playerActors;
        //当前战斗的敌人角色
        public List<BattleActor> enemyActors;
        //获取当前的BattleController
        public BattleController battleController;
        //获取当前的Animator
        public Animator animator;
        //获取当前的Text
        public Text nameText;

        //记录actor的transform
        public Transform myTransform;

        //当前角色身上的buff状态
        public List<Buff> buffEffects;

        //当前眩晕buff的个数（只有个数为0时才解除眩晕）
        public int actorStopNum;

        //当前角色身上的技能
        public List<BaseSkill> skillList { get; private set; }
        //当前释放的技能id
        public int curSkillId = -1;

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

            //死亡
            DEAD,

            //撤退
            RETREAT,

            //胜利
            WIN,

            NUM
        }

        public BattleActor() {
            skillList = new List<BaseSkill>();

            //初始化移速改变比率为1.0
            moveSpeedChangeRate = 1.0f;

            //当前没有战斗目标，也没有选中的战斗目标
            atkTarget = -1;
            selectedAtkTarget = -1;
            //当前处于存活状态
            isAlive = true;
            //当前角色未停止
            isActorStop = false;
            //当前眩晕buff的个数为0
            actorStopNum = 0;
            //默认不是召唤物
            isSummon = false;
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

            //初始化玩家和敌人的对象(会重复初始化，但必须)
            if (isPlayer) {
                playerActors = battleController.playerActors;
                enemyActors = battleController.enemyActors;
            } else {
                playerActors = battleController.enemyActors;
                enemyActors = battleController.playerActors;
            }

            //初始化当前角色规定的朝向正负（玩家为正，敌人为负）
            forwardDir = isPlayer ? 1 : -1;
            //当前角色往认为自己为正的朝向
            isForward = 1;
            //当前角色的前进方向应是正向方向
            curMotionDir = forwardDir;

            //将当前位置赋予角色上
            changeRealPos(pos);

            //初始化当前生命值是满的（更新显示条）
            addHealthPoint(myId, maxHealthPoint);
            //初始化当前行动值为0（更新显示条）
            addActionPoint(myId, 0.0f);

            //绑定当前的Animator
            this.animator = this.GetComponentInChildren<Animator>();

            //初始化子状态控制器
            subStateController = new SubStateController(this, animator);
            //初始化buff状态列表
            buffEffects = new List<Buff>();

            //设置击退距离
            repelDistance = 0.2f;

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

        /// <summary>
        /// 每个角色不同的AI
        /// </summary>
        protected abstract void AIStrategy();

        /// <summary>
        /// 更改显示物体的pos
        /// </summary>
        public void changeRealPos(float newPos) {
            //更改当前pos位置（限制pos的范围为正确的范围）
            pos = Mathf.Clamp(newPos, 0, battleMapLen);

            //获取正常的位置
            Vector3 curPos = orign;
            //
            curPos.x = pos;
            myTransform.position = curPos;
        }

        /// <summary>
        ///获得当前角色移动的方向，正或负
        ///保证必须是1或-1
        /// </summary>
        /// <returns></returns>
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
            //Debug.Log(damageRate + " " + realDamage);

            //如果当前处于休息状态，受到的伤害加倍
            if (subStateController.curActionState == ActionStateEnum.REST) {
                realDamage *= 2.0f;
            }

            //受到伤害后，撤退时间为0
            curRetreatPassTime = 0.0f;

            //减去本次伤害
            addHealthPoint(id, -realDamage);

            return realDamage;
        }

        /// <summary>
        /// 增加生命值
        /// 如果生命值变成0，变成死亡状态
        /// </summary>
        /// <param name="id">造成此次生命值增加的id</param>
        /// <param name="addHp">此次增加的数值</param>
        public void addHealthPoint(int id, float addHp) {
            curHealthPoint = Mathf.Clamp(curHealthPoint + addHp, 0.0f, maxHealthPoint);

            //非召唤物更新面板
            if (isSummon == false) {
                //更新hp的显示
                hpSlider.value = curHealthPoint / maxHealthPoint;
                hpTxt.text = Mathf.Floor(curHealthPoint).ToString();
            }

            //如果当前生命值变成0，无论是技能，攻击，Buff，挂掉
            if (curHealthPoint <= 0.0f) {
                if (!isPlayer)
                    task_kill_handler?.Invoke(task_monsterId);
                changeSubState(ActionStateEnum.DEAD);
            }
        }

        /// <summary>
        /// 增加行动力
        /// </summary>
        /// <param name="id">造成此次行动力增加的id</param>
        /// <param name="addAp">此次增加的数值</param>
        public void addActionPoint(int id, float addAp) {
            curActionPoint = Mathf.Clamp(curActionPoint + addAp, 0.0f, maxActionPoint);

            //非召唤物更新面板
            if (isSummon == false) {
                //更新ap的显示
                apSlider.value = curActionPoint / maxActionPoint;
                apTxt.text = Mathf.Floor(curActionPoint).ToString();

                //人物角色可选择将技能按钮激活
                changeSkillBtn();
            }
        }

        /// <summary>
        /// 当ap改变时提供给人物角色面板
        /// 技能按钮是否激活
        /// </summary>
        protected virtual void changeSkillBtn() { }
      
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
        /// 设置buff效果
        /// </summary>
        /// <param name="buff">叠加buff的类型</param>
        /// <param name="floorNum">叠加的层数</param>
        public void setBuffEffect(Buff addBuff, int floorNum = 1) {
            //查找当前目标有没有对应的buff效果
            Buff findBuff =  buffEffects.Find(delegate(Buff buff) {
                return buff.buffType == addBuff.buffType;
            });

            //如果没有当前的buff效果
            if (findBuff == null) {
                //设置当前的buff效果，包括对应层数
                addBuff.setBuff(floorNum);
                //将其加入对应的buff中
                buffEffects.Add(addBuff);
            } else {
                //直接执行已有的buff
                findBuff.setBuff(floorNum);
            }
        }

        /// <summary>
        /// 设置移动速度动画
        /// </summary>
        public void setMoveSpeedAnimate() {
            //按移动速度设置动画速度
            animator.speed = moveSpeed * moveSpeedChangeRate;
            //Debug.Log(moveSpeedChangeRate);
        }

        /// <summary>
        /// 设置攻击动画的播放速度
        /// </summary>
        public void setAtkSpeedAnimate() {
            //按攻击速度设置动画速度
            animator.speed = 1.0f / atkNeedTime;
        }

        /// <summary>
        /// 每次攻击执行被动buff
        /// 目前假设被动buff都是攻击触发
        /// </summary>
        /// <param name="targetActor"></param>
        public void releaseAttackPassiveSkill(BattleActor targetActor) {
            //遍历技能列表
            foreach (BaseSkill skill in skillList) {
                //只有当前是攻击执行的被动技能再执行
                if (skill.skillType == BaseSkill.SkillType.ATTACK) {
                    //对目标执行被动技能
                    skill.release(targetActor);
                }
            }
        }

        /// <summary>
        /// 释放被动技能（开场）
        /// </summary>
        /// <param name="targetActor"></param>
        public void releasePassiveSkill() {
            //遍历技能列表
            foreach (BaseSkill skill in skillList) {
                //只有当前是攻击执行的被动技能再执行
                if (skill.skillType == BaseSkill.SkillType.PASSIVE) {
                    //对目标执行被动技能
                    skill.release();
                }
            }
        }

        /// <summary>
        /// 根据id执行指定的技能
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetActor"></param>
        public bool releaseSkill(int id, BattleActor targetActor = null) {
            //如果id不合法，返回错误
            if (id < 0 || id >= skillList.Count) {
                return false;
            }

            //如果当前不能释放技能，错误
            if (skillList[id].canReleaseSkill() == false) {
                Debug.Log("当前技能" + id + " ap不够！");
                return false;
            }

            //如果当前已经在技能释放阶段，不释放
            if (subStateController.curActionState == ActionStateEnum.SKILL) {
                return false;
            }

            //记录当前技能释放id
            curSkillId = id;
            //转入释放技能状态
            changeSubState(ActionStateEnum.SKILL);

            return true;
        }

        /// <summary>
        /// 根据id, 添加技能
        /// </summary>
        public void addSkill(int skillId) {
            //如果当前为空，将其初始化
            if (skillList == null) {
                skillList = new List<BaseSkill>();
            }

            //将指定技能id加入到技能列表中
            skillList.Add(SkillFactory.getSkill(skillId, this));
        }
    }
}

