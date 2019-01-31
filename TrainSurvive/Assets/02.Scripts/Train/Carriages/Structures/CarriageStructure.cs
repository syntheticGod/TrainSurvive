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
    /// 设施名称
    /// </summary>
    public bool Enabled { get; set; }

    public CarriageBackend CarriageBackend { get; set; }

    public SerializableDictionary<string, object> States { get; }
    
    /// <summary>
    /// 当设施进入启动状态的回调。
    /// </summary>
    public virtual void OnStart() { }

    public virtual void OnUpgraded(CarriageResearchSetting upgrade) { }

    protected void UpdateState(string state, object value) {
        if (!States.ContainsKey(state))
            States.Add(state, value);
        else
            States[state] = value;
        CarriageBackend.CallOnStructureStateChanged(Name, state, value);
    }

    #region 序列化组
    public CarriageStructure(string name, bool initialEnabled) {
        Name = name;
        Enabled = initialEnabled;
        States = new SerializableDictionary<string, object>();
    }

    protected CarriageStructure(SerializationInfo info, StreamingContext context) {
        Name = info.GetString("Name");
        Enabled = info.GetBoolean("Enabled");
        States = (SerializableDictionary<string, object>) info.GetValue("States", typeof(SerializableDictionary<string, object>));
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("Name", Name);
        info.AddValue("Enabled", Enabled);
        info.AddValue("States", States);
    }
    #endregion
}
