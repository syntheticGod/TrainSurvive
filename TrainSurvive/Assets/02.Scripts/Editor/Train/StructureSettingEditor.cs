/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/11 21:23:04
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StructureSetting))]
public class StructureSettingEditor : Editor {

    private Vector2 mScrollPos;
    
    public override void OnInspectorGUI() {
        SerializedProperty idProperty = serializedObject.FindProperty("ID");
        SerializedProperty nameProperty = serializedObject.FindProperty("Name");
        SerializedProperty descriptionProperty = serializedObject.FindProperty("Description");
        SerializedProperty workAllProperty = serializedObject.FindProperty("WorkAll");
        SerializedProperty spriteProperty = serializedObject.FindProperty("Sprite");
        SerializedProperty classProperty = serializedObject.FindProperty("Class");
        SerializedProperty buildCostsProperty = serializedObject.FindProperty("BuildCosts");
        SerializedProperty requiredTechProperty = serializedObject.FindProperty("RequiredTech");
        SerializedProperty initializerProperty = serializedObject.FindProperty("Initializer");
        SerializedProperty requiredLayersProperty = serializedObject.FindProperty("RequiredLayers");
        SerializedProperty layerProperty = serializedObject.FindProperty("Layer");
        SerializedProperty layerOrientationProperty = serializedObject.FindProperty("LayerOrientation");
        SerializedProperty initializeValuesProperty = serializedObject.FindProperty("InitializeValues");

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(idProperty);
        EditorGUILayout.PropertyField(nameProperty);
        EditorGUILayout.PropertyField(descriptionProperty);
        InitializerProperty(initializerProperty, initializeValuesProperty);
        EditorGUILayout.PropertyField(spriteProperty);
        EditorGUILayout.PropertyField(workAllProperty);
        ClassProperty(classProperty);
        RequiredTechProperty(requiredTechProperty);
        EditorGUILayout.PropertyField(requiredLayersProperty);
        LayerOrientationProperty(layerOrientationProperty);
        layerProperty.intValue = EditorGUILayout.LayerField(layerProperty.displayName, layerProperty.intValue);
        EditorGUILayout.PropertyField(buildCostsProperty, true);

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void ClassProperty(SerializedProperty classProperty) {
        string[] classes = Resources.Load<StructureClassSetting>("Structures/Classes").Classes;
        classProperty.intValue = EditorGUILayout.Popup(classProperty.displayName, classProperty.intValue, classes);
    }

    private void RequiredTechProperty(SerializedProperty requiredTechProperty) {
        EditorGUILayout.LabelField(requiredTechProperty.displayName);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        TechSetting[] techs = Resources.LoadAll<TechSetting>("Techs");
        Array.Sort(techs, (a, b) => a.ID.CompareTo(b.ID));
        mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, GUILayout.Height(Mathf.Min(10, techs.Length) * EditorGUI.GetPropertyHeight(requiredTechProperty)));
        for (int i = 0; i < techs.Length; i++) {
            bool isSelected = EditorGUILayout.ToggleLeft(techs[i].ID + ": " + techs[i].Name, requiredTechProperty.intValue == techs[i].ID);
            if (isSelected) {
                requiredTechProperty.intValue = techs[i].ID;
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }
    
    private void LayerOrientationProperty(SerializedProperty layerOrientationProperty) {
        int selection = 0;
        if (layerOrientationProperty.vector2Value == Vector2.down) {
            selection = 0;
        } else {
            selection = 1;
        }
        selection = EditorGUILayout.Popup(layerOrientationProperty.displayName, selection, new string[] { "Down", "Up" });
        if (selection == 0) {
            layerOrientationProperty.vector2Value = Vector2.down;
        } else {
            layerOrientationProperty.vector2Value = Vector2.up;
        }
    }
    
    private void InitializerProperty(SerializedProperty initializerProperty, SerializedProperty initializeValuesProperty) {
        EditorGUILayout.PropertyField(initializerProperty);
        if (initializerProperty.objectReferenceValue == null) {
            initializeValuesProperty.ClearArray();
            return;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        Type scriptType = ((MonoScript)initializerProperty.objectReferenceValue).GetClass();
        if (scriptType != typeof(Structure) && !scriptType.IsSubclassOf(typeof(Structure))) {
            EditorGUILayout.HelpBox(scriptType.Name + "不是一个有效的Structure类型。", MessageType.Error);
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
