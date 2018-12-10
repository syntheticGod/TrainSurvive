/*
 * 描述：这是每一个单独的地块类
 * 目前地块的属性有气候、地形或者是特殊地块
 * 
 * 作者：王安鑫
 * 创建时间：2018/11/1 11:39:06
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap {
    public class SpawnPoint {
        //地块的种类
        //--------------------------------------------------------------------------
        //每个地块的气候种类
        public enum ClimateEnum {
            NONE = -1,

            //温带
            TEMPERATE,

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

            //湖泊

            //剧情副本
            SCENARIO_AREA,

            //特殊区域
            SPECIAL_AREA,

            //随机小副本
            RANDOM_AREA,

            NUM
        };

        //每个地块上可能拥有的gameObjects（气候，地块，特殊地块，城镇或铁轨）
        public enum SpawnObjectEnum {
            NONE = -1,

            //气候
            CLIMATE = 0,

            //地形
            TERRAIN = 1,

            //城镇
            TOWN = 2,

            //铁轨（城镇和铁轨只会出现一个）
            RAIL = 2,

            NUM
        }

        //当前地块可见的状态
        public enum SpawnViewStateEnum {
            NONE = -1,

            //不可见状态
            INVISIBLE,

            //半可见状态
            HALF_INVISIBLE,

            //可见状态（在所处地块）
            VISBLE,

            NUM
        }

        //地块的共有属性
        //---------------------------------------------------------------------------
        //当前地块的气候
        public ClimateEnum climateType { get; private set; }

        //当前地块属于四种地形
        public TerrainEnum terrainType { get; private set; }

        //当前地块是不是特殊地块
        public SpecialTerrainEnum specialTerrainType { get; private set; }

        //当前地块的可见状态
        public SpawnViewStateEnum viewState { get; private set; }

        //记录当前地块上所有的GameObject
        public List<GameObject> spawnObjects { get; private set; }

        //如果城镇，记录该城镇所在区域大块的位置
        //如果是铁轨记录的是即将到达的城镇
        public Vector2Int townPos { get; private set; }

        //如果是铁轨，将记录起始城镇所在区域大块的位置
        public Vector2Int startTownPos { get; private set; }

        //地块的方法
        //---------------------------------------------------------------------------
        //初始化当前气候为热带，地形为平原，不是特殊地带，不初始化城镇
        public SpawnPoint() {
            climateType = ClimateEnum.TROPIC;
            terrainType = TerrainEnum.NONE;
            specialTerrainType = SpecialTerrainEnum.NONE;
            spawnObjects = new List<GameObject>((int)SpawnObjectEnum.NUM);
            //for (int i = 0; i < (int)SpawnObjectEnum.NUM; i++) {
            //    spawnObjects.Add(new GameObject("none"));
            //}

            //设置初始的地块为不可见的状态
            if (MapGenerate.isFogState == true) {
                viewState = SpawnViewStateEnum.INVISIBLE;
            } else {
                viewState = SpawnViewStateEnum.VISBLE;
            }
        }

        //设置当前地块的气候
        public void SetClimateEnum(ClimateEnum climateType) {
            this.climateType = climateType;
        }

        //设置当前地块的地形
        public void SetTerrainEnum(TerrainEnum terrainType) {
            this.terrainType = terrainType;
        }

        //设置当前的特殊地块
        public void SetSpecialTerrain(SpecialTerrainEnum specialType) {
            specialTerrainType = specialType;
        }

        //设置当前（终点）的城镇位置
        public void SetTownId(Vector2Int townPos) {
            this.townPos = townPos;
        }

        //设置起点的城镇位置
        public void SetStartTownId(Vector2Int startTownPos) {
            this.startTownPos = startTownPos;
        }

        /// <summary>
        /// 设置地块上的gameObject
        /// 并按照是否可见状态设置gameObject
        /// 要保证调用的顺序和Enum的一致
        /// </summary>
        /// <param name="spawnObjectEnum">当前gameObject的类型</param>
        /// <param name="spawnObject">所要设置的gameObject</param>
        public void SetSpawnObject(SpawnObjectEnum spawnObjectEnum, GameObject spawnObject) {
            //spawnObjects[(int)spawnObjectEnum] = spawnObject;
            int curSpawnIndex = (int)spawnObjectEnum;
            if (spawnObjectEnum == SpawnObjectEnum.TOWN ||
                spawnObjectEnum == SpawnObjectEnum.RAIL) {

                //如果没有地形object，减一
                if (terrainType == TerrainEnum.NONE ||
                    terrainType == TerrainEnum.PLAIN) {
                    curSpawnIndex--;
                }
            }
            if (this.spawnObjects.Count <= curSpawnIndex) {
                this.spawnObjects.Insert(curSpawnIndex, spawnObject);
            } else {
                this.spawnObjects[curSpawnIndex] = spawnObject;
            }
            //设置当前地块的可见状态
            UpdateViewStateDisplay(spawnObject);
        }

        //改变当前地块的显示状态，如果当前状态不匹配，更新所有的gameObject的显示状态
        public void SetViewState(SpawnViewStateEnum newViewState) {
            if (viewState != newViewState) {
                viewState = newViewState;
                UpdateViewStateDisplay();
            }
        }

        //更新当前地块上所有gameObject的可见状态
        private void UpdateViewStateDisplay() {
            foreach (GameObject spawnObject in spawnObjects) {
                UpdateViewStateDisplay(spawnObject);
            }
        }

        //设置当前地块其中一个gameObject的可见状态
        private void UpdateViewStateDisplay(GameObject spawnObject) {
            //if (spawnObject.name == "none") {
            //    return;
            //}
            //获取所取类型地块的render
            SpriteRenderer render = spawnObject.GetComponent<SpriteRenderer>();

            //根据地块的可见状态设置地块的color
            switch (viewState) {
                case SpawnViewStateEnum.INVISIBLE:
                    //不可见状态
                    render.color = new Color(0.2f, 0.2f, 0.2f);
                    break;

                case SpawnViewStateEnum.HALF_INVISIBLE:
                    //半可见状态
                    render.color = Color.gray;
                    break;

                case SpawnViewStateEnum.VISBLE:
                    //可见状态
                    render.color = Color.white;
                    break;
            }
        }
    }
}

