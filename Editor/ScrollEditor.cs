using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Scroll))]
public class ScrollEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Scroll scroll = (Scroll)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("scrollAlong"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("customScrollBounds"));

        if (scroll.customScrollBounds)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxScroll"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minScroll"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
