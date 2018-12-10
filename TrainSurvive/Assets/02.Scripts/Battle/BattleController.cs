/*
 * 描述：这是控制Battle的类
 * 作者：王安鑫
 * 创建时间：2018/11/21 19:00:20
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WorldMap;
using WorldMap.Model;

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
        //当前出战敌人人数
        public int enemyNum = 10;

        //设置人物所占大小
        public float personWidth = 0.5f;

        //当前战斗的玩家角色
        public List<BattleActor> playerActors;
        //当前战斗的敌人角色
        public List<BattleActor> enemyActors;

        //当前场景的canvas
        public Canvas curCanvas;
        //给玩家一的panelPrefab
        public GameObject playerPanel;
        //给所有玩家角色的指令panel
        public GameObject teamPanel;
        //玩家的操作命令面板
        private List<GameObject> playerPanels;

        //敌人的显示面板Prefab
        public GameObject enemyPanel;
        //敌人的显示面板
        public List<GameObject> enemyPanels;

        //玩家的gameObject
        public GameObject player;

        //保存玩家的父级
        public GameObject playersRoot;
        //保存敌人的父级
        public GameObject enemysRoot;

        //获胜图标
        public GameObject winText;
        //失败图标
        public GameObject defeatText;

        //设置当前是否为测试状态
        public bool isTest = true;

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

            //初始化敌人
            enemyActors = new List<BattleActor>();
            enemyPanels = new List<GameObject>();
            
            for (int i = 0; i < enemyNum; i++) {
                //生成敌人
                GameObject curPlayer = Instantiate(player,
                    orign + new Vector3(0, battleMapLen, 0),
                    Quaternion.identity);
                curPlayer.transform.rotation = Quaternion.Euler(curPlayer.transform.eulerAngles + new Vector3(0, 180.0f, 0));

                //绑定gameObject
                curPlayer.AddComponent<EnemyAI>();

                //将生成的脚本和person数组绑定
                BattleActor battleActor = curPlayer.GetComponent<EnemyAI>();
                //绑定对应的gameObject
                battleActor.playerPrefab = curPlayer;

                //初始化人物属性
                initBattleEnemy(ref battleActor, i);

                //创建一个新的panel
                GameObject curPanel = Instantiate(enemyPanel);
                //将panel增加到panel列表中
                enemyPanels.Add(curPanel);
                //将当前的panel绑定到canvas中
                curPanel.transform.SetParent(curCanvas.transform);

                RectTransform rect = curPanel.GetComponent<RectTransform>();
                rect.pivot = Vector2.one;
                rect.anchorMin = Vector2.one;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = new Vector3();

                //改变rect的位置
                curPanel.transform.position -=
                    new Vector3(curPanel.GetComponent<RectTransform>().rect.width * i, 0, 0);
                //绑定战斗角色和操作表
                bindEnemyPanel((EnemyAI)battleActor, curPanel.transform);

                //添加到玩家列表中
                enemyActors.Add(battleActor);
            }

            //初始化相应列表
            playerActors = new List<BattleActor>();
            playerPanels = new List<GameObject>();

            //如果当前处于测试状态
            if (isTest) {
                for (int i = 0; i < personNum; i++) {
                    //生成玩家
                    GameObject curPlayer = Instantiate(player,
                        orign + new Vector3(0, 0, 0),
                        Quaternion.identity);

                    //绑定gameObject
                    curPlayer.AddComponent<PersonAI>();

                    //将生成的脚本和person数组绑定
                    BattleActor battleActor = curPlayer.GetComponent<PersonAI>();
                    //获取绑定的游戏对象
                    battleActor.playerPrefab = curPlayer;

                    //初始化人物属性
                    initBattlePerson(ref battleActor, i);

                    //创建一个新的panel
                    GameObject curPanel = Instantiate(playerPanel);
                    //将panel增加到panel列表中
                    playerPanels.Add(curPanel);
                    //将当前的panel绑定到canvas中
                    curPanel.transform.parent = curCanvas.transform;
                    //改变rect的位置
                    curPanel.transform.position +=
                        new Vector3(curPanel.GetComponent<RectTransform>().rect.width * i, 0, 0);
                    //绑定战斗角色和操作表
                    bindPlayerPanel((PersonAI)battleActor, playerPanels[i].transform);

                    //添加到玩家列表中
                    playerActors.Add(battleActor);
                }
            } else {
                initPlayerTeam();
            }

            //初始化小队操作的指令
            initTeamPanel();
        }

        /// <summary>
        /// 初始化玩家小队的操作指令
        /// </summary>
        private void initTeamPanel() {
            //创建一个新的panel
            GameObject curPanel = Instantiate(teamPanel);
            //将panel增加到panel列表中
            //playerPanels.Add(curPanel);
            //将当前的panel绑定到canvas中
            curPanel.transform.parent = curCanvas.transform;
            //改变rect的位置
            curPanel.transform.position +=
                new Vector3(curPanel.GetComponent<RectTransform>().rect.width * personNum, 0, 0);

            //绑定战斗角色和操作表
            Transform panel = curPanel.transform;
            //初始化下拉选取框
            Dropdown dropdown = panel.Find("SelectTarget").GetComponent<Dropdown>();
            //绑定小队选取目标的监听事件
            dropdown.onValueChanged.AddListener(x => {
                foreach (GameObject playerPanel in playerPanels) {
                    playerPanel.transform.Find("SelectTarget")
                    .GetComponent<Dropdown>().value = x;
                }
                //选定攻击目标后设置攻击状态
                panel.Find("ToggleGroup").Find("AttackToggle").GetComponent<Toggle>().isOn = true;
            });
            //初始化选取敌方目标
            for (int i = 0; i < enemyActors.Count; i++) {
                Dropdown.OptionData op = new Dropdown.OptionData();
                op.text = "集火敌方目标" + (i + 1);
                dropdown.options.Add(op);
            }

            //增加攻击监听事件
            panel.Find("ToggleGroup").Find("AttackToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach(GameObject playerPanel in playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("AttackToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });

            //增加控制射程监听事件
            panel.Find("ToggleGroup").Find("RangeControlToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach (GameObject playerPanel in playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("RangeControlToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });

            //增加后退监听事件
            panel.Find("ToggleGroup").Find("BackUpToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach (GameObject playerPanel in playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("BackUpToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });

            //增加休息监听事件
            panel.Find("ToggleGroup").Find("RestToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach (GameObject playerPanel in playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("RestToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });
        }

        //初始化玩家战斗队伍
        private void initPlayerTeam() {
            //初始化当前玩家角色人物为0
            personNum = 0;

            //遍历world中的人物列表，将小队人物参战
            foreach (Person person in World.getInstance().persons) {
                //如果当前人不处于外出状态，返回false
                if (person.ifOuting == false) {
                    continue;
                }

                //生成玩家
                GameObject curPlayer = Instantiate(player,
                    orign + new Vector3(0, 0, 0),
                    Quaternion.identity);

                //绑定gameObject
                curPlayer.AddComponent<PersonAI>();

                //将生成的脚本和person数组绑定
                BattleActor battleActor = curPlayer.GetComponent<PersonAI>();
                //获取绑定的游戏对象
                battleActor.playerPrefab = curPlayer;

                //初始化人物属性
                initTrueBattlePerson(ref battleActor, personNum, person);

                //创建一个新的panel
                GameObject curPanel = Instantiate(playerPanel);
                //将panel增加到panel列表中
                playerPanels.Add(curPanel);
                //将当前的panel绑定到canvas中
                curPanel.transform.parent = curCanvas.transform;
                //改变rect的位置
                curPanel.transform.position +=
                    new Vector3(curPanel.GetComponent<RectTransform>().rect.width * personNum, 0, 0);
                //绑定战斗角色和操作表
                bindPlayerPanel((PersonAI)battleActor, playerPanels[personNum].transform);

                //添加到玩家列表中
                playerActors.Add(battleActor);

                //增加人的数量
                personNum++;
            }
        }

        //给玩家绑定hp和ap的显示，和玩家指令绑定
        void bindPlayerPanel(PersonAI person, Transform panel) {
            //绑定撤退按钮的监听事件
            person.retreatBtn = panel.Find("BackUpBtn").GetComponent<Button>();
            //绑定撤退监听事件
            person.retreatBtn.onClick.AddListener(() => person.startRetreat());
            //设置一开始撤退无效
            person.retreatBtn.interactable = false;

            //绑定技能技能监听事件
            panel.Find("skill1").GetComponent<Button>()
                .onClick.AddListener(() => person.startSkillRelease(0));
            panel.Find("skill2").GetComponent<Button>()
                .onClick.AddListener(() => person.startSkillRelease(1));

            //初始化下拉选取框
            Dropdown dropdown = panel.Find("SelectTarget").GetComponent<Dropdown>();
            //绑定选取目标的监听事件
            dropdown.onValueChanged.AddListener(x => {
                person.changeSelectTarget(x);
                //选取目标后同时设置攻击状态
                panel.Find("ToggleGroup").Find("AttackToggle").GetComponent<Toggle>().isOn = true;
            });
            //初始化选取敌方目标
            for (int i = 0; i < enemyActors.Count; i++) {
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

        /// <summary>
        /// 绑定敌人的panel
        /// </summary>
        /// <param name="person"></param>
        /// <param name="panel"></param>
        void bindEnemyPanel(EnemyAI person, Transform panel) {
            //绑定hpSlider
            person.hpSlider = panel.Find("HpSlider").GetComponent<Slider>();
            //绑定apSlider
            person.apSlider = panel.Find("ApSlider").GetComponent<Slider>();
        }

        private void initTrueBattlePerson(ref BattleActor battleActor, int i, Person person) {
            //初始化人物各属性值(随机)，只做测试使用
            battleActor.maxHealthPoint = (float)person.getHpMax();
            battleActor.maxActionPoint = (float)person.getApMax();
            battleActor.hpRecovery = (float)person.getHpRec();
            battleActor.apRecovery = (float)person.getApRec();
            battleActor.atkNeedTime = 1 / (float)person.getValAts();
            battleActor.moveSpeed = (float)person.getValSpd();
            battleActor.atkDamage = (float)person.getValAtk();
            battleActor.atkRange = (float)person.getRange();
            battleActor.damageRate = 1.0f;
            battleActor.critDamage = (float)person.getValCrd();
            battleActor.critRate = (float)person.getValCrc();
            battleActor.hitRate = (float)person.getValHrate();
            battleActor.dodgeRate = (float)person.getValErate();

            //初始化人物的位置
            battleActor.pos = 0.0f;

            //初始化人物的id
            battleActor.myId = i;

            //初始化人物是否为玩家角色
            battleActor.isPlayer = true;
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
        void initBattlePerson(ref BattleActor person, int i) {
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
            person.dodgeRate = 0.0f;

            //初始化人物的位置
            person.pos = 0.0f;

            //初始化人物的id
            person.myId = i;

            //初始化人物是否为玩家角色
            person.isPlayer = true;
        }

        //敌人数值相应的削弱
        private float enemyPara = 0.2f;
        //初始化出战对应的敌人，并初始化对应出场的位置
        void initBattleEnemy(ref BattleActor person, int i) {
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
            person.dodgeRate = 0.0f;

            //初始化人物的位置
            person.pos = battleMapLen;

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
                foreach (PersonAI player in playerActors) {
                    if (player.isAlive == true) {
                        isBattleEnd = false;
                        break;
                    }
                }
            } else {
                //如果是敌人判断敌人那方是否全部阵亡或逃跑
                foreach (EnemyAI enemy in enemyActors) {
                    if (enemy.isAlive == true) {
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
                    playerActors[i].startWin();
                }
                //显示胜利的文本
                winText.SetActive(true);

                //给队伍背包加随机材料
                Team.Instance.Inventory.PushItem(Good.RandomMaterial().item);
            } else {
                //如果是敌人胜利，设置敌人胜利
                foreach (EnemyAI enemy in enemyActors) {
                    enemy.startWin();
                }
                //显示失败的文本
                defeatText.SetActive(true);
            }

            //开启协程过5秒后跳转场景
            StartCoroutine(changeMapScene());
        }

        IEnumerator changeMapScene() {
            yield return new WaitForSeconds(3.0f);
            TimeController.getInstance().changeScene(false);
            SceneManager.LoadScene("MapScene");
        }
    }
}

