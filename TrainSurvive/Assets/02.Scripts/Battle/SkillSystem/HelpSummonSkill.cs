/*
 * 描述：帮助生成相应的怪物
 * 作者：王安鑫
 * 创建时间：2019/2/22 18:56:43
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class HelpSummonSkill : MonoBehaviour {

        /// <summary>
        /// 帮助生成怪物
        /// </summary>
        /// <param name="monsterId">怪物id</param>
        /// <param name="pos">怪物的位置</param>
        public static void generateMonster(int monsterId, float pos, bool isPlayer) {
            //获取battleController
            BattleController battleController = BattleController.getInstance();

            //生成敌人
            GameObject curPlayer = Instantiate(battleController.player,
                battleController.orign,
                Quaternion.identity);
            //玩家角色无需转向，敌人角色需要
            curPlayer.transform.rotation = 
                Quaternion.Euler(curPlayer.transform.eulerAngles
                + new Vector3(0, isPlayer ? 0.0f : 180.0f, 0));

            BattleActor battleActor;
            //获取当前地块的怪物级别
            MonsterInitializer mi = MonsterInitializer.getInstance();
            //根据怪物的级别生成随机ID的怪物
            mi.initializeMonster(ref curPlayer, monsterId);
            battleActor = mi.getBattleActor();

            //初始化人物的位置
            battleActor.pos = pos;
            //赋值人物id
            battleActor.myId = isPlayer ?
                battleController.playerActors.Count : battleController.enemyActors.Count;
            //初始化人物是否为玩家角色
            battleActor.isPlayer = isPlayer;
            //添加到玩家列表中
            if (isPlayer) {
                battleController.playerActors.Add(battleActor);
            } else {
                battleController.enemyActors.Add(battleActor);
            }

            //设置其为召唤物
            battleActor.isSummon = true;
            //绑定当前的Animator
            battleActor.animator = battleActor.GetComponentInChildren<Animator>();
            //初始化子状态控制器
            battleActor.subStateController = new SubStateController(battleActor, battleActor.animator);

            Debug.Log("是我生成的！ " + monsterId + " " + pos + " " + isPlayer);

            return;
        }
    }
}

