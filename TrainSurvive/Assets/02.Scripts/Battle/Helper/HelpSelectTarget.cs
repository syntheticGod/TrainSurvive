/*
 * 描述：这是一个帮助角色选取对应目标的类
 * 作者：王安鑫
 * 创建时间：2018/12/13 16:37:36
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class HelpSelectTarget {

        //获取选取目标所要比较的函数
        private delegate bool SelectTargetCompare(BattleActor curActor, BattleActor enemyActor, BattleActor selectActor);

        /// <summary>
        /// 选中最近的敌人
        /// 如果玩家已经选中了敌人，则不变
        /// 否则找最近的敌人
        /// </summary>
        public static int selectNearestEnemy(BattleActor battleActor) {
            //如果当前已经存在玩家选中的目标了，朝着目标行动
            if (battleActor.selectedAtkTarget != -1
                && battleActor.enemyActors[battleActor.selectedAtkTarget].isAlive == true) {
                return battleActor.selectedAtkTarget;
            } else {
                //获取距离最近的目标
                return getNearestEnemy(battleActor);
            }
        }

        /// <summary>
        /// 查找最近的目标
        /// 距离相等时则找序号最小的
        /// </summary>
        /// <returns>返回目标的id</returns>
        public static int getNearestEnemy(BattleActor battleActor) {
            return getMapEnemy(battleActor, 
                (BattleActor curActor, BattleActor enemyActor, BattleActor selectedActor) => {
                    return Mathf.Abs(selectedActor.pos - curActor.pos) > Mathf.Abs(enemyActor.pos - curActor.pos);
                });
        }

        /// <summary>
        /// 查找生命值最小的存活目标
        /// 距离相等时则找序号最小的
        /// </summary>
        /// <returns>返回目标的id</returns>
        public static int getHPMinestEnemy(BattleActor battleActor) {
            return getMapEnemy(battleActor,
                (BattleActor curActor, BattleActor enemyActor, BattleActor selectedActor) => {
                    return selectedActor.curHealthPoint > enemyActor.curHealthPoint;
                });
        }

        /// <summary>
        /// 查找生命值百分比最小的存活目标
        /// 距离相等时则找序号最小的
        /// </summary>
        /// <returns>返回目标的id</returns>
        public static int getHPRateMinestEnemy(BattleActor battleActor) {
            return getMapEnemy(battleActor,
                (BattleActor curActor, BattleActor enemyActor, BattleActor selectedActor) => {
                    return selectedActor.curHealthPoint / selectedActor.maxHealthPoint
                    > enemyActor.curHealthPoint / selectedActor.maxHealthPoint;
                });
        }

        /// <summary>
        /// 查找生命值最大的存活目标
        /// 距离相等时则找序号最小的
        /// </summary>
        /// <returns>返回目标的id</returns>
        public static int getHPMaxestEnemy(BattleActor battleActor) {
            return getMapEnemy(battleActor,
                (BattleActor curActor, BattleActor enemyActor, BattleActor selectedActor) => {
                    return selectedActor.curHealthPoint < enemyActor.curHealthPoint;
                });
        }

        /// <summary>
        /// 获取在指定条件下敌人列表中满足条件的对象
        /// 如果敌人列表全部死亡，则返回-1
        /// </summary>
        /// <param name="battleActor">当前调用的角色</param>
        /// <param name="compare">比较函数</param>
        /// <returns>返回指定对象</returns>
        private static int getMapEnemy(BattleActor battleActor, SelectTargetCompare compare) {
            //当前选中的目标
            int selectedId = -1;
            //敌方角色链表
            List<BattleActor> enemyActors = battleActor.enemyActors;

            //根据指定条件选择对应的目标
            foreach (BattleActor enemyActor in enemyActors) {
                //如果当前敌人存活
                if (enemyActor.isAlive) {
                    if (selectedId == -1 || compare(battleActor, enemyActor, enemyActors[selectedId]) == true) { 
                        selectedId = enemyActor.myId;
                    }
                }
            }

            return selectedId;
        }
    }
}

