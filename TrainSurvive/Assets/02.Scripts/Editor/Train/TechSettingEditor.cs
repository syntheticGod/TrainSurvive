/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/11 18:19:13
 * 版本：v0.1
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TechSetting))]
public class TechSettingEditor : Editor {

    private Vector2 mScrollPos;

    public override void OnInspectorGUI() {
        SerializedProperty idProperty = serializedObject.FindProperty("ID");
        SerializedProperty nameProperty = serializedObject.FindProperty("Name");
        SerializedProperty descriptionProperty = serializedObject.FindProperty("Description");
        SerializedProperty totalWorksProperty = serializedObject.FindProperty("TotalWorks");
        SerializedProperty dependenciesProperty = serializedObject.FindProperty("Dependencies");
        SerializedProperty onCompletedProperty = serializedObject.FindProperty("OnCompleted");

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(idProperty);
        EditorGUILayout.PropertyField(nameProperty);
        EditorGUILayout.PropertyField(descriptionProperty);
        EditorGUILayout.PropertyField(totalWorksProperty);
        
        EditorGUILayout.LabelField(dependenciesProperty.displayName);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        TechSetting[] techs = Resources.LoadAll<TechSetting>("Techs");
        Array.Sort(techs, (a, b) => a.ID.CompareTo(b.ID));
        mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, GUILayout.Height(Mathf.Min(10, techs.Length) * EditorGUI.GetPropertyHeight(idProperty)));
        for (int i = 0; i < techs.Length; i++) {
            if (techs[i].ID == idProperty.intValue)
                continue;
            int selected = -1;
            for (int j = 0; j < dependenciesProperty.arraySize; j++) {
                if (dependenciesProperty.GetArrayElementAtIndex(j).intValue == techs[i].ID) {
                    selected = j;
                    break;
                }
            }
            bool isSelected = EditorGUILayout.ToggleLeft(techs[i].ID + ": " +techs[i].Name, selected != -1);
            if (isSelected) {
                if (selected == -1) {
                    dependenciesProperty.arraySize++;
                    dependenciesProperty.GetArrayElementAtIndex(dependenciesProperty.arraySize - 1).intValue = techs[i].ID;
                }
            } else {
                if (selected != -1) {
                    dependenciesProperty.DeleteArrayElementAtIndex(selected);
                }
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(onCompletedProperty);

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
