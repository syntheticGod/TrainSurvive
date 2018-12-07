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
            return Utility.IgnoreZ(worldPosition);
        }
        /// <summary>
        /// 地图平面的2D世界坐标转化为3D世界坐标
        /// </summary>
        /// <param name="mapPosition">地图平面的2D世界坐标</param>
        /// <returns></returns>
        public static Vector3 MapPosToWorldPos(Vector2 mapPosition, int level)
        {
            return Utility.AcceptZ(mapPosition, level);
        }

        private static System.Random rand = new System.Random();
        private static string[] TOWN_NAME = {
            "香格里拉", "枫丹白露", "翡冷翠", "米兰", "墨尔本",
            "爱丁堡", "普罗旺斯", "哥本哈根", "耶路撒冷", "柏林", "布达佩斯",
            "都灵", "伯尔尼", "阿姆斯特布", "布里斯班", "达累斯萨拉姆",
            "斯德哥尔摩", "赫尔辛基", "布依诺斯艾利斯", "凤凰城" };
        private static string[] NPC_NAME_MAN =
        {
            "亚伦", "亚伯", "亚伯拉罕", "亚当", "艾德里安","阿尔瓦", "亚历克斯", "艾伯特" ,"阿尔弗雷德","安德鲁","安迪","安格斯","安东尼","亚瑟","奥斯汀","本森","布兰特","布伦特","卡尔","凯里","卡斯帕","查尔斯","采尼","克里斯蒂安","克里斯多夫","科林","科兹莫","丹尼尔","丹尼斯","德里克","唐纳德","道格拉斯","埃德加","艾德文","艾略特","埃尔维斯","埃里克","埃文","弗朗西斯","弗兰克","加百利","加比","加菲尔德","加里","加文","乔治","基诺","格林","格林顿","哈里森","汉克","霍华德","亨利","伊凡","艾萨克","杰克","杰克逊","雅各布","詹森","杰弗瑞","杰罗姆","杰西","吉姆","吉米","乔","约翰","约翰尼","约瑟夫","约书亚","贾斯汀","凯斯","肯","肯尼斯","肯尼","凯文","兰斯","拉里","劳伦特","劳伦斯","利安德尔","李","雷","雷纳德","利奥波特","劳瑞","劳瑞恩","卢克","马库斯","马西","马克","马尔斯","马丁","马修","迈克尔","麦克","尼尔","保罗","帕特里克","菲利普","菲比","昆廷","兰德尔","伦道夫","兰迪","列得","雷克斯","理查德","里奇","罗伯特","罗宾逊","洛克","罗杰","罗伊","赖安"
        };
        private static string[] NPC_NAME_WOMAN =
        {
            "阿比盖尔","艾比","艾达","阿德莱德","艾德","亚伦","亚伯","亚伯拉罕","亚当","艾德里安","阿尔瓦","亚历克斯","亚历山大","艾伦","艾伯特","阿尔弗雷德","安德鲁","安迪","安格斯","安东尼","亚瑟","奥斯汀","本","本森","比尔","鲍伯","布兰登","布兰特","布伦特","布莱恩","布鲁斯","卡尔","凯里","卡斯帕","查尔斯","采尼","克里斯","克里斯蒂安","克里斯多夫","科林","科兹莫","丹尼尔","丹尼斯","德里克","唐纳德","道格拉斯","大卫","丹尼","埃德加","爱德华","艾德文","艾略特","埃尔维斯","埃里克","埃文","弗朗西斯","弗兰克","富兰克林","弗瑞德","加百利","加比","加菲尔德","加里","加文","乔治","基诺","格林","格林顿","哈里森","雨果","汉克","霍华德","亨利","伊格纳缇伍兹","伊凡","艾萨克","杰克","杰克逊","雅各布","詹姆士","詹森","杰弗瑞","杰罗姆","杰瑞","杰西","吉姆","吉米","乔","约翰","约翰尼","约瑟夫","约书亚","贾斯汀","凯斯","肯","肯尼斯","肯尼","凯文","兰斯","拉里","劳伦特","劳伦斯","利安德尔","李","雷欧","雷纳德","利奥波特","劳伦","劳瑞","劳瑞恩","卢克","马库斯","马西","马克","马科斯","马尔斯","马丁","马修","迈克尔","麦克","尼尔","尼古拉斯","奥利弗","奥斯卡","保罗","帕特里克","彼得","菲利普","菲比","昆廷","兰德尔","伦道夫","兰迪","列得","雷克斯","理查德","里奇","罗伯特","罗宾","罗宾逊","洛克","罗杰","罗伊","赖安","阿比盖尔","艾比","艾达","阿德莱德","艾德"
        };
        public static string RandomTownName()
        {
            return TOWN_NAME[rand.Next(TOWN_NAME.Length)];
        }
        public static string RandomNPCName(bool man)
        {
            if (man)
                return NPC_NAME_MAN[rand.Next(NPC_NAME_MAN.Length)];
            else
                return NPC_NAME_WOMAN[rand.Next(NPC_NAME_WOMAN.Length)];
        }
        /// <summary>
        /// 生成 [0,maxValue) 范围内的整数
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int RandomInt(int maxValue)
        {
            return rand.Next(maxValue);
        }
        /// <summary>
        /// 生成[minValue,maxValue)范围的整数
        /// 如果minValue等于maxValue，则返回minValue
        /// maxValue必须大于等于minValue
        /// 否则异常 ArgumentOutOfRangeException
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int RandomRange(int minValue, int maxValue)
        {
            return rand.Next(minValue, maxValue);
        }
    }
}