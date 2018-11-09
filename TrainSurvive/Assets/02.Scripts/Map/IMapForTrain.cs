/*
 * 描述：列车需要的地图接口，该接口涉及到的坐标都是以方块为单位。
 *          例如，getMapSize返回2X2方块个数（长，宽）。
 * 作者：项叶盛
 * 创建时间：2018/10/31 0:43:33
 * 版本：v0.1
 */
using UnityEngine;

namespace WorldMap
{
    public interface IMapForTrain
    {
        /// <summary>
        /// 判断地图坐标是否是轨道
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns></returns>
        bool IfRail(Vector2Int position);
        /// <summary>
        /// 判断地图坐标是否是城镇
        /// </summary>
        /// <param name="position">地图坐标，不是世界坐标</param>
        /// <returns></returns>
        bool IfTown(Vector2Int position);
        /// <summary>
        /// 获取轨道的两端。
        /// 如果铁轨只有一个点时，两个端点会重合。
        /// </summary>
        /// <param name="railPosition">传入铁轨的地图坐标。</param>
        /// <param name="end1">传出一端的地图坐标</param>
        /// <param name="end2">传出另一端的地图坐标</param>
        /// <returns>false：如果指定点不是铁轨则</returns>
        bool GetEachEndsOfRail(Vector2Int railPosition, out Vector2Int end1, out Vector2Int end2);
    }
}
