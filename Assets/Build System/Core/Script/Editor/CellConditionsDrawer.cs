using System.Collections.Generic;
using CustomBuildSystem;
using CustomBuildSystem.Placing.Conditional;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(CellCondition))]
public class CellConditionsDrawer : PropertyDrawer
{
    private float lineGap = 5;
    SerializedProperty name;
    SerializedProperty conditionType;
    SerializedProperty occupant;
    SerializedProperty invertCondition;
    SerializedProperty floorNumber;
    SerializedProperty isFloorRelative;
    SerializedProperty place;
    SerializedProperty outputWhenFloorDontExist;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
        
        
        conditionType = property.FindPropertyRelative("type");
        occupant = property.FindPropertyRelative("occupants");
        if (conditionType.enumValueIndex >= 2) return EditorGUIUtility.singleLineHeight * 5f + EditorGUI.GetPropertyHeight(occupant) + 20f;
        return EditorGUIUtility.singleLineHeight * 5f + 20f;
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
        place = property.FindPropertyRelative("place");
        outputWhenFloorDontExist = property.FindPropertyRelative("outputWhenFloorDontExist");

        
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, name, new GUIContent("Name"));
        
        rect.width = rect.width * 0.5f;
        rect.y += EditorGUIUtility.singleLineHeight + lineGap;
        invertCondition.boolValue = EditorGUI.ToggleLeft(rect, "Invert Condition", invertCondition.boolValue);
        rect.x += rect.width;
        outputWhenFloorDontExist.boolValue = EditorGUI.ToggleLeft(rect, "When Floor Dont Exist", outputWhenFloorDontExist.boolValue);
        
        rect.x = position.x;
        rect.y += EditorGUIUtility.singleLineHeight + lineGap;
        float width = position.width * 0.5f - 28f;
        rect.width = width;
        EditorGUI.PropertyField(rect, conditionType, GUIContent.none);

        rect.x += rect.width + 10f;
        rect.width = 50f;
        EditorGUI.LabelField(rect, "At place");
        rect.x += 50f;
        rect.width = width;
        EditorGUI.PropertyField(rect, place, GUIContent.none);


        rect.y += EditorGUIUtility.singleLineHeight + lineGap;
        rect.x = position.x;
        rect.width = 50f;
        EditorGUI.LabelField(rect, "On Floor");
        rect.x += 50f;
        rect.width = (position.width - 50f) * 0.5f;
        EditorGUI.PropertyField(rect, floorNumber, GUIContent.none);
        rect.x += rect.width + 3;
        isFloorRelative.boolValue = EditorGUI.ToggleLeft(rect, "Relative To Player", isFloorRelative.boolValue);

        if (conditionType.enumValueIndex >= 2)
        {
            occupant = property.FindPropertyRelative("occupants");
            rect.width = position.width - 20f;
            rect.x = position.x + 20f;
            rect.y += EditorGUIUtility.singleLineHeight + lineGap;
            EditorGUI.PropertyField(rect, occupant, new GUIContent("Occupants"), true);
        }

        EditorGUI.EndProperty();
    }
}