using System;
using System.Collections.Generic;
using System.Data;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class EdgePlaceable : IMonoPlaceable
    {
        [NonSerialized] public EdgePlaceableSO Scriptable;
        [NonSerialized] public EdgeNumber Number;
        [NonSerialized] public int Rotation;
        [NonSerialized] public List<EdgeDecorator> Decorators;

        public void Init(EdgePlaceableSO scriptable, EdgeNumber number, int rotation, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
            this.Rotation = rotation;
            this.Decorators = new List<EdgeDecorator>();
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }

        public override GameObject GetDeletePrefab() => Scriptable.placingError;

        public override void Occupy(BuildSystem buildSystem)
        {
            if (Scriptable.cellsCount <= 1)
            {
                Debug.Log($"Occupy {Number}");
                buildSystem.gridCurrent.OccupyEdge(Number, this);
                return;
            }
            
            if (Number.edgeType == EdgeType.Horizontal)
            {
                int startRow = Number.CellAfter.row - Scriptable.centerCellIndex;
                EdgeNumber first = new EdgeNumber(startRow, Number.CellAfter.column, Number.edgeType);
                CellNumber current = new CellNumber(0, 0);
                for (int _ = 0; _ < Scriptable.cellsCount; _++)
                {
                    EdgeNumber e = first + current;
                    buildSystem.gridCurrent.OccupyEdge(e, this);
                    current.row++;
                }
            }
            else
            {
                int startCol = Number.CellAfter.column - Scriptable.centerCellIndex;
                EdgeNumber first = new EdgeNumber(Number.CellAfter.row, startCol, Number.edgeType);
                CellNumber current = new CellNumber(0, 0);

                for (int _ = 0; _ < Scriptable.cellsCount; _++)
                {
                    EdgeNumber e = first + current;
                    buildSystem.gridCurrent.OccupyEdge(e, this);
                    current.column++;
                }
            }
        }

        public override void UnOccupy(BuildSystem buildSystem)
        {
            if (Scriptable.cellsCount <= 1)
            {
                buildSystem.gridCurrent.EmptyEdge(Number);
                return;
            }

            
            if (Number.edgeType == EdgeType.Horizontal)
            {
                int startRow = Number.CellAfter.row - Scriptable.centerCellIndex;
                EdgeNumber first = new EdgeNumber(startRow, Number.CellAfter.column, Number.edgeType);
                CellNumber current = new CellNumber(0, 0);
                for (int _ = 0; _ < Scriptable.cellsCount; _++)
                {
                    EdgeNumber e = first + current;
                    buildSystem.gridCurrent.EmptyEdge(e);
                    current.row++;
                }
            }
            else
            {
                int startCol = Number.CellAfter.column - Scriptable.centerCellIndex;
                EdgeNumber first = new EdgeNumber(Number.CellAfter.row, startCol, Number.edgeType);
                CellNumber current = new CellNumber(0, 0);

                for (int _ = 0; _ < Scriptable.cellsCount; _++)
                {
                    EdgeNumber e = first + current;
                    buildSystem.gridCurrent.EmptyEdge(e);
                    current.column++;
                }
            }
        }
        
        public void RemoveDecorator(EdgeDecorator deco)
        {
            if (Decorators.Contains(deco))
                this.Decorators.Remove(deco);
        }

        public void AddDecorator(EdgeDecorator deco)
        {
            if (!Decorators.Contains(deco))
                this.Decorators.Add(deco);
        }

        public override bool HasDecorator(PlaceableSOBase scriptable)
        {
            foreach (EdgeDecorator decorator in Decorators)
            {
                if (decorator.Scriptable == scriptable) return true;
            }

            return false;
        }
        
        [Serializable]
        public class Serializer
        {
            public int scriptableID;
            public int row;
            public int column;
            public int edgyType;
            public int rotation;
            public DecoSer[] decorators;

            public EdgeType EdgyType => (EdgeType) edgyType;

            public Serializer()
            {
            }

            public Serializer(EdgePlaceable source)
            {
                this.scriptableID = source.Scriptable.ID;
                this.row = source.Number.CellAfter.row;
                this.column = source.Number.CellAfter.column;
                this.edgyType = (int)source.Number.edgeType;
                this.rotation = source.Rotation;
                decorators = new DecoSer[source.Decorators.Count];
                int i = 0;
                foreach (EdgeDecorator deco in source.Decorators)
                {
                    decorators[i] = new DecoSer(deco);
                    i++;
                }
            }

            public static EdgePlaceable Deserialize(Serializer serializer, BuildSystem buildSystem, Dictionary<int, PlaceableSOBase> allPlaceableData)
            {
                EdgeNumber cellNumber = new EdgeNumber(serializer.row, serializer.column, serializer.EdgyType);
                Vector3 position = buildSystem.gridCurrent.EdgeNumberToPosition(cellNumber);
                Quaternion rotation = Quaternion.Euler(0, serializer.rotation, 0);
                EdgePlaceableSO placeableSo = allPlaceableData[serializer.scriptableID] as EdgePlaceableSO;
                EdgePlaceable parent = Instantiate(placeableSo.placed, position, rotation).AddComponent<EdgePlaceable>();
                parent.Init(placeableSo, cellNumber, serializer.rotation, buildSystem.ProbsLayer);
                parent.Occupy(buildSystem);
                
                foreach (DecoSer decoSer in serializer.decorators)
                {
                    Quaternion decoRot = Quaternion.Euler(0, decoSer.rotation, 0);
                    EdgePlaceableSO decoSO = allPlaceableData[decoSer.scriptableID] as EdgePlaceableSO;
                    EdgeDecorator decoPlaced = Instantiate(decoSO.placed, position, decoRot).AddComponent<EdgeDecorator>();
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

            public DecoSer(EdgeDecorator source)
            {
                scriptableID = source.Scriptable.ID;
                rotation = source.Rotation;
            }
        }
    }
}