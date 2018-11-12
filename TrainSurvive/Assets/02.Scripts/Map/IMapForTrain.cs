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
        /// <param name="start">传出铁轨发出端的地图坐标</param>
        /// <param name="end">传出接入端的地图坐标</param>
        /// <returns>false：如果指定点不是铁轨则</returns>
        bool GetEachEndsOfRail(Vector2Int railPosition, out Vector2Int start, out Vector2Int end);
        /// <summary>
        /// 判断两座城市之间是否存在直接连通的铁轨。
        /// 并返回铁轨生成顺序
        /// </summary>
        /// <param name="twon1Start">城市1，如果连通则返回起点</param>
        /// <param name="twon2End">城市2，如果连通则返回终点</param>
        /// <returns>
        /// TRUE：两座城市之间存在直接连通的铁轨
        /// FALSE：两座城市之间不能直接连通
        /// </returns>
        bool IfConnectedBetweenTowns(ref Vector2Int twon1Start, ref Vector2Int twon2End);
        /// <summary>
        /// 散开迷雾
        /// </summary>
        /// <param name="position">散开迷雾的点</param>
        /// <returns>
        /// TRUE：操作成功
        /// FLASE：坐标点超出地图范围
        /// </returns>
        bool MoveToThisSpawn(Vector2Int position);
        /// <summary>
        /// 判断地图坐标是不是处于可见状态
        /// </summary>
        /// <param name="position"></param>
        /// <returns>
        /// TRUE：坐标处没有迷雾
        /// FALSE：坐标处有迷雾
        /// </returns>
        bool isSpawnVisible(Vector2Int position);
    }
}
