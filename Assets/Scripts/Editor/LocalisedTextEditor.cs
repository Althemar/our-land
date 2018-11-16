using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (LocalisedText))]
public class LocalisedTextEditor : Editor {
    public LocalisedText text {
        get { return target as LocalisedText; }
    }

    SerializedProperty keyProp;

    public void OnEnable () {
        keyProp = serializedObject.FindProperty ("key");

        ConnectToSheet.TextUpdated += TextUpdated;
    }

    public void TextUpdated() {
        text.UpdateText();
    }

    public override void OnInspectorGUI () {
        serializedObject.Update ();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField (keyProp);
        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
            text.UpdateText();
        }

        serializedObject.ApplyModifiedProperties();
    }
}