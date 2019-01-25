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
        public string TypeName;

        public int System_Int32Value;
        public bool System_BooleanValue;
        public float System_SingleValue;
        public string System_StringValue;
        public Item2EnergyStructure.EnergyType Item2EnergyStructure_EnergyTypeValue;
        public Item2EnergyStructure.Conversion[] Item2EnergyStructure_ConversionArrayValue;
        public Item2ItemStructure.Conversion[] Item2ItemStructure_ConversionArrayValue;
        public Seed2ItemStructure.Conversion[] Seed2ItemStructure_ConversionArrayValue;

        public object GetValue() {
            foreach (FieldInfo info in GetType().GetRuntimeFields()) {
                if (info.FieldType.FullName == TypeName && info.Name.EndsWith("Value")) {
                    return info.GetValue(this);
                }
            }
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
    private string Initializer;
    [Tooltip("用于初始化的数据")]
    [SerializeField]
    private InitializeValue[] InitializeValues;
    [SerializeField]
    [HideInInspector]
    private UnityEngine.Object InitializerObject;

    public Structure Instantiate() {
        Type type = Type.GetType(Initializer);
        object o = type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { ID });
        foreach (InitializeValue value in InitializeValues) {
            Type t = type;
            do {
                bool flag = false;
                foreach (FieldInfo info in t.GetRuntimeFields()) {
                    if (info.Name == value.Name) {
                        info.SetValue(o, value.GetValue());
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
                t = t.BaseType;
            } while (t != null);
        }
        return (Structure)o;
    }
    
    public bool HasUnlocked() {
        return TechTreeManager.Instance.Techs[RequiredTech].TechState == Tech.State.COMPLETED;
    }
}
