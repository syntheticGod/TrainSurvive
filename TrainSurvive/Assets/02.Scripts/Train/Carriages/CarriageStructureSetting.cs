/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 18:11:17
 * 版本：v0.7
 */
using System;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "策划/车厢功能设定")]
public class CarriageStructureSetting : ScriptableObject {

    [Serializable]
    public struct InitializeValue {
        public string Name;
        public string TypeName;

        public int System_Int32Value;
        public bool System_BooleanValue;
        public float System_SingleValue;
        public string System_StringValue;
        public int[] System_Int32ArrayValue;
        public Item_EnergyStructure.EnergyType Item_EnergyStructure_EnergyTypeValue;
        public Item_EnergyStructure.Conversion[] Item_EnergyStructure_ConversionArrayValue;
        public Item_ItemStructure.Conversion[] Item_ItemStructure_ConversionArrayValue;
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
    
    [Tooltip("设施名字")]
    public string Name;
    [Tooltip("初始是否启用")]
    public bool InitialEnabled;
    [Tooltip("用于初始化该建筑的类型函数，须要为CarriageStructure类型")]
    [SerializeField]
    private string Initializer;
    [Tooltip("用于初始化的数据")]
    [SerializeField]
    private InitializeValue[] InitializeValues;
    [SerializeField]
    [HideInInspector]
    private UnityEngine.Object InitializerObject;

    public CarriageStructure Instantiate() {
        Type type = Type.GetType(Initializer);
        object o = type.GetConstructor(new Type[] { typeof(string), typeof(bool) }).Invoke(new object[] { Name, InitialEnabled });
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
        return (CarriageStructure)o;
    }
}
