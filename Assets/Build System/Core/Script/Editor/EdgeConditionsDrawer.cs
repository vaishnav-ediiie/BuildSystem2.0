using System.Collections.Generic;
using CustomBuildSystem;
using CustomBuildSystem.Placing.Conditional;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;


[CustomPropertyDrawer(typeof(EdgeCondition))]
public class EdgeConditionsDrawer : PropertyDrawer
{
    SerializedProperty name;
    SerializedProperty conditionType;
    SerializedProperty occupant;
    SerializedProperty invertCondition;
    SerializedProperty floorNumber;
    SerializedProperty isFloorRelative;
    SerializedProperty outputWhenFloorDontExist;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
        
        conditionType = property.FindPropertyRelative("type");
        occupant = property.FindPropertyRelative("occupants");
        if (conditionType.enumValueIndex >= 2) return EditorGUIUtility.singleLineHeight * 4f + EditorGUI.GetPropertyHeight(occupant);
        return EditorGUIUtility.singleLineHeight * 4f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        if (!property.isExpanded) return;

        name = property.FindPropertyRelative("name");
        invertCondition = property.FindPropertyRelative("invertCondition");
        conditionType = property.FindPropertyRelative("type");
        floorNumber = property.FindPropertyRelative("floorNumber");
        isFloorRelative = property.FindPropertyRelative("isFloorRelative");
        outputWhenFloorDontExist = property.FindPropertyRelative("outputWhenFloorDontExist");


        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, name, new GUIContent("Name"));
        
        rect.width = rect.width * 0.5f;
        rect.y += EditorGUIUtility.singleLineHeight;
        invertCondition.boolValue = EditorGUI.ToggleLeft(rect, "Invert Condition", invertCondition.boolValue);
        rect.x += rect.width;
        outputWhenFloorDontExist.boolValue = EditorGUI.ToggleLeft(rect, "When Floor Dont Exist", outputWhenFloorDontExist.boolValue);



        rect.x = position.x;
        rect.y += EditorGUIUtility.singleLineHeight;
        float width = (rect.width * 0.5f) - 100f;
        rect.width = width;
        EditorGUI.PropertyField(rect, conditionType, GUIContent.none);
        rect.x += rect.width + 8;
        rect.width = 60f;
        EditorGUI.LabelField(rect, "On Floor");
        rect.x += 50f;
        rect.width = width - 8;
        EditorGUI.PropertyField(rect, floorNumber, GUIContent.none);
        rect.x += rect.width + 3;
        rect.width = 150;
        isFloorRelative.boolValue = EditorGUI.ToggleLeft(rect, "Relative To Player", isFloorRelative.boolValue);
        
        if (conditionType.enumValueIndex >= 2)
        {
            occupant = property.FindPropertyRelative("occupants");
            rect.width = position.width;
            rect.x = position.x;
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, occupant, new GUIContent("Occupants"), true);
        }
        EditorGUI.EndProperty();
    }
}