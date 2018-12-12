/*
 * 描述：
 * 作者：刘旭涛
 * 创建时间：2018/12/11 01:15:37
 * 版本：v0.1
 */
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableAction), true)]
public class SerializableActionDrawer : PropertyDrawer {
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label) * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        SerializedProperty classProperty = property.FindPropertyRelative("Class");
        SerializedProperty acceptTypesProperty = property.FindPropertyRelative("AcceptTypes");
        SerializedProperty methodCandidateNamesProperty = property.FindPropertyRelative("MethodCandidateNames");
        SerializedProperty candidateSelectionProperty = property.FindPropertyRelative("CandidateSelection");
        
        EditorGUI.LabelField(
            new Rect(position.x, position.y, position.width, position.height / 3),
            label
        );

        // target + method section
        EditorGUI.indentLevel++;
        EditorGUI.BeginChangeCheck(); // if target changes we need to repopulate the candidate method lists

        // select target
        EditorGUI.PropertyField(
            new Rect(position.x, position.y += position.height / 3, position.width, position.height / 3),
            classProperty
        );
        if (classProperty.objectReferenceValue == null) {
            methodCandidateNamesProperty.ClearArray();
            return; // null objects have no methods - don't continue
        }

        // polulate method candidate names
        string[] methodCandidateNames = RepopulateCandidateList(classProperty, acceptTypesProperty, methodCandidateNamesProperty, candidateSelectionProperty);

        // place holder when no candidates are available
        if (methodCandidateNames.Length == 0) {
            EditorGUI.LabelField(
                new Rect(position.x, position.y += position.height / 3, position.width, position.height / 3),
                "Method",
                "none"
            );
            return;
        }

        // select method from candidates
        candidateSelectionProperty.intValue = EditorGUI.Popup(
            new Rect(position.x, position.y += position.height / 3, position.width, position.height / 3),
            "Method",
            candidateSelectionProperty.intValue,
            methodCandidateNames
        );
        
        EditorGUI.indentLevel--;
    }

    public string[] RepopulateCandidateList(
             SerializedProperty classProperty,
             SerializedProperty acceptTypesProperty,
             SerializedProperty methodCandidateNamesProperty,
             SerializedProperty candidateSelectionProperty
     ) {
        System.Type type = ((MonoScript)classProperty.objectReferenceValue).GetClass();
        MethodInfo[] methodInfos = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);

        List<string> candidateNames = new List<string>();
        foreach(MethodInfo methodInfo in methodInfos) {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length != acceptTypesProperty.arraySize) {
                continue;
            }
            bool flag = true;
            StringBuilder stringBuilder = new StringBuilder(methodInfo.Name + "|");
            for (int i = 0; i < parameters.Length; i++) {
                if (parameters[i].ParameterType.FullName != acceptTypesProperty.GetArrayElementAtIndex(i).stringValue) {
                    flag = false;
                    break;
                }
                stringBuilder.Append(parameters[i].ParameterType.FullName + ",");
            }
            if (flag) {
                candidateNames.Add(stringBuilder.ToString().TrimEnd(','));
            }
        }
        // clear/resize/initialize storage containers
        methodCandidateNamesProperty.ClearArray();
        methodCandidateNamesProperty.arraySize = candidateNames.Count;

        // assign storage containers
        int index = 0;
        foreach (SerializedProperty element in methodCandidateNamesProperty) {
            element.stringValue = candidateNames[index];
            index++;
        }
        if (candidateSelectionProperty.intValue >= candidateNames.Count) {
            // reset popup index
            candidateSelectionProperty.intValue = 0;
        }

        return candidateNames.ToArray();
    }
}
