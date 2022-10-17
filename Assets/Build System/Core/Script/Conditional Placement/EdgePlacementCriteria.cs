using System;
using CustomBuildSystem.Placed;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class EdgePlacementCriteria
    {
        [Tooltip("Cells exactly one floor above the object")]
        public EdgeCondition edgeAbove;

        [Tooltip("Cells exactly one floor below the object")]
        public EdgeCondition edgeBelow;

        [Tooltip("Cells that are within the bonds of object")]
        public EdgeCondition edgeCenter = EdgeCondition.CenterCondition;

        
        public bool AreSatisfied(BuildSystem buildSystem, BSS_PlacingEdge placingEdge)
        {
            if (placingEdge.Placeable.cellsCount <= 1) return StatsForSingleCell(buildSystem, placingEdge.EdgeNumber);
            return StatsForMultiCell(buildSystem, placingEdge);
        }

        private bool StatsForSingleCell(BuildSystem system, EdgeNumber edgeNumber)
        {
            // @formatter:off
            if (system.gridAbove != null && edgeAbove.HasViolated(system.gridAbove, edgeNumber)) { return false;}
            if (system.gridBelow != null && edgeBelow.HasViolated(system.gridBelow, edgeNumber)) { return false;}
            if (edgeCenter.HasViolated(system.gridCurrent, edgeNumber)) { return false;}
            return true;
            // @formatter:on
        }

        private bool StatsForMultiCell(BuildSystem system, BSS_PlacingEdge placingEdge)
        {
            if (system.gridAbove != null && CheckWithinBonds(system.gridAbove, edgeAbove, placingEdge)) return false;
            if (system.gridBelow != null && CheckWithinBonds(system.gridBelow, edgeBelow, placingEdge)) return false;
            if (CheckWithinBonds(system.gridCurrent, edgeCenter, placingEdge)) return false;
            return true;
        }

        private bool CheckWithinBonds(DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono> grid, EdgeCondition condition, BSS_PlacingEdge placingEdge)
        {
            if (condition.conditionType == ConditionType.DontCare) return false;

            foreach (EdgeNumber edgeNumber in placingEdge.Placeable.LoopAllEdges(placingEdge.EdgeNumber))
            {
                if (condition.HasViolated(grid, edgeNumber)) return true;
            }
            return false;
        }
    }
}