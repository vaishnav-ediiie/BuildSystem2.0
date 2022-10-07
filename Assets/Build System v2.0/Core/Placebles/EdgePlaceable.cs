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
        [NonSerialized] public List<EdgeDecorator> Decorators;

        public void Init(EdgePlaceableSO scriptable, EdgeNumber number, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
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

    }
}