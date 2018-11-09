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
            ref Vector3 o, ref Vector3 t,
            float smoothTime, ref float xVelocity,
            ref float zVelocity)
        {
            o.x = Mathf.SmoothDamp(o.x,
                    t.x, ref xVelocity, smoothTime);
            o.z = Mathf.SmoothDamp(o.z,
                t.z, ref zVelocity, smoothTime);
        }
        /// <summary>
        /// 先沿着x轴光滑运动，再沿z轴光滑运动
        /// o矢量会被修改
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <param name="smoothTime"></param>
        /// <param name="Veloity"></param>
        /// <returns>如果发生位移则返回真，否则返回假</returns>
        public static bool goByXThanZSmoothly(
            ref Vector3 o, ref Vector3 t,
            float smoothTime, ref float velocity)
        {
            bool positiveX = true;
            bool positiveZ = true;
            //原点 - 目标
            float roadOfX = o.x - t.x;
            float roadOfZ = o.z - t.z;
            //如果 (原点 - 目标) <= 0 为真 则 positive = True (目标 - 原点 > 0)
            //如果 (原点 - 目标) <= 0 为假 则 positive = False (目标 - 原点 < 0)
            //等于0时 positive = True
            if (positiveX = (roadOfX <= 0)) roadOfX = -roadOfX;
            if (positiveZ = (roadOfZ <= 0)) roadOfZ = -roadOfZ;
            if (ApproximatelyInView(roadOfX, 0) && ApproximatelyInView(roadOfZ, 0))
                return false;
            float remainedRoad = roadOfX + roadOfZ;
            //remainedRoad一定是大于0的
            float deltaRoad = remainedRoad - Mathf.SmoothDamp(remainedRoad, 0, ref velocity, smoothTime);
            //Debug.Log("remainedRoad:" + remainedRoad + "delta road is:" + deltaRoad);
            //因为两座城市之间的铁轨是先x轴，再z轴。
            //所以x轴的优先度要高于z轴的优先度
            //Debug.Log("roadOfX:" + roadOfX + " and 0:" + ApproximatelyInView(roadOfX, 0));
            if (!ApproximatelyInView(roadOfX, 0))
            {
                deltaRoad = Mathf.Min(deltaRoad, roadOfX);
                o.x += positiveX ? deltaRoad : -deltaRoad;
            }
            else
            {
                deltaRoad = Mathf.Min(deltaRoad, roadOfZ);
                o.z += positiveZ ? deltaRoad : -deltaRoad;
            }
            return true;
        }
        private const float EPSILON_IN_VIEW = 1.0E-4F;
        public static bool ApproximatelyInView(float a, float b)
        {
            return Mathf.Abs(a - b) < EPSILON_IN_VIEW;
        }
        public static bool ApproximatelyInView(Vector2 a, Vector2 b)
        {
            return ApproximatelyInView(a.x, b.x) && ApproximatelyInView(a.y, b.y);
        }
        public static Vector2 IgnoreY(Vector3 a)
        {
            return new Vector2
            {
                x = a.x,
                y = a.z
            };
        }
        public static Vector3 AcceptY(Vector2 a, float y)
        {
            return new Vector3
            {
                x = a.x,
                y = y,
                z = a.y
            };
        }
    }
    public struct Matrix2x2Int
    {
        /// <summary>
        /// m00 m01
        /// m10 m11
        /// </summary>
        private int m00, m01, m10, m11;
        /// <summary>
        /// 顺时针旋转90度
        ///  0 -1
        ///  1  0
        /// </summary>
        private static Matrix2x2Int roate90Clockwise = new Matrix2x2Int { m01 = -1, m10 = 1 };
        /// <summary>
        /// 逆时针旋转90度
        ///  0  1
        /// -1  0
        /// </summary>
        private static Matrix2x2Int roate90Anticlockwise = new Matrix2x2Int { m01 = 1, m10 = -1 };
        private static Matrix2x2Int roate180 = new Matrix2x2Int { m00 = -1, m11 = -1 };
        /// <summary>
        /// 矩阵右乘向量
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="rhs">矩阵</param>
        /// <returns>转化后的向量</returns>
        private void RightMultipy(ref Vector2Int vector)
        {
            int x = vector.x * m00 + vector.y * m10;
            int y = vector.x * m10 + vector.y * m11;
            vector.x = x;
            vector.y = y;
        }
        /// <summary>
        ///矢量顺时针旋转90度
        /// </summary>
        /// <param name="vector">矢量</param>
        public static void Roate90Clockwise(ref Vector2Int vector)
        {
            roate90Clockwise.RightMultipy(ref vector);
        }
        public static Vector2Int Roate90Clockwise(Vector2Int vector)
        {
            roate90Clockwise.RightMultipy(ref vector);
            return vector;
        }
        /// <summary>
        /// 矢量逆时针旋转90度
        /// </summary>
        /// <param name="vector"></param>
        public static void Roate90Anticlockwise(ref Vector2Int vector)
        {
            roate90Anticlockwise.RightMultipy(ref vector);
        }
        public static Vector2Int Roate90Anticlockwise(Vector2Int vector)
        {
            roate90Anticlockwise.RightMultipy(ref vector);
            return vector;
        }
        public static void Roate180(ref Vector2Int vector)
        {
            vector.x = -vector.x;
            vector.y = -vector.y;
        }
        public static Vector2Int Roate180(Vector2Int vector)
        {
            vector.x = -vector.x;
            vector.y = -vector.y;
            return vector;
        }
    }
}