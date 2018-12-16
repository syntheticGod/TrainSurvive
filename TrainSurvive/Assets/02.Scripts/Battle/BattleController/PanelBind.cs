/*
 * 描述：这是玩家操作面板绑定类
 * 作者：王安鑫
 * 创建时间：2018/12/14 19:16:44
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldBattle {
    public class PanelBind : MonoBehaviour {
        /// <summary>
        /// 给玩家绑定hp和ap的显示，和玩家指令绑定
        /// </summary>
        /// <param name="person">对应玩家脚本</param>
        /// <param name="panel">与其绑定的panel</param>
        public static void bindPlayerPanel(PersonAI person, Transform panel) {
            //获取敌人列表
            List<BattleActor> enemyActors = BattleController.getInstance().enemyActors;

            //绑定撤退按钮的监听事件
            person.retreatBtn = panel.Find("BackUpBtn").GetComponent<Button>();
            //绑定撤退监听事件
            person.retreatBtn.onClick.AddListener(() => person.startRetreat());
            //设置一开始撤退无效
            person.retreatBtn.interactable = false;

            //初始化人物技能
            person.skillBtn = new Button[2];
            //绑定技能按钮
            person.skillBtn[0] = panel.Find("skill1").GetComponent<Button>();
            person.skillBtn[1] = panel.Find("skill2").GetComponent<Button>();

            //绑定技能技能监听事件
            person.skillBtn[0].onClick.AddListener(() => person.startSkillRelease(0));
            person.skillBtn[1].onClick.AddListener(() => person.startSkillRelease(1));

            //设置技能名
            panel.Find("skill1").Find("skillName").GetComponent<Text>().text = "测试" + InitPlayers.skill1;
            panel.Find("skill2").Find("skillName").GetComponent<Text>().text = "测试" + InitPlayers.skill2;

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
            //绑定text
            person.nameText = panel.Find("name").GetComponent<Text>();
            //绑定hpTxt
            person.hpTxt = panel.Find("HpSlider").Find("HpValueTxt").GetComponent<Text>();
            //绑定apTxt
            person.apTxt = panel.Find("ApSlider").Find("ApValueTxt").GetComponent<Text>();

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
        public static void bindEnemyPanel(BattleActor person, Transform panel) {
            //绑定text
            person.nameText = panel.Find("name").GetComponent<Text>();
            //绑定hpSlider
            person.hpSlider = panel.Find("HpSlider").GetComponent<Slider>();
            //绑定apSlider
            person.apSlider = panel.Find("ApSlider").GetComponent<Slider>();
            //绑定hpTxt
            person.hpTxt = panel.Find("HpSlider").Find("HpValueTxt").GetComponent<Text>();
            //绑定apTxt
            person.apTxt = panel.Find("ApSlider").Find("ApValueTxt").GetComponent<Text>();
        }

        /// <summary>
        /// 初始化玩家小队的操作指令
        /// </summary>
        public static void initTeamPanel() {
            //获取battleController
            BattleController battleController = BattleController.getInstance();

            //创建一个新的panel
            GameObject curPanel = Instantiate(battleController.teamPanel);
            //将当前的panel绑定到canvas中
            curPanel.transform.parent = battleController.curCanvas.transform;
            //改变rect的位置
            curPanel.transform.position +=
                new Vector3(curPanel.GetComponent<RectTransform>().rect.width * battleController.personNum, 0, 0);

            //绑定战斗角色和操作表
            Transform panel = curPanel.transform;
            //初始化下拉选取框
            Dropdown dropdown = panel.Find("SelectTarget").GetComponent<Dropdown>();
            //绑定小队选取目标的监听事件
            dropdown.onValueChanged.AddListener(x => {
                foreach (GameObject playerPanel in battleController.playerPanels) {
                    playerPanel.transform.Find("SelectTarget")
                    .GetComponent<Dropdown>().value = x;
                }
                //选定攻击目标后设置攻击状态
                panel.Find("ToggleGroup").Find("AttackToggle").GetComponent<Toggle>().isOn = true;
            });
            //初始化选取敌方目标
            for (int i = 0; i < battleController.enemyActors.Count; i++) {
                Dropdown.OptionData op = new Dropdown.OptionData();
                op.text = "集火敌方目标" + (i + 1);
                dropdown.options.Add(op);
            }

            //增加攻击监听事件
            panel.Find("ToggleGroup").Find("AttackToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach (GameObject playerPanel in battleController.playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("AttackToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });

            //增加控制射程监听事件
            panel.Find("ToggleGroup").Find("RangeControlToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach (GameObject playerPanel in battleController.playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("RangeControlToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });

            //增加后退监听事件
            panel.Find("ToggleGroup").Find("BackUpToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach (GameObject playerPanel in battleController.playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("BackUpToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });

            //增加休息监听事件
            panel.Find("ToggleGroup").Find("RestToggle").GetComponent<Toggle>()
                .onValueChanged.AddListener((bool isOn) => {
                    foreach (GameObject playerPanel in battleController.playerPanels) {
                        playerPanel.transform.Find("ToggleGroup").Find("RestToggle")
                        .GetComponent<Toggle>().isOn = true;
                    }
                });
        }
    }
}

