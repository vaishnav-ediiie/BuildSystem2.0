using System;
using UnityEngine;
using UnityEditor;
using KSRecs.Editor.ResourceWindowDrawModes;
using Object = UnityEngine.Object;


namespace KSRecs.Editor
{
    public class ResourcesWindow : EditorWindow
    {
        #region Statics

        private static string DataPathGuids => Application.persistentDataPath + "\\Guids.dat";
        private static string DataPathColors => Application.persistentDataPath + "\\GuidColors.dat";

        internal static readonly Color[] BackgroundColorsOptions = new Color[]
        {
            new Color(0.322f, 0.188f, 0.188f),
            new Color(0.322f, 0.231f, 0.188f),
            new Color(0.263f, 0.322f, 0.188f),
            new Color(0.219f, 0.321f, 0.188f),
            new Color(0.200f, 0.322f, 0.188f),
            new Color(0.188f, 0.322f, 0.220f),
            new Color(0.188f, 0.322f, 0.286f),
            new Color(0.188f, 0.290f, 0.322f),
            new Color(0.188f, 0.231f, 0.322f),
            new Color(0.239f, 0.188f, 0.322f),
            new Color(0.298f, 0.188f, 0.322f),
            new Color(0.322f, 0.188f, 0.278f)
        };

        #endregion

        #region Properties

        internal GUILayoutOption DropObjectsWidth => GUILayout.Width(this.position.width * 0.3f);
        internal GUILayoutOption MiniButtonWidth => GUILayout.Width(20f);
        internal GUILayoutOption MegaButtonWidth => GUILayout.Width(this.position.width - 120);
        private Vector2 scrollPos;
        #endregion

        #region Variables

        internal ResourcesStorage allResourcesData;
        private RecWinDrawMod DrawMode;
        private float guiAlpha = 0.8f;
        public GUIStyle guiBackStyle;

        #endregion

        #region Unity Callbacks

        [MenuItem("Utils/Resources", priority = 1)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            GetWindow(typeof(ResourcesWindow), false, "Resources").Show();
        }

        private void OnEnable()
        {
            guiBackStyle = new GUIStyle();
            guiBackStyle.normal.background = EditorGUIUtility.whiteTexture;
            guiBackStyle.stretchWidth = true;
            guiBackStyle.margin = new RectOffset(0, 0, 7, 7);
            LoadData();
            if (allResourcesData == null)
            {
                allResourcesData = new ResourcesStorage();
            }
            SwitchToList();
        }

        private void OnGUI()
        {
            try
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width));
                if (DrawMode == null) SwitchToList();
                DrawMode.Draw();
                GUILayout.EndScrollView();
                if (allResourcesData.isLayeredView)
                {
                    if (GUILayout.Button("Switch To Simple View"))
                    {
                        allResourcesData.isLayeredView = false;
                        SaveData();
                        SwitchDrawMode(new ModeSimpleResources(this));
                    }    
                }
                else
                {
                    if (GUILayout.Button("Switch To Layered View"))
                    {
                        allResourcesData.isLayeredView = true;
                        SaveData();
                        SwitchDrawMode(new ModeLayeredResources(this));
                    }
                }
                
            }
            catch (Exception e)
            {
                EditorGUILayout.HelpBox($"Cannot load editor window because of the error:\n{e.Message}",
                    MessageType.Error);
                EditorGUILayout.LabelField($"Following button will fix it but it will erase all the saved data");
                if (GUILayout.Button("Clear All Data"))
                { 
                    if (allResourcesData == null)
                    {
                        allResourcesData = new ResourcesStorage();
                    }
                    else
                    {
                        allResourcesData.Clear();
                    }

                    SaveData();
                }
            }
        }

        #endregion

        #region Save And Load

        internal void SaveData()
        {
            allResourcesData.SaveTo(DataPathGuids);
        }

        internal void LoadData()
        {
            allResourcesData = ResourcesStorage.LoadFrom(DataPathGuids);
        }

        #endregion

        #region DrawModes

        public void SwitchDrawMode(RecWinDrawMod newMode)
        {
            this.DrawMode = newMode;
        }

        public void SwitchToList()
        {
            if (allResourcesData.isLayeredView) SwitchDrawMode(new ModeLayeredResources(this));
            else SwitchDrawMode(new ModeSimpleResources(this));
        }
        
        public void StylizeGUI(int objCount, int offset, Color color)
        {
            Color restCol = GUI.color;
            GUI.color = color;
            Rect backPos = GUILayoutUtility.GetRect(GUIContent.none, guiBackStyle, GUILayout.Height(0));
            backPos.height = objCount * 19 + offset;
            GUI.DrawTexture(backPos, EditorGUIUtility.whiteTexture);
            restCol.a = guiAlpha;
            GUI.color = restCol;
        }

        private bool DrawColorButton(int i)
        {
            GUI.color = BackgroundColorsOptions[i];
            Rect backPos = GUILayoutUtility.GetRect(GUIContent.none, guiBackStyle, GUILayout.Height(19));
            backPos.width -= 6;
            backPos.center = new Vector2(backPos.center.x + 3, backPos.center.y);
            bool tr = GUI.Button(backPos, "");
            GUI.DrawTexture(backPos, EditorGUIUtility.whiteTexture);
            return tr;
        }
        
        public int DrawColorsPicker(int index)
        {
            Color normCol = GUI.color;


            // Selection            
            GUI.color = BackgroundColorsOptions[index];
            Rect backPos = GUILayoutUtility.GetRect(GUIContent.none, guiBackStyle, GUILayout.Height(57));
            GUI.DrawTexture(backPos, EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(backPos, "SELECTION");
            GUI.color = BackgroundColorsOptions[index];


            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 4; i++)
                {
                    if (DrawColorButton(i)) return i;
                }

                EditorGUILayout.EndHorizontal();
            }

            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 4; i < 8; i++)
                {
                    if (DrawColorButton(i)) return i;
                }

                EditorGUILayout.EndHorizontal();
            }

            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 8; i < 12; i++)
                {
                    if (DrawColorButton(i)) return i;
                }

                EditorGUILayout.EndHorizontal();
            }
            GUI.color = normCol;
            return index;
        }

        #endregion

        #region Publics
        public void AddObjectToLayer(string layerName, Object selectedObject)
        {
            if (allResourcesData.ContainsObjectInLayer(layerName, selectedObject))
            {
                return;
            }

            // string displayName = selectedObject.name;
            // if (displayName.Length > 18) displayName = displayName.Substring(0, 15) + "...";
            allResourcesData.AddTo(layerName, new ObjectInfo(selectedObject.name, selectedObject, layerName));
            SaveData();
        }
        #endregion
    }
}