using System;
using CustomGridSystem;
using DebugToScreen;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class CellPlacementCriteria
    {
        [Tooltip("Cells exactly one floor above the object")]
        public Condition cellAbove;

        [Tooltip("Cells exactly one floor below the object")]
        public Condition cellBelow;

        [Tooltip("Cells that are within the bonds of object")]
        public Condition cellCenter = Condition.CenterCellCondition;

        [Tooltip("Cells that share common edge")]
        public Condition cellSide;

        [Tooltip("Cells that share same corner")]
        public Condition cellCorner;

        [Tooltip("Boundary of Object")]
        public Condition edgeBoundary;

        [Tooltip("Edges that are enclosed")]
        public Condition edgeBetween;
        
        
        public bool AreSatisfied(BuildSystem buildSystem, CellLayoutInfo layoutInfo)
        {
            if (layoutInfo.IsSingleCelled) return StatsForSingleCell(buildSystem, layoutInfo);
            return StatsForMultiCell(buildSystem, layoutInfo);
        }

        private bool StatsForSingleCell(BuildSystem system, CellLayoutInfo layoutInfo)
        {
            // @formatter:off
            if (system.gridAbove != null && cellAbove.HasViolatedCell(system.gridAbove, layoutInfo.TopLeft)) { return false;}
            if (system.gridBelow != null && cellBelow.HasViolatedCell(system.gridBelow, layoutInfo.TopLeft)) { return false;}
            if (cellCenter.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft)) { return false;}
            
            
            CellNumber localLeftCn = layoutInfo.Left;
            if (cellSide.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft + localLeftCn)) { return false;}
            if (cellSide.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft - localLeftCn)) { return false;}
            
            CellNumber localFrontCn = layoutInfo.Front;
            if (cellSide.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft + localFrontCn)) { return false;}
            if (cellSide.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft - localFrontCn)) { return false;}
            
            if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft + localLeftCn + localFrontCn)) { return false;}
            if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft + localLeftCn - localFrontCn)) { return false;}
            if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft - localLeftCn + localFrontCn)) { return false;}
            if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft - localLeftCn - localFrontCn)) { return false;}
            
            
            Direction localLeft = Direction.Left.RotateBy(layoutInfo.Rotation);
            if (edgeBoundary.HasViolatedCD(system.gridCurrent, layoutInfo.TopLeft, localLeft)) { return false;}
            if (edgeBoundary.HasViolatedCD(system.gridCurrent, layoutInfo.TopLeft, localLeft.RotateBy(180))) { return false;}
            
            Direction localFront = Direction.Up.RotateBy(layoutInfo.Rotation);
            if (edgeBoundary.HasViolatedCD(system.gridCurrent, layoutInfo.TopLeft, localFront)) { return false;}
            if (edgeBoundary.HasViolatedCD(system.gridCurrent, layoutInfo.TopLeft, localFront.RotateBy(180))) { return false;}
            return true;
            // @formatter:on
        }

        private bool StatsForMultiCell(BuildSystem system, CellLayoutInfo layoutInfo)
        {
            GameDebug.Instance.acceptInput = true;
            GameDebug.UpdateMyInfo(BuildSystem.Instance, "StatsForMultiCell");
            if (system.gridAbove != null && CheckCellsWithinBonds(system.gridAbove, cellAbove, layoutInfo, "gridAbove")) return false;
            if (system.gridBelow != null && CheckCellsWithinBonds(system.gridBelow, cellBelow, layoutInfo, "gridBelow")) return false;
            if (CheckCellsWithinBonds(system.gridCurrent, cellCenter, layoutInfo, "gridCurrent")) return false;
            if (CheckSideCells(system.gridCurrent, layoutInfo)) return false;

            // Check Corners
            GameDebug.AddLineToMyInfo(BuildSystem.Instance, $"Check Corners: {cellCorner.conditionType}> ");
            if (cellCorner.conditionType != ConditionType.DontCare)
            {
                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.BottomLeft))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" BottomLeft ({layoutInfo.BottomLeft}) Violated");
                    return false;
                }

                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopRight))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" TopRight ({layoutInfo.TopRight}) Violated");
                    return false;
                }

                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.BottomRight))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" BottomRight ({layoutInfo.BottomRight}) Violated");
                    return false;
                }

                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" TopLeft ({layoutInfo.TopLeft}) Violated");
                    return false;
                }
            }

            if (CheckEdgesWithin(system.gridCurrent, layoutInfo)) return false;

            GameDebug.AddLineToMyInfo(BuildSystem.Instance, $"No Violation At All");
            return true;
        }

        private bool CheckSideCells(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellLayoutInfo layoutInfo)
        {
            GameDebug.AddLineToMyInfo(BuildSystem.Instance, "CheckSideCells: ");

            if (cellSide.conditionType == ConditionType.DontCare)
            {
                GameDebug.AddToMyInfo(BuildSystem.Instance, "  - DontCare");
                return false;
            }

            GameDebug.AddToMyInfo(BuildSystem.Instance, $"{cellSide.conditionType}> ");
            CellNumber bottomLeft = layoutInfo.BottomLeft;
            CellNumber topRight = layoutInfo.TopRight;

            CellNumber currentOne = new CellNumber(0, bottomLeft.column - 1);
            CellNumber currentTwo = new CellNumber(0, topRight.column + 1);
            for (int i = bottomLeft.row; i <= topRight.row; i++)
            {
                currentOne.row = i;
                currentTwo.row = i;
                GameDebug.AddToMyInfo(BuildSystem.Instance, $"{currentOne}, ");
                if (cellSide.HasViolatedCell(grid, currentOne))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" - Violated {cellSide.conditionType}");
                    return true;
                }

                GameDebug.AddToMyInfo(BuildSystem.Instance, $"{currentTwo}, ");
                if (cellSide.HasViolatedCell(grid, currentTwo))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" - Violated {cellSide.conditionType}");
                    return true;
                }
            }

            currentOne = new CellNumber(bottomLeft.row - 1, 0);
            currentTwo = new CellNumber(topRight.row + 1, 0);

            for (int i = bottomLeft.column; i <= topRight.column; i++)
            {
                currentOne.column = i;
                currentTwo.column = i;
                GameDebug.AddToMyInfo(BuildSystem.Instance, $"{currentOne}, ");
                if (cellSide.HasViolatedCell(grid, currentOne))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" - Violation");
                    return true;
                }

                GameDebug.AddToMyInfo(BuildSystem.Instance, $"{currentTwo}, ");
                if (cellSide.HasViolatedCell(grid, currentTwo))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" - Violation");
                    return true;
                }
            }

            GameDebug.AddToMyInfo(BuildSystem.Instance, "No Violation");
            return false;
        }

        private bool CheckCellsWithinBonds(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, Condition condition, CellLayoutInfo layoutInfo, string name)
        {
            GameDebug.AddLineToMyInfo(BuildSystem.Instance, $"CheckCellsWithinBonds {name} {condition.conditionType}>: ");
            if (condition.conditionType == ConditionType.DontCare)
            {
                GameDebug.AddToMyInfo(BuildSystem.Instance, "  - DontCare");
                return false;
            }

            foreach (CellNumber cellNumber in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight + 1))
            {
                GameDebug.AddToMyInfo(BuildSystem.Instance, $" {cellNumber}, ");
                if (condition.HasViolatedCell(grid, cellNumber))
                {
                    GameDebug.AddToMyInfo(BuildSystem.Instance, $" - Violation");
                    return true;
                }
            }

            GameDebug.AddToMyInfo(BuildSystem.Instance, $" No Violation");
            return false;
        }

        private bool CheckEdgesWithin(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellLayoutInfo layoutInfo)
        {
            GameDebug.AddLineToMyInfo(BuildSystem.Instance, $"CheckEdgesWithin>: {layoutInfo.BottomLeft} -> {layoutInfo.TopRight}");
            if (edgeBetween.conditionType == ConditionType.DontCare) return false;


            bool horizontal = true;
            bool vertical = false;
            for (int row = layoutInfo.BottomLeft.row; row <= layoutInfo.TopRight.row; row++)
            {
                horizontal = false;
                for (int col = layoutInfo.BottomLeft.column; col <= layoutInfo.TopRight.column; col++)
                {
                    GameDebug.AddLineToMyInfo(BuildSystem.Instance, $"({row}, {col}): ");
                    if (vertical && edgeBetween.HasViolatedEdge(grid, new EdgeNumber(row, col, EdgeType.Vertical))) return true;
                    if (horizontal && edgeBetween.HasViolatedEdge(grid, new EdgeNumber(row, col, EdgeType.Horizontal))) return true;
                    horizontal = true;
                }
                vertical = true;
            }
            return false;
        }
    }
}