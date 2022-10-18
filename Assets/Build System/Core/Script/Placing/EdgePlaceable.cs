using System;
using System.Collections.Generic;
using CustomBuildSystem.Placing.Conditional;
using CustomGridSystem;
using DebugToScreen;
using UnityEngine;

namespace CustomBuildSystem.Placing
{
    public class EdgePlaceable : PlaceableMonoBase
    {
        public int cellsCount = 1;
        public int centerCellIndex = 0;
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
            GameDebug.ClearInfo();
            GameDebug.AppendLine("AreBaseConditionsSatisfied: ");
            BuildSystem buildSystem = placingEdge.BuildSystem; 
            EdgeNumber current = placingEdge.EdgeNumber;
            
            foreach (EdgeCondition condition in placementCriteria)
            {
                GameDebug.AppendLine($"    {condition.type}: ");
                foreach (EdgeNumber edge in this.LoopAllEdges(current))
                {
                    GameDebug.AppendLine($"        {current}, ");
                    if (condition.HasViolated(buildSystem, edge))
                    {
                        GameDebug.AppendText("- Violated");
                        return false;
                    }
                }
                GameDebug.AppendText("- Okay");
            }
            GameDebug.AppendLine("All Okay");
            return true;
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