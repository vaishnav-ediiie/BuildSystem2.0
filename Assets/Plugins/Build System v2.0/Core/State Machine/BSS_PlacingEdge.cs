using System;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingEdge : BuiltSystemState
    {
        public static Func<EdgePlaceableSO, EdgeNumber, bool> ShouldPlaceInputChecker;
        private EdgePlaceableSO placeableSo;
        private Vector3 mousePosCenter;
        private EdgeNumber currentEdgeNumber;

        public override void OnUpdate()
        {
            EdgeNumber edgeNumber = GetReferenceEdge(mousePosCenter);
            if (edgeNumber != currentEdgeNumber)
            {
                UpdateVisuals(edgeNumber);
            }

            if (ShouldPlaceInputChecker.Invoke(placeableSo, edgeNumber))
            {
                ConfirmPlacement();
            }
        }

        
        internal void Setup(EdgePlaceableSO placeable)
        {
            this.placeableSo = placeable;
            mousePosCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            placeableSo.InitPlacing(buildSystem);

            ShouldPlaceInputChecker = (ple, num) => ple.canPlace && Input.GetMouseButtonDown(0);
            currentEdgeNumber = GetReferenceEdge(mousePosCenter);
            UpdateVisuals(currentEdgeNumber);
        }

        internal void ConfirmPlacement()
        {
            EdgePlaceable placed = (EdgePlaceable)placeableSo.Place();
            this.OccupyEdges(placed);
            buildSystem.ConfirmBuildInner(placed);
        }

        internal void CancelPlacement()
        {
            placeableSo.EndPlacing();
        }
        
        private void UpdateVisuals(EdgeNumber edgeNumber)
        {
            float rotation = (edgeNumber.edgeType == EdgeType.Horizontal) ? -90f : 0f;
            placeableSo.MoveTo(ActiveGrid.EdgeNumberToPosition(edgeNumber), edgeNumber);
            placeableSo.RotateTo(0, rotation, 0);
            placeableSo.Mark(ValidateEdge(edgeNumber));
            currentEdgeNumber = edgeNumber;
        }

        private bool ValidateEdge(EdgeNumber edgeNumber)
        {
            if (placeableSo.cellsCount <= 1)
            {
                return !buildSystem.activeGrid.IsEdgeOccupied(edgeNumber);
            }

            bool isHorz = (edgeNumber.edgeType == EdgeType.Horizontal);
            EdgeNumber first;
            if (isHorz)
            {
                int startRow = edgeNumber.CellAfter.row - placeableSo.centerCellIndex;
                first = new EdgeNumber(startRow, edgeNumber.CellAfter.column, edgeNumber.edgeType);
            }
            else
            {
                int startCol = edgeNumber.CellAfter.column - placeableSo.centerCellIndex;
                first = new EdgeNumber(edgeNumber.CellAfter.row, startCol, edgeNumber.edgeType);
            }
            CellNumber current = new CellNumber(0, 0);

            for (int _ = 0; _ < placeableSo.cellsCount; _++)
            {
                EdgeNumber en = first + current;
                bool isNumberValid = buildSystem.activeGrid.IsEdgeNumberValid(en);
                bool isOccupied = buildSystem.activeGrid.IsEdgeOccupied(en);
                if (!isNumberValid || isOccupied)
                {
                    return false;
                }
                if (isHorz) current.row++;
                else current.column++;
            }
            return true;
        }

        private void OccupyEdges(EdgePlaceable placed)
        {
            if (currentEdgeNumber.edgeType == EdgeType.Horizontal)
            {
                int startRow = currentEdgeNumber.CellAfter.row - placeableSo.centerCellIndex;
                EdgeNumber first = new EdgeNumber(startRow, currentEdgeNumber.CellAfter.column, currentEdgeNumber.edgeType);
                CellNumber current = new CellNumber(0, 0);
                for (int _ = 0; _ < placeableSo.cellsCount; _++)
                {
                    EdgeNumber e = first + current;
                    buildSystem.activeGrid.OccupyEdge(e, placed);
                    current.row++;
                }
            }
            else
            {
                int startCol = currentEdgeNumber.CellAfter.column - placeableSo.centerCellIndex;
                EdgeNumber first = new EdgeNumber(currentEdgeNumber.CellAfter.row, startCol, currentEdgeNumber.edgeType);
                CellNumber current = new CellNumber(0, 0);

                for (int _ = 0; _ < placeableSo.cellsCount; _++)
                {
                    EdgeNumber e = first + current;
                    buildSystem.activeGrid.OccupyEdge(e, placed);
                    current.column++;
                }
            }
        }
    }
}