/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 18:33:48
 * 版本：v0.7
 */
using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class CarriageStructure : ISerializable {
    
    /// <summary>
    /// 设施名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 当设施进入启动状态的回调。
    /// </summary>
    public virtual void OnStart() { }

    #region 序列化组
    public CarriageStructure(string name) {
        Name = name;
    }

    protected CarriageStructure(SerializationInfo info, StreamingContext context) {
        Name = info.GetString("Name");
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("Name", Name);
    }
    #endregion
}
