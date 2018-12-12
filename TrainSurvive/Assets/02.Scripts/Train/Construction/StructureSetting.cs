/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/11 20:24:01
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "策划/建筑")]
public class StructureSetting : ScriptableObject {
    
    [Serializable]
    public struct InitializeValue {
        public string Name;
        public Type Type;
        public int IntValue;
        public bool BoolValue;
        public float FloatValue;
        public string StringValue;
        public Vector2 Vector2Value;
        public Vector3 Vector3Value;
        public Color ColorValue;

        public object GetValue() {
            if (Type == typeof(int))
                return IntValue;
            if (Type == typeof(bool))
                return BoolValue;
            if (Type == typeof(float))
                return FloatValue;
            if (Type == typeof(string))
                return StringValue;
            if (Type == typeof(Vector2))
                return Vector2Value;
            if (Type == typeof(Vector3))
                return Vector3Value;
            if (Type == typeof(Color))
                return ColorValue;
            return null;
        }
    }

    [Tooltip("ID")]
    public int ID;
    [Tooltip("物体放置方向，默认向下")]
    public Vector2 LayerOrientation = Vector2.down;
    [Tooltip("物体所属Layer。")]
    public int Layer;
    [Tooltip("可以支持该设施的建筑平台的Layers。")]
    public LayerMask RequiredLayers;
    [Tooltip("设施名字")]
    public string Name;
    [Tooltip("设施描述")]
    public string Description;
    [Tooltip("总建造工作量")]
    public float WorkAll = 0.01f;
    [Tooltip("建造耗材")]
    public ItemData[] BuildCosts;
    [Tooltip("Sprite")]
    public Sprite Sprite;
    [Tooltip("所属类型")]
    public int Class;
    [Tooltip("要求科技")]
    public int RequiredTech;
    [Tooltip("用于初始化该建筑的类型函数，须要为Structure类型")]
    [SerializeField]
    private MonoScript Initializer;
    [Tooltip("用于初始化的数据")]
    [SerializeField]
    private InitializeValue[] InitializeValues;

    public Structure Instantiate() {
        object o = Initializer.GetClass().GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { ID });
        foreach (InitializeValue value in InitializeValues) {
            o.GetType().GetField(value.Name).SetValue(o, value.GetValue());
        }
        return (Structure)o;
    }

    public bool HasUnlocked() {
        return TechTreeManager.Instance.Techs[RequiredTech].TechState == Tech.State.COMPLETED;
    }
}
