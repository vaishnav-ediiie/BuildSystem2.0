using System;
using System.Collections.Generic;
using CustomBuildSystem.Placing.Conditional;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem.Placing
{
    public class EdgePlaceable : PlaceableMonoBase
    {
        public int cellsCount = 1;
        public int centerCellIndex = 0;
        [SerializeField] private CombineMode criteriaCombineMode;
        [SerializeField] private List<EdgeCondition> placementCriteria;

        public IEnumerable<EdgeNumber> LoopAllEdges(EdgeNumber globalCordOfCenterEdge)
        {
            if (this.cellsCount <= 1)
            {
                yield return globalCordOfCenterEdge;
                yield break;
            }
            
            if (globalCordOfCenterEdge.edgeType == EdgeType.Vertical)
            {
                int startRow = globalCordOfCenterEdge.CellAfter.row - this.centerCellIndex;
                EdgeNumber first = new EdgeNumber(startRow, globalCordOfCenterEdge.CellAfter.column, globalCordOfCenterEdge.edgeType);
                CellNumber current = new CellNumber(0, 0);
                for (int _ = 0; _ < this.cellsCount; _++)
                {
                    yield return first + current;
                    current.row++;
                }
            }
            else
            {
                int startCol = globalCordOfCenterEdge.CellAfter.column - this.centerCellIndex;
                EdgeNumber first = new EdgeNumber(globalCordOfCenterEdge.CellAfter.row, startCol, globalCordOfCenterEdge.edgeType);
                CellNumber current = new CellNumber(0, 0);

                for (int _ = 0; _ < this.cellsCount; _++)
                {
                    yield return first + current;
                    current.column++;
                }
            }
        }
        
        public bool AreBaseConditionsSatisfied(BSS_PlacingEdge placingEdge)
        {
            BuildSystem buildSystem = placingEdge.BuildSystem; 
            EdgeNumber current = placingEdge.EdgeNumber;

            if (criteriaCombineMode == CombineMode.And)
            {
                foreach (EdgeCondition condition in placementCriteria)
                {
                    foreach (EdgeNumber edge in this.LoopAllEdges(current))
                    {
                        if (condition.HasViolated(buildSystem, edge)) return false;
                    }
                }
                return true;
            }
            
            
            foreach (EdgeCondition condition in placementCriteria)
            {
                foreach (EdgeNumber edge in this.LoopAllEdges(current))
                {
                    if (! condition.HasViolated(buildSystem, edge)) return true;
                }
            }
            return false;
            
        }

        public void AddCondition(EdgeCondition condition)
        {
            placementCriteria.Add(condition);
        }

        public bool HasCondition(ConditionType type, int floorNumber, bool relativeFloor, bool invertCondition)
        {
            foreach (EdgeCondition criterion in placementCriteria)
            {
                if (criterion.type == type
                    && criterion.floorNumber == floorNumber
                    && criterion.isFloorRelative == relativeFloor
                    && criterion.invertCondition == invertCondition
                   )
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            this.placementCriteria = new List<EdgeCondition>()
            {
                new EdgeCondition(ConditionType.MustBeEmpty, 0, isFloorRelative: true, invertCondition: false)
            };
        }
    }
}