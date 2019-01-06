/*
 * 描述：这是生成怪物和特殊战斗id的地方
 * 作者：王安鑫
 * 创建时间：2018/12/26 20:47:01
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldMap.SpawnPoint;

namespace WorldMap {
    public class MonsterGenerate : GenerateBase {
        //地图类
        public Map mapData;

        //表明怪物级别的图片
        public static GameObject[] levelPic;

        //表明特殊区域的图片
        public static GameObject specialAreaPic;

        //表明资源已被采集的图片
        public static GameObject isGatheredPic;

        //怪物对象父类
        public static Transform monsterParent;
        //是否采集的父类
        public static Transform isGatheredParent;

        //怪物图片偏移位置
        public static Vector3 monsterPicOffset = new Vector3(0, 0, -1.0f);

        //资源已被采集图标的位置
        public static Vector3 isGatheredPicOffset = new Vector3(0.0f, -0.05f, -0.03f);

        public override void otherInit() {
            //获取prefab

            //获取怪物等级的prefab
            levelPic = new GameObject[3];
            for (int i = 0; i < 3; i++) {
                levelPic[i] = (GameObject)Resources.Load("Sprite/map/prefab/level" + (i + 1));
            }

            //获取特殊区域的prefab
            specialAreaPic = (GameObject)Resources.Load("Sprite/map/prefab/special");

            //获取资源已被采集的prefab
            isGatheredPic = (GameObject)Resources.Load("Sprite/map/prefab/isGathered");

            //设置保存怪物图片的位置
            monsterParent = new GameObject("monsterId").transform;
            //设置保存资源图片的位置
            isGatheredParent = new GameObject("isGathered").transform;
        }

        public override void generate() {
            //生成怪物布局
            generateMonster();
            //生成特殊区域（特殊战斗）
            //generateSpecialBattle();
        }

        public override void paint() {
            //对怪物布局和特殊区域进行绘画
            paintMonster();
        }
        
        //生成怪物布局
        public void generateMonster() {
            //获取地图类
            Map map = Map.GetInstance();

            //每个网格有25%存在怪物，怪物难度(3/4概率为难度1，3/16概率为难度2，1/16概率为难度3）
            int []monsterCount = new int[3] { 0,0,0 };
            for (int i = 0; i < map.rowNum; i++) {
                for (int j = 0; j < map.colNum; j++) {
                    //如果不为空，跳过
                    if (map.spowns[i, j].specialTerrainType != SpecialTerrainEnum.NONE
                        //是否有25%的概率
                        || Random.Range(0, 4) != 0) {
                        continue;
                    }

                    //怪物的难度
                    int monsterLevel = 0;
                    //随机数
                    int randomNum = Random.Range(0, 16);

                    //记录各等级怪物地区生成的多少
                    if (randomNum < 12) {
                        //3 / 4难度为1
                        monsterLevel = 1;
                        monsterCount[0]++;
                    } else if (randomNum < 15) {
                        //3/16概率为难度2
                        monsterLevel = 2;
                        monsterCount[1]++;
                    } else {
                        //1/16概率难度为3
                        monsterLevel = 3;
                        monsterCount[2]++;
                    }

                    //设置怪物和怪物难度
                    map.spowns[i, j].SetSpecialTerrain(SpecialTerrainEnum.MONSTER);
                    map.spowns[i, j].SetMonsterId(monsterLevel);
                }
            }
            for (int i = 0; i < 3; i++) {
                Debug.Log("有" + monsterCount[i] + "个" + (i + 1) + "级怪物区域");
            }
        }

        //生成特殊区域
        public void generateSpecialBattle() {
            //获取地图类
            Map map = Map.GetInstance();

            //初始化时有25%概率生成特殊战斗
            int specialCount = 0;
            for (int i = 0; i < map.rowNum; i++) {
                for (int j = 0; j < map.colNum; j++) {
                    //如果不为空，跳过
                    if (map.spowns[i, j].specialTerrainType != SpecialTerrainEnum.NONE
                        //是否有25%的概率
                        || Random.Range(0, 4) != 0) {
                        continue;
                    }

                    //记录特殊区域生成的多少
                    specialCount++;
                    //设置特殊区域和区域id
                    map.spowns[i, j].SetSpecialTerrain(SpecialTerrainEnum.SPECIAL_AREA);
                    map.spowns[i, j].SetMonsterId(1);
                }
            }

            Debug.Log("有" + specialCount + "个特殊区域");
        }

        //对怪物布局和特殊区域进行绘画
        public void paintMonster() {
            //获取地图类
            Map map = Map.GetInstance();

            //放置怪物等级或者特殊区域标识
            for (int i = 0; i < map.rowNum; i++) {
                for (int j = 0; j < map.colNum; j++) {
                    //如果为怪物区域
                    switch (map.spowns[i, j].specialTerrainType) {
                        //对怪物等级图标进行绘画
                        case SpecialTerrainEnum.MONSTER: {
                            ObjectGenerate.paintMonster(new Vector2Int(i, j));
                        }
                        break;

                        //如果为特殊区域
                        case SpecialTerrainEnum.SPECIAL_AREA: {
                            ObjectGenerate.paintSpecialArea(new Vector2Int(i, j));
                        }
                        break;
                    }

                    //绘制当前资源是否被采集
                    if (map.spowns[i, j].isGathered == true) {
                        ObjectGenerate.paintIsGather(new Vector2Int(i, j));
                    }
                }
            }
        }


    }
}

