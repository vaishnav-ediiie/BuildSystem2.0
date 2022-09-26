using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace KSRecs.Deprecated.BuildSystemSpace.Editor
{
    public class BuildProbSOEditor : EditorWindow
    {
        protected SerializedObject serializedObject;

        [MenuItem("CustomEditors/BuildProbSO Editor")]
        public static void Open()
        {
            GetWindow<BuildProbSOEditor>("Game Data Editor");
        }


        public static void Open(BuildProbSO dataObject)
        {
            BuildProbSOEditor window = GetWindow<BuildProbSOEditor>("Game Data Editor");
            window.serializedObject = new SerializedObject(dataObject);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150f), GUILayout.ExpandHeight(true));
            DrawSidebar();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            if (serializedObject != null)
            {
                EditorGUILayout.LabelField("Showing: " + serializedObject.targetObject.name, EditorStyles.whiteLargeLabel);
                EditorGUILayout.Space(10f);
                DrawObject(serializedObject);
            }
            else
            {
                EditorGUILayout.LabelField("No Item Selected");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawObject(SerializedObject serializedObject)
        {
            SerializedProperty itter = serializedObject.GetIterator();
            itter.Next(true);
            while (itter.Next(false))
            {
                DrawProperty(itter, true);
            }
        }

        protected void DrawProperty(SerializedProperty prop, bool drawChildren)
        {
            if (prop == null)
            {
                Debug.LogWarning("Property not found!!!");
                return;
            }

            EditorGUILayout.PropertyField(prop, drawChildren);
            string lastPropPath = prop.propertyPath;
            foreach (SerializedProperty p in prop)
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath))
                {
                    continue;
                }

                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }

        // Returns selected asset name
        protected void DrawSidebar()
        {
            var guids = AssetDatabase.FindAssets("t:BuildProbSO");
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                BuildProbSO goo = AssetDatabase.LoadAssetAtPath<BuildProbSO>(path);
                if (GUILayout.Button(goo.name))
                {
                    serializedObject = new SerializedObject(goo);
                }
            }
        }
    }


    [CustomEditor(typeof(BuildProbSO))]
    public class BuildProbSOCustomEditor : UnityEditor.Editor
    {
        bool showDefaultInspector = false;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                BuildProbSOEditor.Open((BuildProbSO)target);
            }

            showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
            if (showDefaultInspector)
            {
                EditorGUILayout.Space(20f);
                base.OnInspectorGUI();
            }
        }

        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            BuildProbSO obje = EditorUtility.InstanceIDToObject(instanceID) as BuildProbSO;
            if (obje != null)
            {
                BuildProbSOEditor.Open(obje);
                return true;
            }

            return false;
        }
    }
}