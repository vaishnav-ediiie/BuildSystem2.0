using System;
using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class EdgePlaceable : IMonoPlaceable
    {
        [HideInInspector] public EdgePlaceableSO Scriptable;
        [HideInInspector] public EdgeNumber Number;
        [HideInInspector] public List<IPlaceable> Decorators;

        public void Init(EdgePlaceableSO scriptable, EdgeNumber number, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
            this.Decorators = new List<IPlaceable>();
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }

        public override GameObject GetDeletePrefab() => Scriptable.placingError;

        public override void Occupy(BuildSystem buildSystem)
        {
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
    }
}