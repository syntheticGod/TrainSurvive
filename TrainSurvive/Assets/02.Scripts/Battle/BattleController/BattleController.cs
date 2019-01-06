/*
 * 描述：这是控制Battle的类
 * 作者：王安鑫
 * 创建时间：2018/11/21 19:00:20
 * 版本：v0.1
 */
using Assets._02.Scripts.zhxUIScripts;
using System.Collections;
using System.Collections.Generic;
using TTT.Item;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        public int enemyNum = 1;

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
        public List<GameObject> playerPanels;
        //掉落战利品的gameObject
        public GameObject dropSpoilsPanelPrefab;
        private GameObject dropSpoilsPanel;

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
        public bool isTest = false;
        //敌人数值相应的削弱
        public float enemyPara = 0.2f;

        //当前战斗场景结束时释放资源
        public void clear() {
            battleController = null;
        }

        //暂时只为测试用
        private void Awake() {
            BuffFactory.initBuffSystem();
            battleController = this;
        }

        // Use this for initialization
        void Start() {
            //此处限定60帧（强制提高帧率）（测试）
            Application.targetFrameRate = 60;

            //保存玩家的父级
            playersRoot = new GameObject("players");
            //保存敌人的父级
            enemysRoot = new GameObject("enemys");

            //初始化敌人
            InitEnemys.initEnemys();

            //初始化玩家（必须先初始化敌人）
            InitPlayers.initPlayers();

            //初始化小队操作的指令
            PanelBind.initTeamPanel();
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
                foreach (BattleActor player in playerActors) {
                    if (player.isAlive == true) {
                        isBattleEnd = false;
                        break;
                    }
                }
            } else {
                //如果是敌人判断敌人那方是否全部阵亡或逃跑
                foreach (BattleActor enemy in enemyActors) {
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

                //开启协程过1秒后转入到战利品界面
                StartCoroutine(displayDropSpoils());
                
            } else {
                //如果是敌人胜利，设置敌人胜利
                foreach (BattleActor enemy in enemyActors) {
                    enemy.startWin();
                }
                //显示失败的文本
                defeatText.SetActive(true);

                //开启协程过3秒后跳转场景
                StartCoroutine(changeMapScene());
            }
        }

        //过1秒后转入到战利品界面
        IEnumerator displayDropSpoils() {
            yield return new WaitForSeconds(1.0f);

            //启动战利品panel
            dropSpoilsPanel = Instantiate(dropSpoilsPanelPrefab);
            //设置父框
            dropSpoilsPanel.transform.parent = curCanvas.transform;
            //设置战利品框为居中显示
            HelpGameObjectAnchor.setCenter(dropSpoilsPanel);

            //获取掉落的战利品
            for (int i = 0; i < 12; i++) {
                //获取一个随机的材料
                ItemData assets = ItemData.RandomMaterial();

                DropSpoils.setItem(dropSpoilsPanel.transform, assets, i);
                
                 //给队伍背包加随机材料
                 World.getInstance().storage.AddItem(assets);
            }

            //绑定button事件为跳转到map
            Button button = dropSpoilsPanel.transform.Find("ConfirmBtn").GetComponent<Button>();
            button.onClick.AddListener(() => { changeToMapScene(); });
        }

        //开启协程过3秒后跳转场景
        IEnumerator changeMapScene() {
            yield return new WaitForSeconds(3.0f);
            //改变到大地图场景
            changeToMapScene();
        }

        /// <summary>
        /// 改变到大地图场景
        /// </summary>
        private void changeToMapScene() {
            //返回正常帧率
            Application.targetFrameRate = -1;

            TimeController.getInstance()?.changeScene(false);
            SceneManager.LoadScene("MapScene");
        }

    }
}

