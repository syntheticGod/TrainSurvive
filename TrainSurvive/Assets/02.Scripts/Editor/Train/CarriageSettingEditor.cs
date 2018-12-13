/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/12 16:49:32
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CarriageSetting))]
public class CarriageSettingEditor : Editor {

    private Vector2 mScrollPos;

    public override void OnInspectorGUI() {
        SerializedProperty idProperty = serializedObject.FindProperty("ID");
        SerializedProperty nameProperty = serializedObject.FindProperty("Name");
        SerializedProperty descriptionProperty = serializedObject.FindProperty("Description");
        SerializedProperty workAllProperty = serializedObject.FindProperty("WorkAll");
        SerializedProperty sizeProperty = serializedObject.FindProperty("Size");
        SerializedProperty prefabProperty = serializedObject.FindProperty("Prefab");
        SerializedProperty buildCostsProperty = serializedObject.FindProperty("BuildCosts");
        SerializedProperty requiredTechProperty = serializedObject.FindProperty("RequiredTech");

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(idProperty);
        EditorGUILayout.PropertyField(nameProperty);
        EditorGUILayout.PropertyField(descriptionProperty);
        EditorGUILayout.PropertyField(sizeProperty);
        EditorGUILayout.PropertyField(workAllProperty);
        EditorGUILayout.PropertyField(prefabProperty);
        RequiredTechProperty(requiredTechProperty);
        EditorGUILayout.PropertyField(buildCostsProperty, true);

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
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

}
