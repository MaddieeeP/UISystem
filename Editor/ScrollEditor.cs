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

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_scrollAlong"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_scrollSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_keepOffsetOnInitialize"));

        if (!scroll.keepOffsetOnInitialize)
        {
            if (scroll.scrollAlong == Dimension.x)
            {
                XAlign align = (XAlign)scroll.initializeAlign;
                align = (XAlign)EditorGUILayout.EnumPopup("Initialize Align", align);
                scroll.initializeAlign = (int)align;
            }
            else if (scroll.scrollAlong == Dimension.y)
            {
                YAlign align = (YAlign)scroll.initializeAlign;
                align = (YAlign)EditorGUILayout.EnumPopup("Initialize Align", align);
                scroll.initializeAlign = (int)align;
            }
            else if (scroll.scrollAlong == Dimension.z)
            {
                ZAlign align = (ZAlign)scroll.initializeAlign;
                align = (ZAlign)EditorGUILayout.EnumPopup("Initialize Align", align);
                scroll.initializeAlign = (int)align;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
