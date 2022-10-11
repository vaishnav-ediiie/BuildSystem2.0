using UnityEngine;
using System;
# if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Only draws the field only if a condition is met. Supports enum, bools, int, floats, long, double.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfBoolAttribute : PropertyAttribute
{
    public string comparedBoolName;
    public bool comparedValue;

    public DrawIfBoolAttribute(string comparedBoolName, bool comparedValue)
    {
        this.comparedBoolName = comparedBoolName;
        this.comparedValue = comparedValue;
    }
    
}

# if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DrawIfBoolAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer
{
    DrawIfBoolAttribute drawIf;
    SerializedProperty comparedField;

    private bool ShowMe(SerializedProperty property)
    {
        if (drawIf == null) drawIf = attribute as DrawIfBoolAttribute;
        if (comparedField == null) comparedField = property.serializedObject.FindProperty(drawIf.comparedBoolName);
        if (comparedField == null)
        {
            Debug.LogWarning("Cannot find property with name: " + drawIf.comparedBoolName);
            return true;
        }

        return drawIf.comparedValue == comparedField.boolValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = ShowMe(property);
        EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
        GUI.enabled = true;
    }
}
# endif