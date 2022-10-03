using System;
using CustomGridSystem;
using DebugToScreen;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingCell : BuiltSystemState
    {
        public static Func<CellPlaceableSO, CellNumber, bool> ShouldPlaceInputChecker;
        public static Func<bool> ShouldRotateInputChecker;
        private CellPlaceableSO placeableSo;
        private Vector3 mousePosCenter;
        private CellNumber currentCellNumber;
        private int rotation;

        internal void Setup(CellPlaceableSO placeable)
        {
            GameDebug.StartMyInfo(this, title: $"Placing: {placeable.name}", isExpanded: true);
            this.placeableSo = placeable;
            mousePosCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            placeableSo.InitPlacing(buildSystem);
            rotation = 90;
            ShouldPlaceInputChecker = (pl, nu) => pl.canPlace && Input.GetMouseButtonDown(0);
            ShouldRotateInputChecker = () => Input.GetKeyDown(KeyCode.R);
            currentCellNumber = GetReferenceCell(mousePosCenter);
            
            UpdateVisuals(currentCellNumber);
        }

        public override void OnUpdate()
        {
            CellNumber cellNumber = GetReferenceCell(mousePosCenter);
            if (cellNumber != currentCellNumber)
            {
                UpdateVisuals(cellNumber);
            }

            if (ShouldPlaceInputChecker.Invoke(placeableSo, cellNumber))
            {
                ConfirmPlacement();
            }

            if (ShouldRotateInputChecker.Invoke())
            {
                Rotate();
            }
        }
        
        internal void ConfirmPlacement()
        {
            CellPlaceable placed = (CellPlaceable)placeableSo.Place();
            OccupyCells(placed);
            buildSystem.ConfirmBuildInner(placed);
        }

        internal void CancelPlacement()
        {
            placeableSo.EndPlacing();
        }
        
        private void Rotate()
        {
            placeableSo.RotateBy(0f, -90f, 0f);
            if (rotation == 0) rotation = 270;
            else rotation -= 90;
            
            placeableSo.Mark(ValidateCell(currentCellNumber));
        }
        
        private void UpdateVisuals(CellNumber cellNumber)
        {
            placeableSo.MoveTo(ActiveGrid.CellNumberToPosition(cellNumber), cellNumber);
            placeableSo.Mark(ValidateCell(cellNumber)); // Notice Not (!)
            currentCellNumber = cellNumber;
        }

        private bool ValidateCell(CellNumber cellNumber)
        {
            if (placeableSo.IsSingleCelled)
            {
                return !buildSystem.activeGrid.IsCellOccupied(cellNumber);
            }
            
            CellLayoutInfo layoutInfo = placeableSo.LayoutInfo;
            layoutInfo.UpdateBonds(cellNumber, rotation, this);

            if (!buildSystem.activeGrid.IsCellNumberValid(layoutInfo.TopLeft) || 
                !buildSystem.activeGrid.IsCellNumberValid(layoutInfo.BottomRight)) // Notice not ! 
            {
                GameDebug.UpdateMyInfo(this, "Out of grid");
                return false;
            }
            GameDebug.AddLineToMyInfo(this, $"    Checking: ");
            
            foreach (CellNumber number in CellNumber.LoopCells(layoutInfo.TopLeft, layoutInfo.BottomRight+CellNumber.One))
            {
                GameDebug.AddToMyInfo(this, $"{number}, ");
                if (buildSystem.activeGrid.IsCellOccupied(number))
                {
                    GameDebug.AddToMyInfo(this, " - Ended");
                    return false;
                }
            }
            return true;
        }
        
        private void OccupyCells(CellPlaceable placed)
        {
            if (placeableSo.IsSingleCelled)
            {
                buildSystem.activeGrid.OccupyCell(currentCellNumber, placed);
                return;
            }

            CellLayoutInfo layoutInfo = placeableSo.LayoutInfo;
            layoutInfo.UpdateBonds(currentCellNumber, rotation, this);


            
            foreach (CellNumber number in CellNumber.LoopCells(layoutInfo.TopLeft, layoutInfo.BottomRight+CellNumber.One))
            {
                buildSystem.activeGrid.OccupyCell(number, placed);
            }
        }
    }
    
    public class CellLayoutInfo
    {
        public CellNumber Center;
        public CellNumber TotalSize;
        public CellNumber TopLeft;
        public CellNumber BottomRight;

        public CellLayoutInfo(CellNumber center, CellNumber totalSize)
        {
            Center = center;
            TotalSize = totalSize;
        }
    
        public void UpdateBonds(CellNumber current, int angle, BSS_PlacingCell logger)
        {
            // if (angle == 0) angle = 270;
            // else angle -= 90;
            
            CellNumber v1 = current - RotatePoint(Center, angle);
            CellNumber v2 = v1 + RotatePoint(TotalSize, angle) - RotatePoint(CellNumber.One, angle);
            
            TopLeft = new CellNumber(Mathf.Min(v1.row, v2.row), Mathf.Min(v1.column, v2.column));
            BottomRight = new CellNumber(Mathf.Max(v1.row, v2.row), Mathf.Max(v1.column, v2.column));
            
            GameDebug.UpdateMyInfo(logger, $"Update Bonds: ");
            GameDebug.AddLineToMyInfo(logger, $"    Ct: {Center}");
            GameDebug.AddLineToMyInfo(logger, $"    Ts: {TotalSize}");
            GameDebug.AddLineToMyInfo(logger, $"    Ro: {angle}");
            GameDebug.AddLineToMyInfo(logger, $"    v1: {v1}");
            GameDebug.AddLineToMyInfo(logger, $"    v2: {v2}");
            GameDebug.AddLineToMyInfo(logger, $"    TopLeft: {TopLeft}");
            GameDebug.AddLineToMyInfo(logger, $"    BottomRight: {BottomRight}");
        }
    
        private CellNumber RotatePoint(CellNumber point, int angle)
        {
            if (angle == 0)   return new CellNumber(+point.row,    +point.column);
            if (angle == 90)  return new CellNumber(-point.column, +point.row);
            if (angle == 180) return new CellNumber(-point.row,    -point.column);
            if (angle == 270) return new CellNumber(+point.column, -point.row);
            return point;
        }
    
    }
}


