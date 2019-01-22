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

namespace WorldBattle
{
    public class InitEnemys : MonoBehaviour
    {
        /// <summary>
        /// 初始化敌人
        /// 创建GameObject
        /// 创建AI
        /// 绑定panel
        /// </summary>
        public static void initEnemys()
        {
            //获取battleController
            BattleController battleController = BattleController.getInstance();

            //根据玩家出战人数n，随机产生敌人数量
            { 
                int countOfFighter = World.getInstance().CountOfFighter;
                int randomInt = MathTool.RandomInt(100);
                if (randomInt < 20)
                {//20% 人数+1
                    battleController.enemyNum = countOfFighter + 1;
                }
                else if (randomInt < 70)
                {//50% 人数
                    battleController.enemyNum = countOfFighter;
                }
                else
                {//30% 人数-1，至少为1
                    if (countOfFighter != 1)
                        battleController.enemyNum = countOfFighter - 1;
                    else
                        battleController.enemyNum = 1;
                }
            }
            //获取探险队所在地块的怪物难度系数
            int monsterLevel;
            {
                Vector2Int teamPosition = new Vector2Int(World.getInstance().posTeamX, World.getInstance().posTeamY);
                monsterLevel = WorldMap.Map.GetInstance().GetMonsterLevel(teamPosition);
                if (monsterLevel <= 0)
                    Debug.LogError("当前区块不是怪物区块 坐标：(" + teamPosition.x + "," + teamPosition.y + ")");
                Debug.Log("进入难度级别为" + monsterLevel + "的地块。");
            }
            //初始化敌人
            battleController.enemyActors = new List<BattleActor>();
            battleController.enemyPanels = new List<GameObject>();
            for (int i = 0; i < battleController.enemyNum; i++)
            {
                //生成敌人
                GameObject curPlayer = Instantiate(battleController.player,
                    battleController.orign,
                    Quaternion.identity);
                curPlayer.transform.rotation = Quaternion.Euler(curPlayer.transform.eulerAngles + new Vector3(0, 180.0f, 0));
                
                BattleActor battleActor;
                if (battleController.isTest == false)
                {
                    //获取当前地块的怪物级别
                    MonsterInitializer mi = new MonsterInitializer();
                    //根据怪物的级别生成随机ID的怪物
                    mi.randomMonster(ref curPlayer, monsterLevel);
                    battleActor = mi.getBattleActor();
                }
                else
                {
                    //测试用自己的
                    battleActor = curPlayer.AddComponent<EnemyAI>();
                    battleActor.playerPrefab = curPlayer;
                    InitPlayers.initPersonPara(ref battleActor, 1.0f);
                }

                /*
                //怪物AI目前有问题
                battleActor = curPlayer.AddComponent<EnemyAI>();
                battleActor.playerPrefab = curPlayer;
                InitPlayers.initPersonPara(ref battleActor, 1.0f);
                */
                //初始化人物的位置
                battleActor.pos = battleController.battleMapLen;
                //赋值人物id
                battleActor.myId = i;
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
                    new Vector3(curPanel.GetComponent<RectTransform>().rect.width * i, 0, 0);
                //绑定战斗角色和操作表
                PanelBind.bindEnemyPanel(battleActor, curPanel.transform);

                //绑定玩家的姓名
                battleActor.nameText.text = "敌人" + (i + 1);

                //添加到玩家列表中
                battleController.enemyActors.Add(battleActor);
            }
        }
    }
}

