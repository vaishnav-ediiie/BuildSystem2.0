using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [CreateAssetMenu]
    public class EdgePlaceableSO : PlaceableSOBase
    {
        public int cellsCount = 1;
        public int centerCellIndex = 0;
        public EdgePlacementCriteria placementCriteria;
        
        
        public IEnumerable<EdgeNumber> LoopAllEdges(EdgeNumber globalCordOfCenterEdge)
        {
            if (this.cellsCount <= 1)
            {
                yield return globalCordOfCenterEdge;
                yield break;
            }
            
            if (globalCordOfCenterEdge.edgeType == EdgeType.Horizontal)
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

        
        
        public bool AreBaseConditionsSatisfied(BSS_PlacingEdge placingEdge) => this.placementCriteria.AreSatisfied(placingEdge.BuildSystem, placingEdge);

    }
}