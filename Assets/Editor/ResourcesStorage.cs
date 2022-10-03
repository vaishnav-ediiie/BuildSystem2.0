using System;
using System.Collections.Generic;
using System.IO;
using KSRecs.Editor.ResourceWindowDrawModes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace KSRecs.Editor
{
    public class ResourcesStorage
    {
        public Dictionary<string, LayerInfo> _allLayers;
        public bool isLayeredView = true;

        public int LayerCount => _allLayers.Count;

        public ResourcesStorage()
        {
            _allLayers = new Dictionary<string, LayerInfo>();
        }

        private ResourcesStorage(RSSer rsSer)
        {
            this.isLayeredView = rsSer.isLayeredView;
            _allLayers = new Dictionary<string, LayerInfo>();
            foreach (LISer layer in rsSer.layers)
            {
                LayerInfo ly = new LayerInfo(layer);
                _allLayers.Add(ly.name, ly);
            }
        }

        public LayerInfo this[string name] => _allLayers[name];

        public IEnumerable<LayerInfo> LoopLayers()
        {
            foreach (LayerInfo layer in _allLayers.Values)
            {
                yield return layer;
            }

            yield break;
        }

        public IEnumerable<ObjectInfo> LoopLayer(string layerName)
        {
            if (!ContainsLayer(layerName)) yield break;

            foreach (ObjectInfo objInf in this[layerName].allObjects)
            {
                yield return objInf;
            }

            yield break;
        }

        public bool SaveTo(string filename)
        {
            try
            {
                string json = JsonUtility.ToJson(new RSSer(this));
                File.WriteAllText(filename, json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Cannot save Resources Data: {e}");
                return false;
            }
        }

        public static ResourcesStorage LoadFrom(string filename)
        {
            RSSer s = JsonUtility.FromJson<RSSer>(File.ReadAllText(filename));
            return new ResourcesStorage(s);
        }

        public void RenameLayer(string oldName, string newName)
        {
            if (!ContainsLayer(oldName)) return;
            if (ContainsLayer(newName)) return;
            var layer = this._allLayers[oldName];
            layer.name = newName;
            this._allLayers.Add(newName, layer);
            this._allLayers.Remove(oldName);
        }

        public bool ContainsLayer(string layerName)
        {
            return !string.IsNullOrEmpty(layerName) && _allLayers.ContainsKey(layerName);
        }

        public bool ContainsObjectInLayer(string layerName, Object obje)
        {
            if (!ContainsLayer(layerName)) return false;

            foreach (ObjectInfo objectInfo in this[layerName].allObjects)
            {
                if (objectInfo.ReferenceObject.Equals(obje)) return true;
            }
             

            return false;
        }

        public void AddTo(string layerName, ObjectInfo obj)
        {
            AddLayer(layerName).allObjects.Add(obj);
        }

        public void Clear()
        {
            _allLayers.Clear();
        }

        public void RemoveFrom(string layerName, ObjectInfo obj)
        {
            LayerInfo layerInfo = this[layerName];
            if (layerInfo == null) return;

            if (layerInfo.allObjects.Contains(obj))
            {
                layerInfo.allObjects.Remove(obj);
            }
        }

        public void RemoveLayer(string layerName)
        {
            LayerInfo layerInfo = this[layerName];

            if (layerInfo != null)
            {
                _allLayers.Remove(layerInfo.name);
            }
        }

        public LayerInfo AddLayer(string name)
        {
            if (_allLayers.ContainsKey(name)) return _allLayers[name];
            LayerInfo info = new LayerInfo(name, UnityEngine.Random.Range(0, ResourcesWindow.BackgroundColorsOptions.Length));
            this._allLayers.Add(info.name, info);
            return info;
        }

        public string GetFirstLayerName()
        {
            foreach (KeyValuePair<string,LayerInfo> keyValuePair in _allLayers)
            {
                return keyValuePair.Key;
            }

            AddLayer("Default");
            Debug.Log($"Added Default Layer");
            return "Default";
        }

        public void AddLayer(LayerInfo info)
        {
            if (ContainsLayer(info.name))
            {
                return;
            }

            this._allLayers.Add(info.name, info);
        }

        public void MoveResourceLayer(ObjectInfo target, string newLayerName)
        {
            
            if (target.LayerName == newLayerName) return;
            
            string oldLayer = target.LayerName;
            if (!string.IsNullOrEmpty(oldLayer) && _allLayers.ContainsKey(oldLayer) && _allLayers[oldLayer].allObjects.Contains(target))
            {
                _allLayers[oldLayer].allObjects.Remove(target);
            }

            if (!_allLayers.ContainsKey(newLayerName))
            {
                _allLayers.Add(newLayerName, new LayerInfo(newLayerName, UnityEngine.Random.Range(0, ResourcesWindow.BackgroundColorsOptions.Length)));
            }
            _allLayers[newLayerName].allObjects.Add(target);
            target.LayerName = newLayerName;
        }
    }


    [Serializable]
    public class RSSer
    {
        public LISer[] layers;
        public bool isLayeredView;


        public RSSer(ResourcesStorage storage)
        {
            this.isLayeredView = storage.isLayeredView;
            layers = new LISer[storage.LayerCount];
            int i = 0;
            foreach (LayerInfo layerInfo in storage._allLayers.Values)
            {
                layers[i] = new LISer(layerInfo);
                i++;
            }
        }
    }

    [Serializable]
    public class LISer
    {
        public int color;
        public OISer[] obejctInfos;
        public bool isExpanded;
        public string name;

        public LISer(LayerInfo layerInfo)
        {
            this.color = layerInfo.colorIndex;
            this.isExpanded = layerInfo.isExpanded;
            this.name = layerInfo.name;
            this.obejctInfos = new OISer[layerInfo.allObjects.Count];

            int i = 0;
            foreach (ObjectInfo objectInfo in layerInfo.allObjects)
            {
                this.obejctInfos[i] = new OISer(objectInfo);
                i++;
            }
        }
    }

    
    [Serializable]
    public class OISer
    {
        public string DisplayName;
        public string ReferenceObject;

        public OISer(ObjectInfo objectInfo)
        {
            this.DisplayName = objectInfo.DisplayName;
            this.ReferenceObject = objectInfo.ReferenceObject.GetGuid();
        }
    }
    
    public class LayerInfo
    {
        public int colorIndex;
        public List<ObjectInfo> allObjects;
        public bool isExpanded;
        public string name;

        public LayerInfo(string name, int colorIndex, bool isExpanded = false)
        {
            this.colorIndex = colorIndex;
            this.isExpanded = isExpanded;
            this.name = name;
            allObjects = new List<ObjectInfo>();
        }

        public LayerInfo(LISer ser)
        {
            this.colorIndex = ser.color;
            this.isExpanded = ser.isExpanded;
            this.name = ser.name;
            this.allObjects = new List<ObjectInfo>();

            foreach (OISer oiSer in ser.obejctInfos)
            {
                allObjects.Add(new ObjectInfo(oiSer, this.name));
            }
        }

        public Color Color()
        {
            return ResourcesWindow.BackgroundColorsOptions[colorIndex];
        }
    }

    public class ObjectInfo
    {
        private static readonly string namePrefix = "        ";
        public string DisplayName;
        public string LayerName;
        public Object ReferenceObject;
        public Texture Icon;

        public ObjectInfo(OISer oiSer, string layerName)
        {
            ReferenceObject = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(oiSer.ReferenceObject));
            DisplayName = oiSer.DisplayName;
            this.LayerName = layerName;
            Icon = ReferenceObject.GetIcon();
        }

        public ObjectInfo(string displayName, Object referenceObject, string layerName)
        {
            this.DisplayName = namePrefix + displayName;
            this.LayerName = layerName;
            this.ReferenceObject = referenceObject;
            Icon = ReferenceObject.GetIcon();
            // if (DisplayName.Length > 18) DisplayName = DisplayName.Substring(0, 15) + "...";
        }

        public void RenameTo(string newName)
        {
            this.DisplayName = namePrefix + newName;
        }

        public void StripDisplayName()
        {
            this.DisplayName = this.DisplayName.Substring(namePrefix.Length);
        }
    }
}