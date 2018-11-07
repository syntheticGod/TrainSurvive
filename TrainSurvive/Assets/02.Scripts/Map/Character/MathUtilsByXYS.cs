/*
 * 描述：数学坐标工具类
 * 作者：项叶盛
 * 创建时间：2018/11/1 0:50:32
 * 版本：v0.1
 */
using UnityEngine;
namespace WorldMap
{
    public class MathUtilsByXYS
    {
        /// <summary>
        /// 判断 两个向量的X与Z 是否已经临近
        /// </summary>
        /// <param name="a">向量1</param>
        /// <param name="b">向量2</param>
        /// <returns>如果靠近则返回TRUE，反之FALSE</returns>
        public static bool ifCloselyXZ(Vector3 a, Vector3 b)
        {
            if (Mathf.Abs(a.x - b.x) >= Mathf.Epsilon)
                return false;
            if (Mathf.Abs(a.z - b.z) >= Mathf.Epsilon)
                return false;
            return true;
        }
        /// <summary>
        /// 判断 两个向量的X与Z 是否已经临近，
        /// 使用引用参数是为了避免值拷贝，
        /// 同时该函数保证不对参数进行修改。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ifCloselyXZ(ref Vector3 a,
            ref Vector3 b)
        {
            if (Mathf.Abs(a.x - b.x) >= Mathf.Epsilon)
                return false;
            if (Mathf.Abs(a.z - b.z) >= Mathf.Epsilon)
                return false;
            return true;
        }
        /// <summary>
        /// 光滑的从起始点移动到终点
        /// </summary>
        /// <param name="o">起始点坐标</param>
        /// <param name="t">终点坐标</param>
        /// <param name="smoothTime"></param>
        /// <param name="xVelocity"></param>
        /// <param name="zVelocity"></param>
        /// <returns>如果新的坐标</returns>
        public static Vector3 goStraightSmoothlyXZ(
            Vector3 o, Vector3 t,
            float smoothTime, ref float xVelocity, 
            ref float zVelocity)
        {
            o.x = Mathf.SmoothDamp(o.x,
                    t.x, ref xVelocity, smoothTime);
            o.z = Mathf.SmoothDamp(o.z,
                t.z, ref zVelocity, smoothTime);
            return o;
        }
        /// <summary>
        /// 光滑的从起始点移动到终点
        /// 引用模式，修改原点的坐标
        /// </summary>
        /// <param name="o">起始点坐标</param>
        /// <param name="t">终点坐标</param>
        /// <param name="smoothTime"></param>
        /// <param name="xVelocity"></param>
        /// <param name="zVelocity"></param>
        public static void goStraightSmoothlyXZ(
            ref Vector3 o, Vector3 t,
            float smoothTime, ref float xVelocity,
            ref float zVelocity)
        {
            o.x = Mathf.SmoothDamp(o.x,
                    t.x, ref xVelocity, smoothTime);
            o.z = Mathf.SmoothDamp(o.z,
                t.z, ref zVelocity, smoothTime);
        }
    }
}