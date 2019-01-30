/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/25 13:08:15
 * 版本：v0.7
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public interface IConversion {
    float GetProcessTime { get; }
}

[Serializable]
public class Formula<C> : ISerializable where C : IConversion {
    public C Conversion { get; set; }
    public float Progress {
        get {
            return _progress;
        }
        set {
            _progress = value;
            OnProgressChanged?.Invoke(Priority, Conversion.GetProcessTime, value);
        }
    }
    public int Count {
        get {
            _count = OnAcquireCount?.Invoke(Priority) ?? _count;
            return _count;
        }
        set {
            _count = value;
            OnCountChanged?.Invoke(Priority, value);
        }
    }
    public int Priority { get; set; }

    public event Action<int, float, float> OnProgressChanged;
    public event Action<int, int> OnCountChanged;
    public event Func<int, int> OnAcquireCount;

    private float _progress;
    private int _count;

    public Formula(C conversion, int priority) {
        Conversion = conversion;
        Priority = priority;
    }

    private Formula(SerializationInfo info, StreamingContext context) {
        Conversion = (C)info.GetValue("Conversion", typeof(C));
        Progress = info.GetSingle("Progress");
        Count = info.GetInt32("Count");
        Priority = info.GetInt32("Priority");
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("Conversion", Conversion);
        info.AddValue("Progress", Progress);
        info.AddValue("Count", Count);
        info.AddValue("Priority", Priority);
    }
}
