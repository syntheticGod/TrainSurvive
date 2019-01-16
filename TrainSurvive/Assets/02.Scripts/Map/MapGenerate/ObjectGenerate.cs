/*
 * 描述：这是管理大地图的对象（目前只管理特殊战斗）
 * 作者：王安鑫
 * 创建时间：2019/1/5 13:42:42
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldMap.SpawnPoint;

namespace WorldMap {
    public class ObjectGenerate : MonoBehaviour {

        /// <summary>
        /// 生成特殊战斗区域的图标（显示）
        /// </summary>
        public static void paintSpecialArea(Vector2Int pos) {
            //生成特殊战斗区域对象
            GameObject o = Instantiate(MonsterGenerate.specialAreaPic,
                                    MapGenerate.orign + MonsterGenerate.monsterPicOffset
                                    + new Vector3(MapGenerate.spawnOffsetX * pos.x, MapGenerate.spawnOffsetZ * pos.y, 0),
                                    Quaternion.identity);
            //设置父节点
            o.transform.parent = MonsterGenerate.monsterParent;
            o.GetComponent<SpriteRenderer>().sortingOrder = 7;

            //设置特殊区域对象
            Map.GetInstance().spowns[pos.x, pos.y].SetSpawnObject(SpawnObjectEnum.SPECIAL_AREA, o);
        }

        /// <summary>
        /// 生成是否采集的图标（显示）
        /// </summary>
        public static void paintIsGather(Vector2Int pos) {
            //生成是否采集对象
            GameObject o = Instantiate(MonsterGenerate.isGatheredPic,
                                    MapGenerate.orign + MonsterGenerate.isGatheredPicOffset
                                    + new Vector3(MapGenerate.spawnOffsetX * pos.x, MapGenerate.spawnOffsetZ * pos.y, 0),
                                    Quaternion.identity);
            //设置父节点
            o.transform.parent = MonsterGenerate.isGatheredParent;
            o.GetComponent<SpriteRenderer>().sortingOrder = 5;
            //设置是否采集对象
            Map.GetInstance().spowns[pos.x, pos.y].SetSpawnObject(SpawnObjectEnum.IS_GATHERED, o);
        }

        /// <summary>
        /// 生成怪物等级图标
        /// </summary>
        public static void paintMonster(Vector2Int pos) {
            Map map = Map.GetInstance();
            //生成怪物等级对象
            GameObject o = Instantiate(MonsterGenerate.levelPic[map.spowns[pos.x, pos.y].monsterId - 1],
                                MapGenerate.orign + MonsterGenerate.monsterPicOffset
                                + new Vector3(MapGenerate.spawnOffsetX * pos.x, MapGenerate.spawnOffsetZ * pos.y, 0),
                                Quaternion.identity);
            //设置父节点
            o.transform.parent = MonsterGenerate.monsterParent;
            o.GetComponent<SpriteRenderer>().sortingOrder = 6;

            //设置怪物等级对象
            map.spowns[pos.x, pos.y].SetSpawnObject(SpawnObjectEnum.MONSTER_LEVEL, o);
        }
    }
}
