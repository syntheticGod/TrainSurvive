using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap
{
    //功能类函数
    public static class Utility
    {
        public static void Swap<T>(ref T my, ref T other)
        {
            T temp = my;
            my = other;
            other = temp;
        }
        private const float EPSILON_IN_VIEW = 1.0E-4F;
        /// <summary>
        /// 判断两个值是否在视觉上接近
        /// 差值在1.0x10^4以内的范围的视为相同
        /// </summary>
        /// <param name="a">向量1</param>
        /// <param name="b">向量2</param>
        /// <returns>
        /// TRUE：在视觉上接近
        /// FALSE：在视觉上不接近
        /// </returns>
        public static bool ApproximatelyInView(float a, float b)
        {
            return Mathf.Abs(a - b) < EPSILON_IN_VIEW;
        }
        /// <summary>
        /// 判断两个向量是否在视觉上接近
        /// x轴和y轴差值都在1.0x10^4以内的范围的视为相同
        /// </summary>
        /// <param name="a">向量1</param>
        /// <param name="b">向量2</param>
        /// <returns>
        /// TRUE：在视觉上接近
        /// FALSE：在视觉上不接近
        /// </returns>
        public static bool ApproximatelyInView(Vector2 a, Vector2 b)
        {
            return ApproximatelyInView(a.x, b.x) && ApproximatelyInView(a.y, b.y);
        }
        public static bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x - b.x, 0.0F) && Mathf.Approximately(a.y - b.y, 0.0F);
        }
        /// <summary>
        /// 忽略3维向量的y轴
        /// (x,y,z) => (x,z)
        /// </summary>
        /// <param name="a"></param>
        /// <returns>2D向量</returns>
        public static Vector2 IgnoreY(Vector3 a)
        {
            return new Vector2
            {
                x = a.x,
                y = a.z
            };
        }
        /// <summary>
        /// 添加y轴
        /// (x,z) => (x,y,z)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="y"></param>
        /// <returns>3D向量</returns>
        public static Vector3 AcceptY(Vector2 a, float y)
        {
            return new Vector3
            {
                x = a.x,
                y = y,
                z = a.y
            };
        }
        /// <summary>
        /// 判断value是否在范围[edge1,edge2]或[edge2,edge1]之间
        /// </summary>
        /// <param name="edge1">范围边界1</param>
        /// <param name="edge2">范围边界2</param>
        /// <param name="value">值</param>
        /// <returns>
        /// TRUE：value在范围内
        /// FALSE：不在范围内
        /// </returns>
        public static bool IfBetweenBoth(float edge1, float edge2, float value)
        {
            if (Mathf.Approximately(edge1, value) || Mathf.Approximately(edge2, value))
                return true;
            return (value > edge1 && value < edge2) || (value > edge2 && value < edge1);
        }
        public static bool IfBetweenLeft(float edge1, float edge2, float value)
        {
            if (Mathf.Approximately(edge1, value))
                return true;
            if (Mathf.Approximately(edge2, value))
                return false;
            return (value > edge1 && value < edge2) || (value > edge2 && value < edge1);
        }
        public static bool Between(int left, int right, int value)
        {
            return value >= left && value <= right;
        }
    }
    public struct Matrix2x2Int
    {
        /// <summary>
        /// m00 m01
        /// m10 m11
        /// </summary>
        private int m00, m01, m10, m11;

        public Matrix2x2Int(int m00, int m01, int m10, int m11)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m10 = m10;
            this.m11 = m11;
        }
        /// <summary>
        /// 顺时针旋转90度
        ///  0 -1
        ///  1  0
        /// </summary>
        private static Matrix2x2Int roate90Clockwise = new Matrix2x2Int
        {
            m00 = 0, m01 = -1,
            m10 = 1, m11 = 0
        };
        /// <summary>
        /// 逆时针旋转90度
        ///  0  1
        /// -1  0
        /// </summary>
        private static Matrix2x2Int roate90Anticlockwise = new Matrix2x2Int
        {
            m00 = 0, m01 = 1,
            m10 = -1, m11 = 0
        };
        /// <summary>
        /// 旋转180度
        ///  -1   0
        ///   0  -1
        /// </summary>
        private static Matrix2x2Int roate180 = new Matrix2x2Int
        {
            m00 = -1, m01 = 0,
            m10 = 0, m11 = -1
        };
        /// <summary>
        /// 矩阵右乘向量 
        /// vector = vector * matrix
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="rhs">矩阵</param>
        /// <returns>转化后的向量</returns>
        private void RightMultipy(ref Vector2Int vector)
        {
            int x = vector.x;
            int y = vector.y;
            vector.x = x * m00 + y * m10;
            vector.y = x * m01 + y * m11;
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
        /// <summary>
        /// 矢量选择180度
        /// </summary>
        /// <param name="vector"></param>
        public static void Roate180(ref Vector2Int vector)
        {
            vector.x = -vector.x;
            vector.y = -vector.y;
        }
        public static void Roate180(ref Vector2 vector)
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
    public struct Pair<T1, T2>
    {
        public T1 one;
        public T2 two;
    }
}
