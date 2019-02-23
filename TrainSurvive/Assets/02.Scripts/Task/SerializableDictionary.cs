/*
 * 描述：
 * 作者：NONE
 * 创建时间：2018/12/22 22:24:20
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> _keys = new List<TKey>();
    [SerializeField]
    private List<TValue> _values = new List<TValue>();
    public SerializableDictionary() { }
    public SerializableDictionary(SerializationInfo info, StreamingContext context)
    {
        _keys = (List<TKey>)info.GetValue("keys", typeof(List<TKey>));
        _values = (List<TValue>)info.GetValue("values", typeof(List<TValue>));
        OnAfterDeserialize();
    }
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        OnBeforeSerialize();
        info.AddValue("keys", _keys);
        info.AddValue("values", _values);
    }
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        _keys.Capacity = this.Count;
        _values.Capacity = this.Count;
        foreach (var kvp in this)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        int count = Mathf.Min(_keys.Count, _values.Count);
        for (int i = 0; i < count; ++i)
        {
            this.Add(_keys[i], _values[i]);
        }
    }
}
