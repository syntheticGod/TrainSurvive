using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapModelCreate : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Create Map Model")) {
            Map map = target as Map;
            map.createModel();
        }
    }
}
