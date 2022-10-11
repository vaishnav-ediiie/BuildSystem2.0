using System;
using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class CellPlaceable : IMonoPlaceable
    {
        [NonSerialized] public CellPlaceableSO Scriptable;
        [NonSerialized] public CellNumber Number;
        [NonSerialized] public int Rotation;
        [NonSerialized] public List<CellDecorator> Decorators;

        public void Init(CellPlaceableSO scriptable, CellNumber number, int rotation, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
            this.Rotation = rotation;
            this.Decorators = new List<CellDecorator>();
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }
        
        public override GameObject GetDeletePrefab() => Scriptable.placingError;
        
        public override void Occupy(BuildSystem buildSystem)
        {
            CellLayoutInfo layoutInfo = Scriptable.LayoutInfo(Number, Rotation);
            
            if (layoutInfo.IsSingleCelled)
            {
                buildSystem.gridCurrent.OccupyCell(Number, this);
                return;
            }

            foreach (CellNumber number in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight+1))
            {
                buildSystem.gridCurrent.OccupyCell(number, this);
            }
        }

        public override void UnOccupy(BuildSystem buildSystem)
        {
            CellLayoutInfo layoutInfo = Scriptable.LayoutInfo(Number, Rotation);
            if (layoutInfo.IsSingleCelled)
            {
                buildSystem.gridCurrent.EmptyCell(Number);
                return;
            }

            foreach (CellDecorator decorator in Decorators)
            {
                Destroy(decorator.gameObject);
            }
            
            foreach (CellNumber cellNumber in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight+1))
            {
                buildSystem.gridCurrent.EmptyCell(cellNumber);
            }
        }

        public override bool HasDecorator(PlaceableSOBase scriptable)
        {
            foreach (CellDecorator decorator in Decorators)
            {
                if (decorator.Scriptable == scriptable) return true;
            }
            return false;
        }

        public override int GetScriptableID() => Scriptable.ID;

        public void RemoveDecorator(CellDecorator deco)
        {
            if (Decorators.Contains(deco))
            {
                this.Decorators.Remove(deco);
            }
        }

        public void AddDecorator(CellDecorator deco)
        {
            if (!Decorators.Contains(deco))
                this.Decorators.Add(deco);
        }

        
        public override IEnumerable<IMonoPlaceable> Children
        {
            get
            {
                foreach (CellDecorator decorator in Decorators)
                {
                    yield return decorator;
                }
            }
        }

        [Serializable]
        public class Serializer
        {
            public int scriptableID;
            public int row;
            public int column;
            public int rotation;
            public DecoSer[] decorators;

            public Serializer()
            {
                
            }
            
            public Serializer(CellPlaceable source)
            {
                this.scriptableID = source.Scriptable.ID;
                this.row = source.Number.row;
                this.column = source.Number.column;
                this.rotation = source.Rotation;
                decorators = new DecoSer[source.Decorators.Count];
                int i = 0;
                foreach (CellDecorator deco in source.Decorators)
                {
                    decorators[i] = new DecoSer(deco);
                    i++;
                }
            }

            public static CellPlaceable Deserialize(Serializer serializer, BuildSystem buildSystem)
            {
                CellPlaceableSO placeableSo = buildSystem.Brain.AllPlaceableData[serializer.scriptableID] as CellPlaceableSO;
                if (placeableSo == null)
                {
                    foreach (KeyValuePair<int,PlaceableSOBase> placeableSoBase in buildSystem.Brain.AllPlaceableData)
                    {
                        Debug.Log($" We Have: {placeableSoBase.Key} as {placeableSoBase.Value.GetType()}");
                    }
                    Debug.Log($"Not found with id: {serializer.scriptableID} as CellPlaceableSO");
                }
                CellNumber cellNumber = new CellNumber(serializer.row, serializer.column);
                Vector3 position = buildSystem.gridCurrent.CellNumberToPosition(cellNumber);
                Quaternion rotation = Quaternion.Euler(0, serializer.rotation, 0);
                CellPlaceable parent = Instantiate(placeableSo.placed, position, rotation).AddComponent<CellPlaceable>();
                parent.Init(placeableSo, cellNumber, serializer.rotation, buildSystem.ProbsLayer);
                parent.Occupy(buildSystem);
                
                foreach (DecoSer decoSer in serializer.decorators)
                {
                    CellPlaceableSO decoSO = buildSystem.Brain.AllPlaceableData[decoSer.scriptableID] as CellPlaceableSO;
                    Quaternion decoRot = Quaternion.Euler(0, decoSer.rotation, 0);
                    CellDecorator decoPlaced = Instantiate(decoSO.placed, position, decoRot).AddComponent<CellDecorator>();
                    decoPlaced.Init(decoSO, parent, decoSer.rotation, buildSystem.ProbsLayer);
                    decoPlaced.Occupy(buildSystem);

                }
                
                return parent;
            }
        }
        
        [Serializable]
        public class DecoSer
        {
            public int scriptableID;
            public int rotation;

            public DecoSer()
            {
            }

            public DecoSer(CellDecorator source)
            {
                scriptableID = source.Scriptable.ID;
                rotation = source.Rotation;
            }
        }
    }
}