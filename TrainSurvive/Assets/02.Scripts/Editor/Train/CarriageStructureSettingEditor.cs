/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2019/1/29 18:24:47
 * 版本：v0.7
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CarriageStructureSetting))]
public class CarriageStructureSettingEditor : Editor {

    private Vector2 mScrollPos;

    public override void OnInspectorGUI() {
        SerializedProperty nameProperty = serializedObject.FindProperty("Name");
        SerializedProperty initializerProperty = serializedObject.FindProperty("Initializer");
        SerializedProperty initializerObjectProperty = serializedObject.FindProperty("InitializerObject");
        SerializedProperty initializeValuesProperty = serializedObject.FindProperty("InitializeValues");

        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.PropertyField(nameProperty);
        InitializerProperty(initializerProperty, initializerObjectProperty, initializeValuesProperty);
    
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
    
    private void InitializerProperty(SerializedProperty initializerProperty, SerializedProperty initializerObjectProperty, SerializedProperty initializeValuesProperty) {
        EditorGUILayout.ObjectField(initializerObjectProperty, typeof(MonoScript));
        if (initializerObjectProperty.objectReferenceValue == null) {
            initializerProperty.stringValue = null;
            initializeValuesProperty.ClearArray();
            return;
        } else {
            initializerProperty.stringValue = ((MonoScript)initializerObjectProperty.objectReferenceValue).GetClass().FullName;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        Type scriptType = ((MonoScript)initializerObjectProperty.objectReferenceValue).GetClass();
        if (scriptType != typeof(CarriageStructure) && !scriptType.IsSubclassOf(typeof(CarriageStructure))) {
            EditorGUILayout.HelpBox(scriptType.Name + "不是一个有效的CarriageStructure类型。", MessageType.Error);
            EditorGUILayout.EndHorizontal();
            return;
        }
        LinkedList<FieldInfo> fieldInfos = GetAllFieldsWithAttr(scriptType);
        for (int i = 0; i < initializeValuesProperty.arraySize; i++) {
            bool found = false;
            foreach (FieldInfo info in fieldInfos) {
                SerializedProperty p = initializeValuesProperty.GetArrayElementAtIndex(i);
                if (info.Name == p.FindPropertyRelative("Name").stringValue && info.FieldType.FullName == p.FindPropertyRelative("TypeName").stringValue) {
                    found = true;
                }
            }
            if (!found) {
                initializeValuesProperty.DeleteArrayElementAtIndex(i);
                i--;
            }
        }
        EditorGUILayout.BeginVertical();
        foreach (FieldInfo info in fieldInfos) {
            StructurePublicFieldAttribute attr = info.GetCustomAttribute<StructurePublicFieldAttribute>();
            SerializedProperty property = null;
            for (int i = 0; i < initializeValuesProperty.arraySize; i++) {
                SerializedProperty p = initializeValuesProperty.GetArrayElementAtIndex(i);
                if (p.FindPropertyRelative("Name").stringValue == info.Name) {
                    property = p;
                    break;
                }
            }
            if (property == null) {
                initializeValuesProperty.arraySize++;
                property = initializeValuesProperty.GetArrayElementAtIndex(initializeValuesProperty.arraySize - 1);
                property.FindPropertyRelative("Name").stringValue = info.Name;
                property.FindPropertyRelative("TypeName").stringValue = info.FieldType.FullName;
            }
            string fieldName = info.FieldType.FullName.Replace("[]", "Array").Replace('.', '_').Replace('+', '_') + "Value";
            SerializedProperty valueProperty = property.FindPropertyRelative(fieldName);
            if (valueProperty == null) {
                Debug.LogError("InitializeValue Not Defined: " + info.FieldType.FullName + " " + fieldName);
                continue;
            }
            EditorGUILayout.PropertyField(valueProperty, new GUIContent(info.Name, attr.Tooltip), true);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private LinkedList<FieldInfo> GetAllFieldsWithAttr(Type type) {
        LinkedList<FieldInfo> fields = new LinkedList<FieldInfo>();
        Type t = type;
        do {
            foreach (FieldInfo info in t.GetRuntimeFields()) {
                if (!info.IsStatic && info.GetCustomAttribute<StructurePublicFieldAttribute>() != null) {
                    fields.AddLast(info);
                }
            }
            t = t.BaseType;
        } while (t != null);
        return fields;
    }
}
