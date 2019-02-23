/*
 * 描述：
 * 作者：项叶盛
 * 创建时间：2019/2/23 17:15:46
 * 版本：v0.7
 */
using UnityEngine;
using System;

[Serializable]
public class SerializableVector2Int
{
    public int x;
    public int y;

    public SerializableVector2Int(Vector2Int vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public SerializableVector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        SerializableVector2Int other = obj as SerializableVector2Int;
        if (other == null) return false;
        return (x == other.x) && (y == other.y);
    }

    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}
