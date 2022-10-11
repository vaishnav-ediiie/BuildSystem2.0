using UnityEngine;
using UnityEditor;
using CustomGridSystem;

//
// [CustomPropertyDrawer(typeof(CellNumber))]
// public class CellNumberDrawer : PropertyDrawer
// {
//     private static float LINE_HEIGHT = 18f;
//     SerializedProperty row;
//     SerializedProperty column;
//
//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         return LINE_HEIGHT;
//     }
//
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         row = property.FindPropertyRelative("row");
//         column = property.FindPropertyRelative("column");
//
//         EditorGUI.BeginProperty(position, label, property);
//         float oneWidth = position.width / 10f;
//         Rect rect = new Rect(position.x, position.y, oneWidth, position.height);
//         EditorGUI.LabelField(rect, label);
//         rect.x += oneWidth * 2f;
//         EditorGUI.PropertyField(rect,row);
//         rect.x += oneWidth + 3f;
//         EditorGUI.PropertyField(rect,column);
//         EditorGUI.EndProperty();
//     }
// }

[CustomPropertyDrawer(typeof(CellNumber))]
public class CellNumberDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float width = (position.width - EditorGUIUtility.labelWidth - 50f) / 2f ;
        Rect rect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);

        EditorGUI.LabelField(rect, label);
        
        rect.x += rect.width;
        rect.width = 30f;
        EditorGUI.PrefixLabel(rect, new GUIContent("Row"));
        rect.x += 30f;
        rect.width = width-10f;
        EditorGUI.PropertyField(rect, FindPropertyChild(property, "row"), GUIContent.none);
        
        rect.x += width;
        rect.width = 30f;
        EditorGUI.PrefixLabel(rect, new GUIContent("Col"));
        rect.x += 30f;
        rect.width = width-10f;
        EditorGUI.PropertyField(rect, FindPropertyChild(property, "column"), GUIContent.none);
        EditorGUI.EndProperty();   
    }
    
    public static SerializedProperty FindPropertyChild(SerializedProperty property, string childName)
    {
        string parentPath = property.propertyPath;
        SerializedProperty iterator = property.Copy();
        while (iterator.Next(true))
        {
            if (iterator.name == childName && iterator.propertyPath.Contains(parentPath))
            {
                return iterator;
            }
        }

        return null;
    }
}