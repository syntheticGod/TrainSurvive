/*
 * 描述：静态资源的暂时存放处，地图大小，方块大小等信息应该从配置文件中获取。
 * 作者：项叶盛
 * 创建时间：2018/11/21 23:11:44
 * 版本：v0.1
 */
using UnityEngine;

namespace WorldMap
{
    public interface IMapForTrainTemp
    {
        /// <summary>
        /// 获取地图的X轴方块个数，和Z轴的方块个数
        /// </summary>
        /// <returns>整数向量为（X轴个数, Z轴个数）</returns>
        Vector2Int GetMapSize();
        /// <summary>
        /// 获取第一块方块中心的世界坐标
        /// </summary>
        /// <returns>浮点向量为（X轴世界坐标，Z轴世界坐标）</returns>
        Vector2 GetMapOrigin();
        /// <summary>
        /// 获取一块的长和宽
        /// </summary>
        /// <returns>浮点向量为（X轴长度，Z轴长度）</returns>
        Vector2 GetBlockSize();
    }
}