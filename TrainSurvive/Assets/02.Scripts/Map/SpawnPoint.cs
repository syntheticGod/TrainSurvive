/*
 * 描述：这是每一个单独的地块类
 * 目前地块的属性有气候、地形或者是特殊地块
 * 
 * 作者：王安鑫
 * 创建时间：2018/11/1 11:39:06
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap {
    public class SpawnPoint {
        //每个地块的气候种类
        public enum ClimateEnum {
            NONE = -1,

            //热带
            TROPIC,

            //冻原
            TUNDRA,

            //炎热
            HEAT,

            //极寒
            COLD,

            NUM
        }

        //每个地块单独的种类
        public enum TerrainEnum {
            NONE = -1,

            //平原
            PLAIN,

            //丘陵
            HILL,

            //森林
            FOREST,

            //山地
            MOUNTAIN,

            NUM
        };

        //当前地块是不是其中一个特殊地块
        public enum SpecialTerrainEnum {
            NONE = -1,

            //城镇
            TOWN,

            //铁轨
            RAIL,

            //剧情副本
            SCENARIO_AREA,

            //特殊区域
            SPECIAL_AREA,

            //随机小副本
            RANDOM_AREA,

            NUM
        };

        //当前地块属于四种地形
        public TerrainEnum terrainType { get; private set; }

        //当前地块是不是特殊地块
        public SpecialTerrainEnum specialTerrainType { get; private set; }

        //初始化当前地形为平原，不是特殊地带
        public SpawnPoint() {
            terrainType = TerrainEnum.PLAIN;
            specialTerrainType = SpecialTerrainEnum.NONE;
        }

        //设置当前地块的地形
        public void SetTerrainEnum(TerrainEnum terrainType) {
            this.terrainType = terrainType;
        }

        //设置当前的特殊地块
        public void SetSpecialTerrain(SpecialTerrainEnum specialType) {
            specialTerrainType = specialType;
        }
    }
}

