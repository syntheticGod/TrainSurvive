/*
 * 描述：初始化敌人
 * 作者：王安鑫
 * 创建时间：2018/12/14 19:52:41
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using TTT.Utility;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace WorldBattle {
    public class InitEnemys : MonoBehaviour {
        /// <summary>
        /// 用于判断是否是通过对话进入的战斗，注意在经过一次敌人生成完毕后要重新设置为false
        /// </summary>
        private static bool nextBattleIs_talkbattle = false;
        private static int nextBatttle_talk_id;
        /// <summary>
        /// 初始化敌人
        /// 创建GameObject
        /// 创建AI
        /// 绑定panel
        /// 设置战利品（每个怪物及特殊战斗的额外战利品）
        /// </summary>
        public static void initEnemys() {
            //注意对于对话进入战斗场景，可以直接切换至battlescene但是要采取不同的怪物初始化方法

            //获取battleController
            BattleController battleController = BattleController.getInstance();

            //如果是测试
            if (battleController.isTest == true) {

                battleController.enemyActors = new List<BattleActor>();
                battleController.enemyPanels = new List<GameObject>();
                for (int i = 0; i < battleController.enemyNum; i++) {
                    //生成敌人
                    GameObject curPlayer = Instantiate(battleController.player,
                        battleController.orign,
                        Quaternion.identity);
                    curPlayer.transform.rotation = Quaternion.Euler(curPlayer.transform.eulerAngles + new Vector3(0, 180.0f, 0));

                    BattleActor battleActor;
                    //测试用自己的
                    battleActor = curPlayer.AddComponent<EnemyAI>();
                    battleActor.playerPrefab = curPlayer;
                    InitPlayers.initPersonPara(ref battleActor, 1.0f);

                    //初始化人物的位置
                    setBattleActorInfo(ref battleActor, i);
                }
            } else if (!nextBattleIs_talkbattle)//不是对话进入的战斗，而是通过进入地图地块触发的战斗
              {

                //获取探险队所在地块的怪物难度系数或者特殊战斗id
                Vector2Int teamPosition = World.getInstance().PMarker.TeamMapPos;
                int monsterLevel = WorldMap.Map.GetInstance().GetMonsterLevel(teamPosition); ;
                switch (WorldMap.Map.GetInstance().spowns[teamPosition.x, teamPosition.y].specialTerrainType) {
                    case WorldMap.SpawnPoint.SpecialTerrainEnum.MONSTER:
                        //根据玩家出战人数n，随机产生敌人数量
                        {
                        int countOfFighter = World.getInstance().Persons.CountOfFighter();
                        int randomInt = MathTool.RandomInt(100);
                        if (randomInt < 20) {//20% 人数+1
                            battleController.enemyNum = countOfFighter + 1;
                        } else if (randomInt < 70) {//50% 人数
                            battleController.enemyNum = countOfFighter;
                        } else {//30% 人数-1，至少为1
                            if (countOfFighter != 1)
                                battleController.enemyNum = countOfFighter - 1;
                            else
                                battleController.enemyNum = 1;
                        }
                    }

                    if (monsterLevel <= 0)
                        Debug.LogError("当前区块不是怪物区块 坐标：(" + teamPosition.x + "," + teamPosition.y + ")");
                    Debug.Log("进入难度级别为" + monsterLevel + "的地块。");
                    //初始化敌人
                    battleController.enemyActors = new List<BattleActor>();
                    battleController.enemyPanels = new List<GameObject>();
                    for (int i = 0; i < battleController.enemyNum; i++) {
                        //生成敌人
                        GameObject curPlayer = Instantiate(battleController.player,
                            battleController.orign,
                            Quaternion.identity);
                        curPlayer.transform.rotation = Quaternion.Euler(curPlayer.transform.eulerAngles + new Vector3(0, 180.0f, 0));

                        BattleActor battleActor;
                        //获取当前地块的怪物级别
                        MonsterInitializer mi = MonsterInitializer.getInstance();
                        //根据怪物的级别生成随机ID的怪物
                        mi.randomMonster(ref curPlayer, monsterLevel);
                        battleActor = mi.getBattleActor();

                        //初始化人物的位置
                        setBattleActorInfo(ref battleActor, i);
                    }
                    break;

                    case WorldMap.SpawnPoint.SpecialTerrainEnum.SPECIAL_AREA:
                        int specialBattle_id = monsterLevel;
                        //根据id获取特殊战斗信息
                        SpecialBattle battleInfo = SpecialBattleInitializer.getInstance().getBattle(specialBattle_id);
                        foreach (ValueTuple<int, int> monsterTurple in battleInfo.monsterList) {
                            int index = 0;
                            for (int i = 0; i < monsterTurple.Item2; i++) {
                                //生成敌人
                                GameObject curPlayer = Instantiate(battleController.player, battleController.orign, Quaternion.identity);
                                curPlayer.transform.rotation = Quaternion.Euler(curPlayer.transform.eulerAngles + new Vector3(0, 180.0f, 0));

                                BattleActor battleActor;
                                MonsterInitializer mi = MonsterInitializer.getInstance();
                                mi.initializeMonster(ref curPlayer, monsterTurple.Item1);
                                battleActor = mi.getBattleActor();
                                setBattleActorInfo(ref battleActor, index);
                                index++;
                            }
                        }
                        foreach (ValueTuple<int, int> rewardTurple in battleInfo.rewardList)//添加战利品，目前仅特殊战斗有指定战利品
                        {
                            battleController.dropsList.Add(rewardTurple);
                        }
                        break;
                }
            } else {//是对话进入的战斗
                //根据id获取特殊战斗信息
                SpecialBattle battleInfo = SpecialBattleInitializer.getInstance().getBattle(nextBatttle_talk_id);
                foreach (ValueTuple<int, int> monsterTurple in battleInfo.monsterList) {
                    int index = 0;
                    for (int i = 0; i < monsterTurple.Item2; i++) {
                        //生成敌人
                        GameObject curPlayer = Instantiate(battleController.player, battleController.orign, Quaternion.identity);
                        curPlayer.transform.rotation = Quaternion.Euler(curPlayer.transform.eulerAngles + new Vector3(0, 180.0f, 0));

                        BattleActor battleActor;
                        MonsterInitializer mi = MonsterInitializer.getInstance();
                        mi.initializeMonster(ref curPlayer, monsterTurple.Item1);
                        battleActor = mi.getBattleActor();
                        setBattleActorInfo(ref battleActor, index);
                        index++;
                    }
                }
                foreach (ValueTuple<int, int> rewardTurple in battleInfo.rewardList)//添加战利品，目前仅特殊战斗有指定战利品
                {
                    battleController.dropsList.Add(rewardTurple);
                }
                nextBattleIs_talkbattle = false;
            }

        }

        private static void setBattleActorInfo(ref BattleActor battleActor, int index) {
            //初始化人物的位置
            BattleController battleController = BattleController.getInstance();
            battleActor.pos = battleController.battleMapLen;
            //赋值人物id
            battleActor.myId = index;
            //初始化人物是否为玩家角色
            battleActor.isPlayer = false;
            //创建一个新的panel
            GameObject curPanel = Instantiate(battleController.enemyPanel);
            //将panel增加到panel列表中
            battleController.enemyPanels.Add(curPanel);
            //将当前的panel绑定到canvas中
            curPanel.transform.SetParent(battleController.curCanvas.transform);
            //设置敌人是右上角（从右向左）
            RectTransform rect = curPanel.GetComponent<RectTransform>();
            rect.pivot = Vector2.one;
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = new Vector3();
            //改变rect的位置
            curPanel.transform.position -=
                new Vector3(curPanel.GetComponent<RectTransform>().rect.width * index, 0, 0);
            //绑定战斗角色和操作表
            PanelBind.bindEnemyPanel(battleActor, curPanel.transform);
            //绑定玩家的姓名
            battleActor.nameText.text = "敌人" + (index + 1);
            //添加到玩家列表中
            battleController.enemyActors.Add(battleActor);
        }

        /// <summary>
        /// 只有在进入对话战斗场景才能调用
        /// </summary>
        /// <param name="battleId"></param>
        public static void setNextTalkBattle(int battleId) {
            nextBattleIs_talkbattle = true;
            nextBatttle_talk_id = battleId;
        }
    }
}

