using System.Collections.Generic;
using CustomBuildSystem;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Condition))]
public class CellConditionsDrawer : PropertyDrawer
{
    protected static Dictionary<int, Color> ConditionalColors = new Dictionary<int, Color>()
    {
        { 0, new Color(0.8f, 0.8f, 0.8f) }, // ConditionType.DontCare
        { 1, new Color(0.5f, 1f, 0.5f) }, // ConditionType.MustBeEmpty
        { 2, new Color(1f, 0.5f, 0.5f) }, // ConditionType.OccupiedByAny
        { 3, new Color(1f, 0.5f, 1f) } // ConditionType.OccupiedBySpecific
    };

    SerializedProperty conditionType;
    SerializedProperty occupant;


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (conditionType == null) conditionType = property.FindPropertyRelative("conditionType");
        if (occupant == null) occupant = property.FindPropertyRelative("occupant");

        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect rect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        Color def = GUI.color;
        GUI.color = ConditionalColors[conditionType.enumValueIndex];
        GUI.Label(rect, label);
        GUI.color = def;
        rect.x += rect.width;
        rect.width = position.width - EditorGUIUtility.labelWidth;

        if (conditionType.enumValueIndex == 3)
        {
            rect.width *= 0.5f;
            EditorGUI.PropertyField(rect, conditionType, GUIContent.none);
            rect.x += rect.width;
            if (occupant.objectReferenceValue == null) GUI.color = Color.red;
            EditorGUI.PropertyField(rect, occupant, GUIContent.none);
            GUI.color = def;
        }
        else
        {
            EditorGUI.PropertyField(rect, conditionType, GUIContent.none);
        }
    }
}