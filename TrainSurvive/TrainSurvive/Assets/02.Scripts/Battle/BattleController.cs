/*
 * 描述：这是控制Battle的类
 * 作者：王安鑫
 * 创建时间：2018/11/21 19:00:20
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldBattle {
    public class BattleController : MonoBehaviour {

        //设置战斗控制类为单例模式
        private static BattleController battleController = null;
        public static BattleController getInstance() {
            if (battleController == null) {
                battleController = new BattleController();
            }
            return battleController;
        }

        private BattleController() { }

        //战斗地图的起始坐标
        public Vector3 orign;
        //战斗地图的卷轴长度（假设战斗地图以x,z为平面）
        public int battleMapLen = 40;

        //当前出战人数
        public int personNum = 5;
        //当前玩家角色剩余人数
        //public int personRemainNum = 5;
        //当前出战敌人人数
        public int enemyNum = 10;
        //当前出战剩余敌人人数
        //public int enemyRemainNum = 10;

        //设置人物所占大小
        public float personWidth = 0.5f;

        //当前战斗的玩家角色
        public BattleAI[] person;
        //当前战斗的敌人角色
        public BattleAI[] enemy;

        //给玩家一的panel（测试）
        public Transform[] panel;
        //玩家的gameObject
        public GameObject player;

        //保存玩家的父级
        public GameObject playersRoot;
        //保存敌人的父级
        public GameObject enemysRoot;

        //当前战斗场景结束时释放资源
        public void clear() {
            battleController = null;
        }

        //暂时只为测试用
        private void Awake() {
            battleController = this;
        }

        // Use this for initialization
        void Start() {
            //保存玩家的父级
            playersRoot = new GameObject("players");
            //保存敌人的父级
            enemysRoot = new GameObject("enemys");

            Debug.Log(player.GetComponent<Collider>().bounds.size);

            person = new BattleAI[personNum];
            enemy = new BattleAI[enemyNum];
            for (int i = 0; i < personNum; i++) {
                //生成玩家
                //GameObject curPlayer = Instantiate(player, playersRoot.transform);
                GameObject curPlayer = Instantiate(player,
                    orign + new Vector3(0, i * personWidth, 0),
                    Quaternion.identity);

                //绑定gameObject
                curPlayer.AddComponent<PersonAI>();

                //将生成的脚本和person数组绑定
                person[i] = curPlayer.GetComponent<PersonAI>();

                //初始化人物属性
                initBattlePerson(ref person[i], i);

                //绑定Panel
                bindPanel((PersonAI)person[i], panel[i]);
            }
            for (int i = 0; i < enemyNum; i++) {
                //生成敌人
                //GameObject curPlayer = Instantiate(player, enemysRoot.transform);
                GameObject curPlayer = Instantiate(player,
                    orign + new Vector3(0, battleMapLen - i * personWidth, 0),
                    Quaternion.identity);
                curPlayer.transform.rotation = Quaternion.Euler(curPlayer.transform.eulerAngles + new Vector3(0, 180.0f, 0));

                //绑定gameObject
                curPlayer.AddComponent<PersonAI>();

                //将生成的脚本和person数组绑定
                enemy[i] = curPlayer.GetComponent<PersonAI>();

                //初始化人物属性
                initBattleEnemy(ref enemy[i], i);

                //绑定Panel
                bindPanel((PersonAI)enemy[i], panel[1]);
            }
           
            //初始化剩余人数
            //personRemainNum = personNum;
            //enemyRemainNum = enemyNum;
        }

        //给玩家绑定hp和ap的显示，和玩家指令绑定
        void bindPanel(PersonAI person, Transform panel) {
            //绑定撤退按钮的监听事件
            person.retreatBtn = panel.Find("BackUpBtn").GetComponent<Button>();
            //绑定撤退监听事件
            person.retreatBtn.onClick.AddListener(() => person.startRetreat());
            //设置一开始撤退无效
            person.retreatBtn.interactable = false;

            //初始化下拉选取框
            Dropdown dropdown = panel.Find("SelectTarget").GetComponent<Dropdown>();
            //绑定选取目标的监听事件
            dropdown.onValueChanged.AddListener(x => person.changeSelectTarget(x));
            //初始化选取敌方目标
            for (int i = 0; i < enemyNum; i++) {
                Dropdown.OptionData op = new Dropdown.OptionData();
                op.text = "集火敌方目标" + (i + 1);
                dropdown.options.Add(op);
            }

            //绑定hpSlider
            person.hpSlider = panel.Find("HpSlider").GetComponent<Slider>();
            //绑定apSlider
            person.apSlider = panel.Find("ApSlider").GetComponent<Slider>();

            //增加攻击监听事件
            panel.Find("ToggleGroup").Find("AttackToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                person.changeAttackState(isOn);
            });

            //增加控制射程监听事件
            panel.Find("ToggleGroup").Find("RangeControlToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    person.changeControlRangeState(isOn);
                });

            //增加撤退监听事件
            panel.Find("ToggleGroup").Find("BackUpToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    person.changeBackUpState(isOn);
                });

            //增加休息监听事件
            panel.Find("ToggleGroup").Find("RestToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    person.changeRestState(isOn);
                });
        }

        //以下属性只做测试用
        private float maxHealthPoint = 200.0f;
        private float minHealthPoint = 150.0f;
        private float maxActionPoint = 100.0f;
        private float minActionPoint = 50.0f;
        private float maxAtkSpeed = 1.5f;
        private float minAtkSpeed = 0.5f;
        private float maxAtkDamage = 10f;
        private float minAtkDamage = 5f;
        private float minAtkRange = 1f;
        private float maxAtkRange = 5f;

        //初始化出战对应的人，并初始化对应出场的位置
        void initBattlePerson(ref BattleAI person, int i) {
            //初始化人物各属性值(随机)，只做测试使用
            person.maxHealthPoint = Random.Range(minHealthPoint, maxHealthPoint);
            person.maxActionPoint = Random.Range(minActionPoint, maxActionPoint);
            person.hpRecovery = 5;
            person.apRecovery = 5;
            person.atkNeedTime = 1 / Random.Range(minAtkSpeed, maxAtkSpeed);
            person.moveSpeed = 1;
            person.atkDamage = Random.Range(minAtkDamage, maxAtkDamage);
            person.atkRange = Random.Range(minAtkRange, maxAtkRange);
            person.damageRate = 1.0f;
            person.critDamage = 1.6f;
            person.critRate = 0.2f;
            person.hitRate = 1.0f;
            person.dodgeRate = 0.2f;

            //初始化人物的位置
            person.pos = i * personWidth;

            //初始化人物的id
            person.myId = i;

            //初始化人物是否为玩家角色
            person.isPlayer = true;
        }

        //敌人数值相应的削弱
        private float enemyPara = 0.8f;
        //初始化出战对应的敌人，并初始化对应出场的位置
        void initBattleEnemy(ref BattleAI person, int i) {
            //初始化人物各属性值(随机)，只做测试使用
            person.maxHealthPoint = Random.Range(minHealthPoint, maxHealthPoint) * enemyPara;
            person.maxActionPoint = Random.Range(minActionPoint, maxActionPoint) * enemyPara;
            person.hpRecovery = 5;
            person.apRecovery = 5;
            person.atkNeedTime = 1 / (Random.Range(minAtkSpeed, maxAtkSpeed) * enemyPara);
            person.moveSpeed = 1;
            person.atkDamage = Random.Range(minAtkDamage, maxAtkDamage) * enemyPara;
            person.atkRange = Random.Range(minAtkRange, maxAtkRange) * enemyPara;
            person.damageRate = 1.0f;
            person.critDamage = 1.6f;
            person.critRate = 0.2f;
            person.hitRate = 1.0f;
            person.dodgeRate = 0.2f;

            //初始化人物的位置
            person.pos = battleMapLen - i * personWidth;

            //初始化人物的id
            person.myId = i;

            //初始化人物是否为玩家角色
            person.isPlayer = false;
        }

        /// <summary>
        /// 判断本次战斗是否结束
        /// 每次有角色死亡时调用
        /// 通过是哪方调用判断本次战斗是否结束
        /// </summary>
        /// <param name="isPlayer">是否由玩家这方角色调用(玩家这方死人，失败)</param>
        /// <returns>返回本场战斗是否结束</returns>
        public bool isBattleEnd(bool isPlayer) {
            bool isBattleEnd = true;

            if (isPlayer) {
                //如果是玩家判断玩家那方是否全部阵亡或逃跑
                for (int i = 0; i < personNum; i++) {
                    if (person[i].isAlive == true) {
                        isBattleEnd = false;
                        break;
                    }
                }
            } else {
                //如果是敌人判断敌人那方是否全部阵亡或逃跑
                for (int i = 0; i < enemyNum; i++) {
                    if (enemy[i].isAlive == true) {
                        isBattleEnd = false;
                        break;
                    }
                }
            }

            //如果战斗结束
            if (isBattleEnd == true) {
                //播放战斗胜利或战斗失败的动画
                battleEnd(!isPlayer);
            }

            return isBattleEnd;
        }

        /// <summary>
        /// 战斗结束
        /// 播放玩家胜利或玩家战斗的动画
        /// </summary>
        /// <param name="isPlayer">是否为玩家胜利</param>
        private void battleEnd(bool isPlayer) {
            //播放胜利或失败？

            if (isPlayer) {
                //如果是玩家胜利，设置玩家胜利
                for (int i = 0; i < personNum; i++) {
                    person[i].startWin();
                }
            } else {
                //如果是敌人胜利，设置敌人胜利
                for (int i = 0; i < enemyNum; i++) {
                    enemy[i].startWin();
                }
            }
        }
    }
}

