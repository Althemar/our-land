using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Reward))]
public class RewardDrawer : PropertyDrawer
{
    private float fieldHeight = EditorGUIUtility.singleLineHeight;
    private float padding = 2;
    private float fieldCount = 3;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(position, label);


        SerializedProperty type = property.FindPropertyRelative("rewardType");

        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;
        position.height = fieldHeight;

        EditorGUI.PropertyField(position, type, GUIContent.none);

       
        RewardType rewardType = (RewardType)type.intValue;

        Rect rectType = new Rect(position);
        position.height = fieldHeight;
        position.y += fieldHeight + padding;
        if (rewardType != RewardType.NewPopPoint) {
            if (rewardType == RewardType.Resource) {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("resource"), GUIContent.none);
            }
            else {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("bonus"), GUIContent.none);
            }
            position.y += fieldHeight + padding;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("amount"), GUIContent.none);
        }
        

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return fieldHeight * fieldCount + fieldCount * padding;
    }

}
