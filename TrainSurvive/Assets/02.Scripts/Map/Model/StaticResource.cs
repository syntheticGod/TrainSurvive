/*
 * 描述：静态变量
 * 作者：项叶盛
 * 创建时间：2018/11/12 21:46:39
 * 版本：v0.1
 */
using UnityEngine;

namespace WorldMap
{
    public class StaticResource
    {
        private StaticResource() { }
        public static StaticResource staticResource;
        public static StaticResource Instance()
        {
            if (staticResource == null) staticResource = new StaticResource();
            return staticResource;
        }
        //方块大小
        public Vector2 BlockSize
        {
            set;
            get;
        }
        //地图第一块的中心坐标
        public Vector2 MapOrigin
        {
            set;
            get;
        }
        /// <summary>
        ///方便块索引计算的 常量
        ///(position - (o - b / 2)) / blockSize;
        /// </summary>
        public Vector2 MapOriginUnit
        {
            set;
            get;
        }
        /// <summary>
        /// 根据世界坐标计算地图坐标
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <returns>地图坐标</returns>
        public static Vector2Int BlockIndex(Vector2 position)
        {
            //因为块中心位于原点坐标处，所以要减去blockSize/2
            //(position - (o - b / 2)) / blockSize;
            //公式优化如下
            //position / blockSize - ((mapOrigin - blockSize/2)/blockSize)
            Vector2 index2F = position / staticResource.BlockSize - staticResource.MapOriginUnit;
            return new Vector2Int(Mathf.FloorToInt(index2F.x), Mathf.FloorToInt(index2F.y));
        }
        /// <summary>
        /// 计算指定块中心的世界坐标
        /// </summary>
        /// <param name="index2d">地图坐标</param>
        /// <returns>世界坐标</returns>
        public static Vector2 BlockCenter(Vector2Int index2d)
        {
            //索引 * 块大小 = 原点到块中心的向量
            //再加上 原块的中心坐标 就是世界坐标
            return index2d * staticResource.BlockSize + staticResource.MapOrigin;
        }
        /// <summary>
        /// 获取当前坐标所在块的中心坐标
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <returns></returns>
        public static Vector2 FormatBlock(Vector2 position)
        {
            return BlockCenter(BlockIndex(position));
        }
        /// <summary>
        /// 3D世界坐标转化为地图平面的2D世界坐标
        /// </summary>
        /// <param name="worldPosition">3D世界坐标</param>
        /// <returns></returns>
        public static Vector2 WorldPosToMapPos(Vector3 worldPosition)
        {
            return Utility.IgnoreY(worldPosition);
        }
        /// <summary>
        /// 地图平面的2D世界坐标转化为3D世界坐标
        /// </summary>
        /// <param name="mapPosition">地图平面的2D世界坐标</param>
        /// <returns></returns>
        public static Vector3 MapPosToWorldPos(Vector2 mapPosition, int level)
        {
            return Utility.AcceptY(mapPosition, level);
        }

        private static System.Random rand = new System.Random();
        private static string[] TOWN_NAME = {
            "香格里拉", "枫丹白露", "翡冷翠", "米兰", "墨尔本",
            "爱丁堡", "普罗旺斯", "哥本哈根", "耶路撒冷", "柏林", "布达佩斯",
            "都灵", "伯尔尼", "阿姆斯特布", "布里斯班", "达累斯萨拉姆",
            "斯德哥尔摩", "赫尔辛基", "布依诺斯艾利斯", "凤凰城" };
        public static string RandomName()
        {
            return TOWN_NAME[rand.Next(TOWN_NAME.Length)];
        }
    }
}