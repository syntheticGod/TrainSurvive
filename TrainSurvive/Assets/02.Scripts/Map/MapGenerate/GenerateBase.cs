/*
 * 描述：这是生成的基类
 * 作者：王安鑫
 * 创建时间：2018/12/26 20:47:01
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldMap.SpawnPoint;

namespace WorldMap {

    public abstract class GenerateBase : MonoBehaviour {
        //获取总的地图属性
        protected MapGenerate mapGenerate;

        /// <summary>
        /// 开始生成相应物体并进行绘画
        /// </summary>
        public void StartGenerate() {
            //对各对象进行初始化
            mapGenerate = GameObject.Find("MapBuild").GetComponent<MapGenerate>();
            //如果有需要，对其余进行初始化
            otherInit();
            //如果是第一次载入就先生成
            if (mapGenerate.isCreateMap) {
                generate();
            }
            //对相应的生成进行绘画
            paint();
        }

        /// <summary>
        /// 其余需要初始化的地方
        /// </summary>
        public abstract void otherInit();

        /// <summary>
        /// 开始生成相应物品
        /// </summary>
        public abstract void generate();

        /// <summary>
        /// 对生成的目标进行绘画
        /// </summary>
        public abstract void paint();
    }
}
