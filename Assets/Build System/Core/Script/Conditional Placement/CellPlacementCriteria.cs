using System;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class CellPlacementCriteria
    {
        [Tooltip("Cells exactly one floor above the object")]
        public CellCondition cellAbove;

        [Tooltip("Cells exactly one floor below the object")]
        public CellCondition cellBelow;

        [Tooltip("Cells that are within the bonds of object")]
        public CellCondition cellCenter = CellCondition.CenterCondition;

        [Tooltip("Cells that share common edge")]
        public CellCondition cellSide;

        [Tooltip("Cells that share same corner")]
        public CellCondition cellCorner;

        [Tooltip("Boundary of Object")]
        public CellCondition edgeBoundary;

        [Tooltip("Edges that are enclosed")]
        public CellCondition edgeBetween;
        
        
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
            if (system.gridAbove != null && CheckCellsWithinBonds(system.gridAbove, cellAbove, layoutInfo, "gridAbove")) return false;
            if (system.gridBelow != null && CheckCellsWithinBonds(system.gridBelow, cellBelow, layoutInfo, "gridBelow")) return false;
            if (CheckCellsWithinBonds(system.gridCurrent, cellCenter, layoutInfo, "gridCurrent")) return false;
            if (CheckSideCells(system.gridCurrent, layoutInfo)) return false;

            // Check Corners
            if (cellCorner.conditionType != ConditionType.DontCare)
            {
                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.BottomLeft))
                {
                    return false;
                }

                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopRight))
                {
                    return false;
                }

                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.BottomRight))
                {
                    return false;
                }

                if (cellCorner.HasViolatedCell(system.gridCurrent, layoutInfo.TopLeft))
                {
                    return false;
                }
            }

            if (CheckEdgesWithin(system.gridCurrent, layoutInfo)) return false;

            return true;
        }

        private bool CheckSideCells(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellLayoutInfo layoutInfo)
        {
            if (cellSide.conditionType == ConditionType.DontCare)
            {
                return false;
            }

            CellNumber bottomLeft = layoutInfo.BottomLeft;
            CellNumber topRight = layoutInfo.TopRight;

            CellNumber currentOne = new CellNumber(0, bottomLeft.column - 1);
            CellNumber currentTwo = new CellNumber(0, topRight.column + 1);
            for (int i = bottomLeft.row; i <= topRight.row; i++)
            {
                currentOne.row = i;
                currentTwo.row = i;
                if (cellSide.HasViolatedCell(grid, currentOne))
                {
                    return true;
                }

                if (cellSide.HasViolatedCell(grid, currentTwo))
                {
                    return true;
                }
            }

            currentOne = new CellNumber(bottomLeft.row - 1, 0);
            currentTwo = new CellNumber(topRight.row + 1, 0);

            for (int i = bottomLeft.column; i <= topRight.column; i++)
            {
                currentOne.column = i;
                currentTwo.column = i;
                if (cellSide.HasViolatedCell(grid, currentOne))
                {
                    return true;
                }

                if (cellSide.HasViolatedCell(grid, currentTwo))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckCellsWithinBonds(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellCondition condition, CellLayoutInfo layoutInfo, string name)
        {
            if (condition.conditionType == ConditionType.DontCare)
            {
                return false;
            }

            foreach (CellNumber cellNumber in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight + 1))
            {
                if (condition.HasViolatedCell(grid, cellNumber))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckEdgesWithin(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellLayoutInfo layoutInfo)
        {
            if (edgeBetween.conditionType == ConditionType.DontCare) return false;


            bool horizontal = true;
            bool vertical = false;
            for (int row = layoutInfo.BottomLeft.row; row <= layoutInfo.TopRight.row; row++)
            {
                horizontal = false;
                for (int col = layoutInfo.BottomLeft.column; col <= layoutInfo.TopRight.column; col++)
                {
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