/*
 * 描述：这是技能工厂类
 * 作者：王安鑫
 * 创建时间：2018/12/15 9:36:33
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class SkillFactory : MonoBehaviour {

        //通过id返回对应的技能
        private static Dictionary<int, Skill> skillMap = null;

        /// <summary>
        /// 通过id返回对应的技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Skill getSkill(int id, BattleActor battleActor) {
            //如果当前没初始化，将其初始化
            if (skillMap == null) {
                initSkillFactory();
            }

            //根据id获取指定技能
            Skill noneSkill = new NoneSkill(battleActor);
            if (skillMap.TryGetValue(id, out noneSkill) == false) {
                Debug.Log("正在获取未知的buff？" + id + battleActor);
                Debug.Break();
            }

            //绑定到当前的对象
            noneSkill.battleActor = battleActor;
            return noneSkill.Clone(battleActor);
        }

        /// <summary>
        /// 初始化技能工厂
        /// </summary>
        private static void initSkillFactory() {
            //初始化技能映射表
            skillMap = new Dictionary<int, Skill>();

            //将技能放入工厂中
            //放入重击技能
            skillMap.Add(1, new ThumpSkill(null));
            //放入加速技能
            skillMap.Add(2, new SpeedUpSkill(null));
            //放入毒素技能
            skillMap.Add(3, new PosionSkill(null));
            //放入二连击技能
            skillMap.Add(10, new DoubleAttackSkill(null));
        }
    }
}

