using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace KSRecs.Editor.ResourceWindowDrawModes
{
    public abstract class RecWinDrawMod
    {
        private static readonly float LINE_HEIGHT = 20;
        private static GUIStyle _buttonStyle;
        protected ResourcesWindow Window;

        public abstract void Draw();
        
        protected bool DrawSingleResource(ObjectInfo resource, float spaceBefore, float spaceAfter)
        {
            if (resource.ReferenceObject == null) return true;

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
                _buttonStyle.alignment = TextAnchor.MiddleLeft;
            }
            
            
            EditorGUILayout.Space(5);

            Rect posRect = GUILayoutUtility.GetRect(Window.position.width, LINE_HEIGHT - 4);
            posRect.x += spaceBefore;
            posRect.height += 3;
            posRect.width -= LINE_HEIGHT * 3 + 30 + spaceAfter;
            if (GUI.Button(posRect, resource.DisplayName, _buttonStyle))
            {
                if (resource.ReferenceObject.GetType().IsAssignableFrom(typeof(UnityEditor.SceneAsset)))
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(resource.ReferenceObject));
                }
                else
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(resource.ReferenceObject.GetFirstAsset());
                }
            }

            posRect.width = LINE_HEIGHT;
            posRect.x += 6;
            GUI.Label(posRect, resource.Icon);
            posRect.x += Window.position.width - LINE_HEIGHT * 3 - 30 - spaceAfter;

            if (GUI.Button(posRect, "*"))
            {
                Window.SwitchDrawMode(new ModeRenameResource(resource, Window));
                return false;
            }

            posRect.x += LINE_HEIGHT + 4;
            if (GUI.Button(posRect, "X")) return true;
            posRect.x += LINE_HEIGHT + 4;
            Object toMove = EditorGUI.ObjectField(posRect, null, typeof(Object), false);
            if (toMove) toMove.MoveAssetTo(resource.ReferenceObject);
            return false;
        }
    }

    public class ModeLayeredResources : RecWinDrawMod
    {
        public ModeLayeredResources(ResourcesWindow window)
        {
            this.Window = window;
        }

        (ObjectInfo, bool) DrawLayerResources(LayerInfo layerInfo)
        {
            int objCount = layerInfo.allObjects.Count;
            if (layerInfo.isExpanded) Window.StylizeGUI(objCount + 2, 22, layerInfo.Color());
            else Window.StylizeGUI(2, 0, layerInfo.Color());

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            layerInfo.isExpanded = EditorGUILayout.Foldout(layerInfo.isExpanded, $"{layerInfo.name} ({objCount})");
            if (EditorGUI.EndChangeCheck())
            {
                Window.SaveData();
            }

            Object objectToAdd = EditorGUILayout.ObjectField(null, typeof(Object), false, Window.DropObjectsWidth);
            if (GUILayout.Button("*", Window.MiniButtonWidth))
            {
                EditorGUILayout.EndHorizontal();
                Window.SwitchDrawMode(new ModeRenameLayer(Window, layerInfo));
                return (null, false);
            }

            if (GUILayout.Button("X", Window.MiniButtonWidth))
            {
                EditorGUILayout.EndHorizontal();
                return (null, true);
            }

            EditorGUILayout.LabelField("", Window.MiniButtonWidth);
            EditorGUILayout.EndHorizontal();

            if (objectToAdd != null)
            {
                Window.AddObjectToLayer(layerInfo.name, objectToAdd);
            }

            if (!layerInfo.isExpanded)
            {
                EditorGUILayout.Space(10);
                return (null, false);
            }

            GUILayout.Space(9);
            Color color = GUI.backgroundColor;
            color.a = 0.5f;
            GUI.backgroundColor = color;

            foreach (ObjectInfo objectInfo in Window.allResourcesData.LoopLayer(layerInfo.name))
            {
                if (DrawSingleResource(objectInfo, 30, 40))
                {
                    return (objectInfo, false);
                }
            }

            EditorGUILayout.Space(20);
            return (null, false);
        }

        public override void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (Window.allResourcesData.LayerCount != 0 && GUILayout.Button("Clear All", Window.MegaButtonWidth))
            {
                Window.allResourcesData.Clear();
                Window.SaveData();
                return;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            foreach (LayerInfo layer in Window.allResourcesData.LoopLayers())
            {
                (ObjectInfo, bool) variable = DrawLayerResources(layer);
                if (variable.Item1 != null)
                {
                    Window.allResourcesData.RemoveFrom(layer.name, variable.Item1);
                    Window.SaveData();
                    break;
                }

                if (variable.Item2)
                {
                    Window.allResourcesData.RemoveLayer(layer.name);
                    Window.SaveData();
                    break;
                }
            }

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Section", Window.MegaButtonWidth))
            {
                Window.SwitchDrawMode(new ModeAddLayer(Window));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    public class ModeSimpleResources : RecWinDrawMod
    {
        public ModeSimpleResources(ResourcesWindow window)
        {
            Window = window;
        }

        public override void Draw()
        {
            if (Window.allResourcesData.LayerCount != 0 && GUILayout.Button("Clear All"))
            {
                Window.allResourcesData.Clear();
                Window.SaveData();
                return;
            }

            Object objectToAdd = EditorGUILayout.ObjectField(null, typeof(Object), false);

            if (objectToAdd != null)
            {
                string layerName = Window.allResourcesData.GetFirstLayerName();
                Window.AddObjectToLayer(layerName, objectToAdd);
                return;
            }

            foreach (LayerInfo layerInfo in Window.allResourcesData.LoopLayers())
            {
                foreach (ObjectInfo objectInfo in Window.allResourcesData.LoopLayer(layerInfo.name))
                {
                    bool deleteObject = DrawSingleResource(objectInfo, 10, 20);
                    if (deleteObject)
                    {
                        Window.allResourcesData.RemoveFrom(layerInfo.name, objectInfo);
                        Window.SaveData();
                        break;
                    }
                }
            }
        }
    }

    public class ModeRenameLayer : RecWinDrawMod
    {
        private string _curSelSecName;
        private int _curSelColorIndex = 0;
        private LayerInfo _layerInfo;

        public ModeRenameLayer(ResourcesWindow window, LayerInfo layerInfo)
        {
            this._layerInfo = layerInfo;
            this._curSelColorIndex = layerInfo.colorIndex;
            this._curSelSecName = layerInfo.name;
            this.Window = window;
        }

        public override void Draw()
        {
            GUILayout.Space(20);
            _curSelSecName = EditorGUILayout.TextField("New Name", _curSelSecName);
            _curSelColorIndex = Window.DrawColorsPicker(_curSelColorIndex);
            EditorGUILayout.LabelField($"Index: {_curSelColorIndex}");
            bool notAlowed = Window.allResourcesData.ContainsLayer(_curSelSecName) && _curSelColorIndex == _layerInfo.colorIndex;
            if (notAlowed)
            {
                EditorGUILayout.HelpBox($"Change either color or name to confirm", MessageType.Error);
                GUI.enabled = false;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                Debug.Log("Confirm - 1");
                if (_layerInfo.name != _curSelSecName)
                {
                    Debug.Log("Confirm - 2");
                    Window.allResourcesData.RenameLayer(_layerInfo.name, _curSelSecName);
                    Debug.Log("Confirm - 3");
                }

                Debug.Log("Confirm - 4");
                _layerInfo.colorIndex = _curSelColorIndex;
                Debug.Log("Confirm - 5");

                Window.SaveData();
                Window.SwitchToList();
            }

            GUI.enabled = true;
            if (GUILayout.Button("Cancel"))
            {
                Window.SwitchToList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public class ModeAddLayer : RecWinDrawMod
    {
        private string _curSelSecName;
        private int _curSelColorIndex = 0;

        public ModeAddLayer(ResourcesWindow window)
        {
            Window = window;
            _curSelSecName = "";
        }


        public override void Draw()
        {
            GUILayout.Space(20);
            _curSelSecName = EditorGUILayout.TextField("Name", _curSelSecName);
            _curSelColorIndex = Window.DrawColorsPicker(_curSelColorIndex);
            bool notAlowed = string.IsNullOrEmpty(_curSelSecName) || Window.allResourcesData.ContainsLayer(_curSelSecName);

            if (notAlowed)
            {
                EditorGUILayout.HelpBox($"Duplicate or Null layer names are not allowed", MessageType.Error);
                GUI.enabled = false;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm") && !notAlowed)
            {
                Window.allResourcesData.AddLayer(new LayerInfo(_curSelSecName, _curSelColorIndex));
                Window.SaveData();
                Window.SwitchToList();
            }

            GUI.enabled = true;
            if (GUILayout.Button("Cancel"))
            {
                Window.SwitchToList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public class ModeRenameResource : RecWinDrawMod
    {
        private ObjectInfo _target;
        private string _currentName;
        private string[] _allLayers;
        private int _selectedLayer;

        public ModeRenameResource(ObjectInfo target, ResourcesWindow window)
        {
            this._target = target;
            this.Window = window;
            this._target.StripDisplayName();
            this._currentName = target.DisplayName;
            this._allLayers = new string[Window.allResourcesData._allLayers.Count];
            int i = 0;
            foreach (KeyValuePair<string, LayerInfo> layerInfo in Window.allResourcesData._allLayers)
            {
                _allLayers[i] = layerInfo.Key;
                i++;
            }

            _selectedLayer = Array.IndexOf(this._allLayers, target.LayerName);
        }

        public override void Draw()
        {
            _currentName = EditorGUILayout.TextField("New Name", _currentName);
            _selectedLayer = EditorGUILayout.Popup(_selectedLayer, _allLayers);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                _target.RenameTo(_currentName);

                if (_selectedLayer >= 0 && _selectedLayer < _allLayers.Length)
                {
                    Window.allResourcesData.MoveResourceLayer(_target, _allLayers[_selectedLayer]);
                }

                Window.SaveData();
                Window.SwitchToList();
            }

            if (GUILayout.Button("Cancel"))
            {
                _target.RenameTo(_target.DisplayName);
                Window.SwitchToList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
    
    
    
     public static class UnityObjectEditorExtensions
    {
        /// <summary>
        /// Editor Functions--
        /// Moves the asset inside target if its a folder, else same folder as target asset.
        /// </summary>
        /// <param name="toMove">Asset to move</param>
        /// <param name="target">Target Directory or Asset</param>
        public static void MoveAssetTo(this Object toMove, Object target)
        {
            FileInfo resourcePath = new FileInfo(AssetDatabase.GetAssetPath(target.GetFirstAsset()));

            FileInfo s = new FileInfo(AssetDatabase.GetAssetPath(toMove));
            FileInfo sM = new FileInfo(s.FullName + ".meta");
            FileInfo t = new FileInfo(resourcePath.DirectoryName + "/" + s.Name);

            if (s.DirectoryName == t.DirectoryName)
            {
                return;
            }

            UnityEditor.FileUtil.MoveFileOrDirectory(s.FullName, t.FullName);
            if (sM.Exists)
            {
                UnityEditor.FileUtil.MoveFileOrDirectory(sM.FullName, t.FullName + ".meta");
            }

            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Returns first asset in resource if its a folder, else resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static Object GetFirstAsset(this Object resource)
        {
            string path = AssetDatabase.GetAssetPath(resource);
            if (!Directory.Exists(path)) return resource;

            foreach (string subPath in Directory.GetDirectories(path))
            {
                if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(subPath)))
                    return AssetDatabase.LoadAssetAtPath<Object>(subPath);
            }

            foreach (string subPath in Directory.GetFiles(path))
            {
                if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(subPath)))
                    return AssetDatabase.LoadAssetAtPath<Object>(subPath);
            }

            return resource;
        }

        public static Texture GetIcon(this Object resource)
        {
            Type type = resource.GetType();
            
            if (Directory.Exists(AssetDatabase.GetAssetPath(resource))) return EditorGUIUtility.IconContent("Folder Icon").image;
            
            if (typeof(GameObject).IsAssignableFrom(type)) return PrefabUtility.GetIconForGameObject((GameObject) resource);
            if (typeof(ScriptableObject).IsAssignableFrom(type)) return EditorGUIUtility.IconContent("ScriptableObject Icon").image;
            
            if (type == typeof(SceneAsset))         return EditorGUIUtility.IconContent("UnityLogo").image;
            if (type == typeof(TextAsset))          return EditorGUIUtility.IconContent("TextAsset Icon").image;
            if (type == typeof(MonoScript))         return EditorGUIUtility.IconContent("cs Script Icon").image;
            if (type == typeof(AnimatorController)) return EditorGUIUtility.IconContent("AnimatorController Icon").image;
            if (type == typeof(LightingDataAsset))  return EditorGUIUtility.IconContent("SceneviewLighting").image;
            if (type == typeof(Cubemap))            return EditorGUIUtility.IconContent("PreMatCube").image;
            if (type == typeof(Shader))             return EditorGUIUtility.IconContent("Shader Icon").image;
            if (type == typeof(Texture2D))          return EditorGUIUtility.IconContent("PreTextureRGB").image;
            return EditorGUIUtility.IconContent("").image;
        }

        public static string GetGuid(this Object resource)
        {
            return AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(resource)).ToString();
        }
        
    }
}