using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerate))]
public class MapModelCreate : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Create Map Model")) {
            MapGenerate map = target as MapGenerate;
            map.CreateModel();
        }
    }
}
